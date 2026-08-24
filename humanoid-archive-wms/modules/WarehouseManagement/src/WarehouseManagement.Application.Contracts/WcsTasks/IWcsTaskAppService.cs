using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using WarehouseManagement.WcsTasks.Dto;


namespace WarehouseManagement.WcsTasks
{
    public interface IWcsTaskAppService : IApplicationService
    {
        Task<bool> CancelTask();
        //获取通讯状态
        Task<ResultCommuStatesDto> CommuState();
        //龙门归零
        Task<ResultWcsTaskDto> ArmHome();
        //打开柜门
        Task<ResultWcsTaskDto> OpenDoor(OpenDoorDto openDoor);
        //打开区档口
        Task<OpenDoorForOrderDto> OpenDoorForOrder(OpenDoorDto orderCode);
        //PLC点位查询
        Task<ResultPlcNodeDto> PlcNode(PlcNodeDto plcNode);
        //获取任务异常
        //Task GetWcsTaskException();
    }
}
