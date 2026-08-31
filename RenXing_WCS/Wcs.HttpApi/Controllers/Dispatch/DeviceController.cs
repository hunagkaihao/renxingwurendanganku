using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Wcs.Controllers;
using Wcs.Dispatch.Device;
using Wcs.Nodes;
using Wcs.Nodes.Models;
using Wcs.PlcTool;
using Microsoft.AspNetCore.Mvc;

namespace Wcs.Dispatch;

[Route("wcs/dispatch")]
[ApiController]
public class DeviceController : WcsController, IDeviceService
{
    private readonly IDeviceService _deviceService;

    private readonly PlcHelper _PlcService;

    private readonly NodeManager _nodeManager;

    public DeviceController(IDeviceService deviceService, PlcHelper plcService, NodeManager nodeManager)
    {
        _deviceService = deviceService;
        _PlcService = plcService;
        _nodeManager = nodeManager;
    }

    [HttpGet("device/commuState")]
    public async Task<DeviceConnStatesDto> GetDeviceConnStateAsync()
    {
        return await _deviceService.GetDeviceConnStateAsync().ConfigureAwait(false);
    }

    [HttpGet("device/doorState")]
    public async Task<DoorStateDto> GetDoorStateAsync(string doorCode)
    {
        return await _deviceService.GetDoorStateAsync(doorCode).ConfigureAwait(false);
    }

    [HttpPost("device/openDoor")]
    public ResponseDto OpenDoorAsync(string doorCode)
    {
        return _deviceService.OpenDoorAsync(doorCode);
    }

    [HttpPost("device/armHome")]
    public ResponseDto ArmHome()
    {
        try
        {
            List<byte> list = new List<byte>();
            list.Add((byte)((1 & 0xFF00) >> 8));
            list.Add((byte)(1 & 0xFF));
            list.Add((byte)((999 & 0xFF00) >> 8));
            list.Add((byte)(999 & 0xFF));
            list.Add((byte)0);
            list.Add((byte)0);
            list.Add((byte)0);
            list.Add((byte)0);
            list.Add((byte)0);
            list.Add((byte)0);
            list.Add((byte)0);
            list.Add((byte)0);
            list.Add((byte)0);
            list.Add((byte)0);
            list.Add((byte)0);
            list.Add((byte)0);
            list.Add((byte)0);
            list.Add((byte)0);
            list.Add((byte)0);
            list.Add((byte)0);
            list.Add((byte)0);
            list.Add((byte)0);
            list.Add((byte)0);
            list.Add((byte)0);
            ushort crc = CrcHelper.CreateCrc16Code(list.ToArray(), 40961);
            list.Add((byte)((crc & 0xFF00) >> 8));
            list.Add((byte)(crc & 0xFF));
            string command = System.Text.Encoding.GetEncoding(28591).GetString(list.ToArray());

            DispatchNode? node = _nodeManager.GetNodeByNodeCodeAsync("13001").Result;
            if (node == null)
                return new ResponseDto() { success = false, message = $"����13001������" };

            string[] sects = node.CmdTagName.Split(".");
            if (sects.Length != 2 || sects[0] == "" || sects[1] == "")
                return new ResponseDto() { success = false, message = $"����13001��ַ{node.CmdTagName}����" };

            bool ret = _PlcService.WritePlcTag(sects[0], sects[1], command);
            if (!ret)
                return new ResponseDto() { success = false, message = $"��{node.CmdTagName}����ָ��ʧ��" };

            return new ResponseDto() { success = true, message = "success" };
        }
        catch (Exception ex)
        {
            return new ResponseDto() { success = false, message = ex.Message };
        }
    }

    [HttpPost("device/resetCommand")]
    public ResponseDto ResetCommand()
    {
        try
        {
            // 向PLC的Reset_Command点位写入true
            bool ret = _PlcService.WritePlcTag("Plc1", "Reset_Command", "True");
            if (!ret)
                return new ResponseDto() { success = false, message = "向PLC写入复位命令失败" };

            return new ResponseDto() { success = true, message = "复位命令执行成功" };
        }
        catch (Exception ex)
        {
            return new ResponseDto() { success = false, message = ex.Message };
        }
    }

    [HttpGet("device/alarmId")]
    public ResponseDto GetAlarmId()
    {
        try
        {
            // 读取PLC的Alarm_ID值
            PlcTagValue alarmIdTag = _PlcService.ReadPlcTag("Plc1", "Alarm_ID");
            if (alarmIdTag == null)
                return new ResponseDto() { success = false, message = "读取Alarm_ID失败" };

            // 解析10进制值
            if (!int.TryParse(alarmIdTag.Value, out int decimalValue))
                return new ResponseDto() { success = false, message = "Alarm_ID值不是有效的数字" };

            // 转换为16进制
            string hexValue = decimalValue.ToString("X");
            
            // 控制台显示
            Console.WriteLine($"报警编号 - 十进制：{decimalValue}，十六进制：0x{hexValue}");

            return new ResponseDto() 
            { 
                success = true, 
                message = $"报警ID读取成功 - 10进制: {decimalValue}, 16进制: 0x{hexValue}" 
            };
        }
        catch (Exception ex)
        {
            return new ResponseDto() { success = false, message = ex.Message };
        }
    }

    public static class CrcHelper
    {
        public static ushort CreateCrc16Code(byte[] data, ushort divisor = 40961)
        {
            ushort num = ushort.MaxValue;
            for (int i = 0; i < data.Length; i++)
            {
                num = (ushort)(num ^ data[i]);
                for (int j = 0; j < 8; j++)
                {
                    int num2 = num & 1;
                    num = (ushort)(num >> 1);
                    if (num2 == 1)
                    {
                        num = (ushort)(num ^ divisor);
                    }
                }
            }
            return (ushort)(~num);
        }

        public static bool Crc16Check(byte[] data, ushort divisor = 40961)
        {
            if (data.Length < 3)
            {
                return false;
            }
            ushort num = (ushort)((data[data.Length - 2] << 8) | data[data.Length - 1]);
            byte[] data2 = new byte[data.Length - 2];
            return num == CreateCrc16Code(data2, divisor);
        }

        public static ushort CreateCrc16Modbus(byte[] data)
        {
            ushort num = ushort.MaxValue;
            for (int i = 0; i < data.Count(); i++)
            {
                num = (ushort)(data[i] ^ num);
                for (int j = 0; j < 8; j++)
                {
                    if (((uint)num & (true ? 1u : 0u)) != 0)
                    {
                        num = (ushort)(num >> 1);
                        num = (ushort)(num ^ 0xA001u);
                    }
                    else
                    {
                        num = (ushort)(num >> 1);
                    }
                }
            }
            return (ushort)((num >> 8) | (num << 8));
        }
    }

}
