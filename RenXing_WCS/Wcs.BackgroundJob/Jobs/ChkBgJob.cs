using Wcs.Cells;
using Wcs.ConfigTool;
using Wcs.Jobs.JobCmds;
using Wcs.LogTool;
using Wcs.Notifiers;
using Wcs.Orders;
using Wcs.PlcTool;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Wcs.Jobs.CheckBgJob;

/// <summary>
/// 盘点任务订单处理
/// </summary>
public class ChkBgJob : IHostedService, IDisposable
{
    private readonly ILogger<ChkBgJob> _logger;
    private readonly PlcHelper _plcHelper;
    private readonly ICellRepository _cellRepository;
    private readonly CheckMsgQHelper _checkMsgQHelper;
    private readonly OrderManager _orderManager;
    private readonly NotifierManager _notifierManager;


    public ChkBgJob(
        PlcHelper plcHelper,
        ICellRepository cellRepository,
        CheckMsgQHelper checkMsgQHelper,
        OrderManager orderManager,
        NotifierManager notifierManager,
        ILogger<ChkBgJob> logger)
    {
        _plcHelper = plcHelper;
        _cellRepository = cellRepository;
        _checkMsgQHelper = checkMsgQHelper;
        _orderManager = orderManager;
        _notifierManager = notifierManager;
        _logger = logger;
    }

    public void Dispose()
    {
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        Task.Run(() =>
        {
            while (true)
            {
                Thread.Sleep(Settings.Options.ChkTime);

                var msg = _checkMsgQHelper.GetMessage();
                if (msg == null)
                    continue;

                while (true)
                {
                    Thread.Sleep(Settings.Options.ChkTime);

                    if (_plcHelper.IsPlcTagValueChange("Plc1", "CellChkFinished"))
                    {
                        //读取节号
                        var sectionNoTag = _plcHelper.ReadPlcTag("Plc1", "SectionNoChked");
                        if (sectionNoTag == null || sectionNoTag.Quality == EnumQuality.Bad)
                        {
                            _logger.Error("收到PLC的单库位盘点完成信号，但从PLC读取变量SectionNoChked失败");
                            continue;
                        }
                        if (!int.TryParse(sectionNoTag.Value, out int sectionNo))
                        {
                            _logger.Error($"收到PLC的单库位盘点完成信号，但从PLC读取的变量SectionNoChked值为{sectionNoTag.Value}, 无法转换为int");
                            continue;
                        }

                        //读取节中的列号
                        var colNoChkedTag = _plcHelper.ReadPlcTag("Plc1", "ColNoChked");
                        if (colNoChkedTag == null || colNoChkedTag.Quality == EnumQuality.Bad)
                        {
                            _logger.Error($"收到PLC的单库位盘点完成信号，但从PLC读取变量ColNoChked失败");
                            continue;
                        }
                        if (!int.TryParse(colNoChkedTag.Value, out int colNoInSection))
                        {
                            _logger.Error($"收到PLC的单库位盘点完成信号，但从PLC读取的变量ColNoChked值为{colNoChkedTag.Value}, 无法转换为int");
                            continue;
                        }

                        //读取档案盒码
                        var barcodeChkedTag = _plcHelper.ReadPlcTag("Plc1", "BarcodeChked");
                        if (barcodeChkedTag == null || barcodeChkedTag.Quality == EnumQuality.Bad)
                        {
                            _logger.Error($"收到PLC的单库位盘点完成信号，但从PLC读取变量BarcodeChked失败");
                            continue;
                        }
                        if (!int.TryParse(barcodeChkedTag.Value, out int barcode))
                        {
                            _logger.Error($"收到PLC的单库位盘点完成信号，但从PLC读取的变量BarcodeChked值为{barcodeChkedTag.Value}, 无法转换为int");
                            continue;
                        }

                        //查询库位
                        var cell = _cellRepository.FindByPlcCellXYZAsync(msg.PlcRow, msg.PlcLayer, sectionNo, colNoInSection).GetAwaiter().GetResult();
                        if (cell == null)
                        {
                            _logger.Error($"收到PLC的单库位盘点完成信号，但根据{msg.PlcRow}排，{msg.PlcLayer}层，{sectionNo}节，{colNoInSection}列查询不到库位");
                            continue;
                        }

                        //更新库位的盘点结果
                        bool ret = _orderManager.UpdatePlateCodeOfChkOrderRsltAsync(
                            msg.OrderCode, cell.CellCode, barcode.ToString()).Result;
                        if (!ret)
                        {
                            _logger.Error($"收到PLC的单库位盘点完成信号，但更新OrderCode为{msg.OrderCode}，CellCode为{cell.CellCode}的盘点结果为{barcode}失败");
                            continue;
                        }

                        _logger.Info($"收到PLC的单库位盘点完成信号，成功更新OrderCode为{msg.OrderCode}，CellCode为{cell.CellCode}的盘点结果为{barcode}");
                    }

                    if (_plcHelper.IsPlcTagValueChange("Plc1", "AllCheckFinished"))
                    {
                        _logger.Info($"收到PLC的全部盘点完成信号，盘点订单{msg.OrderCode}盘点结束");
                        break;
                    }

                    if (true == _notifierManager.IsNotifierValChanged(WcsConsts.StopCheckOrderNotifierName))
                    {
                        _logger.Info($"收到停止盘点通知，盘点订单{msg.OrderCode}盘点结束");
                        break;
                    }
                }

                _checkMsgQHelper.DequeueMessage();
            }
        });
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}