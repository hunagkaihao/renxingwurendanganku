using Ecs.Cells;
using Ecs.Dispatch;
using Ecs.Jobs.JobCmds;
using Ecs.LogTool;
using Ecs.Notifiers;
using Ecs.Orders;
using Ecs.PlcTool;
using Ecs.WMS;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Ecs.Jobs.CheckBgJob;

/// <summary>
/// 从WMS获取盘点任务
/// </summary>
public class ChkTaskJob : IHostedService, IDisposable
{
    private readonly ILogger<ChkTaskJob> _logger;
    private readonly PlcHelper _plcHelper;
    private readonly ICellRepository _cellRepository;
    private readonly CheckMsgQHelper _checkMsgQHelper;
    private readonly OrderManager _orderManager;
    private readonly NotifierManager _notifierManager;
    private readonly IWMSService _wmsService;
    private readonly IOrderService _orderService;


    public ChkTaskJob(
        PlcHelper plcHelper,
        ICellRepository cellRepository,
        CheckMsgQHelper checkMsgQHelper,
        OrderManager orderManager,
        NotifierManager notifierManager,
        IWMSService wmsService,
        IOrderService orderService,
        ILogger<ChkTaskJob> logger)
    {
        _plcHelper = plcHelper;
        _cellRepository = cellRepository;
        _checkMsgQHelper = checkMsgQHelper;
        _orderManager = orderManager;
        _notifierManager = notifierManager;
        _wmsService = wmsService;
        _orderService = orderService;
        _logger = logger;
    }

    public void Dispose()
    {
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        Task.Run(async () =>
        {
            while (true)
            {
                Thread.Sleep(3000);
                //获取盘点任务
                List<CheckOrder> chkTask = await _wmsService.GetChkTask(new ChkTaskDto());
                if (chkTask == null)
                {
                    continue;
                }
                foreach (var item in chkTask)
                {
                    AddCheckOrderDto addCheckOrderDto = new AddCheckOrderDto
                    {
                        orderCode = item.id,
                        endCellCode = item.endCellCode,
                        startCellCode = item.startCellCode,
                        priority = item.priorityLevel,
                    };
                    //下发盘点任务
                    await _orderService.ChkOrderDown(addCheckOrderDto);
                }
            }
        });
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}