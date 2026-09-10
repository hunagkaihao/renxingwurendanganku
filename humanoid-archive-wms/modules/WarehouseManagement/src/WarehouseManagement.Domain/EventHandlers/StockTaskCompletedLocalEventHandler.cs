using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus;
using Volo.Abp.EventBus.Distributed;

using WarehouseManagement.Plans;
using WarehouseManagement.StockTasks;
using WarehouseManagement.StockTasks.Aggregates;
using TaskStatus = WarehouseManagement.StockTasks.TaskStatus;

namespace WarehouseManagement.EventHandlers
{
    public class StockTaskCompletedLocalEventHandler : ILocalEventHandler<StockTaskCompletedEto>,
         ITransientDependency
    {
        private readonly IDistributedEventBus _eventBus;
        private readonly StockTaskManager _stockTaskManager;
        private readonly PlanManager _planManager;

        

        public StockTaskCompletedLocalEventHandler(IDistributedEventBus eventBus,StockTaskManager stockTaskManager
            , PlanManager planManager)
        {
            _eventBus = eventBus;
            _stockTaskManager = stockTaskManager;
            _planManager= planManager;
        }

        public async Task HandleEventAsync(StockTaskCompletedEto eventData)
        {
            var stockTask = await _stockTaskManager.FindByIdAsync(eventData.StockTaskId);
            if (eventData.ManageStatus == TaskStatus.WaitingExecute.ToString())
            {
                await _planManager.UpdateExcuteQtyAsync(stockTask.Details);
            }
            else if (eventData.ManageStatus == TaskStatus.Complete.ToString())
            {
                await _planManager.UpdateCompleteQtyAsync(stockTask.Details);

            }
            else if (eventData.ManageStatus == TaskStatus.Cancel.ToString())
            {
                await _planManager.UpdateCancelQtyAsync(stockTask.Details);
            }
            //var stockTask = await _stockTaskManager.FindByIdAsync(eventData.StockTaskId);
            if (eventData.TaskTypeCode == StockTasks.TaskType.NPFullStockIn)
            {
                //await _storageBoxManager.UpdateStockCellAsync(eventData.StockBarcode, eventData.EndCellId);
            }
            if (eventData.TaskTypeCode == TaskType.NpFullStockOut
                || eventData.TaskTypeCode == TaskType.NPSortStockOut
                || eventData.TaskTypeCode == TaskType.EmptyStockOut
                || eventData.TaskTypeCode == TaskType.HpAnnualCheckDown)
            {
                //await _storageBoxManager.UpdateOutStockCellAsync(eventData.StockBarcode, eventData.StartCellId, stockTask.Details);
            }
            //Console.WriteLine(eventData.StockTaskId);
            //throw new NotImplementedException();
        }
        //public async Task HandleEventAsync(PaymentRequestCompletedEto eventData)
        //{
        //    if (!int.TryParse(eventData.OrderId, out var orderId))
        //    {
        //        throw new BusinessException(OrderingServiceErrorCodes.OrderIdIdNotint);
        //    }

        //    var acceptedOrder = await _orderManager.AcceptOrderAsync(
        //        orderId, eventData.PaymentRequestId, eventData.State.ToString()
        //    );

        //    await _eventBus.PublishAsync(new OrderAcceptedEto
        //    {
        //        Items = eventData.Products.Select(MapProductToOrderItem).ToList(),
        //        PaymentStatus = acceptedOrder.PaymentStatus,
        //        Buyer = new BuyerEto
        //        {
        //            BuyerId = acceptedOrder.Buyer.Id,
        //            BuyerEmail = acceptedOrder.Buyer.Email,
        //            BuyerName = acceptedOrder.Buyer.Name
        //        },
        //        OrderId = acceptedOrder.Id
        //    });
        //}

        //private static OrderItemEto MapProductToOrderItem(PaymentRequestProductEto arg)
        //{
        //    return new OrderItemEto
        //    {
        //        Units = arg.Quantity,
        //        ProductId = int.Parse(arg.ReferenceId)
        //    };
        //}


    }
}
