using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Ecs.ConfigTool;
using System.Collections.Generic;
using Volo.Abp.Domain.Repositories;
using Ecs.HttpApiTool;
using Microsoft.Extensions.Logging;
using Ecs.LogTool;
using Ecs.Cells;
using Ecs.Nodes.Models;
using Ecs.Orders.Models;

namespace Ecs.Dispatch;

/// <summary>
/// 后台定时清理调度订单数量任务
/// </summary>
public class TestJob : IHostedService, IDisposable
{
    private readonly TestMsgHelper _testMsgHelper;
    private readonly ICellRepository _cellRepository;
    private readonly IRepository<DispatchNode, int> _nodeRepository;
    private readonly IRepository<DispatchOrder, int> _orderRepository;
    private readonly ILogger<TestJob> _logger;



    
    public TestJob(
        TestMsgHelper testMsgHelper,
        ICellRepository cellRepository,
        IRepository<DispatchNode, int> nodeRepository,
        IRepository<DispatchOrder, int> orderRepository,
        ILogger<TestJob> logger)
    {
        _testMsgHelper = testMsgHelper;
        _cellRepository = cellRepository;
        _nodeRepository = nodeRepository;
        _orderRepository = orderRepository;
        _logger = logger;
    }

    public void Dispose()
    {
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        Task.Run(async () => {

            while(true)
            {
                Thread.Sleep(500);

                var msg = _testMsgHelper.GetMessage();
                if(msg == null)
                    continue;
                
                _testMsgHelper.DequeueMessage();

                try
                {
                    if(msg.Command == EnumTestMessageCmd.Start || msg.Command == EnumTestMessageCmd.Restart)
                    {
                        List<TestItem> testItems = Settings.Options.Test;

                        for(int itemNo = 0; itemNo < testItems.Count; itemNo++)
                        {
                            if(testItems[itemNo].RowNo <= 0)
                                continue;
                            if(testItems[itemNo].StartColNo <= 0 || testItems[itemNo].EndColNo <= 0)
                                continue;
                            if(testItems[itemNo].StartColNo > testItems[itemNo].EndColNo)
                                continue;
                            if(testItems[itemNo].StartLayerNo <= 0 || testItems[itemNo].EndLayerNo <= 0)
                                continue;
                            if(testItems[itemNo].StartLayerNo > testItems[itemNo].EndLayerNo)
                                continue;
                            var doors = await _nodeRepository.GetListAsync(
                                o => o.DASpecs == testItems[itemNo].Specs && 
                                o.NodeTypeCode == "12")
                                .ConfigureAwait(false);
                            if(doors == null || doors.Count <= 0)
                                continue;
                            
                            for(int colNo = testItems[itemNo].StartColNo; colNo <= testItems[itemNo].EndColNo; colNo++)
                            {
                                for(int layerNo = testItems[itemNo].StartLayerNo; layerNo <= testItems[itemNo].EndLayerNo; layerNo++)
                                {
                                    var cell = await _cellRepository.FindByWmsCellXYZAsync(testItems[itemNo].RowNo, colNo, layerNo).ConfigureAwait(false);
                                    if(cell == null)
                                        continue;
                                    if(cell.CellSpecs != testItems[itemNo].Specs)
                                        continue;
                                    
                                    //先出库
                                    AddStockOrderDto orderDto = new AddStockOrderDto()
                                    {
                                        orderCode = DateTime.Now.ToString("yyyyMMddHHmmssfff"),
                                        plateCode = "50000100",
                                        startNode = cell.CellCode,
                                        endNode = doors[0].NodeCode,
                                        priority = 1
                                    };

                                    var response = await HttpApiHelper.PostAsync<ResponseDto>(
                                        "http://localhost:3270", 
                                        "ecs/dispatch/order/stockOrderCreate", 
                                        orderDto)
                                        .ConfigureAwait(false);
                                    
                                    if(!response.success)
                                        continue;
                                    
                                    //开始等待任务结束
                                    while(true)
                                    {
                                        await Task.Delay(50).ConfigureAwait(false);
                                        var orders = await _orderRepository.GetListAsync(
                                            o => o.OrderCode == orderDto.orderCode)
                                            .ConfigureAwait(false);
                                        if(orders == null || orders.Count == 0)
                                            break;
                                        if (orders[0].State == EnumDispatchOrderState.Done ||
                                            orders[0].State == EnumDispatchOrderState.ForceDone ||
                                            orders[0].State == EnumDispatchOrderState.Canceled)
                                            break;
                                    }

                                    if(null != _testMsgHelper.GetMessage())
                                    {
                                        if(EnumTestMessageCmd.Start == _testMsgHelper.GetMessage().Command)
                                            _testMsgHelper.DequeueMessage();
                                        else if(EnumTestMessageCmd.Restart == _testMsgHelper.GetMessage().Command)
                                            break;
                                        else //停止
                                        {
                                            while(true) //等待接收启动或重启命令
                                            {
                                                await Task.Delay(500).ConfigureAwait(false);
                                                TestMessage m = _testMsgHelper.GetMessage();
                                                if(m == null)
                                                    continue;
                                                if(m.Command == EnumTestMessageCmd.Stop)
                                                {
                                                    _testMsgHelper.DequeueMessage();
                                                    continue;
                                                }
                                                if(m.Command == EnumTestMessageCmd.Restart)
                                                {
                                                    break;
                                                }
                                                if(m.Command == EnumTestMessageCmd.Start)
                                                {
                                                    _testMsgHelper.DequeueMessage();
                                                    break;
                                                }
                                            }
                                            if(_testMsgHelper.GetMessage() != null) //重启命令
                                                break;
                                        }
                                    }
                                    

                                    //再入库
                                    orderDto = new AddStockOrderDto()
                                    {
                                        orderCode = DateTime.Now.ToString("yyyyMMddHHmmssfff"),
                                        plateCode = "50000100",
                                        startNode = doors[0].NodeCode,
                                        endNode = cell.CellCode,
                                        priority = 1
                                    };

                                    response = await HttpApiHelper.PostAsync<ResponseDto>(
                                        "http://localhost:3270", 
                                        "ecs/dispatch/order/stockOrderCreate", 
                                        orderDto)
                                        .ConfigureAwait(false);
                                    
                                    if(!response.success)
                                        continue;
                                    
                                    //开始等待任务结束
                                    while(true)
                                    {
                                        await Task.Delay(500).ConfigureAwait(false);
                                        var orders = await _orderRepository.GetListAsync(
                                            o => o.OrderCode == orderDto.orderCode)
                                            .ConfigureAwait(false);
                                        if(orders == null || orders.Count == 0)
                                            break;
                                        if (orders[0].State == EnumDispatchOrderState.Done ||
                                            orders[0].State == EnumDispatchOrderState.ForceDone ||
                                            orders[0].State == EnumDispatchOrderState.Canceled)
                                            break;
                                    }

                                    if(null != _testMsgHelper.GetMessage())
                                    {
                                        if(EnumTestMessageCmd.Start == _testMsgHelper.GetMessage().Command)
                                            _testMsgHelper.DequeueMessage();
                                        else if(EnumTestMessageCmd.Restart == _testMsgHelper.GetMessage().Command)
                                            break;
                                        else //停止
                                        {
                                            while(true) //等待接收启动或重启命令
                                            {
                                                await Task.Delay(500).ConfigureAwait(false);
                                                TestMessage m = _testMsgHelper.GetMessage();
                                                if(m == null)
                                                    continue;
                                                if(m.Command == EnumTestMessageCmd.Stop)
                                                {
                                                    _testMsgHelper.DequeueMessage();
                                                    continue;
                                                }
                                                if(m.Command == EnumTestMessageCmd.Restart)
                                                {
                                                    break;
                                                }
                                                if(m.Command == EnumTestMessageCmd.Start)
                                                {
                                                    _testMsgHelper.DequeueMessage();
                                                    break;
                                                }
                                            }
                                            if(_testMsgHelper.GetMessage() != null) //重启命令
                                                break;
                                        }
                                    }
                                }

                                if(null != _testMsgHelper.GetMessage())
                                {
                                    if(EnumTestMessageCmd.Start == _testMsgHelper.GetMessage().Command)
                                        _testMsgHelper.DequeueMessage();
                                    else if(EnumTestMessageCmd.Restart == _testMsgHelper.GetMessage().Command)
                                        break;
                                    else //停止
                                    {
                                        while(true) //等待接收启动或重启命令
                                        {
                                            await Task.Delay(500).ConfigureAwait(false);
                                            TestMessage m = _testMsgHelper.GetMessage();
                                            if(m == null)
                                                continue;
                                            if(m.Command == EnumTestMessageCmd.Stop)
                                            {
                                                _testMsgHelper.DequeueMessage();
                                                continue;
                                            }
                                            if(m.Command == EnumTestMessageCmd.Restart)
                                            {
                                                break;
                                            }
                                            if(m.Command == EnumTestMessageCmd.Start)
                                            {
                                                _testMsgHelper.DequeueMessage();
                                                break;
                                            }
                                        }
                                        if(_testMsgHelper.GetMessage() != null) //重启命令
                                            break;
                                    }
                                }
                            }

                            if(null != _testMsgHelper.GetMessage())
                            {
                                if(EnumTestMessageCmd.Start == _testMsgHelper.GetMessage().Command)
                                    _testMsgHelper.DequeueMessage();
                                else if(EnumTestMessageCmd.Restart == _testMsgHelper.GetMessage().Command)
                                    break;
                                else //停止
                                {
                                    while(true) //等待接收启动或重启命令
                                    {
                                        await Task.Delay(500).ConfigureAwait(false);
                                        TestMessage m = _testMsgHelper.GetMessage();
                                        if(m == null)
                                            continue;
                                        if(m.Command == EnumTestMessageCmd.Stop)
                                        {
                                            _testMsgHelper.DequeueMessage();
                                            continue;
                                        }
                                        if(m.Command == EnumTestMessageCmd.Restart)
                                        {
                                            break;
                                        }
                                        if(m.Command == EnumTestMessageCmd.Start)
                                        {
                                            _testMsgHelper.DequeueMessage();
                                            break;
                                        }
                                    }
                                    if(_testMsgHelper.GetMessage() != null) //重启命令
                                        break;
                                }
                            }
                        }
                        
                    }
                    else
                    {
                        _testMsgHelper.DequeueMessage();
                    }
                }
                catch(Exception ex)
                {
                    _logger.Error(ex.Message);
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