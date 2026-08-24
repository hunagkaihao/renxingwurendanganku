using WarehouseManagement.TaskHiss.Dto;
using WarehouseManagement.Permissions;
using Lion.AbpPro.Extension.Customs.Dtos;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;
using WarehouseManagement.TaskHiss.Aggregates;
using Lion.AbpPro.Extension.System;
using System.Linq;
using WarehouseManagement.Goodss;
using WarehouseManagement.Cells;
using WarehouseManagement.StockTasks;
using WarehouseManagement.Archives;

namespace WarehouseManagement.TaskHiss
{
    //[Authorize(WarehouseManagementPermissions.TaskHisManagement.Default)]
    public class TaskHisAppService : WarehouseManagementAppService,
         ITaskHisAppService //implement the ITaskHisAppService
    {
        //private readonly IRepository<TaskHis, Guid> _taskHisRepository;
        /// <summary>
        ///  注意 为了快速直接注入仓库层 规范上是不允许的
        ///  这里注入仓储也只是为了查询分页
        ///  如果是其他的操作全部通过对应manger进行操作
        /// </summary>
        private readonly ITaskHisRepository _taskHisRepository;
        private readonly TaskHisManager _taskHisManagement;
        private readonly ITaskHisDetailRepository _taskHisDetailRepository;
        private readonly IGoodsRepository _goodsRepository;
        private readonly IStockTaskRepository _stockTaskRepository;
        private readonly ICellRepository _cellRepository;
        private readonly IArchiveRepository _archiveRepository;
        public TaskHisAppService(ITaskHisRepository taskHisRepository, TaskHisManager taskHisManagement,
            ITaskHisDetailRepository taskHisDetailRepository, IGoodsRepository goodsRepository,
            IStockTaskRepository stockTaskRepository, ICellRepository cellRepository, IArchiveRepository archiveRepository)
        {
            _taskHisRepository = taskHisRepository;
            _taskHisManagement = taskHisManagement;
            _taskHisDetailRepository = taskHisDetailRepository;
            _goodsRepository = goodsRepository;
            _stockTaskRepository = stockTaskRepository;
            _cellRepository = cellRepository;
            _archiveRepository = archiveRepository;
        }
        //[Authorize(WarehouseManagementPermissions.TaskHisManagement.Create)]
        //public async Task<TaskHisDto> CreateAsync(CreateTaskHisDto input)
        //{
        //    var stockTaskObj = await _stockTaskRepository.FindByIdAsync(input.StockTaskId);
        //    var taskHis = await _taskHisManagement.CreateAsync(stockTaskObj, stockTaskObj.Details);
        //    return  base.ObjectMapper.Map<TaskHis, TaskHisDto>(taskHis);
        //}
       

        public async Task<PagedResultDto<TaskHisDto>> GetPagingListAsync(PagingTaskHisListInput input)
        {


            //Get the IQueryable<Book> from the repository
            var queryable = await _taskHisRepository.GetQueryableAsync();

            //Prepare a query to join books and authors
            var query = from taskHis in queryable
                        //join scelltemp in await _cellRepository.GetQueryableAsync() on taskHis.StartCellId equals scelltemp.Id into sc
                        //from scell in sc.DefaultIfEmpty()
                        //join ecelltemp in await _cellRepository.GetQueryableAsync() on taskHis.EndCellId equals ecelltemp.Id into ec
                        //from ecell in ec.DefaultIfEmpty()
                        where taskHis.CreationTime >= input.StartCreationTime & taskHis.CreationTime <= input.EndCreationTime 
                        & taskHis.StockBarcode.Contains(input.Filter.IsNullOrEmpty() ? "" : input.Filter.Trim())
                        & (input.ManageStatus == "All" ? 1 == 1 : taskHis.ManageStatus == Enum.Parse<ManageStatus>(input.ManageStatus))
                        orderby taskHis.CreationTime descending
                        select new { taskHis};

            //Paging
            query = query
                //.OrderBy(NormalizeSorting(input.Sorting))
                .OrderByDescending(f => f.taskHis.Id)
                .Skip(input.SkipCount)
                .Take(1000);
            //.Take(input.MaxResultCount);

            //Execute the query and get a list
            var queryResult = await AsyncExecuter.ToListAsync(query);

            //Convert the query result to a list of BookDto objects
            var taskHisDtos = queryResult.Select(x =>
            {
                var taskHisDtos = ObjectMapper.Map<TaskHis, TaskHisDto>(x.taskHis);
                //taskHisDtos.StartCellCode = x.scell?.CellCode;
                //taskHisDtos.EndCellCode = x.ecell?.CellCode;
                return taskHisDtos;
            }).ToList();

            //Get the total count with another query
            //var totalCount = await _taskHisDetailRepository.GetCountAsync();
            var totalCount = queryResult.Count()+ input.SkipCount;

            return new PagedResultDto<TaskHisDto>(
                totalCount,
                taskHisDtos
            );
        }

        public async Task<PagedResultDto<TaskHisDetailDto>> GetPagingDetailListAsync(
    PagingTaskHisDetailInput input)
        {
            //Get the IQueryable<Book> from the repository
            var queryable = await _taskHisDetailRepository.GetQueryableAsync();

            //Prepare a query to join books and authors
            var query = from taskHisDetail in queryable
                        join archive in await _archiveRepository.GetQueryableAsync() on taskHisDetail.GoodsId equals archive.Id
                        join taskHis in await _taskHisRepository.GetQueryableAsync() on taskHisDetail.TaskHisId equals taskHis.Id
                        where taskHisDetail.TaskHisId == input.TaskHisId
                        select new { taskHisDetail, archive, taskHis };

            //Paging
            query = query
                //.OrderBy(NormalizeSorting(input.Sorting))
                .OrderBy(f => f.taskHisDetail.Id)
                .Skip(input.SkipCount)
                .Take(1000);
            //.Take(input.MaxResultCount);

            //Execute the query and get a list
            var queryResult = await AsyncExecuter.ToListAsync(query);

            //Convert the query result to a list of BookDto objects
            var taskHisDetailDtos = queryResult.Select(x =>
            {
                var taskHisDetailDtos = ObjectMapper.Map<TaskHisDetail, TaskHisDetailDto>(x.taskHisDetail);
                taskHisDetailDtos.StockBarcode = x.taskHis.StockBarcode;
                taskHisDetailDtos.GoodsCode = x.archive.ArchivesCode;
                taskHisDetailDtos.GoodsName = x.archive.ArchivesName;
                taskHisDetailDtos.GoodsSpec = x.archive.GoodsSpec;
                taskHisDetailDtos.Quantity = x.taskHisDetail.TaskHisDetailQuantity;                
                return taskHisDetailDtos;
            }).ToList();

            //Get the total count with another query
            //var totalCount = await _taskHisDetailRepository.GetCountAsync();
            var totalCount = queryResult.Count();

            return new PagedResultDto<TaskHisDetailDto>(
                totalCount,
                taskHisDetailDtos
            );
        }
        

        /// <summary>
        /// 删除用户
        /// </summary>
        //[Authorize(WarehouseManagementPermissions.TaskHisManagement.Delete)]
        //public virtual async Task DeleteAsync(IdIntInput input)
        //{
        //    await _taskHisManagement.DeleteAsync(input.Id);
        //    //await _taskHisRepository.DeleteAsync(input.Id);
        //}
       

    }
}
