using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Threading.Tasks;

using Wcs.Backups;
using Wcs.Cells;
using Wcs.Cells.Models;
using Wcs.DahSpecss;
using Wcs.LogTool;
using Wcs.Nodes;
using Wcs.Nodes.Models;
using Wcs.Notifiers;
using Wcs.Orders;
using Wcs.Orders.Models;
using Wcs.WMS;

using Microsoft.Extensions.Logging;

using Volo.Abp;

namespace Wcs.Dispatch;

public class OrderService : WcsAppService, IOrderService
{
    private readonly OrderManager _orderManager;
    private readonly ICellRepository _cellRepository;
    private readonly NodeManager _nodeManager;
    private readonly NotifierManager _notifierManager;
    private readonly IDahSpecsRepository _dahSpecsRepository;
    private readonly BackupManager _backupManager;
    private readonly ILogger<OrderService> _logger;
    private readonly IWMSService _wmsService;

    /// <summary>
    /// 将 WCS 内部订单状态转换为 WMS 使用的任务生命周期状态。
    /// </summary>
    private static WcsTaskStatus ToWmsTaskStatus(string orderState)
    {
        if (!Enum.TryParse<EnumDispatchOrderState>(orderState, true, out var state))
            return WcsTaskStatus.Unknown;

        return state switch
        {
            EnumDispatchOrderState.Created => WcsTaskStatus.Accepted,
            EnumDispatchOrderState.Doing => WcsTaskStatus.Executing,
            EnumDispatchOrderState.Done => WcsTaskStatus.Completed,
            EnumDispatchOrderState.Canceled => WcsTaskStatus.Canceled,
            EnumDispatchOrderState.ForceDone => WcsTaskStatus.ForceCompleted,
            _ => WcsTaskStatus.Unknown
        };
    }

    /// <summary>
    /// 根据 WCS 保存的现场盘点值生成明确的采集状态。
    /// PlateCode 只保存现场事实：waiting、empty、error 或扫码得到的数字条码。
    /// </summary>
    private static WcsCheckCellStatus ToCheckCellStatus(string plateCode)
    {
        return plateCode switch
        {
            "waiting" => WcsCheckCellStatus.Waiting,
            "empty" => WcsCheckCellStatus.Empty,
            "error" => WcsCheckCellStatus.ScanError,
            _ when !string.IsNullOrWhiteSpace(plateCode) => WcsCheckCellStatus.Scanned,
            _ => WcsCheckCellStatus.Unknown
        };
    }

    public OrderService(
        ILogger<OrderService> logger,
        OrderManager orderManager,
        ICellRepository cellRepository,
        NodeManager nodeManager,
        NotifierManager notifierManager,
        IDahSpecsRepository dahSpecsRepository,
        IWMSService wmsService,
        BackupManager backupManager)
    {
        _logger = logger;
        _orderManager = orderManager;
        _cellRepository = cellRepository;
        _nodeManager = nodeManager;
        _notifierManager = notifierManager;
        _dahSpecsRepository = dahSpecsRepository;
        _backupManager = backupManager;
        _wmsService = wmsService;
    }

    /// <summary>
    /// 创建单个取放货订单
    /// </summary>
    /// <param name="para"></param>
    /// <returns></returns>
    public async Task<ResponseDto> CreateStockOrder(AddStockOrderDto para)
    {
        try
        {
            Check.NotNullOrEmpty(para.orderCode, nameof(para.orderCode));
            Check.NotNullOrEmpty(para.plateCode, nameof(para.plateCode));
            Check.NotNullOrEmpty(para.startNode, nameof(para.startNode));
            Check.NotNullOrEmpty(para.endNode, nameof(para.endNode));

            Dictionary<string, DispatchNode> nodes = await _nodeManager.GetAllNodesAsync().ConfigureAwait(false);
            if (nodes == null || nodes.Count == 0)
                return new ResponseDto() { success = false, message = "内部错误1" };

            List<string> nodeCodes = nodes.Keys.ToList();
            if (!nodeCodes.Contains(para.startNode) && para.startNode.Split("-").Length != 3)
            {
                return new ResponseDto() { success = false, message = $"StartNode:{para.startNode}，无法识别" };
            }
            else if (!nodeCodes.Contains(para.endNode) && para.endNode.Split("-").Length != 3)
            {
                return new ResponseDto() { success = false, message = $"EndNode:{para.endNode}，无法识别" };
            }
            //else if(!nodeCodes.Contains(para.startNode) && !nodeCodes.Contains(para.endNode))
            //{
            //    return new ResponseDto(){ success = false, message = $"出入库任务的起点和终点不能都是库位" };
            //}
            else if (nodeCodes.Contains(para.startNode) && nodeCodes.Contains(para.endNode))
            {
                return new ResponseDto() { success = false, message = $"出入库及移库任务的起点和终点必须有一个是库位" };
            }

            string cellSpecs = string.Empty;
            string nodeSpecs = string.Empty;
            if (!nodeCodes.Contains(para.startNode) && nodeCodes.Contains(para.endNode)) //出库任务
            {
                var cell = await _cellRepository.FindByCellCodeAsync(para.startNode).ConfigureAwait(false);
                if (cell == null)
                    return new ResponseDto() { success = false, message = $"出库任务的起点库位{para.startNode}不存在" };
                cellSpecs = cell.CellSpecs;
                var node = await _nodeManager.GetNodeByNodeCodeAsync(para.endNode).ConfigureAwait(false);
                if (node == null)
                    return new ResponseDto() { success = false, message = $"出库任务的终点设备{para.endNode}不存在" };
                nodeSpecs = node.DASpecs;
            }
            else if (!nodeCodes.Contains(para.endNode) && nodeCodes.Contains(para.startNode)) //入库任务
            {
                var cell = await _cellRepository.FindByCellCodeAsync(para.endNode).ConfigureAwait(false);
                if (cell == null)
                    return new ResponseDto() { success = false, message = $"入库任务的终点库位{para.endNode}不存在" };
                cellSpecs = cell.CellSpecs;
                var node = await _nodeManager.GetNodeByNodeCodeAsync(para.startNode).ConfigureAwait(false);
                if (node == null)
                    return new ResponseDto() { success = false, message = $"入库任务的起点设备{para.startNode}不存在" };
                nodeSpecs = node.DASpecs;
            }
            else if (!nodeCodes.Contains(para.startNode) && !nodeCodes.Contains(para.endNode)) //移库任务
            {
                var cell = await _cellRepository.FindByCellCodeAsync(para.startNode).ConfigureAwait(false);
                if (cell == null)
                    return new ResponseDto() { success = false, message = $"移库任务的起点库位{para.startNode}不存在" };
                cellSpecs = cell.CellSpecs;
                cell = await _cellRepository.FindByCellCodeAsync(para.endNode).ConfigureAwait(false);
                if (cell == null)
                    return new ResponseDto() { success = false, message = $"移库任务的终点库位{para.endNode}不存在" };
                nodeSpecs = cell.CellSpecs;
            }

            if (cellSpecs != nodeSpecs)
                return new ResponseDto() { success = false, message = $"起点和终点的档案盒规格不一致，一个为{cellSpecs}，另一个为{nodeSpecs}" };

            var spec = await _dahSpecsRepository.FindBySpecsCodeAsync(cellSpecs).ConfigureAwait(false);
            if (spec == null)
                return new ResponseDto() { success = false, message = $"规格{cellSpecs}无法识别" };

            EnumDispatchOrderType orderType = EnumDispatchOrderType.StockIn;
            if (nodeCodes.Contains(para.startNode) && !nodeCodes.Contains(para.endNode))
            {
                orderType = EnumDispatchOrderType.StockIn;
            }
            else if (!nodeCodes.Contains(para.startNode) && nodeCodes.Contains(para.endNode))
            {
                orderType = EnumDispatchOrderType.StockOut;
            }
            else if (!nodeCodes.Contains(para.startNode) && !nodeCodes.Contains(para.endNode))
            {
                orderType = EnumDispatchOrderType.Move;
            }

            DispatchOrder order = new DispatchOrder(para.orderCode, orderType, para.startNode, para.endNode, para.priority);

            order.SetPlate(para.plateCode, cellSpecs);

            bool ret = await _orderManager.AddStockOrderAsync(order).ConfigureAwait(false);
            if (!ret)
                return new ResponseDto() { success = false, message = "Failed" };
            else
                return new ResponseDto() { success = true, message = "Succeeded" };
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            return new ResponseDto() { success = false, message = ex.Message };
        }
    }

    /// <summary>
    /// 创建多个取放货订单
    /// </summary>
    /// <param name="para"></param>
    /// <returns></returns>
    public async Task<ResponseDto> CreateStockOrders(AddStockOrdersDto para)
    {
        List<DispatchOrder> orders = new List<DispatchOrder>();
        foreach (AddStockOrderDto od in para.stockOrders)
        {
            Check.NotNullOrEmpty(od.orderCode, nameof(od.orderCode));
            Check.NotNullOrEmpty(od.plateCode, nameof(od.plateCode));
            Check.NotNullOrEmpty(od.startNode, nameof(od.startNode));
            Check.NotNullOrEmpty(od.endNode, nameof(od.endNode));

            Dictionary<string, DispatchNode> nodes = await _nodeManager.GetAllNodesAsync().ConfigureAwait(false);
            if (nodes == null || nodes.Count == 0)
                return new ResponseDto() { success = false, message = "内部错误1" };

            List<string> nodeCodes = nodes.Keys.ToList();
            if (!nodeCodes.Contains(od.startNode) && od.startNode.Split("-").Length != 3)
            {
                return new ResponseDto() { success = false, message = $"StartNode:{od.startNode}，无法识别" };
            }
            else if (!nodeCodes.Contains(od.endNode) && od.endNode.Split("-").Length != 3)
            {
                return new ResponseDto() { success = false, message = $"EndNode:{od.endNode}，无法识别" };
            }
            //else if(!nodeCodes.Contains(para.startNode) && !nodeCodes.Contains(para.endNode))
            //{
            //    return new ResponseDto(){ success = false, message = $"出入库任务的起点和终点不能都是库位" };
            //}
            else if (nodeCodes.Contains(od.startNode) && nodeCodes.Contains(od.endNode))
            {
                return new ResponseDto() { success = false, message = $"出入库及移库任务的起点和终点必须有一个是库位" };
            }

            string cellSpecs = string.Empty;
            string nodeSpecs = string.Empty;
            if (!nodeCodes.Contains(od.startNode) && nodeCodes.Contains(od.endNode)) //出库任务
            {
                var cell = await _cellRepository.FindByCellCodeAsync(od.startNode).ConfigureAwait(false);
                if (cell == null)
                    return new ResponseDto() { success = false, message = $"出库任务的起点库位{od.startNode}不存在" };
                cellSpecs = cell.CellSpecs;
                var node = await _nodeManager.GetNodeByNodeCodeAsync(od.endNode).ConfigureAwait(false);
                if (node == null)
                    return new ResponseDto() { success = false, message = $"出库任务的终点设备{od.endNode}不存在" };
                nodeSpecs = node.DASpecs;
            }
            else if (!nodeCodes.Contains(od.endNode) && nodeCodes.Contains(od.startNode)) //入库任务
            {
                var cell = await _cellRepository.FindByCellCodeAsync(od.endNode).ConfigureAwait(false);
                if (cell == null)
                    return new ResponseDto() { success = false, message = $"入库任务的终点库位{od.endNode}不存在" };
                cellSpecs = cell.CellSpecs;
                var node = await _nodeManager.GetNodeByNodeCodeAsync(od.startNode).ConfigureAwait(false);
                if (node == null)
                    return new ResponseDto() { success = false, message = $"入库任务的起点设备{od.startNode}不存在" };
                nodeSpecs = node.DASpecs;
            }
            else if (!nodeCodes.Contains(od.startNode) && !nodeCodes.Contains(od.endNode)) //移库任务
            {
                var cell = await _cellRepository.FindByCellCodeAsync(od.startNode).ConfigureAwait(false);
                if (cell == null)
                    return new ResponseDto() { success = false, message = $"移库任务的起点库位{od.startNode}不存在" };
                cellSpecs = cell.CellSpecs;
                cell = await _cellRepository.FindByCellCodeAsync(od.endNode).ConfigureAwait(false);
                if (cell == null)
                    return new ResponseDto() { success = false, message = $"移库任务的终点库位{od.endNode}不存在" };
                nodeSpecs = cell.CellSpecs;
            }

            if (cellSpecs != nodeSpecs)
                return new ResponseDto() { success = false, message = $"起点和终点的档案盒规格不一致，一个为{cellSpecs}，另一个为{nodeSpecs}" };

            var spec = await _dahSpecsRepository.FindBySpecsCodeAsync(cellSpecs).ConfigureAwait(false);
            if (spec == null)
                return new ResponseDto() { success = false, message = $"规格{cellSpecs}无法识别" };

            EnumDispatchOrderType orderType = EnumDispatchOrderType.StockIn;
            if (nodeCodes.Contains(od.startNode) && !nodeCodes.Contains(od.endNode))
            {
                orderType = EnumDispatchOrderType.StockIn;
            }
            else if (!nodeCodes.Contains(od.startNode) && nodeCodes.Contains(od.endNode))
            {
                orderType = EnumDispatchOrderType.StockOut;
            }
            else if (!nodeCodes.Contains(od.startNode) && !nodeCodes.Contains(od.endNode))
            {
                orderType = EnumDispatchOrderType.Move;
            }

            DispatchOrder order = new DispatchOrder(od.orderCode, orderType, od.startNode, od.endNode, od.priority);
            order.SetPlate(od.plateCode, cellSpecs);
            orders.Add(order);
        }

        bool ret = await _orderManager.AddStockOrdersAsync(orders).ConfigureAwait(false);
        if (!ret)
            return new ResponseDto() { success = false, message = "Failed" };
        else
            return new ResponseDto() { success = true, message = "Succeeded" };
    }

    /// <summary>
    /// 创建盘点订单
    /// </summary>
    public class CheckOrder
    {
        public AddCheckOrderDto Order { get; set; } = new AddCheckOrderDto();
        public int Row { get; set; }
        public int Col { get; set; }
        public int Layer { get; set; }
    }
    public async Task<AddChkOrderResultDto> CreateCheckDownOrders(AddCheckOrderDto para)
    {
        try
        {
            DispatchCell cell = await _cellRepository.FindByCellCodeAsync(para.startCellCode).ConfigureAwait(false);
            if (cell == null)
                return new AddChkOrderResultDto() { success = false, message = $"盘点起始库位{para.startCellCode}不存在", queryCode = string.Empty };

            cell = await _cellRepository.FindByCellCodeAsync(para.endCellCode).ConfigureAwait(false);
            if (cell == null)
                return new AddChkOrderResultDto() { success = false, message = $"盘点终止库位{para.endCellCode}不存在", queryCode = string.Empty };

            DispatchOrder chkOrder = new DispatchOrder(para.orderCode, EnumDispatchOrderType.CheckDown,
                para.startCellCode, para.endCellCode, para.priority);

            string queryCode = string.IsNullOrWhiteSpace(para.queryCode) ? para.orderCode : para.queryCode;
            bool ret = await _orderManager.AddCheckDownOrderAsync(chkOrder, queryCode);
            if (ret)
                return new AddChkOrderResultDto() { success = true, message = "添加成功", queryCode = queryCode };
            else
                return new AddChkOrderResultDto() { success = false, message = "添加失败", queryCode = "" };
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            return new AddChkOrderResultDto() { success = false, message = ex.Message, queryCode = string.Empty };
        }
    }

    //250925下发盘点订单，通知wms下发成功
    public async Task<AddChkOrderResultDto> ChkOrderDown(AddCheckOrderDto para)
    {
        try
        {
            DispatchCell cell = await _cellRepository.FindByCellCodeAsync(para.startCellCode).ConfigureAwait(false);
            if (cell == null)
                return new AddChkOrderResultDto() { success = false, message = $"盘点起始库位{para.startCellCode}不存在", queryCode = string.Empty };

            cell = await _cellRepository.FindByCellCodeAsync(para.endCellCode).ConfigureAwait(false);
            if (cell == null)
                return new AddChkOrderResultDto() { success = false, message = $"盘点终止库位{para.endCellCode}不存在", queryCode = string.Empty };

            DispatchOrder chkOrder = new DispatchOrder(para.orderCode, EnumDispatchOrderType.CheckDown,
                para.startCellCode, para.endCellCode, para.priority);

            string queryCode = string.IsNullOrWhiteSpace(para.queryCode) ? para.orderCode : para.queryCode;
            bool ret = await _orderManager.AddCheckDownOrderAsync(chkOrder, queryCode);
            if (ret)
            {
                ChkStatusDto chkStatusDto = new ChkStatusDto()
                {
                    orderCode = para.orderCode,
                    execState = "WCS_CATCHED",
                };
                bool flag = await _wmsService.SendChkStatus(chkStatusDto);
                _logger.Info($"订单号：{para.orderCode}推送盘点状态：{flag}");
                return new AddChkOrderResultDto() { success = true, message = "添加成功", queryCode = queryCode };
            }
            else
                return new AddChkOrderResultDto() { success = false, message = "添加失败", queryCode = "" };
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            return new AddChkOrderResultDto() { success = false, message = ex.Message, queryCode = string.Empty };
        }
    }
    /// <summary>
    /// 根据查询码查询盘点结果
    /// </summary>
    /// <param name="queryCode"></param>
    /// <returns></returns>
    public async Task<CheckOrderResultsDto> GetChkOdRsltByQueryCode(string queryCode)
    {
        try
        {
            Check.NotNullOrEmpty(queryCode, nameof(queryCode));
            var results = await _backupManager.GetChkResultsByQueryCodeInRedisAsync(queryCode).ConfigureAwait(false);
            CheckOrderResultsDto ret = new CheckOrderResultsDto();
            foreach (var r in results)
            {
                ret.cells.Add(new CheckOrderResultDto()
                {
                    cellCode = r.CellCode,
                    orderCode = r.OrderCode,
                    status = ToCheckCellStatus(r.PlateCode),
                    plateCode = r.PlateCode
                });
            }
            return ret;
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            return new CheckOrderResultsDto();
        }
    }

    /// <summary>
    /// 根据订单码查询盘点结果
    /// </summary>
    /// <param name="orderCode"></param>
    /// <returns></returns>
    public async Task<CheckOrderResultsDto> GetChkOdRsltByOrderCode(string orderCode)
    {
        try
        {
            Check.NotNullOrEmpty(orderCode, nameof(orderCode));
            var results = await _backupManager.GetChkResltsByOrderCodeInRedisAsync(orderCode).ConfigureAwait(false);
            CheckOrderResultsDto ret = new CheckOrderResultsDto();
            foreach (var r in results)
            {
                ret.cells.Add(new CheckOrderResultDto()
                {
                    cellCode = r.CellCode,
                    orderCode = r.OrderCode,
                    status = ToCheckCellStatus(r.PlateCode),
                    plateCode = r.PlateCode
                });
            }
            return ret;
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            return new CheckOrderResultsDto();
        }
    }

    /// <summary>
    /// 允许订单打开柜门
    /// </summary>
    /// <param name="para"></param>
    /// <returns></returns>
    public async Task<ResponseDto> AllowOrderToOpenDoor(OpenDoorForOrderDto para)
    {
        try
        {
            Check.NotNullOrEmpty(para.orderCode, nameof(para.orderCode));
            var result = await _orderManager.GetDispatchOrderByOrderCodeAsync(para.orderCode).ConfigureAwait(false);
            if (result == null)
                throw new Exception($"订单号为{para.orderCode}的订单不存在");

            if (result.State != EnumDispatchOrderState.Created && result.State != EnumDispatchOrderState.Doing)
                throw new Exception($"订单号为{para.orderCode}的订单已完成");

            await _orderManager.AllowDispatchOrderToOpenDoorAsync(para.orderCode).ConfigureAwait(false);
            return new ResponseDto() { success = true, message = "" };
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            return new ResponseDto() { success = false, message = ex.Message };
        }
    }

    /// <summary>
    /// 查询单个订单的执行状态
    /// </summary>
    /// <param name="orderCode"></param>
    /// <returns></returns>
    public async Task<OrderStateDto> GetDispatchOrderState(string orderCode)
    {
        try
        {
            Check.NotNullOrEmpty(orderCode, nameof(orderCode));
            var result = await _backupManager.GetOrderWithOrderCodeInRedisAsync(orderCode).ConfigureAwait(false);
            if (result == null)
            {
                return new OrderStateDto()
                {
                    orderCode = orderCode,
                    status = WcsTaskStatus.Unknown,
                    execState = "未知订单",
                    errorInfo = "没有此订单或查询失败",
                    happenTime = ""
                };
            }

            return new OrderStateDto()
            {
                orderCode = result.orderCode,
                status = ToWmsTaskStatus(result.orderState),
                execState = result.execStep,
                errorInfo = result.hasError ? result.execInfo : string.Empty,
                happenTime = result.execUpdateTime
            };
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            return new OrderStateDto()
            {
                orderCode = orderCode,
                status = WcsTaskStatus.Unknown,
                execState = "未知订单",
                errorInfo = ex.Message,
                happenTime = ""
            };
        }
    }

    /// <summary>
    /// 查询所有订单的执行状态
    /// </summary>
    /// <returns></returns>
    public async Task<OrderStatesDto> GetDispatchOrderStates()
    {
        try
        {
            List<OrderInRedis> orders = await _backupManager.GetAllOrdersInRedisAsync().ConfigureAwait(false);
            List<OrderStateDto> stateDtos = new List<OrderStateDto>();
            foreach (OrderInRedis o in orders)
            {
                OrderStateDto stateDto = new OrderStateDto()
                {
                    orderCode = o.orderCode,
                    status = ToWmsTaskStatus(o.orderState),
                    execState = o.execStep,
                    errorInfo = o.hasError ? o.execInfo : string.Empty,
                    happenTime = o.execUpdateTime
                };
                stateDtos.Add(stateDto);
            }
            OrderStatesDto result = new OrderStatesDto()
            {
                orderStates = stateDtos
            };
            return result;
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            return new OrderStatesDto() { orderStates = new List<OrderStateDto>() };
        }
    }

    /// <summary>
    /// 查询所有未完成的订单详细信息
    /// </summary>
    /// <returns></returns>
    public async Task<List<OrderInfoDto>> GetUnFinishedDispatchOrderDtos()
    {
        try
        {
            var orders = await _backupManager.GetUnFinishedOrdersInRedisAsync().ConfigureAwait(false);
            if (orders == null || orders.Count == 0)
                return new List<OrderInfoDto>();

            List<OrderInfoDto> ret = new List<OrderInfoDto>();
            foreach (var o in orders)
            {
                OrderInfoDto orderInfo = new OrderInfoDto()
                {
                    orderCode = o.orderCode,
                    orderType = o.orderType.ToString(),
                    orderState = o.orderState,
                    plateCode = o.plateCode,
                    startNode = o.startNode,
                    endNode = o.endNode,
                    priority = o.priority,
                    openDoorImme = o.openDoorImme,
                    createTime = o.createTime ?? DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),

                    execStep = o.execStep,
                    execInfo = o.execInfo,
                    hasError = o.hasError,
                    execUpdateTime = o.execUpdateTime,

                    pathId = o.pathId,
                    taskId = o.taskId,
                    taskState = o.taskState,
                    jobs = new List<JobInfoDto>()
                };
                foreach (var j in o.jobs)
                {
                    JobInfoDto jobInfo = new JobInfoDto()
                    {
                        id = j.id,
                        pathStep = j.pathStep,
                        nextTrueStep = j.nextTrueStep,
                        nextFalseStep = j.nextFalseStep,
                        state = j.state,
                        priority = j.priority,
                        execInfo = j.execInfo,
                        createTime = j.createTime,
                        cmdName = j.cmdName,
                        nodeName = j.nodeName
                    };
                    orderInfo.jobs.Add(jobInfo);
                }
                ret.Add(orderInfo);
            }
            return ret;
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            return new List<OrderInfoDto>();
        }
    }

    /// <summary>
    /// 查询所有订单的详细信息，包括已完成的订单
    /// </summary>
    /// <returns></returns>
    public async Task<List<OrderInfoDto>> GetAllDispatchOrderDtos()
    {
        try
        {
            var orders = await _backupManager.GetAllOrdersInRedisAsync().ConfigureAwait(false);
            if (orders == null || orders.Count == 0)
                return new List<OrderInfoDto>();

            List<OrderInfoDto> ret = new List<OrderInfoDto>();
            foreach (var o in orders)
            {
                OrderInfoDto orderInfo = new OrderInfoDto()
                {
                    orderCode = o.orderCode,
                    orderType = o.orderType.ToString(),
                    orderState = o.orderState,
                    plateCode = o.plateCode,
                    startNode = o.startNode,
                    endNode = o.endNode,
                    priority = o.priority,
                    openDoorImme = o.openDoorImme,
                    createTime = o.createTime ?? DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),

                    execStep = o.execStep,
                    execInfo = o.execInfo,
                    hasError = o.hasError,
                    execUpdateTime = o.execUpdateTime,

                    pathId = o.pathId,
                    taskId = o.taskId,
                    taskState = o.taskState,
                    jobs = new List<JobInfoDto>()
                };
                foreach (var j in o.jobs)
                {
                    JobInfoDto jobInfo = new JobInfoDto()
                    {
                        id = j.id,
                        pathStep = j.pathStep,
                        nextTrueStep = j.nextTrueStep,
                        nextFalseStep = j.nextFalseStep,
                        state = j.state,
                        priority = j.priority,
                        execInfo = j.execInfo,
                        createTime = j.createTime,
                        cmdName = j.cmdName,
                        nodeName = j.nodeName
                    };
                    orderInfo.jobs.Add(jobInfo);
                }
                ret.Add(orderInfo);
            }
            return ret;
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            return new List<OrderInfoDto>();
        }
    }

    /// <summary>
    /// 根据订单码查询单个订单的详细信息
    /// </summary>
    /// <param name="orderCode"></param>
    /// <returns></returns>
    public async Task<OrderInfoDto> GetOneDispatchOrderDto(string orderCode)
    {
        try
        {
            var order = await _backupManager.GetOrderWithOrderCodeInRedisAsync(orderCode).ConfigureAwait(false);
            if (order == null)
                return null;

            OrderInfoDto orderInfo = new OrderInfoDto()
            {
                orderCode = order.orderCode,
                orderType = order.orderType.ToString(),
                orderState = order.orderState,
                plateCode = order.plateCode,
                startNode = order.startNode,
                endNode = order.endNode,
                priority = order.priority,
                openDoorImme = order.openDoorImme,
                createTime = order.createTime ?? DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),

                execStep = order.execStep,
                execInfo = order.execInfo,
                hasError = order.hasError,
                execUpdateTime = order.execUpdateTime,

                pathId = order.pathId,
                taskId = order.taskId,
                taskState = order.taskState,
                jobs = new List<JobInfoDto>()
            };
            foreach (var j in order.jobs)
            {
                JobInfoDto jobInfo = new JobInfoDto()
                {
                    id = j.id,
                    pathStep = j.pathStep,
                    nextTrueStep = j.nextTrueStep,
                    nextFalseStep = j.nextFalseStep,
                    state = j.state,
                    priority = j.priority,
                    execInfo = j.execInfo,
                    createTime = j.createTime,
                    cmdName = j.cmdName,
                    nodeName = j.nodeName
                };
                orderInfo.jobs.Add(jobInfo);
            }

            return orderInfo;
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            return null;
        }
    }

    /// <summary>
    /// 强制结束指定的订单
    /// </summary>
    /// <param name="para"></param>
    /// <returns></returns>
    public async Task<ResponseDto> ForceDoneDispatchOrderAsync(ForceDoneDto para)
    {
        try
        {
            _notifierManager.IsNotifierValWithParaChanged(WcsConsts.DispatchOrderForceDoneRespNotifierName, out string msg);//先判断一次，进行复位
            _notifierManager.NotifyDispatchSvrWithPara(WcsConsts.DispatchOrderForceDoneNotifierName, para.orderCode);
            long firstPoint = DateTime.Now.Ticks;
            while (true)
            {
                await Task.Delay(20).ConfigureAwait(false);

                long thisPoint = DateTime.Now.Ticks;
                TimeSpan ts = new TimeSpan(thisPoint - firstPoint);
                if (true == _notifierManager.IsNotifierValWithParaChanged(WcsConsts.DispatchOrderForceDoneRespNotifierName, out msg))
                    return new ResponseDto() { success = msg == string.Empty, message = msg };

                if (ts.TotalSeconds > 5)
                    return new ResponseDto() { success = false, message = "服务器无反应" };
            }
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            return new ResponseDto() { success = false, message = ex.Message };
        }
    }

    public async Task<ResponseDto> CancelDispatchOrderAsync(CancelOrderDto para)
    {
        try
        {
            _notifierManager.IsNotifierValWithParaChanged(WcsConsts.DispatchOrderCancelRespNotifierName, out string msg);//先判断一次，进行复位
            _notifierManager.NotifyDispatchSvrWithPara(WcsConsts.DispatchOrderCancelNotifierName, para.orderCode);
            long firstPoint = DateTime.Now.Ticks;
            while (true)
            {
                await Task.Delay(20).ConfigureAwait(false);

                long thisPoint = DateTime.Now.Ticks;
                TimeSpan ts = new TimeSpan(thisPoint - firstPoint);
                if (true == _notifierManager.IsNotifierValWithParaChanged(WcsConsts.DispatchOrderCancelRespNotifierName, out msg))
                    return new ResponseDto() { success = msg == string.Empty, message = msg };

                if (ts.TotalSeconds > 5)
                    return new ResponseDto() { success = false, message = "服务器无反应" };
            }
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            return new ResponseDto() { success = false, message = ex.Message };
        }
    }


}
