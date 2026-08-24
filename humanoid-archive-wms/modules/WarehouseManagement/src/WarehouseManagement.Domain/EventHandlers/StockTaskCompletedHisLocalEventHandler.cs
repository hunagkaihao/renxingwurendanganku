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
using WarehouseManagement.TaskHiss;

namespace WarehouseManagement.EventHandlers
{
    public class StockTaskCompletedHisLocalEventHandler : ILocalEventHandler<StockTaskCompletedEto>,
         ITransientDependency
    {
        private readonly IDistributedEventBus _eventBus;
        private readonly StockTaskManager _stockTaskManager;
        private readonly TaskHisManager _taskHisManager;

        public StockTaskCompletedHisLocalEventHandler(IDistributedEventBus eventBus, StockTaskManager stockTaskManager,
            TaskHisManager taskHisManager)
        {
            _eventBus = eventBus;
            _stockTaskManager = stockTaskManager;
            _taskHisManager= taskHisManager;

        }
        
        public async Task HandleEventAsync(StockTaskCompletedEto eventData)
        {
            if (eventData.ManageStatus == ManageStatus.Complete.ToString())
            {
                var stockTask = await _stockTaskManager.FindByIdAsync(eventData.StockTaskId);

                //创建历史记录
                await _taskHisManager.CreateAsync(stockTask, stockTask.Details);
                //删除管理任务
                await _stockTaskManager.DeleteAsync(eventData.StockTaskId);
            }

            //Console.WriteLine(eventData.StockTaskId);
            //_taskHisManager.
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
