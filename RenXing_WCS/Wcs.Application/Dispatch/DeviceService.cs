using Wcs.Dispatch.Device;
using Wcs.LogTool;
using Wcs.Mjj;
using Wcs.Nodes;
using Wcs.Nodes.Models;
using Wcs.PlcMonitor;
using Wcs.PlcTool;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Wcs.Dispatch;

public class DeviceService : WcsAppService, IDeviceService
{
    private readonly PlcMonitorManager _plcMonitorManager;
    private readonly MjjManager _mjjManager;
    private readonly NodeManager _nodeManager;
    private readonly PlcHelper _plcHelper;
    private readonly ILogger<DeviceService> _logger;

    public DeviceService(
        PlcMonitorManager plcMonitorManager,
        MjjManager mjjManager,
        NodeManager nodeManager,
        PlcHelper plcHelper,
        ILogger<DeviceService> logger)
    {
        _plcMonitorManager = plcMonitorManager;
        _mjjManager = mjjManager;
        _nodeManager = nodeManager;
        _plcHelper = plcHelper;
        _logger = logger;
    }

    public async Task<DeviceConnStatesDto> GetDeviceConnStateAsync()
    {
        try
        {
            DeviceConnStatesDto result = new DeviceConnStatesDto
            {
                commuStates = new List<DeviceConnState>()
                {
                    new DeviceConnState() { objectName = "PLC", state = false },
                    new DeviceConnState() { objectName = "MJJ", state = false }
                }
            };

            var monitorTags = await _plcMonitorManager.GetAllMonitorValuesAsync().ConfigureAwait(false);
            if(monitorTags != null)
            {
                foreach(var tag in monitorTags)
                {
                    if(tag.monitorTagName == "Plc1.HeartBeatFromPlc")
                    {
                        if(tag.monitorTagQuality == "Good" && tag.monitorTagValue.ToLower() == "true")
                            result.commuStates[0].state = true;
                        break;
                    }
                }
            }

            MjjStatus status = await _mjjManager.GetMjjStatusAync().ConfigureAwait(false);
            if(status != null && status.ColumnStatus != "error" && status.ColumnStatus != "none")
                result.commuStates[1].state = true;

            return result;
        }
        catch(Exception ex)
        {
            _logger.Error(ex.Message);
            return new DeviceConnStatesDto
            {
                commuStates = new List<DeviceConnState>()
                {
                    new DeviceConnState() { objectName = "PLC", state = false },
                    new DeviceConnState() { objectName = "MJJ", state = false }
                }
            };
        }
    }

    public ResponseDto OpenDoorAsync(string doorCode)
    {
        try
        {
            bool ret = _plcHelper.WritePlcTag("Plc1", $"Cmd_{doorCode}", "True");
            if (!ret)
                return new ResponseDto() { success = false, message = $"向Plc1.Cmd_{doorCode}发送开门指令失败" };

            _logger.Info($"成功发送非流程内开柜门命令");
            return new ResponseDto() { success = true, message = $"向Plc1.Cmd_{doorCode}发送开门指令成功" };
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            return new ResponseDto() { success = false, message = ex.Message };
        }
    }

    public async Task<DoorStateDto> GetDoorStateAsync(string doorCode)
    {
        try
        {
            PlcTagValue value = await _plcHelper.ReadPlcTagAsync("Plc1", $"Status_{doorCode}");
            if (value == null || value.Quality == EnumQuality.Bad)
                return new DoorStateDto() { success = false, message = $"读取变量Plc1.Status_{doorCode}失败", doorState = false };
            if (bool.TryParse(value.Value, out bool state))
                return new DoorStateDto() { success = true, message = string.Empty, doorState = state };
            else
                return new DoorStateDto() { success = false, message = $"读取到的变量Plc1.Status_{doorCode}的值无效", doorState = false };
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            return new DoorStateDto() { success = false, message = ex.Message, doorState = false };
        }
    }
}