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

        /// <summary>
        /// 将 WCS 现场扫描事实与 WMS 下发时冻结的账面条码做精确比较。
        /// Flag 沿用现有盘点历史逻辑：2=一致、3=盘亏、4=盘盈、5=错位、6=扫描异常。
        /// WCS 只提供实际扫描事实，盘盈盘亏等业务结论必须由 WMS 在这里生成。
        /// </summary>
        private static (bool CanComplete, int Flag, string Remark) CompareCheckResult(
            string expectedPlateCode,
            Cells actualResult)
        {
            string expected = expectedPlateCode?.Trim() ?? string.Empty;
            string actual = actualResult.PlateCode?.Trim() ?? string.Empty;

            switch (actualResult.Status)
            {
                case WcsCheckCellStatus.Unknown:
                case WcsCheckCellStatus.Waiting:
                case WcsCheckCellStatus.Scanning:
                    // Unknown 可能来自旧版响应、字段缺失或短暂序列化异常，不能第一次出现就结束盘点。
                    // 与 Waiting、Scanning 一样保留执行明细，等待下一轮查询或超时告警处理。
                    return (false, 0, $"等待现场扫描形成最终结果，当前状态：{actualResult.Status}");

                case WcsCheckCellStatus.Empty:
                    return string.IsNullOrEmpty(expected)
                        ? (true, 2, "盘点一致：账面为空，现场扫描也为空")
                        : (true, 3, $"盘亏：账面档案盒为{expected}，现场扫描为空");

                case WcsCheckCellStatus.Scanned:
                    if (string.IsNullOrEmpty(actual))
                        return (true, 6, "扫描异常：WCS标记扫描成功但未返回实际条码");

                    if (string.IsNullOrEmpty(expected))
                        return (true, 4, $"盘盈：账面为空，现场扫描到档案盒{actual}");

                    if (string.Equals(expected, actual, StringComparison.Ordinal))
                        return (true, 2, $"盘点一致：账面与现场均为档案盒{actual}");

                    return (true, 5, $"错位：账面档案盒为{expected}，现场扫描为{actual}");

                case WcsCheckCellStatus.ScanError:
                    return (true, 6, "扫码异常：二维码未能识别");

                case WcsCheckCellStatus.DeviceError:
                    return (true, 6, "设备异常：机械定位、通讯或扫码设备执行失败");

                default:
                    return (true, 6, $"扫描异常：无法识别的现场状态{actualResult.Status}");
            }
        }

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
                                try
                                {
                                    Cells actualResult = res.Cells[i];

                                    // WCS 的扫描段 OrderCode 不再等于单个 StockTask.Id，
                                    // 因此使用“当前盘点计划 + 实际库位码”定位 WMS 冻结的单库位快照任务。
                                    // PlanId 条件用于隔离不同批次，避免历史未清理任务中存在相同库位码时串单。
                                    var stock = stocks.Find(f =>
                                        f.ManageTypeCode == ManageType.HpAnnualCheckDown &&
                                        f.PlanId == check[0].Id &&
                                        string.Equals(f.EndCellCode, actualResult.CellCode, StringComparison.Ordinal));
                                    if (stock == null)
                                    {
                                        // 查询接口会重复返回本批次已经完成的库位，下一轮 stocks 中已没有对应任务；
                                        // 这种情况属于正常幂等跳过，不应每两秒产生一条告警。
                                        Log.Debug("跳过已处理或不存在的WCS盘点结果：CellCode={CellCode}, OrderCode={OrderCode}",
                                            actualResult.CellCode, actualResult.OrderCode);
                                        continue;
                                    }

                                    var comparison = CompareCheckResult(stock.ArchiveBoxRfid, actualResult);
                                    if (!comparison.CanComplete)
                                        continue;

                                    // WMS 在一个工作单元内完成历史落地、库位解锁和任务结束；
                                    // 盘点结果不会在此直接修改档案盒正式库位或库存。
                                    await _checkAppService.CompleteCheckCellAsync(
                                        stock.Id,
                                        comparison.Flag,
                                        comparison.Remark,
                                        actualResult.Status == WcsCheckCellStatus.Scanned
                                            ? actualResult.PlateCode
                                            : string.Empty);
                                }
                                catch (Exception ex)
                                {
                                    // 单个异常库位不能阻断同一批次其他库位的盘点结果落地。
                                    Cells failedResult = res.Cells[i];
                                    Log.Error(ex,
                                        "处理WCS盘点结果失败：CheckId={CheckId}, QueryCode={QueryCode}, OrderCode={OrderCode}, CellCode={CellCode}",
                                        check[0].Id,
                                        checkOrderResultDto.QueryCode,
                                        failedResult?.OrderCode,
                                        failedResult?.CellCode);
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

