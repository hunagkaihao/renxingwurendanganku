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
        
        private static string OrderCode { get; set; }
        private static string CellCode { get; set; }

        public WcsApiManager(IHttpClientFactory httpClientFactory, IOptionsSnapshot<WCSOptions> wCSOptions)
        {
            _httpClientFactory = httpClientFactory;
            WCSServer = wCSOptions.Value.Server;
            WCSEnable = wCSOptions.Value.Enable;
            WCSSimulation = wCSOptions.Value.Simulation;
        }
        /// <summary>
        /// 创建出入库订单
        /// </summary>
        /// <returns></returns>
        public async Task<ResultWcsTaskDto> StockOrderCreate(string orderCode, string plateCode, string startNode, string endNode, string taskType, int priority)
        {
            if (WCSSimulation)
            {
                Log.Information("WCS 模拟创建出入库任务");
                var responsetest = new ResultWcsTaskDto(true, "模拟测试");
                OrderCode = orderCode;
                return responsetest;
            }
            
            if (!WCSEnable)
            {
                Log.Information("WCS服务配置为不可用");
                return null;
            }
            Log.Information($"WCS创建出入库 => 订单号:[{orderCode}] 档案盒号:[{plateCode}] 开始位置:[{startNode}] 终点位置:[{endNode}]");
            StockOrderCreateDto stockOrderCreate = new StockOrderCreateDto(orderCode, plateCode, startNode, endNode, taskType, priority);
            var response = await _httpClientFactory.PostAsync<StockOrderCreateDto, ResultWcsTaskDto>("TTWCS",
                  $"{WCSServer}/wcs/dispatch/order/stockOrderCreate", stockOrderCreate);

            return response;
        }
        /// <summary>
        /// 创建盘点订单
        /// </summary>
        /// <returns></returns>
        public async Task<ResultWcsTaskDto> CheckOrderCreate(CheckOrderCreateDto checkOrderCreate)
        {
            if (WCSSimulation)
            {
                Log.Information("WCS模拟创建盘点订单");
                var responseTest = new ResultWcsTaskDto(true, "虚拟创建盘点订单");
                responseTest.QueryCode = "模拟测试";
                OrderCode = checkOrderCreate.Orders[0].OrderCode;
                CellCode = checkOrderCreate.Orders[0].CellCode;
                return responseTest;
            }
            if (!WCSEnable)
            {
                Log.Information("WCS服务配置为不可用");
                return null;
            }
            Log.Information("WCS创建盘点订单");
            var response = await _httpClientFactory.PostAsync<CheckOrderCreateDto, ResultWcsTaskDto>("TTWCS",
                                            $"{WCSServer}/wcs/dispatch/order/checkOrderCreate", checkOrderCreate);
            return response;
        }
        /// <summary>
        /// 查询盘点结果
        /// </summary>
        /// <returns></returns>
        public async Task<ResultCheckDto> CheckOrderResult(CheckOrderResultDto checkOrderCreate)
        {
            if (WCSSimulation)
            {
                Log.Information("WCS模拟查询盘点结果");
                var responseTest = new ResultCheckDto
                {
                    Cells = new List<Dto.Cells>
                    {
                       new Dto.Cells
                       {
                           OrderCode = OrderCode,
                           CellCode = CellCode,
                           PlateCode = "empty"
                       }
                    }
                };
                return responseTest;
            }
            if (!WCSEnable)
            {
                Log.Information("WCS服务配置为不可用");
                return null;
            }
            Log.Information("WCS查询盘点结果");
            var response = await _httpClientFactory.GetAsync<ResultCheckDto>("TTWCS",
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
            if (WCSSimulation)
            {
                Log.Information("WCS 模拟查询状态");
                var responsetest = new ListResultSstatesDto
                {
                    orderStates = new List<ResultStatesDto>
                    {
                        new ResultStatesDto 
                        { 
                            OrderCode = OrderCode, 
                            Status = WcsTaskStatus.Completed
                        }
                    }
                };
                return responsetest;
            }
            
            if (!WCSEnable)
            {
                Log.Information("WCS服务配置为不可用");
                return null;
            }
            Log.Information("WCS查询所有订单的执行状态");
            var response = await _httpClientFactory.GetAsync<ListResultSstatesDto>("TTWCS",
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
  $"{WCSServer}/wcs/dispatch/core/pause",null);

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
            if (WCSSimulation)
            {
                Log.Information("WCS 模拟打开取档口");
                var responsetest = new OpenDoorForOrderDto
                {
                    success = true,
                    message = "模拟测试"
                };
                return responsetest;
            }
            if (!WCSEnable)
            { 
                Log.Information("WCS服务配置为不可用");
                return null;
            }
            Log.Information("WCS打开取档口");
            var response = await _httpClientFactory.PostAsync<OpenDoorDto, OpenDoorForOrderDto>("TTWCS",
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
  $"{WCSServer}/wcs/dispatch/device/plcNode"+plcNode);

            return response;

        }


    }
}

