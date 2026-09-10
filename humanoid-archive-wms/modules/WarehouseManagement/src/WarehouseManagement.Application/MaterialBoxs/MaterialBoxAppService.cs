using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using WarehouseManagement.Material;
using WarehouseManagement.Cells;
using WarehouseManagement.MaterialBoxs.Aggregates;
using WarehouseManagement.MaterialBoxs.Dto;
using WarehouseManagement.RfidCodes;

namespace WarehouseManagement.MaterialBoxs
{
    public class MaterialBoxAppService : WarehouseManagementAppService, IMaterialBoxAppService
    {
        private readonly MaterialBoxManager _materialBoxManager;
        private readonly IMaterialBoxRepository _materialBoxRepository;
        private readonly ICellRepository _cellRepository;
        private readonly RfidCodeManager _rfidManager;
        private readonly MaterialManager _materialManager;
        private readonly MaterialBoxDetailManager _materialBoxDetailManager;

        public MaterialBoxAppService(MaterialBoxManager materialBoxManager
            , IMaterialBoxRepository materialBoxRepository    
            , RfidCodeManager rfidManager
            , MaterialManager materialManager
            , MaterialBoxDetailManager materialBoxDetailManager
            , ICellRepository cellRepository
        )
        {
            _materialBoxManager = materialBoxManager;
            _materialBoxRepository = materialBoxRepository;
            _rfidManager = rfidManager;
            _materialManager = materialManager;
            _materialBoxDetailManager = materialBoxDetailManager;
            _cellRepository = cellRepository;
        }

        public async Task<MaterialBoxDto> CreateAsync(CreateMaterialBoxDto createMaterialBox)
        {
            //检查标签是否存在
            if (!createMaterialBox.MaterialBoxRfid.IsNullOrEmpty() && !await _rfidManager.CheckExistRfidCode(createMaterialBox.MaterialBoxRfid, 2))
            {
                throw new UserFriendlyException("数据库中不存在标签" + createMaterialBox.MaterialBoxRfid);
            }
            //检查标签是否绑定
            if (!createMaterialBox.MaterialBoxRfid.IsNullOrEmpty() && await _materialBoxManager.CheckUsedBoxRfid(createMaterialBox.MaterialBoxRfid))
            {
                throw new UserFriendlyException(createMaterialBox.MaterialBoxRfid + "标签已被绑定");
            }
            //检查物料号不能为空
            if (createMaterialBox.StockBarcode.IsNullOrEmpty())
            {
                throw new UserFriendlyException("物料号不能为空");
            }
            //检查物料容器里类型不能为空
            if (createMaterialBox.CellModel.IsNullOrEmpty())
            {
                throw new UserFriendlyException("物料容器类型不能为空");
            }
            var entity = base.ObjectMapper.Map<CreateMaterialBoxDto, MaterialBox>(createMaterialBox);
            
            var archivebox = await _materialBoxRepository.InsertAsync(entity);
            return base.ObjectMapper.Map<MaterialBox, MaterialBoxDto>(archivebox);
        }

        public async Task DeleteAsync(CreateMaterialBoxDto input)
        {
            await _materialBoxManager.DeleteAsync(input.Id);
        }
        public async Task<MaterialBoxDto> UpdateAsync(CreateMaterialBoxDto input)
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
            if (input.StockBarcode.IsNullOrEmpty())
            {
                throw new UserFriendlyException("档号不能为空");
            }
            //检查档案盒尺寸不能为空
            if (input.CellModel.IsNullOrEmpty())
            {
                throw new UserFriendlyException("档号不能为空");
            }
            var entity = await _materialBoxRepository.FindByIdAsync(input.Id);
            if (entity == null)
                throw new UserFriendlyException(message: "档案盒不存在");
            entity = base.ObjectMapper.Map<CreateMaterialBoxDto, MaterialBox>(input,entity);

            var archivebox = await _materialBoxRepository.UpdateAsync(entity);

            return base.ObjectMapper.Map<MaterialBox, MaterialBoxDto>(archivebox);
        }
        
        public async Task<PagedResultDto<MaterialBoxDto>> PageAsync(PagingMaterialBoxListInput input)
        {
            var queryable = await _materialBoxRepository.GetQueryableAsync();

            //Prepare a query to join books and authors
            var query = from archiveBox in queryable
                        join celltemp in await _cellRepository.GetQueryableAsync() on archiveBox.CellId equals celltemp.Id into sc
                        from cell in sc.DefaultIfEmpty()
                        where archiveBox.MaterialBoxName.Contains(input.Filter.IsNullOrEmpty() ? "" : input.Filter.Trim())
                        select new { archiveBox ,cell };

            //Paging
            query = query
                .OrderByDescending(f => f.archiveBox.Id)
                .Skip(input.SkipCount)
                .Take(1000);
            //.Take(input.MaxResultCount);

            //Execute the query and get a list
            var queryResult = await AsyncExecuter.ToListAsync(query);

            //Convert the query result to a list of BookDto objects
            var archiveBoxDtos = queryResult.Select(x =>
            {
                var archiveBoxDtos = ObjectMapper.Map<MaterialBox, MaterialBoxDto>(x.archiveBox);
                archiveBoxDtos.CellCode = x.cell?.CellCode;

                return archiveBoxDtos;
            }).Take(input.PageSize).ToList();

            var totalCount = queryResult.Count() + input.SkipCount;

            return new PagedResultDto<MaterialBoxDto>(
                totalCount,
                archiveBoxDtos
            );
        }
        public async Task<PagedResultDto<MaterialBoxDetailDto>> DetailAsync(PagingMaterialBoxDetailInput input)
        {
            return await _materialBoxDetailManager.GetDetailAsync(input);
        }
        public async Task<MaterialBoxDto> BindRfid(CreateMaterialBoxDto input)
        {
            try
            {
                var entity =await _materialBoxRepository.FindByIdAsync(input.Id);
                //检查标签是否存在
                if (!input.MaterialBoxRfid.IsNullOrEmpty() && !await _rfidManager.CheckExistRfidCode(input.MaterialBoxRfid, 2))
                {
                    throw new UserFriendlyException("数据库中不存在标签" + input.MaterialBoxRfid);
                }
                //检测标签是否被绑定
                if (!input.MaterialBoxRfid.IsNullOrEmpty() && await _materialBoxManager.CheckUsedBoxRfid(input.MaterialBoxRfid))
                {
                    throw new UserFriendlyException( input.MaterialBoxRfid + "标签已被绑定");
                }
                entity.MaterialBoxBarcode = input.MaterialBoxRfid;
                var archivebox = await _materialBoxRepository.UpdateAsync(entity);

                return base.ObjectMapper.Map<MaterialBox, MaterialBoxDto>(archivebox);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message.ToString());
            }
        }

        public async Task<Boolean> BindArchive(string MaterialBoxRfid,string MaterialRfid)
        {
            var archiveBox = await _materialBoxManager.GetMaterialBoxByRfidCode(MaterialBoxRfid);
            if (archiveBox == null)
            {
                throw new UserFriendlyException(message: "档案盒不存在");
            }
            var Material = await _materialManager.GetArchiveByRfidCode(MaterialRfid);
            if (Material == null)
            {
                throw new UserFriendlyException("档案文件不存在");
            }
            //检查档案是否已绑定
            var detail = await _materialBoxDetailManager.GetDetailByArchiveId(Material.Id);
            if (detail != null)
            {
                throw new UserFriendlyException("档案文件已经绑定在档案盒" + detail.MaterialBoxId);
            }

            archiveBox.AddDetail(archiveBox.Id, Material.Id);
            await _materialBoxRepository.UpdateAsync(archiveBox);
            return true;

        }

    }
}
