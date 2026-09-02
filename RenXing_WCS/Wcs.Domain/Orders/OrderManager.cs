using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Volo.Abp.Domain.Repositories;
using Wcs.LogTool;
using Volo.Abp.Uow;
using System.Linq;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Local;
using Wcs.Backups;
using Wcs.Tasks;
using Wcs.Dispatch;
using Wcs.Orders.Models;
using Wcs.Tasks.Etos;

namespace Wcs.Orders;

public class OrderManager : ISingletonDependency
{
    private readonly IRepository<DispatchOrder, int> _orderRepository;
    private readonly IRepository<DispatchChkOrderRslt, int> _chkRsltRepository;
    private readonly ILogger<TaskManager> _logger;
    private readonly BackupManager _orderBackUpHelper;
    private readonly ILocalEventBus _eventBus;
    private readonly IUnitOfWorkManager _uowManager;

    public OrderManager(
        IRepository<DispatchOrder, int> orderRepository,
        IRepository<DispatchChkOrderRslt, int> chkRsltRepository,
        BackupManager orderBackUpHelper,
        ILocalEventBus eventBus,
        IUnitOfWorkManager uowManager,
        ILogger<TaskManager> logger)
    {
        _orderRepository = orderRepository;
        _chkRsltRepository = chkRsltRepository;
        _orderBackUpHelper = orderBackUpHelper;
        _eventBus = eventBus;
        _uowManager = uowManager;
        _logger = logger;
    }

    /// <summary>
    /// 添加订单，用于出入库订单的添加
    /// </summary>
    /// <param name="order">被添加的订单</param>
    /// <returns>true：成功，false：失败</returns>
    public async Task<bool> AddStockOrderAsync(DispatchOrder order)
    {
        try
        {
            if (order.OrderType == EnumDispatchOrderType.CheckDown)
                throw new Exception("订单类型为盘点订单，无法添加");

            if (!order.Validate(out string failedReason))
                throw new Exception(failedReason);

            List<DispatchOrder> orders = await _orderRepository.GetListAsync(
                o => o.OrderCode == order.OrderCode).ConfigureAwait(false);

            if (orders != null && orders.Count > 0)
                throw new Exception($"订单号为{order.OrderCode}的调度订单已存在，添加失败");

            await _orderRepository.InsertAsync(order).ConfigureAwait(false);

            //同时备份到redis
            OrderInRedis orderInfo = new OrderInRedis(order);
            await _orderBackUpHelper.SetOrderInfoInRedisAsync(orderInfo).ConfigureAwait(false);
            return true;
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            return false;
        }
    }

    /// <summary>
    /// 添加多个订单，用于出入库订单的批量添加
    /// </summary>
    /// <param name="orders"></param>
    /// <returns></returns>
    [UnitOfWork]
    public async Task<bool> AddStockOrdersAsync(List<DispatchOrder> orders)
    {
        try
        {
            if (orders.Count == 0)
                throw new Exception("需要添加的订单数量为0");

            foreach (DispatchOrder order in orders)
            {
                if (order.OrderType == EnumDispatchOrderType.CheckDown)
                    throw new Exception("存在订单类型为盘点的订单，无法添加");

                if (!order.Validate(out string failedReason))
                    throw new Exception(failedReason);

                if (orders.Where(o => o.OrderCode == order.OrderCode).ToList().Count() > 1)
                    throw new Exception($"存在多个订单号为{order.OrderCode}的待添加调度订单，添加失败");

                int count = await _orderRepository.CountAsync(o => o.OrderCode == order.OrderCode).ConfigureAwait(false);

                if (count > 0)
                    throw new Exception($"订单号为{order.OrderCode}的调度订单已存在，添加失败");
            }

            foreach (DispatchOrder order in orders)
            {
                await _orderRepository.InsertAsync(order).ConfigureAwait(false);
            }

            foreach (DispatchOrder order in orders)
            {
                OrderInRedis orderInfo = new OrderInRedis(order);
                await _orderBackUpHelper.SetOrderInfoInRedisAsync(orderInfo).ConfigureAwait(false);
            }
            return true;
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            return false;
        }
    }

    /// <summary>
    /// 添加多个盘点订单
    /// </summary>
    /// <param name="orders"></param>
    /// <param name="queryCode"></param>
    /// <returns></returns>
    [UnitOfWork]
    public async Task<bool> AddCheckDownOrdersAsync(List<DispatchOrder> orders, string queryCode)
    {
        try
        {
            if (orders.Count == 0)
                throw new Exception("需要添加的盘点订单数量为0");

            foreach (DispatchOrder order in orders)
            {
                if (order.OrderType != EnumDispatchOrderType.CheckDown)
                    throw new Exception("存在订单类型非盘点的订单，添加失败");

                if (orders.Where(o => o.OrderCode == order.OrderCode).Count() > 1)
                    throw new Exception($"存在多个订单号为{order.OrderCode}的待添加订单，添加失败");

                int count = await _orderRepository.CountAsync(o => o.OrderCode == order.OrderCode).ConfigureAwait(false);
                if (count > 0)
                    throw new Exception($"订单号为{order.OrderCode}的调度订单已存在，添加失败");

                if (!order.Validate(out string failedReason))
                    throw new Exception(failedReason);
            }

            //添加到数据库
            foreach (DispatchOrder order in orders)
            {
                await _orderRepository.InsertAsync(order).ConfigureAwait(false);
                //将盘点结果预先写入数据库，等待更新
                List<string> cellsToChk = order.OutputCellCodesToChk();
                foreach (string cellCode in cellsToChk)
                {
                    DispatchChkOrderRslt rslt = new DispatchChkOrderRslt(order.OrderCode, cellCode, "waiting", queryCode);
                    await AddChkOrderResultAsync(rslt).ConfigureAwait(false);
                }
            }

            //添加到Redis
            foreach (DispatchOrder order in orders)
            {
                OrderInRedis orderInfo = new OrderInRedis(order);
                await _orderBackUpHelper.SetOrderInfoInRedisAsync(orderInfo).ConfigureAwait(false);
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            return false;
        }
    }

    /// <summary>
    /// 添加单个盘点任务
    /// </summary>
    /// <param name="chkOrder"></param>
    /// <param name="queryCode"></param>
    /// <returns></returns>
    [UnitOfWork]
    public async Task<bool> AddCheckDownOrderAsync(DispatchOrder chkOrder, string queryCode)
    {
        try
        {
            if (chkOrder.OrderType != EnumDispatchOrderType.CheckDown)
                throw new Exception("存在订单类型非盘点的订单，添加失败");

            int count = await _orderRepository.CountAsync(o => o.OrderCode == chkOrder.OrderCode).ConfigureAwait(false);
            if (count > 0)
                throw new Exception($"订单号为{chkOrder.OrderCode}的调度订单已存在，添加失败");

            if (!chkOrder.Validate(out string failedReason))
                throw new Exception(failedReason);

            await _orderRepository.InsertAsync(chkOrder).ConfigureAwait(false);
            // 将盘点结果预先写入数据库，初始状态必须是 waiting。
            // 不能初始化为 empty：empty 表示 PLC 已实际扫描并确认库位为空；
            // 如果初始化为 empty，WMS 会在机械手尚未扫描时误判为盘点完成并生成盘盈盘亏结果。
            List<string> cellsToChk = chkOrder.OutputCellCodesToChk();
            foreach (string cellCode in cellsToChk)
            {
                DispatchChkOrderRslt rslt = new DispatchChkOrderRslt(chkOrder.OrderCode, cellCode, "waiting", queryCode);
                await AddChkOrderResultAsync(rslt).ConfigureAwait(false);
            }

            //添加到Redis
            OrderInRedis orderInfo = new OrderInRedis(chkOrder);
            await _orderBackUpHelper.SetOrderInfoInRedisAsync(orderInfo).ConfigureAwait(false);

            return true;
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            return false;
        }
    }

    /// <summary>
    /// 保存盘点订单结果
    /// </summary>
    /// <param name="orderRslt"></param>
    /// <returns></returns>
    public async Task<bool> AddChkOrderResultAsync(DispatchChkOrderRslt orderRslt)
    {
        try
        {
            DispatchChkOrderRslt result = new DispatchChkOrderRslt(
                orderRslt.OrderCode,
                orderRslt.CellCode,
                orderRslt.PlateCode,
                orderRslt.QueryCode);

            var chkResult = await _chkRsltRepository.InsertAsync(result).ConfigureAwait(false);

            await _orderBackUpHelper.SetChkOrderRsltInRedisAsync(chkResult).ConfigureAwait(false);

            return true;
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            return false;
        }
    }

    /// <summary>
    /// 删除调度订单
    /// </summary>
    /// <param name="orderCode"></param>
    /// <returns></returns>
    [UnitOfWork]
    public async Task<bool> RemoveDispatchOrderAsync(string orderCode)
    {
        try
        {
            List<DispatchOrder> orders = await _orderRepository.GetListAsync(o => o.OrderCode == orderCode).ConfigureAwait(false);
            if (orders.Count == 0) //不存在orderCode的订单，认为删除是成功的
                return true;

            foreach (var order in orders) //正常情况都是只有一个订单
                await _orderRepository.DeleteAsync(order).ConfigureAwait(false);

            await _orderBackUpHelper.RemoveOrderInRedisAsync(orderCode).ConfigureAwait(false);
            return true;
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            return false;
        }
    }

    /// <summary>
    /// 删除多个订单
    /// </summary>
    /// <param name="ordersToDel"></param>
    /// <returns></returns>
    [UnitOfWork]
    public async Task<bool> RemoveDispatchOrdersAsync(List<DispatchOrder> ordersToDel)
    {
        try
        {
            foreach (var order in ordersToDel)
            {
                await _orderRepository.DeleteAsync(order).ConfigureAwait(false);
            }

            foreach (var order in ordersToDel)
            {
                await _orderBackUpHelper.RemoveOrderInRedisAsync(order.OrderCode).ConfigureAwait(false);
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            return false;
        }
    }

    /// <summary>
    /// 更新盘点结果
    /// </summary>
    /// <param name="orderCode"></param>
    /// <param name="cellCode"></param>
    /// <param name="plateCode"></param>
    /// <returns></returns>
    public async Task<bool> UpdatePlateCodeOfChkOrderRsltAsync(string orderCode, string cellCode, string plateCode)
    {
        try
        {
            List<DispatchChkOrderRslt> results = await _chkRsltRepository.GetListAsync(
                o => o.OrderCode == orderCode && o.CellCode == cellCode)
                .ConfigureAwait(false);

            if (results.Count == 0)
                throw new Exception($"不存在OrderCode为{orderCode}，CellCode为{cellCode}的盘点任务结果");

            if (results.Count > 1)
                throw new Exception($"OrderCode为{orderCode}，CellCode为{cellCode}的盘点任务结果不止1个");

            results[0].PlateCode = plateCode;
            await _chkRsltRepository.UpdateAsync(results[0]).ConfigureAwait(false);

            await _orderBackUpHelper.SetChkOrderRsltInRedisAsync(results[0]).ConfigureAwait(false);

            return true;
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            return false;
        }
    }

    /// <summary>
    /// 强制完成订单
    /// </summary>
    /// <param name="forceDoneOrderCode"></param>
    /// <returns></returns>
    public async Task<OpResultInDispatchSvc> ForceDoneDispatchOrderAsync(string forceDoneOrderCode)
    {
        using (var unit = _uowManager.Begin(isTransactional: true))
        {
            try
            {
                List<DispatchOrder> orders = await _orderRepository.GetListAsync(o => o.OrderCode == forceDoneOrderCode).ConfigureAwait(false);

                if (orders.Count == 0)
                    throw new Exception($"OrderCode为{forceDoneOrderCode}的调度订单查询不到，强制完成失败");

                if (orders.Count > 1)
                    throw new Exception($"OrderCode为{forceDoneOrderCode}的调度订单多于1个，强制完成失败");

                //更新订单状态
                orders[0].SetOrderState(EnumDispatchOrderState.ForceDone);
                orders[0].SetExecInfo(string.Empty, false);
                orders[0].SetExecStep("已强制完成");
                await _orderRepository.UpdateAsync(orders[0]).ConfigureAwait(false);

                //更新盘点结果
                if (orders[0].OrderType == EnumDispatchOrderType.CheckDown)
                {
                    var chkRsltList = await _chkRsltRepository.GetListAsync(
                        o => o.OrderCode == orders[0].OrderCode &&
                        o.PlateCode == "waiting")
                        .ConfigureAwait(false);

                    foreach (var chkRslt in chkRsltList)
                    {
                        chkRslt.PlateCode = "error";
                        await _chkRsltRepository.UpdateAsync(chkRslt).ConfigureAwait(false);
                    }
                }

                await _eventBus.PublishAsync(
                    new RemoveTaskOfOrderEvent() { OrderCode = orders[0].OrderCode }).ConfigureAwait(false);

                //该语句执行时，事件总线连接的事件会被执行，若事件执行抛出错误，该语句也会抛出相同的错误，数据库操作会被回滚，
                //只要该语句没有抛异常，数据库操作就会提交，所以事件若发生错误，一定要抛异常，
                //否则，事件中的数据库操作失败了，但当前函数中的数据库操作还是会成功提交
                await unit.CompleteAsync().ConfigureAwait(false);

                OrderInRedis orderInfo = await _orderBackUpHelper.GetOrderWithOrderCodeInRedisAsync(forceDoneOrderCode).ConfigureAwait(false);
                if (orderInfo == null)
                {
                    orderInfo = new OrderInRedis(orders[0]);
                }
                else
                {
                    orderInfo.orderState = orders[0].State.ToString();
                    orderInfo.execStep = orders[0].ExecStep;
                    orderInfo.execInfo = orders[0].ExecInfo;
                    orderInfo.hasError = orders[0].HasError;
                    orderInfo.execUpdateTime = orders[0].ExecUpdateTime;
                    orderInfo.taskId = 0;
                    orderInfo.pathId = 0;
                    orderInfo.taskState = string.Empty;
                    orderInfo.jobs = new List<JobInfo>();
                }
                await _orderBackUpHelper.SetOrderInfoInRedisAsync(orderInfo).ConfigureAwait(false);

                return new OpResultInDispatchSvc() { IsOK = true, Message = string.Empty };
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                return new OpResultInDispatchSvc() { IsOK = false, Message = ex.Message };
            }
        }
    }

    /// <summary>
    /// 取消订单
    /// </summary>
    /// <param name="cancelOrderCode"></param>
    /// <returns></returns>
    public async Task<OpResultInDispatchSvc> CancelDispatchOrderAsync(string cancelOrderCode)
    {
        using (var unit = _uowManager.Begin())
        {
            try
            {
                List<DispatchOrder> orders = await _orderRepository.GetListAsync(o => o.OrderCode == cancelOrderCode).ConfigureAwait(false);

                if (orders.Count == 0)
                    throw new Exception($"OrderCode为{cancelOrderCode}的调度订单查询不到，取消失败");

                if (orders.Count > 1)
                    throw new Exception($"OrderCode为{cancelOrderCode}的调度订单多于1个，取消失败");

                if (orders[0].State != EnumDispatchOrderState.Created)
                    throw new Exception($"OrderCode为{cancelOrderCode}的调度订单已经在执行中或已经结束，无法取消");

                //更新订单
                orders[0].SetOrderState(EnumDispatchOrderState.Canceled);
                orders[0].SetExecStep("已取消");
                orders[0].SetExecInfo(string.Empty, false);
                await _orderRepository.UpdateAsync(orders[0]).ConfigureAwait(false);

                //若是盘点订单，更新盘点结果
                if (orders[0].OrderType == EnumDispatchOrderType.CheckDown)
                {
                    var chkRsltList = await _chkRsltRepository.GetListAsync(
                        o => o.OrderCode == orders[0].OrderCode &&
                        o.PlateCode == "waiting")
                        .ConfigureAwait(false);

                    foreach (var chkRslt in chkRsltList)
                    {
                        chkRslt.PlateCode = "error";
                        await _chkRsltRepository.UpdateAsync(chkRslt).ConfigureAwait(false);
                    }
                }

                //删除task
                await _eventBus.PublishAsync(
                    new RemoveTaskOfOrderEvent() { OrderCode = orders[0].OrderCode }).ConfigureAwait(false);

                await unit.CompleteAsync().ConfigureAwait(false);

                //备份到Redis
                OrderInRedis orderInfo = await _orderBackUpHelper.GetOrderWithOrderCodeInRedisAsync(cancelOrderCode).ConfigureAwait(false);
                if (orderInfo == null)
                {
                    orderInfo = new OrderInRedis(orders[0]);
                }
                else
                {
                    orderInfo.orderState = orders[0].State.ToString();
                    orderInfo.execStep = orders[0].ExecStep;
                    orderInfo.execInfo = orders[0].ExecInfo;
                    orderInfo.hasError = orders[0].HasError;
                    orderInfo.execUpdateTime = orders[0].ExecUpdateTime;
                    orderInfo.taskId = 0;
                    orderInfo.pathId = 0;
                    orderInfo.taskState = string.Empty;
                    orderInfo.jobs = new List<JobInfo>();
                }
                await _orderBackUpHelper.SetOrderInfoInRedisAsync(orderInfo).ConfigureAwait(false);

                return new OpResultInDispatchSvc() { IsOK = true, Message = string.Empty };
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                return new OpResultInDispatchSvc() { IsOK = false, Message = ex.Message };
            }
        }
    }

    /// <summary>
    /// 结束调度任务
    /// </summary>
    /// <param name="orderCode"></param>
    /// <returns></returns>
    public async Task<OpResultInDispatchSvc> FinishDispatchOrderAsync(string orderCode)
    {
        using (var unit = _uowManager.Begin(isTransactional: true))
        {
            try
            {
                //判断task有效性
                List<DispatchOrder> orders = await _orderRepository.GetListAsync(o => o.OrderCode == orderCode).ConfigureAwait(false);

                if (orders.Count == 0)
                    throw new Exception($"OrderCode为{orderCode}的调度订单查询不到，结束调度订单失败");

                if (orders.Count > 1)
                    throw new Exception($"OrderCode为{orderCode}的调度订单不止1个，结束调度订单失败");

                //更新OrderDispatch
                orders[0].SetOrderState(EnumDispatchOrderState.Done);
                orders[0].SetExecStep("已完成");
                orders[0].SetExecInfo(string.Empty, false);
                await _orderRepository.UpdateAsync(orders[0]).ConfigureAwait(false);

                await _eventBus.PublishAsync(new RemoveTaskOfOrderEvent() { OrderCode = orderCode }).ConfigureAwait(false);

                await unit.CompleteAsync().ConfigureAwait(false);

                OrderInRedis orderInfo = await _orderBackUpHelper.GetOrderWithOrderCodeInRedisAsync(orderCode).ConfigureAwait(false);
                if (orderInfo == null)
                {
                    orderInfo = new OrderInRedis(orders[0]);
                }
                else
                {
                    orderInfo.orderState = orders[0].State.ToString();
                    orderInfo.execStep = orders[0].ExecStep;
                    orderInfo.execInfo = orders[0].ExecInfo;
                    orderInfo.hasError = orders[0].HasError;
                    orderInfo.execUpdateTime = orders[0].ExecUpdateTime;
                    orderInfo.taskId = 0;
                    orderInfo.pathId = 0;
                    orderInfo.taskState = string.Empty;
                    orderInfo.jobs = new List<JobInfo>();
                }
                await _orderBackUpHelper.SetOrderInfoInRedisAsync(orderInfo).ConfigureAwait(false);

                return new OpResultInDispatchSvc() { IsOK = true, Message = string.Empty };
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                return new OpResultInDispatchSvc() { IsOK = false, Message = ex.Message };
            }
        }
    }

    /// <summary>
    /// 更新订单状态
    /// </summary>
    /// <param name="orderCode"></param>
    /// <param name="state"></param>
    /// <returns></returns>
    public async Task<bool?> UpdateDispatchOrderStateAsync(string orderCode, EnumDispatchOrderState state)
    {
        try
        {
            List<DispatchOrder> orders = await _orderRepository.GetListAsync(o => o.OrderCode == orderCode).ConfigureAwait(false);

            if (orders.Count == 0) //没有查询到该orderCode的任务
                return false;

            orders[0].SetOrderState(state);
            await _orderRepository.UpdateAsync(orders[0]).ConfigureAwait(false);

            await _orderBackUpHelper.UpdateOrderStateOfOrderInRedisAsync(orderCode, state.ToString());
            return true;
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            return null;
        }
    }

    public async Task<bool?> AllowDispatchOrderToOpenDoorAsync(string orderCode)
    {
        try
        {
            List<DispatchOrder> orders = await _orderRepository.GetListAsync(o => o.OrderCode == orderCode).ConfigureAwait(false);

            if (orders.Count == 0) //没有查询到该orderCode的任务
                return false;

            orders[0].SetCanOpenDoorImmediate(true);
            await _orderRepository.UpdateAsync(orders[0]).ConfigureAwait(false);

            await _orderBackUpHelper.UpdateOpenDoorImmeOfOrderInRedisAsync(orderCode, true);
            return true;
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            return null;
        }
    }

    /// <summary>
    /// 更新订单执行步骤
    /// </summary>
    /// <param name="orderCode"></param>
    /// <param name="execStep"></param>
    /// <returns></returns>
    public async Task<bool?> UpdateExecStepOfDispatchOrderAsync(string orderCode, string execStep)
    {
        try
        {
            List<DispatchOrder> orders = await _orderRepository.GetListAsync(o => o.OrderCode == orderCode).ConfigureAwait(false);
            if (orders.Count == 0) //没有查询到该orderCode的任务
                return false;

            orders[0].SetExecStep(execStep);
            await _orderRepository.UpdateAsync(orders[0]).ConfigureAwait(false);

            await _orderBackUpHelper.UpdateExecStepOfOrderInRedisAsync(orderCode, execStep.ToString());
            return true;
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            return null;
        }
    }

    /// <summary>
    /// 更新订单执行信息
    /// </summary>
    /// <param name="orderCode"></param>
    /// <param name="execInfo"></param>
    /// <param name="hasError"></param>
    /// <returns></returns>
    public async Task<bool?> UpdateExecInfoOfDispatchOrderAsync(string orderCode, string execInfo, bool hasError)
    {
        try
        {
            List<DispatchOrder> orders = await _orderRepository.GetListAsync(o => o.OrderCode == orderCode).ConfigureAwait(false);
            if (orders.Count == 0) //没有查询到该orderCode的任务
                return false;

            orders[0].SetExecInfo(execInfo, hasError);
            await _orderRepository.UpdateAsync(orders[0]).ConfigureAwait(false);

            await _orderBackUpHelper.UpdateExecInfoOfOrderInRedisAsync(orderCode, execInfo).ConfigureAwait(false);
            await _orderBackUpHelper.UpdateErrorOfOrderInRedisAsync(orderCode, hasError).ConfigureAwait(false);
            return true;
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            return null;
        }
    }
    /// <summary>
    /// 获取所有订单
    /// </summary>
    /// <returns></returns>
    public async Task<List<DispatchOrder>> GetAllDispatchOrdersAsync()
    {
        try
        {
            var orders = await _orderRepository.GetListAsync().ConfigureAwait(false);
            return orders.OrderBy(o => o.Id).ToList();
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            return null;
        }
    }

    public async Task<DispatchOrder> GetDispatchOrderByOrderCodeAsync(string orderCode)
    {
        try
        {
            List<DispatchOrder> orders = await _orderRepository.GetListAsync(o => o.OrderCode == orderCode).ConfigureAwait(false);

            if (orders.Count == 0)
                throw new Exception($"订单号为{orderCode}的订单不存在");

            if (orders.Count > 1)
                throw new Exception($"订单号为{orderCode}的订单超过1个，数据异常");

            return orders[0];
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            return null;
        }
    }

    /// <summary>
    /// 获取待执行订单
    /// </summary>
    /// <returns></returns>
    public async Task<List<DispatchOrder>> GetAllDispatchOrdersToDoAsync()
    {
        try
        {
            List<DispatchOrder> orders = await _orderRepository.GetListAsync(o => o.State == EnumDispatchOrderState.Created).ConfigureAwait(false);
            return orders.OrderByDescending(o => o.Priority).ThenBy(o => o.Id).ToList();
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            return null;
        }
    }

    /// <summary>
    /// 获取第一个待执行订单
    /// </summary>
    /// <returns></returns>
    public async Task<DispatchOrder> GetFirstDispatchOrderToDoAsync()
    {
        try
        {
            List<DispatchOrder> orders = await _orderRepository.GetListAsync(o => o.State == EnumDispatchOrderState.Created).ConfigureAwait(false);
            orders = orders.OrderByDescending(d => d.Priority).ThenBy(d => d.Id).ToList();

            if (orders.Count == 0)
                return null;

            return orders[0];
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            return null;
        }
    }
    /// <summary>
    /// 获取指定类型的待执行订单
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public async Task<List<DispatchOrder>> GetAllDispatchOrdersToDoWithTypeAsync(EnumDispatchOrderType type)
    {
        try
        {
            var orders = await _orderRepository.GetListAsync(o => o.State == EnumDispatchOrderState.Created && o.OrderType == type).ConfigureAwait(false);
            return orders.OrderByDescending(d => d.Priority).ThenBy(d => d.Id).ToList();
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            return null;
        }
    }

    public async Task<List<DispatchOrder>> GetUnFinishedDispatchOrdersAsync()
    {
        try
        {
            var orders = await _orderRepository.GetListAsync(o => o.State == EnumDispatchOrderState.Created || o.State == EnumDispatchOrderState.Doing).ConfigureAwait(false);
            return orders.OrderBy(o => o.Id).ToList();
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            return null;
        }
    }

    public async Task<List<DispatchOrder>> GetDispatchOrdersBeforeTimeAsync(DateTime beforeTime)
    {
        try
        {
            var orders = await _orderRepository.GetListAsync().ConfigureAwait(false);
            return orders.Where(o =>
            {
                if (!DateTime.TryParse(o.CreateTime, out DateTime createTime))
                    return false;
                return createTime < beforeTime;
            }).OrderBy(o => o.Id).ToList();
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            return null;
        }
    }

    /// <summary>
    /// 获取所有盘点结果
    /// </summary>
    /// <returns></returns>
    public async Task<List<DispatchChkOrderRslt>> GetAllChkOrderResultsAsync()
    {
        try
        {
            var results = await _chkRsltRepository.GetListAsync().ConfigureAwait(false);
            return results.OrderBy(o => o.Id).ToList();
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            return new List<DispatchChkOrderRslt>();
        }
    }

    public async Task<List<DispatchChkOrderRslt>> GetChkOrderRsltsByQueryCodeAsync(string queryCode)
    {
        try
        {
            var results = await _chkRsltRepository.GetListAsync(o => o.QueryCode == queryCode).ConfigureAwait(false);
            return results.OrderBy(o => o.Id).ToList();
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            return new List<DispatchChkOrderRslt>();
        }
    }

    public async Task<DispatchChkOrderRslt> GetChkOrderRsltByOrderCodeAsync(string orderCode)
    {
        try
        {
            var results = await _chkRsltRepository.GetListAsync(o => o.OrderCode == orderCode).ConfigureAwait(false);

            if (results.Count == 0)
                throw new Exception($"OrderCode为{orderCode}的盘点任务结果不存在");

            if (results.Count > 1)
                throw new Exception($"OrderCode为{orderCode}的盘点任务结果不止1个");

            return results[0];
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            return null;
        }
    }

    /// <summary>
    /// 删除盘点结果
    /// </summary>
    /// <param name="chkOrderRslts"></param>
    /// <returns></returns>
    [UnitOfWork]
    public async Task<bool> DelChkOrderRsltsAsync(List<DispatchChkOrderRslt> chkOrderRslts)
    {
        try
        {
            foreach (var result in chkOrderRslts)
                await _chkRsltRepository.DeleteAsync(result.Id).ConfigureAwait(false);

            foreach (var result in chkOrderRslts)
                await _orderBackUpHelper.RemoveOrderRsltInRedisAsync($"{result.OrderCode}.{result.CellCode}").ConfigureAwait(false);

            return true;
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            return false;
        }
    }
}
