using PlcServer.Devices.IDeviceServices;
using PlcServer.Devices.Models;
using Shared.Config;
using System.Configuration;

namespace PlcServer.Devices.DeviceServices.DeviceServiceByFile
{
    public class DeviceServiceInFile : IDeviceService
    {
        public List<PlcDevice> GetAllPlcDevices()
        {
            List<PlcSetting> plcSettings = Settings.ConfigData.PlcSettings;
            List<PlcDevice> plcDevices = new List<PlcDevice>();
            foreach(var plcSetting in plcSettings)
            {
                PlcDevice device = new PlcDevice();
                device.PlcId = plcSetting.PlcId;
                device.PlcName = plcSetting.PlcName;
                device.DriverAssemblyName = plcSetting.DriverAssemblyName;
                device.DriverClassName = plcSetting.DriverClassName;
                device.ConnectParameter = plcSetting.ConnectParameter;
                plcDevices.Add(device);
            }
            return plcDevices;
        }

        public List<PlcNode> GetAllPlcNodes()
        {
            List<PlcNodeSetting> plcNodeSettings = Settings.ConfigData.PlcNodeSettings;
            List<PlcNode> plcNodes = new List<PlcNode>();
            foreach (var plcNodeSetting in plcNodeSettings)
            {
                PlcNode node = new PlcNode();
                node.NodeId = plcNodeSetting.NodeId;
                node.NodeName = plcNodeSetting.NodeName;
                node.NodeAddr = plcNodeSetting.NodeAddr;
                node.NodeType = plcNodeSetting.NodeType;
                node.NodeAccess = plcNodeSetting.NodeAccess;
                node.IsPublish = plcNodeSetting.IsPublish;
                node.PlcName = plcNodeSetting.PlcName;
                node.Remark = plcNodeSetting.Remark;
                plcNodes.Add(node);
            }
            return plcNodes;
        }

        public List<PlcNode> GetAllPlcNodesInPlc(string plcName)
        {
            List<PlcNodeSetting> plcNodeSettings = Settings.ConfigData.PlcNodeSettings;
            List<PlcNode> plcNodes = new List<PlcNode>();
            foreach (var plcNodeSetting in plcNodeSettings)
            {
                if(plcNodeSetting.PlcName == plcName)
                {
                    PlcNode node = new PlcNode();
                    node.NodeId = plcNodeSetting.NodeId;
                    node.NodeName = plcNodeSetting.NodeName;
                    node.NodeAddr = plcNodeSetting.NodeAddr;
                    node.NodeType = plcNodeSetting.NodeType;
                    node.NodeAccess = plcNodeSetting.NodeAccess;
                    node.IsPublish = plcNodeSetting.IsPublish;
                    node.PlcName = plcNodeSetting.PlcName;
                    node.Remark = plcNodeSetting.Remark;
                    plcNodes.Add(node);
                }
            }
            return plcNodes;
        }
    }
}
