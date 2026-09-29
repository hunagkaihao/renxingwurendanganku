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
using WarehouseManagement.Material;
using WarehouseManagement.MaterialBoxs;
using TaskStatus = WarehouseManagement.StockTasks.TaskStatus;

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
        private readonly IMaterialRepository _archiveRepository;
        private readonly IMaterialBoxRepository _materialBoxRepository;
        public TaskHisAppService(ITaskHisRepository taskHisRepository, TaskHisManager taskHisManagement,
            ITaskHisDetailRepository taskHisDetailRepository, IGoodsRepository goodsRepository,
            IStockTaskRepository stockTaskRepository, ICellRepository cellRepository, IMaterialRepository archiveRepository,
            IMaterialBoxRepository materialBoxRepository)
        {
            _taskHisRepository = taskHisRepository;
            _taskHisManagement = taskHisManagement;
            _taskHisDetailRepository = taskHisDetailRepository;
            _goodsRepository = goodsRepository;
            _stockTaskRepository = stockTaskRepository;
            _cellRepository = cellRepository;
            _archiveRepository = archiveRepository;
            _materialBoxRepository = materialBoxRepository;
        }
        //[Authorize(WarehouseManagementPermissions.TaskHisManagement.Create)]
        //public async Task<TaskHisDto> CreateAsync(CreateTaskHisDto input)
        //{
        //    var stockTaskObj = await _stockTaskRepository.FindByIdAsync(input.StockTaskId);
        //    var taskHis = await _taskHisManagement.CreateAsync(stockTaskObj, stockTaskObj.Details);
        //    return  base.ObjectMapper.Map<TaskHis, TaskHisDto>(taskHis);
        //}
       
        /// <summary>
        /// 获取页列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PagedResultDto<TaskHisDto>> GetPagingListAsync(PagingTaskHisListInput input)
        {
            // 获取任务历史表
            var queryable = await _taskHisRepository.GetQueryableAsync();

            // 查询符合条件的数据
            var query = from taskHis in queryable
                        where taskHis.CreationTime >= input.StartCreationTime & 
                              taskHis.CreationTime <= input.EndCreationTime  & 
                              taskHis.MaterialBarcode.Contains(input.Filter.IsNullOrEmpty() ? "" : input.Filter.Trim()) & 
                              (input.TaskStatus == "All" ? 1 == 1 : taskHis.TaskStatus == Enum.Parse<TaskStatus>(input.TaskStatus))
                              orderby taskHis.CreationTime descending
                              select new { taskHis};

            // 降序排序
            query = query
                    .OrderByDescending(f => f.taskHis.Id)
                    .Skip(input.SkipCount)
                    .Take(input.PageSize);

            var materialBoxQueryable = await _materialBoxRepository.GetQueryableAsync();
            var materialQueryable = await _archiveRepository.GetQueryableAsync();
            // 分页后关联基础物料编码，保留无法关联物料的历史记录。
            var queryResult = await AsyncExecuter.ToListAsync(query.Select(x => new
            {
                x.taskHis,
                materialCode = (from box in materialBoxQueryable
                                from detail in box.Details
                                join material in materialQueryable on detail.MaterialId equals material.Id
                                where box.MaterialBoxBarcode == x.taskHis.MaterialBarcode && !detail.IsDeleted
                                orderby box.Id, detail.Id
                                select material.MaterialCode).FirstOrDefault()
                    // 兼容旧记录直接使用基础编码作条码的情况，不截取条码猜测编码。
                    ?? materialQueryable.Where(material => material.MaterialCode == x.taskHis.MaterialBarcode)
                        .Select(material => material.MaterialCode).FirstOrDefault()
            }));

            // 返回对象数据
            var taskHisDtos = queryResult.Select(x =>
            {
                var taskHisDtos = ObjectMapper.Map<TaskHis, TaskHisDto>(x.taskHis);
                taskHisDtos.MaterialCode = x.materialCode;
                return taskHisDtos;
            }).ToList();

            // 统计总数
            var totalCount = queryResult.Count()+ input.SkipCount;

            return new PagedResultDto<TaskHisDto>(totalCount, taskHisDtos);
        }

        public async Task<PagedResultDto<TaskHisDetailDto>> GetPagingDetailListAsync(
    PagingTaskHisDetailInput input)
        {
            var taskHisQueryable = await _taskHisRepository.GetQueryableAsync();
            var materialBoxQueryable = await _materialBoxRepository.GetQueryableAsync();
            var materialQueryable = await _archiveRepository.GetQueryableAsync();
            var query = from taskHis in taskHisQueryable
                        join materialBox in materialBoxQueryable
                            on taskHis.MaterialBarcode equals materialBox.MaterialBoxBarcode
                        where taskHis.Id == input.TaskHisId
                        select new
                        {
                            taskHis,
                            materialBox,
                            // 实物条码与基础物料编码不同，优先通过现有物料明细关系取编码。
                            materialCode = (from detail in materialBox.Details
                                            join material in materialQueryable on detail.MaterialId equals material.Id
                                            where !detail.IsDeleted
                                            orderby detail.Id
                                            select material.MaterialCode).FirstOrDefault()
                                // 兼容旧预约直接使用基础编码作条码的记录，不截取或猜测条码。
                                ?? materialQueryable
                                    .Where(material => material.MaterialCode == materialBox.MaterialBoxBarcode)
                                    .Select(material => material.MaterialCode)
                                    .FirstOrDefault()
                        };

            var queryResult = await AsyncExecuter.ToListAsync(query);
            var taskHisDetailDtos = queryResult.Select(x => new TaskHisDetailDto
            {
                Id = x.taskHis.Id,
                StockBarcode = x.taskHis.MaterialBarcode,
                GoodsCode = x.materialCode,
                GoodsName = x.materialBox.MaterialBoxName,
                GoodsSpec = x.materialBox.CellModel,
                GoodsUnits = x.materialBox.MaterialUnit,
                GoodsBand = x.materialBox.RetentionPeriod,
                GoodsBatchNo = x.materialBox.MaterialPeople,
                CreationTime = x.materialBox.CreationTime
            }).ToList();

            var totalCount = taskHisDetailDtos.Count;

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
