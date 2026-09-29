using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.ObjectMapping;
using Volo.Abp;
using WarehouseManagement.Material;
using MaterialAggregate = WarehouseManagement.Material.Aggregates.Material;
using WarehouseManagement.MaterialBoxs.Dto;
using WarehouseManagement.MaterialBoxs;
using WarehouseManagement.Material.Dto;
using WarehouseManagement.RfidCodes;
using Volo.Abp.Application.Dtos;
using WarehouseManagement.MaterialBoxs.Aggregates;
using Lion.AbpPro.Extension.Customs.Dtos;
using WarehouseManagement.Goodss;
using WarehouseManagement.StockTasks.Aggregates;

namespace WarehouseManagement.Material
{
    public class MaterialAppService : WarehouseManagementAppService, IMaterialAppService
    {
        private readonly IMaterialRepository _materialRepository;
        private readonly IMaterialBoxRepository _materialBoxRepository;
        private readonly IArchiveBoxDetailRepository _materialBoxDetailRepository;
        private readonly MaterialManager _materialManager;
        private readonly RfidCodeManager _rfidManager;


        public MaterialAppService(IMaterialRepository materialRepository, MaterialManager materialManager
            ,RfidCodeManager rfidCodeManager, IMaterialBoxRepository materialBoxRepository,IArchiveBoxDetailRepository materialBoxDetailRepository)
        {
            _materialRepository = materialRepository;
            _materialManager = materialManager;
            _rfidManager = rfidCodeManager;
            _materialBoxRepository = materialBoxRepository;
            _materialBoxDetailRepository = materialBoxDetailRepository;
        }

        /// <summary>
        /// 创建基础物料，仅映射基础信息，不处理档案标签绑定。
        /// </summary>
        /// <param name="input">物料基础信息。</param>
        /// <returns>已保存的物料 ID 和七项基础信息。</returns>
        public async Task<CreateMaterialResultDto> CreateAsync(CreateMaterialInputDto input)
        {
            // 沿用现有物料码校验及 HTTP 错误响应。
            if (input.MaterialCode.IsNullOrEmpty())
            {
                throw new UserFriendlyException("档号不能为空");
            }
            var entity = base.ObjectMapper.Map<CreateMaterialInputDto, MaterialAggregate>(input);

            // 先保存以取得数据库生成的主键，避免返回尚未持久化的默认 ID。
            var archive = await _materialRepository.InsertAsync(entity, autoSave: true);
            return base.ObjectMapper.Map<MaterialAggregate, CreateMaterialResultDto>(archive);
        }

        public async Task DeleteAsync(CreateMaterialDto input)
        {
            await _materialRepository.DeleteAsync(input.Id);
            Console.WriteLine("删除成功");
        }

        public async Task<PagedResultDto<MaterialDto>> PageAsync(PagingMaterialListInput input)
        {
            var materialQueryable = await _materialRepository.GetQueryableAsync();

            // 左连接 Material 和 ArchiveBoxDetail
            var query = materialQueryable
                .Where(material => string.IsNullOrEmpty(input.Filter) || material.MaterialName.Contains(input.Filter.Trim()))
                .Select(material => new { Material = material, MaterialBox = (MaterialBox)null });

            var totalCount = await AsyncExecuter.CountAsync(query);

            var pagedQuery = query
                .OrderByDescending(x => x.Material.Id)
                .Skip(input.SkipCount)
                .Take(input.PageSize);

            var queryResult = await AsyncExecuter.ToListAsync(pagedQuery);

            var materialDtos = queryResult.Select(x =>
            {
                var materialDto = ObjectMapper.Map<MaterialAggregate, MaterialDto>(x.Material);
                if (x.MaterialBox != null)  // 注意：可能为 null
                {
                    materialDto.MaterialBoxRfid = x.MaterialBox.MaterialBoxBarcode;
                    materialDto.MaterialBoxName = x.MaterialBox.MaterialBoxName;
                    materialDto.MaterialBoxId = x.MaterialBox.Id;
                }
                return materialDto;
            }).ToList();

            return new PagedResultDto<MaterialDto>(totalCount, materialDtos);
        }

        public async Task<MaterialDto> UpdateAsync(CreateMaterialDto input)
        {
            //检查标签是否存在
            if (!input.MaterialBoxRfid.IsNullOrEmpty() && !await _rfidManager.CheckExistRfidCode(input.MaterialBoxRfid, 2))
            {
                throw new UserFriendlyException("数据库中不存在标签" + input.MaterialBoxRfid);
            }
            //检查标签是否绑定
            //if (!input.MaterialBoxRfid.IsNullOrEmpty() && await _archiveBoxManager.CheckUsedBoxRfid(input.MaterialBoxRfid))
            //{
            //    throw new UserFriendlyException(input.MaterialBoxRfid + "标签已被绑定");
            //}
            //检查档号不能为空
            if (input.MaterialCode.IsNullOrEmpty())
            {
                throw new UserFriendlyException("档号不能为空");
            }
            var entity = await _materialRepository.FindByRfidCodeAsync(input.MaterialRfid);
            if (entity == null)
                throw new UserFriendlyException(message: "档案盒不存在");
            entity = base.ObjectMapper.Map<CreateMaterialDto, MaterialAggregate>(input, entity);
            var archivebox = await _materialRepository.UpdateAsync(entity);

            return base.ObjectMapper.Map<MaterialAggregate, MaterialDto>(archivebox);
        }
    }
}
