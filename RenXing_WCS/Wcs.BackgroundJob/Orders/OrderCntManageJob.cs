using Wcs.ConfigTool;
using Wcs.Dispatch;
using Wcs.Orders.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Wcs.Orders;

/// <summary>
/// 后台定时清理调度订单数量任务
/// </summary>
public class OrderCntManageJob : IHostedService, IDisposable
{
    private readonly Timer mTimer;
    private readonly int mDelayTime;

    private readonly IOptions<ConfigOptions> _options;
    private readonly OrderManager _orderManager;

    public OrderCntManageJob(
        IOptions<ConfigOptions> options,
        OrderManager orderManager)
    {
        _options = options;
        _orderManager = orderManager;
        mDelayTime = _options.Value.DispatchOrderCntMngInterval;
        mTimer = new Timer(DoWork, null, Timeout.Infinite, Timeout.Infinite);
    }

    public void Dispose()
    {
        mTimer.Dispose();
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        mTimer.Change(5000, Timeout.Infinite); //开始任务时清理一次
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        mTimer.Change(Timeout.Infinite, Timeout.Infinite);
        return Task.CompletedTask;
    }

    private void DoWork(object obj)
    {
        mTimer.Change(Timeout.Infinite, Timeout.Infinite);

        //Do something
        int holdTime = _options.Value.DispatchOrderRecordHoldTime;
        DateTime ago = DateTime.Now.AddDays(-holdTime);

        Task<List<DispatchOrder>> ordersAgoTask = _orderManager.GetDispatchOrdersBeforeTimeAsync(ago);
        Task<List<DispatchChkOrderRslt>> allChkRsltsTask = _orderManager.GetAllChkOrderResultsAsync();

        List<DispatchOrder> ordersAgo = ordersAgoTask.GetAwaiter().GetResult();
        List<DispatchChkOrderRslt> allChkRslts = allChkRsltsTask.GetAwaiter().GetResult();

        if (ordersAgo != null && ordersAgo.Count > 0)
        {
            //盘点结果字典
            Dictionary<string, List<DispatchChkOrderRslt>> chkRsltPairs = new Dictionary<string, List<DispatchChkOrderRslt>>();
            foreach (var r in allChkRslts)
            {
                if (!chkRsltPairs.Keys.Contains(r.OrderCode))
                    chkRsltPairs.Add(r.OrderCode, new List<DispatchChkOrderRslt>());
                chkRsltPairs[r.OrderCode].Add(r);
            }

            List<DispatchOrder> ordersToDel = new List<DispatchOrder>();
            List<DispatchChkOrderRslt> rsltsToDel = new List<DispatchChkOrderRslt>();

            for (int i = 0; i < ordersAgo.Count; i++)
            {
                if (ordersAgo[i].State != EnumDispatchOrderState.Created &&
                    ordersAgo[i].State != EnumDispatchOrderState.Doing) //没完成的不删
                {
                    ordersToDel.Add(ordersAgo[i]);

                    if (chkRsltPairs.Keys.Contains(ordersAgo[i].OrderCode))  //盘点结果集合中包含此订单，删
                        rsltsToDel.AddRange(chkRsltPairs[ordersAgo[i].OrderCode]);
                }
            }
            Task<bool> tsk1 = _orderManager.RemoveDispatchOrdersAsync(ordersToDel);
            Task<bool> tsk2 = _orderManager.DelChkOrderRsltsAsync(rsltsToDel);
            tsk1.Wait();
            tsk2.Wait();
        }

        mTimer.Change(mDelayTime, Timeout.Infinite);
    }
}