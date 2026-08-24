using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading.Tasks;
using WarehouseManagement.WcsTasks.Dto;

namespace WarehouseManagement.WcsTasks
{
    [Route("WcsTasks")]
    public class WcsTasksController : WarehouseManagementController, IWcsTaskAppService
    {

        private readonly IWcsTaskAppService _wcsTaskAppService;

        public WcsTasksController(IWcsTaskAppService wcsTaskAppService)
        {
            _wcsTaskAppService = wcsTaskAppService;
        }

        [HttpPost("cancel")]
        [SwaggerOperation(summary: "取消任务", Tags = new[] { "WcsTasks" })]
        public async Task<bool> CancelTask()
        {
            return await _wcsTaskAppService.CancelTask();
        }

        [HttpPost("commuState")]
        [SwaggerOperation(summary: "PLC、密集架控制器通讯状态查询", Tags = new[] { "WcsTasks" })]
        public async Task<ResultCommuStatesDto> CommuState()
        {
            return await _wcsTaskAppService.CommuState();
        }

        [HttpPost("armhome")]
        [SwaggerOperation(summary: "龙门机械手回原点", Tags = new[] { "WcsTasks" })]
        public async Task<ResultWcsTaskDto> ArmHome()
        {
            return await _wcsTaskAppService.ArmHome();
        }

        [HttpPost("openDoor")]
        [SwaggerOperation(summary: "不能随便打开柜门", Tags = new[] { "WcsTasks" })]
        public Task<ResultWcsTaskDto> OpenDoor(OpenDoorDto openDoor)
        {
            return _wcsTaskAppService.OpenDoor(openDoor);
        }

        [HttpPost("openDoorForOrder")]
        [SwaggerOperation(summary: "通过订单号打开取档口", Tags = new[] { "WcsTasks" })]
        public Task<OpenDoorForOrderDto> OpenDoorForOrder(OpenDoorDto orderCode)
        {
            return _wcsTaskAppService.OpenDoorForOrder(orderCode);
        }


        [HttpPost("plcNode")]
        [SwaggerOperation(summary: "站点状态", Tags = new[] { "WcsTasks" })]
        public Task<ResultPlcNodeDto> PlcNode(PlcNodeDto plcNode)
        {
            return _wcsTaskAppService.PlcNode(plcNode);
        }
    }


}
