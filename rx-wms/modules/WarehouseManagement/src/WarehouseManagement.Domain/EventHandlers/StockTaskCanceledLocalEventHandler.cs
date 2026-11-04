using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus;
using Volo.Abp.EventBus.Distributed;
using WarehouseManagement.StockTasks;
using WarehouseManagement.StockTasks.Aggregates;

namespace WarehouseManagement.EventHandlers
{
    public class StockTaskCanceledLocalEventHandler : ILocalEventHandler<StockTaskCanceledEto>,
         ITransientDependency
    {
        private readonly IDistributedEventBus _eventBus;
        private readonly StockTaskManager _stockTaskManager;

        public StockTaskCanceledLocalEventHandler(IDistributedEventBus eventBus,StockTaskManager stockTaskManager)
        {
            _eventBus = eventBus;
            _stockTaskManager = stockTaskManager;
        }

        public async Task HandleEventAsync(StockTaskCanceledEto eventData)
        {
           
        }
        


    }
}
