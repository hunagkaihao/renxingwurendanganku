using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseManagement.WcsTasks.Dto;

namespace WarehouseManagement.WcsTasks
{
    public class WcsTaskAppService : WarehouseManagementAppService, IWcsTaskAppService
    {
        private readonly WcsApiManager _wcsApiManager;

        public WcsTaskAppService(WcsApiManager wcsApiManager)
        {
            _wcsApiManager = wcsApiManager;
        }
        public async Task<bool> CancelTask()
        {
            //暂停任务接受
            await _wcsApiManager.Pause();
            //强制任务完成
            //await _wcsApiManager.
            //恢复任务接受
            //await _wcsApiManager.Restart();
            //throw new NotImplementedException();
            return true;
        }

        public async Task<ResultCommuStatesDto> CommuState()
        {
            return await _wcsApiManager.CommuState();
        }
        public async Task<ResultWcsTaskDto> ArmHome()
        {
            return await _wcsApiManager.ArmHome();
        }

        public async Task<ResultWcsTaskDto> OpenDoor(OpenDoorDto openDoor)
        {
            return await _wcsApiManager.OpenDoor(openDoor);
        }

        public async Task<ResultPlcNodeDto> PlcNode(PlcNodeDto plcNode)
        {
            return await _wcsApiManager.PlcNode(plcNode);
        }

        public async Task<OpenDoorForOrderDto> OpenDoorForOrder(OpenDoorDto orderCode)
        {
           return await _wcsApiManager.OpenDoorForOrder(orderCode);
        }
    }
}
