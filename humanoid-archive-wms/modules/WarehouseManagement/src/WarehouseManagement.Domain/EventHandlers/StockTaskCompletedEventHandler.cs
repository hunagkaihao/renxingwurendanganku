using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using WarehouseManagement.StockTasks.Aggregates;

namespace WarehouseManagement.EventHandlers
{
    public class StockTaskCompletedEventHandler : IDistributedEventHandler<StockTaskCompletedEto>,
         ITransientDependency
    {
        private readonly IDistributedEventBus _eventBus;

        public StockTaskCompletedEventHandler(IDistributedEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        public async Task HandleEventAsync(StockTaskCompletedEto eventData)
        {
            Console.WriteLine(eventData.StockTaskId);
            //throw new NotImplementedException();
        }


    }
}
