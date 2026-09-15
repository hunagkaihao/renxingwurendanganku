using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using WarehouseManagement.Cells;
using WarehouseManagement.WcsTasks.Dto;

namespace WarehouseManagement.WcsTasks
{
    public class WcsTaskAppService : WarehouseManagementAppService, IWcsTaskAppService
    {
        private readonly WcsApiManager _wcsApiManager;
        private readonly ICellRepository _cellRepository;

        public WcsTaskAppService(WcsApiManager wcsApiManager, ICellRepository cellRepository)
        {
            _wcsApiManager = wcsApiManager;
            _cellRepository = cellRepository;
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
            if (openDoor == null || string.IsNullOrWhiteSpace(openDoor.OrderCode))
            {
                throw new UserFriendlyException("请选择柜门库位");
            }

            var cell = await _cellRepository.FindByCodeAsync(openDoor.OrderCode.Trim());
            if (cell == null)
            {
                throw new UserFriendlyException("柜门库位不存在");
            }
            if (cell.CellType != CellType.Station)
            {
                throw new UserFriendlyException("选中的库位不是柜门类型，无法执行开门指令");
            }

            openDoor.OrderCode = cell.CellCode;
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
