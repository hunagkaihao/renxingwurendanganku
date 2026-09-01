using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Serilog;
using StackExchange.Redis;
using WarehouseManagement.Checks;
using WarehouseManagement.Plans;
using WarehouseManagement.StockTasks;
using WarehouseManagement.WcsTasks;
using WarehouseManagement.WcsTasks.Dto;

namespace Lion.AbpPro.Jobs
{
    /// <summary>
    /// 后台定时任务Demo
    /// </summary>
    public class ScheduledDemoJob : IHostedService, IDisposable
    {
        private readonly Timer mTimer;
        private const int mDelayTime = 3000;
        private readonly StockTaskManager _stockTaskManager;
        private readonly WcsApiManager _wcsApiManager;
        private readonly CheckAppService _checkAppService;
        private readonly CheckManager _checkManager;
        private readonly PlanManager _planManager;

        public ScheduledDemoJob(StockTaskManager stockTaskManager,WcsApiManager wcsApiManager, CheckAppService checkAppService, CheckManager checkManager, PlanManager planManager)
        {
            mTimer = new Timer(DoWork, null, Timeout.Infinite, Timeout.Infinite);
            _stockTaskManager = stockTaskManager;
            _wcsApiManager = wcsApiManager;
            _checkAppService = checkAppService;
            _checkManager = checkManager;
            _planManager = planManager;
        }

        public void Dispose()
        {
            mTimer.Dispose();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            mTimer.Change(mDelayTime, Timeout.Infinite);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            mTimer.Change(Timeout.Infinite, Timeout.Infinite);
            return Task.CompletedTask;
        }

        private async void DoWork(object? obj)
        {
            //mTimer.Change(Timeout.Infinite, Timeout.Infinite);
            Log.Debug("开始查询未完成的库存、执行中的盘点和执行中的计划任务");
            //Do something
            //空请求体 ky
            while (true)
            {
                try
                {
                    await Task.Delay(2000);
                    var stocks = await _stockTaskManager.GetNoCompleteAsync(); // 获取未完成的库存任务
                    var check = await _checkManager.GetExcetingCheck();          // 获取执行中的盘点任务
                    var plans = await _planManager.GetExcetingPlan();              // 获取执行中的计划任务

                    if (plans.Count > 0)
                    {
                        CheckOrderResultDto checkOrderResultDto = new();
                        checkOrderResultDto.QueryCode = plans[0].HdDefineStr1;
                        var res = await _wcsApiManager.CheckOrderResult(checkOrderResultDto);
                        if (res != null)
                        {
                            for (var i = 0; i < res.Cells.Count; i++)
                            {
                                if (res.Cells[i].PlateCode == "waiting")
                                    continue;
                                //处理盘点结果
                                //await _stockTaskManager.CheckResults(Convert.ToInt32(res.Cells[i].OrderCode), res.Cells[i].PlateCode);
                                var stock = stocks.Find(f => f.Id == Convert.ToInt32(res.Cells[i].OrderCode));
                                if (stock != null)
                                {
                                    //档案入库
                                    await _stockTaskManager.PlanResults(Convert.ToInt32(res.Cells[i].OrderCode), res.Cells[i].PlateCode);
                                }
                            }

                        }
                        //计划任务完成
                        var s = stocks.Find(f => f.ManageTypeCode == ManageType.HPBatchStockIn);
                        if (s == null)
                        {
                            await _planManager.SetAsCompletedAsync(plans[0].Id);
                        }
                    }

                    if (check.Count != 0)
                    {
                        CheckOrderResultDto checkOrderResultDto = new();
                        checkOrderResultDto.QueryCode = check[0].BatchNo;
                        var res = await _wcsApiManager.CheckOrderResult(checkOrderResultDto);
                        if (res != null)
                        {
                            for (var i = 0; i < res.Cells.Count; i++)
                            {
                                if (res.Cells[i].PlateCode != "waiting")
                                {
                                    //处理盘点结果
                                    await _stockTaskManager.CheckResults(Convert.ToInt32(res.Cells[i].OrderCode), res.Cells[i].PlateCode);
                                    var stock = stocks.Find(f => f.Id == Convert.ToInt32(res.Cells[i].OrderCode));
                                    if (stock != null)
                                    {
                                        var flag = 1;
                                        if (res.Cells[i].PlateCode == "empty" && stock.ArchiveBoxRfid == "")
                                        {
                                            flag = 2;
                                        }
                                        else if (res.Cells[i].PlateCode == "empty" && stock.ArchiveBoxRfid != "")
                                        {
                                            flag = 3;
                                        }
                                        else if (res.Cells[i].PlateCode != "empty" && stock.ArchiveBoxRfid == "")
                                        {
                                            flag = 4;
                                        }
                                        else if (res.Cells[i].PlateCode != "empty" && stock.ArchiveBoxRfid != "")
                                        {
                                            flag = 2;
                                        }
                                        //盘点任务完成
                                        await _checkAppService.Complete(Convert.ToInt32(res.Cells[i].OrderCode), flag, res.Cells[i].PlateCode);
                                    }
                                }
                            }
                        }
                    }

                    if (stocks.Count != 0)
                    {
                        var states = await _wcsApiManager.States();
                        if (states == null)
                        {
                            Log.Warning($"WCS 状态为 [{states}]");
                            continue;
                        }

                        if (states.orderStates.Count != 0)
                        {
                            for (int i = 0; i < stocks.Count; i++)
                            {
                                if (stocks[i].ManageTypeCode == ManageType.HpAnnualCheckDown || stocks[i].ManageTypeCode == ManageType.HPBatchStockIn)
                                {
                                    //CheckOrderResultDto checkOrderResultDto = new();
                                    //checkOrderResultDto.QueryCode = stocks[i].ManageRemark;
                                    //var res = await _wcsApiManager.CheckOrderResult(checkOrderResultDto);
                                    //获取执行中的盘点任务
                                    //if (res != null)
                                    //{
                                    //    if (res.Cells[0].PlateCode == "waiting")
                                    //        return;
                                    //    //处理盘点结果
                                    //    await _stockTaskManager.CheckResults(Convert.ToInt32(res.Cells[0].OrderCode), res.Cells[0].PlateCode);
                                    //    var flag = 1;
                                    //    if (res.Cells[0].PlateCode == "empty" && stocks[i].ArchiveBoxRfid == "")
                                    //    {
                                    //        flag = 2;
                                    //    }
                                    //    else if (res.Cells[0].PlateCode == "empty" && stocks[i].ArchiveBoxRfid != "")
                                    //    {
                                    //        flag = 3;
                                    //    }
                                    //    else if (res.Cells[0].PlateCode != "empty" && stocks[i].ArchiveBoxRfid == "")
                                    //    {
                                    //        flag = 4;
                                    //    }
                                    //    else if (res.Cells[0].PlateCode != "empty" && stocks[i].ArchiveBoxRfid != "")
                                    //    {
                                    //        flag = 2;
                                    //    }
                                    //    //盘点任务完成
                                    //    await _checkAppService.CompleteOne(stocks[i].Id, stocks[i].ArchiveBoxRfid, flag, res.Cells[0].PlateCode);
                                    //}
                                }
                                else
                                {
                                    var state = states.orderStates.Find(f => f.OrderCode == Convert.ToString(stocks[i].Id));
                                    if (state != null)
                                    {
                                        await _stockTaskManager.UpdateStatusAsync(stocks[i].Id, state.Status);
                                    }
                                }
                            }
                        }
                    }
                    
                    //mTimer.Change(mDelayTime, Timeout.Infinite);
                }
                catch (Exception ex)
                {
                    Log.Debug(ex.ToString());
                }
            }
        }
    }
}

