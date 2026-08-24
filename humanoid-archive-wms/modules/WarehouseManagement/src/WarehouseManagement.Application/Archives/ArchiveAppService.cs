using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.ObjectMapping;
using Volo.Abp;
using WarehouseManagement.Archives.Aggregates;
using WarehouseManagement.ArchiveBoxs.Dto;
using WarehouseManagement.ArchiveBoxs;
using WarehouseManagement.Archives.Dto;
using WarehouseManagement.RfidCodes;
using Volo.Abp.Application.Dtos;
using WarehouseManagement.ArchiveBoxs.Aggregates;
using Lion.AbpPro.Extension.Customs.Dtos;
using WarehouseManagement.Goodss;
using WarehouseManagement.StockTasks.Aggregates;

namespace WarehouseManagement.Archives
{
    public class ArchiveAppService : WarehouseManagementAppService, IArchiveAppService
    {
        private readonly IArchiveRepository _archiveRepository;
        private readonly IArchiveBoxRepository _archiveBoxRepository;
        private readonly IArchiveBoxDetailRepository _archiveBoxDetailRepository;
        private readonly ArchiveManager _archiveManager;
        private readonly RfidCodeManager _rfidManager;


        public ArchiveAppService(IArchiveRepository archiveRepository, ArchiveManager archiveManager
            ,RfidCodeManager rfidCodeManager, IArchiveBoxRepository archiveBoxRepository,IArchiveBoxDetailRepository archiveBoxDetailRepository)
        {
            _archiveRepository = archiveRepository;
            _archiveManager = archiveManager;
            _rfidManager = rfidCodeManager;
            _archiveBoxRepository = archiveBoxRepository;
            _archiveBoxDetailRepository = archiveBoxDetailRepository;
        }

        public async Task<ArchiveDto> CreateAsync(CreateArchiveDto createArchiveBox)
        {
            //检查标签是否存在
            if (!createArchiveBox.RfidId.IsNullOrEmpty() && !await _rfidManager.CheckExistRfidCode(createArchiveBox.RfidId, 1))
            {
                throw new UserFriendlyException("数据库中不存在标签" + createArchiveBox.ArchiveBoxRfid);
            }
            //检查标签是否绑定
            if (!createArchiveBox.ArchivesRfid.IsNullOrEmpty() && await _archiveManager.CheckUsedBoxRfid(createArchiveBox.ArchivesRfid))
            {
                throw new UserFriendlyException(createArchiveBox.ArchivesRfid + "标签已被绑定");
            }
            //检查档号不能为空
            if (createArchiveBox.ArchivesCode.IsNullOrEmpty())
            {
                throw new UserFriendlyException("档号不能为空");
            }
            var entity = base.ObjectMapper.Map<CreateArchiveDto, Archive>(createArchiveBox);

            var archive = await _archiveRepository.InsertAsync(entity);
            return base.ObjectMapper.Map<Archive, ArchiveDto>(archive);
        }

        public async Task DeleteAsync(CreateArchiveDto input)
        {
            await _archiveRepository.DeleteAsync(input.Id);
            Console.WriteLine("删除成功");
        }

        public async Task<PagedResultDto<ArchiveDto>> PageAsync(PagingArchiveListInput input)
        {
            var archiveQueryable = await _archiveRepository.GetQueryableAsync();
            var boxDetailQueryable = await _archiveBoxDetailRepository.GetQueryableAsync();
            var boxQueryable = await _archiveBoxRepository.GetQueryableAsync();

            // 左连接 Archive 和 ArchiveBoxDetail
            var query = from archive in archiveQueryable
                        join archiveBoxDetail in boxDetailQueryable
                            on archive.Id equals archiveBoxDetail.ArchiveId into archiveBoxDetailGroup
                        from abd in archiveBoxDetailGroup.DefaultIfEmpty()  // LEFT JOIN
                        join archiveBox in boxQueryable
                            on abd.ArchiveBoxId equals archiveBox.Id into archiveBoxGroup
                        from ab in archiveBoxGroup.DefaultIfEmpty()  // LEFT JOIN
                        where string.IsNullOrEmpty(input.Filter) ||
                              archive.ArchivesName.Contains(input.Filter.Trim())
                        select new { Archive = archive, ArchiveBox = ab };

            var totalCount = await AsyncExecuter.CountAsync(query);

            var pagedQuery = query
                .OrderByDescending(x => x.Archive.Id)
                .Skip(input.SkipCount)
                .Take(input.PageSize);

            var queryResult = await AsyncExecuter.ToListAsync(pagedQuery);

            var archiveDtos = queryResult.Select(x =>
            {
                var archiveDto = ObjectMapper.Map<Archive, ArchiveDto>(x.Archive);
                if (x.ArchiveBox != null)  // 注意：可能为 null
                {
                    archiveDto.ArchiveBoxRfid = x.ArchiveBox.ArchiveBoxRfid;
                    archiveDto.ArchiveBoxName = x.ArchiveBox.ArchiveBoxName;
                    archiveDto.ArchiveBoxId = x.ArchiveBox.Id;
                }
                return archiveDto;
            }).ToList();

            return new PagedResultDto<ArchiveDto>(totalCount, archiveDtos);
        }

        public async Task<ArchiveDto> UpdateAsync(CreateArchiveDto input)
        {
            //检查标签是否存在
            if (!input.ArchiveBoxRfid.IsNullOrEmpty() && !await _rfidManager.CheckExistRfidCode(input.ArchiveBoxRfid, 2))
            {
                throw new UserFriendlyException("数据库中不存在标签" + input.ArchiveBoxRfid);
            }
            //检查标签是否绑定
            //if (!input.ArchiveBoxRfid.IsNullOrEmpty() && await _archiveBoxManager.CheckUsedBoxRfid(input.ArchiveBoxRfid))
            //{
            //    throw new UserFriendlyException(input.ArchiveBoxRfid + "标签已被绑定");
            //}
            //检查档号不能为空
            if (input.ArchivesCode.IsNullOrEmpty())
            {
                throw new UserFriendlyException("档号不能为空");
            }
            var entity = await _archiveRepository.FindByRfidCodeAsync(input.ArchivesRfid);
            if (entity == null)
                throw new UserFriendlyException(message: "档案盒不存在");
            entity = base.ObjectMapper.Map<CreateArchiveDto, Archive>(input, entity);
            var archivebox = await _archiveRepository.UpdateAsync(entity);

            return base.ObjectMapper.Map<Archive, ArchiveDto>(archivebox);
        }
    }
}
