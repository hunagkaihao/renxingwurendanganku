using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using WarehouseManagement.ArchiveBoxs.Aggregates;
using WarehouseManagement.ArchiveBoxs.Dto;
using WarehouseManagement.Archives;
using WarehouseManagement.Cells;
using WarehouseManagement.RfidCodes;

namespace WarehouseManagement.ArchiveBoxs
{
    public class ArchiveBoxAppService : WarehouseManagementAppService, IArchiveBoxAppService
    {
        private readonly ArchiveBoxManager _archiveBoxManager;
        private readonly IArchiveBoxRepository _archiveBoxRepository;
        private readonly ICellRepository _cellRepository;
        private readonly RfidCodeManager _rfidManager;
        private readonly ArchiveManager _archiveManager;
        private readonly ArchiveBoxDetailManager _archiveBoxDetailManager;

        public ArchiveBoxAppService(ArchiveBoxManager archiveBoxManager
            , IArchiveBoxRepository archiveBoxRepository    
            , RfidCodeManager rfidManager
            , ArchiveManager archiveManager
            , ArchiveBoxDetailManager archiveBoxDetailManager
            , ICellRepository cellRepository
        )
        {
            _archiveBoxManager = archiveBoxManager;
            _archiveBoxRepository = archiveBoxRepository;
            _rfidManager = rfidManager;
            _archiveManager = archiveManager;
            _archiveBoxDetailManager = archiveBoxDetailManager;
            _cellRepository = cellRepository;
        }

        public async Task<ArchiveBoxDto> CreateAsync(CreateArchiveBoxDto createArchiveBox)
        {
            //检查标签是否存在
            if (!createArchiveBox.ArchiveBoxRfid.IsNullOrEmpty() && !await _rfidManager.CheckExistRfidCode(createArchiveBox.ArchiveBoxRfid, 2))
            {
                throw new UserFriendlyException("数据库中不存在标签" + createArchiveBox.ArchiveBoxRfid);
            }
            //检查标签是否绑定
            if (!createArchiveBox.ArchiveBoxRfid.IsNullOrEmpty() && await _archiveBoxManager.CheckUsedBoxRfid(createArchiveBox.ArchiveBoxRfid))
            {
                throw new UserFriendlyException(createArchiveBox.ArchiveBoxRfid + "标签已被绑定");
            }
            //检查档号不能为空
            if (createArchiveBox.StockBarcode.IsNullOrEmpty())
            {
                throw new UserFriendlyException("档号不能为空");
            }
            //检查档案盒尺寸不能为空
            if (createArchiveBox.CellModel.IsNullOrEmpty())
            {
                throw new UserFriendlyException("尺寸不能为空");
            }
            var entity = base.ObjectMapper.Map<CreateArchiveBoxDto, ArchiveBox>(createArchiveBox);
            
            var archivebox = await _archiveBoxRepository.InsertAsync(entity);
            return base.ObjectMapper.Map<ArchiveBox, ArchiveBoxDto>(archivebox);
        }

        public async Task DeleteAsync(CreateArchiveBoxDto input)
        {
            await _archiveBoxManager.DeleteAsync(input.Id);
        }
        public async Task<ArchiveBoxDto> UpdateAsync(CreateArchiveBoxDto input)
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
            if (input.StockBarcode.IsNullOrEmpty())
            {
                throw new UserFriendlyException("档号不能为空");
            }
            //检查档案盒尺寸不能为空
            if (input.CellModel.IsNullOrEmpty())
            {
                throw new UserFriendlyException("档号不能为空");
            }
            var entity = await _archiveBoxRepository.FindByIdAsync(input.Id);
            if (entity == null)
                throw new UserFriendlyException(message: "档案盒不存在");
            entity = base.ObjectMapper.Map<CreateArchiveBoxDto, ArchiveBox>(input,entity);

            var archivebox = await _archiveBoxRepository.UpdateAsync(entity);

            return base.ObjectMapper.Map<ArchiveBox, ArchiveBoxDto>(archivebox);
        }
        
        public async Task<PagedResultDto<ArchiveBoxDto>> PageAsync(PagingArchiveBoxListInput input)
        {
            var queryable = await _archiveBoxRepository.GetQueryableAsync();

            //Prepare a query to join books and authors
            var query = from archiveBox in queryable
                        join celltemp in await _cellRepository.GetQueryableAsync() on archiveBox.CellId equals celltemp.Id into sc
                        from cell in sc.DefaultIfEmpty()
                        where archiveBox.ArchiveBoxName.Contains(input.Filter.IsNullOrEmpty() ? "" : input.Filter.Trim())
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
                var archiveBoxDtos = ObjectMapper.Map<ArchiveBox, ArchiveBoxDto>(x.archiveBox);
                archiveBoxDtos.CellCode = x.cell?.CellCode;

                return archiveBoxDtos;
            }).Take(input.PageSize).ToList();

            var totalCount = queryResult.Count() + input.SkipCount;

            return new PagedResultDto<ArchiveBoxDto>(
                totalCount,
                archiveBoxDtos
            );
        }
        public async Task<PagedResultDto<ArchiveBoxDetailDto>> DetailAsync(PagingArchiveBoxDetailInput input)
        {
            return await _archiveBoxDetailManager.GetDetailAsync(input);
        }
        public async Task<ArchiveBoxDto> BindRfid(CreateArchiveBoxDto input)
        {
            try
            {
                var entity =await _archiveBoxRepository.FindByIdAsync(input.Id);
                //检查标签是否存在
                if (!input.ArchiveBoxRfid.IsNullOrEmpty() && !await _rfidManager.CheckExistRfidCode(input.ArchiveBoxRfid, 2))
                {
                    throw new UserFriendlyException("数据库中不存在标签" + input.ArchiveBoxRfid);
                }
                //检测标签是否被绑定
                if (!input.ArchiveBoxRfid.IsNullOrEmpty() && await _archiveBoxManager.CheckUsedBoxRfid(input.ArchiveBoxRfid))
                {
                    throw new UserFriendlyException( input.ArchiveBoxRfid + "标签已被绑定");
                }
                entity.ArchiveBoxRfid = input.ArchiveBoxRfid;
                var archivebox = await _archiveBoxRepository.UpdateAsync(entity);

                return base.ObjectMapper.Map<ArchiveBox, ArchiveBoxDto>(archivebox);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message.ToString());
            }
        }

        public async Task<Boolean> BindArchive(string archiveBoxRfid,string archiveRfid)
        {
            var archiveBox = await _archiveBoxManager.GetArchiveBoxByRfidCode(archiveBoxRfid);
            if (archiveBox == null)
            {
                throw new UserFriendlyException(message: "档案盒不存在");
            }
            var archive = await _archiveManager.GetArchiveByRfidCode(archiveRfid);
            if (archive == null)
            {
                throw new UserFriendlyException("档案文件不存在");
            }
            //检查档案是否已绑定
            var detail = await _archiveBoxDetailManager.GetDetailByArchiveId(archive.Id);
            if (detail != null)
            {
                throw new UserFriendlyException("档案文件已经绑定在档案盒" + detail.ArchiveBoxId);
            }

            archiveBox.AddDetail(archiveBox.Id, archive.Id);
            await _archiveBoxRepository.UpdateAsync(archiveBox);
            return true;

        }

    }
}
