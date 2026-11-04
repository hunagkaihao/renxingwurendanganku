using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Uow;
using WarehouseManagement.Cells;
using WarehouseManagement.StockTasks;
using WarehouseManagement.StockTasks.Aggregates;
using WarehouseManagement.TaskHiss;

namespace WarehouseManagement.EventHandlers
{
    public class StockTaskCanceledHisLocalEventHandler : ILocalEventHandler<StockTaskCanceledEto>,
         ITransientDependency
    {
        private readonly IDistributedEventBus _eventBus;
        private readonly StockTaskManager _stockTaskManager;
        private readonly TaskHisManager _taskHisManager;
        private readonly CellManager _cellManager;

        public StockTaskCanceledHisLocalEventHandler(IDistributedEventBus eventBus, StockTaskManager stockTaskManager,
            TaskHisManager taskHisManager,CellManager cellManager)
        {
            _eventBus = eventBus;
            _stockTaskManager = stockTaskManager;
            _taskHisManager= taskHisManager;
            _cellManager = cellManager;
        }
        [UnitOfWork]
        public async Task HandleEventAsync(StockTaskCanceledEto eventData)
        {
            var stockTask = await _stockTaskManager.FindByIdAsync(eventData.StockTaskId);
            //解锁库位
            if(stockTask.StartCellId != null & stockTask.StartCellId != 0) { await _cellManager.SetAsEnableAsync((int)stockTask.StartCellId); }

            if (stockTask.EndCellCode != null & stockTask.StartCellId != 0) { await _cellManager.SetAsEnableAsync((int)stockTask.EndCellId); }

            //创建历史记录
            await _taskHisManager.CreateAsync(stockTask, stockTask.Details);

            //删除管理任务
            await _stockTaskManager.DeleteAsync(eventData.StockTaskId);


        }
    }
}
