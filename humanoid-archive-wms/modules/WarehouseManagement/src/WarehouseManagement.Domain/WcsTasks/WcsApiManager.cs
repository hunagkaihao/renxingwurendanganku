using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Lion.AbpPro.ConfigurationOptions;
using Lion.AbpPro.Extension.Customs.Http;
using Microsoft.Extensions.Options;
using Serilog;
using WarehouseManagement.WcsTasks.Dto;

namespace WarehouseManagement.WcsTasks
{
    public class WcsApiManager : WcsTaskDomainService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public string WCSServer { get; set; }
        public bool WCSEnable { get; set; }
        public bool WCSSimulation { get; set; }

        public WcsApiManager(IHttpClientFactory httpClientFactory, IOptionsSnapshot<WCSOptions> wCSOptions)
        {
            _httpClientFactory = httpClientFactory;
            WCSServer = wCSOptions.Value.Server;
            WCSEnable = wCSOptions.Value.Enable;
            WCSSimulation = wCSOptions.Value.WCSSimulation;
        }

        /// <summary>
        /// 创建出入库订单
        /// </summary>
        /// <returns></returns>
        public async Task<ResultWcsTaskDto> StockOrderCreate(string orderCode, string plateCode, string startNode,
            string endNode, string taskType, int priority)
        {
            if (!WCSEnable)
            {
                Log.Information("WCS服务配置为不可用");
                return null;
            }

            Log.Information("WCS创建出入库订单号：" + orderCode + "档案盒号" + plateCode + "开始位置：" + startNode + "终点位置：" + endNode);
            StockOrderCreateDto stockOrderCreate =
                new StockOrderCreateDto(orderCode, plateCode, startNode, endNode, taskType, priority);
            var response =
                await _httpClientFactory.PostAsync<StockOrderCreateDto, ResultWcsTaskDto>("TTWCS",
                    $"{WCSServer}/wcs/dispatch/order/stockOrderCreate", stockOrderCreate);

            return response;
        }

        /// <summary>
        /// 创建盘点订单
        /// </summary>
        /// <returns></returns>
        public async Task<ResultWcsTaskDto> CheckOrderCreate(CheckOrderCreateDto checkOrderCreate)
        {
            if (!WCSEnable)
            {
                Log.Information("WCS服务配置为不可用");
                return null;
            }

            if (checkOrderCreate?.Orders == null || checkOrderCreate.Orders.Count == 0)
                return new ResultWcsTaskDto(false, "盘点计划没有可下发的扫描段");

            string queryCode = string.IsNullOrWhiteSpace(checkOrderCreate.QueryCode)
                ? $"CHECK-{DateTime.Now:yyyyMMddHHmmssfff}"
                : checkOrderCreate.QueryCode;

            ResultWcsTaskDto lastResponse = null;
            foreach (OrderDto segment in checkOrderCreate.Orders.OrderBy(x => x.Sequence))
            {
                // 兼容旧的单库位请求：未提供起终点时，将 CellCode 同时作为起点和终点。
                string startCellCode = string.IsNullOrWhiteSpace(segment.StartCellCode)
                    ? segment.CellCode
                    : segment.StartCellCode;
                string endCellCode = string.IsNullOrWhiteSpace(segment.EndCellCode)
                    ? startCellCode
                    : segment.EndCellCode;

                WcsCheckOrderRequestDto request = new()
                {
                    OrderCode = segment.OrderCode,
                    QueryCode = queryCode,
                    StartCellCode = startCellCode,
                    EndCellCode = endCellCode,
                    Priority = checkOrderCreate.Priority
                };

                Log.Information(
                    "向WCS下发盘点扫描段：QueryCode={QueryCode}, OrderCode={OrderCode}, Start={Start}, End={End}, Sequence={Sequence}",
                    queryCode, segment.OrderCode, startCellCode, endCellCode, segment.Sequence);

                lastResponse = await _httpClientFactory.PostAsync<WcsCheckOrderRequestDto, ResultWcsTaskDto>(
                    "TTWCS",
                    $"{WCSServer}/wcs/dispatch/order/checkOrderCreate",
                    request);

                if (lastResponse == null || !lastResponse.Success)
                    return lastResponse ?? new ResultWcsTaskDto(false, $"盘点扫描段{segment.OrderCode}下发失败");
            }

            // 所有扫描段使用同一个查询码，WMS 后续只需按该查询码获取整批实际扫描结果。
            lastResponse.QueryCode = queryCode;
            return lastResponse;
        }

        /// <summary>
        /// 查询盘点结果
        /// </summary>
        /// <returns></returns>
        public async Task<ResultCheckDto> CheckOrderResult(CheckOrderResultDto checkOrderCreate)
        {
            if (!WCSEnable)
            {
                Log.Information("WCS服务配置为不可用");
                return null;
            }

            if (WCSSimulation)
            {
                if (checkOrderCreate == null || string.IsNullOrWhiteSpace(checkOrderCreate.OrderCode) ||
                    string.IsNullOrWhiteSpace(checkOrderCreate.CellCode))
                {
                    Log.Information("WCS模拟查询盘点结果但未传入对应库存信息：QueryCode={QueryCode}, OrderCode={OrderCode}, CellCode={CellCode}",
                        checkOrderCreate?.QueryCode, checkOrderCreate?.OrderCode, checkOrderCreate?.CellCode);
                    return new ResultCheckDto { Cells = new List<Cells>() };
                }

                Log.Information("WCS模拟查询盘点结果：QueryCode={QueryCode}, OrderCode={OrderCode}, CellCode={CellCode}",
                    checkOrderCreate.QueryCode, checkOrderCreate.OrderCode, checkOrderCreate.CellCode);

                return new ResultCheckDto
                {
                    Cells = new List<Cells>
                    {
                        new Cells
                        {
                            OrderCode = checkOrderCreate.OrderCode,
                            CellCode = checkOrderCreate.CellCode,
                            Status = WcsCheckCellStatus.Empty,
                            PlateCode = "empty"
                        }
                    }
                };
            }

            Log.Information("WCS查询盘点结果");
            var response =
                await _httpClientFactory.GetAsync<ResultCheckDto>("TTWCS",
                    $"{WCSServer}/wcs/dispatch/order/checkOrderResultsGetByQueryCode?queryCode={Uri.EscapeDataString(checkOrderCreate.QueryCode)}");

            return response;
        }

        /// <summary>
        /// 查询单个订单的执行状态
        /// </summary>
        /// <returns></returns>
        public async Task<ResultStatesDto> State(StockOrderCreateDto stockOrderCreate)
        {
            if (!WCSEnable)
            {
                Log.Information("WCS服务配置为不可用");
                return null;
            }

            Log.Information("WCS查询单个订单的执行状态");
            var response =
                await _httpClientFactory.GetAsync<ResultStatesDto>("TTWCS",
                    $"{WCSServer}/wcs/dispatch/order/state?orderCode={Uri.EscapeDataString(stockOrderCreate.OrderCode)}");

            return response;
        }

        /// <summary>
        /// 查询所有订单的执行状态
        /// </summary>
        /// <returns></returns>
        public async Task<ListResultSstatesDto> States()
        {
            if (!WCSEnable)
            {
                Log.Information("WCS服务配置为不可用");
                return null;
            }

            Log.Information("WCS查询所有订单的执行状态");
            var response =
                await _httpClientFactory.GetAsync<ListResultSstatesDto>("TTWCS",
                    $"{WCSServer}/wcs/dispatch/order/states");

            return response;
        }

        /// <summary>
        /// 无人库暂停执行所有订单，若当前设备正在进行动作，则该动作会继续执行，直到完成，但下一步动作不会启动。在暂停期间，请勿改变设备状态，否则恢复执行后会出现难以预料的问题
        /// </summary>
        /// <returns></returns>
        public async Task<ResultWcsTaskDto> Pause()
        {
            if (!WCSEnable)
            {
                Log.Information("WCS服务配置为不可用");
                return null;
            }

            Log.Information("WCS暂停执行");
            var stockOrderCreate = new StockOrderCreateDto();
            var response =
                await _httpClientFactory.PostAsync<StockOrderCreateDto, ResultWcsTaskDto>("TTWCS",
                    $"{WCSServer}/wcs/dispatch/core/pause", null);

            return response;
        }

        /// <summary>
        /// 无人库从暂停状态恢复
        /// </summary>
        /// <returns></returns>
        public async Task<ResultWcsTaskDto> Restart()
        {
            if (!WCSEnable)
            {
                Log.Information("WCS服务配置为不可用");
                return null;
            }

            Log.Information("WCS恢复执行");
            var stockOrderCreate = new StockOrderCreateDto();
            var response =
                await _httpClientFactory.PostAsync<StockOrderCreateDto, ResultWcsTaskDto>("TTWCS",
                    $"{WCSServer}/wcs/dispatch/core/restart", null);

            return response;
        }

        /// <summary>
        /// 将指定的订单强制结束。在强制结束前，需要先暂停无人库
        /// </summary>
        /// <returns></returns>
        public async Task<ResultWcsTaskDto> ForceDone(int taskId)
        {
            if (!WCSEnable)
            {
                Log.Information("WCS服务配置为不可用");
                return null;
            }

            Log.Information("WCS订单强制结束");
            OrderCodeDto orderCode = new OrderCodeDto();
            orderCode.OrderCode = taskId.ToString();
            var response =
                await _httpClientFactory.PostAsync<OrderCodeDto, ResultWcsTaskDto>("TTWCS",
                    $"{WCSServer}/wcs/dispatch/order/forceDone", orderCode);

            return response;
        }

        /// <summary>
        /// 取消指定的订单，仅限于尚未执行的订单
        /// </summary>
        /// <returns></returns>
        public async Task<ResultWcsTaskDto> CancelOrder(int taskId)
        {
            if (!WCSEnable)
            {
                Log.Information("WCS服务配置为不可用");
                return null;
            }

            Log.Information("WCS订单取消");
            OrderCodeDto orderCode = new OrderCodeDto();
            orderCode.OrderCode = taskId.ToString();
            var response =
                await _httpClientFactory.PostAsync<OrderCodeDto, ResultWcsTaskDto>("TTWCS",
                    $"{WCSServer}/wcs/dispatch/order/cancelOrder", orderCode);

            return response;
        }

        /// <summary>
        /// 打开取档口门
        /// </summary>
        /// <returns></returns>
        public async Task<ResultWcsTaskDto> OpenDoor(OpenDoorDto openDoor)
        {
            if (!WCSEnable)
            {
                Log.Information("WCS服务配置为不可用");
                return null;
            }

            Log.Information("WCS打开取档口");
            var response =
                await _httpClientFactory.PostAsync<OpenDoorDto, ResultWcsTaskDto>("TTWCS",
                    $"{WCSServer}/wcs/dispatch/order/doorCanOpenByOrder", openDoor);

            return response;
        }


        /// <summary>
        /// 打开取档口
        /// </summary>
        /// <param name="openDoor"></param>
        /// <returns></returns>
        public async Task<OpenDoorForOrderDto> OpenDoorForOrder(OpenDoorDto orderCode)
        {
            if (!WCSEnable)
            {
                Log.Information("WCS服务配置为不可用");
                return null;
            }

            Log.Information("WCS打开取档口");
            var response =
                await _httpClientFactory.PostAsync<OpenDoorDto, OpenDoorForOrderDto>("TTWCS",
                    $"{WCSServer}/wcs/dispatch/order/doorCanOpenByOrder", orderCode);

            return response;
        }


        /// <summary>
        /// 龙门机械手回原点
        /// </summary>
        /// <returns></returns>
        public async Task<ResultWcsTaskDto> ArmHome()
        {
            if (!WCSEnable)
            {
                Log.Information("WCS服务配置为不可用");
                return null;
            }

            Log.Information("WCS龙门机械手回原点");
            var response =
                await _httpClientFactory.PostAsync<OpenDoorDto, ResultWcsTaskDto>("TTWCS",
                    $"{WCSServer}/wcs/dispatch/device/armHome", null);

            return response;
        }

        /// <summary>
        /// PLC、密集架控制器通讯状态查询
        /// </summary>
        /// <returns></returns>
        public async Task<ResultCommuStatesDto> CommuState()
        {
            if (!WCSEnable)
            {
                Log.Information("WCS服务配置为不可用");
                return null;
            }

            Log.Information("WCSPLC、密集架控制器通讯状态查询");
            var response =
                await _httpClientFactory.GetAsync<ResultCommuStatesDto>("TTWCS",
                    $"{WCSServer}/wcs/dispatch/device/commuState", null);

            return response;
        }

        /// <summary>
        /// PLC点位查询
        /// </summary>
        /// <returns></returns>
        public async Task<ResultPlcNodeDto> PlcNode(PlcNodeDto plcNode)
        {
            if (!WCSEnable)
            {
                Log.Information("WCS服务配置为不可用");
                return null;
            }

            Log.Information("WCSPLC点位查询");
            var response =
                await _httpClientFactory.GetAsync<ResultPlcNodeDto>("TTWCS",
                    $"{WCSServer}/wcs/dispatch/device/plcNode" + plcNode);

            return response;
        }
    }
}
