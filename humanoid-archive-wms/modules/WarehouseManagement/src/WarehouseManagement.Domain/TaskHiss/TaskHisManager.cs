using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using WarehouseManagement.StockTasks;
using WarehouseManagement.StockTasks.Aggregates;
using WarehouseManagement.TaskHiss.Aggregates;

namespace WarehouseManagement.TaskHiss
{
    public class TaskHisManager : TaskHisDomainService
    {
        private readonly ITaskHisRepository _taskHisRepository;
        //private readonly IDistributedCache<TaskHis> _cache;//设置缓存

        //    public TaskHisManager(
        //ITaskHisRepository TaskHisRepository,
        //IDistributedCache<TaskHisDto> cache)
        //    {
        //        _TaskHisRepository = TaskHisRepository;
        //        _cache = cache;
        //    }

        public TaskHisManager(
            ITaskHisRepository taskHisRepository)
        {
            _taskHisRepository = taskHisRepository;
        }

        /// <summary>
        /// 创建字典类型
        /// </summary>
        /// <param name="code"></param>
        /// <param name="displayText"></param>
        /// <param name="description"></param>
        public Task<TaskHis> CreateAsync(StockTask stockTask,List<StockTaskDetail> stockTaskDetails)
        {
            var entity = new TaskHis(stockTask, stockTaskDetails);
            //foreach (var item in storageBoxDetails)
            //{
            //    entity.AddDetail(item.Id,item.GoodsId, item.Quantity, null);
            //}
            return _taskHisRepository.InsertAsync(entity);
        }
        

        public async Task DeleteAsync(int taskHisId)
        {
            var entity = await _taskHisRepository.FindByIdAsync(taskHisId);
            if (entity == null)
                throw new UserFriendlyException(message: "任务不存在");
            await _taskHisRepository.DeleteAsync(entity);
        }
        public async Task<TaskHis> FindByIdAsync(int taskHisId)
        {
            var entity = await _taskHisRepository.FindByIdAsync(taskHisId);
            if (entity == null)
                throw new UserFriendlyException(message: "任务不存在");
            return entity;
        }

        public async Task<TaskHis> FindByTaskIdAsync(int taskId)
        {
            var entity = await _taskHisRepository.GetListAsync(f=>f.StockTaskId== taskId);
            if (entity.Count==0)
                throw new UserFriendlyException(message: "任务不存在");
            return entity.FirstOrDefault();
        }
        //获取七日出入库数据
        public async Task<List<TaskHis>> GetSevenDayHisAsync()
        {
            var entity = await _taskHisRepository.GetListAsync(x => x.ManageStatus == ManageStatus.Complete & (x.ManageTypeCode == ManageType.NPFullStockIn|| x.ManageTypeCode == ManageType.NPSortStockOut || x.ManageTypeCode == ManageType.HPSortStockOut));
            return entity;
        }
    }
}
