using PlcServer.Devices.Models;

namespace PlcServer.Devices.IDeviceServices
{
    public interface IDeviceService
    {
        public List<PlcDevice> GetAllPlcDevices();

        public List<PlcNode> GetAllPlcNodes();

        public List<PlcNode> GetAllPlcNodesInPlc(string plcName);
    }
}