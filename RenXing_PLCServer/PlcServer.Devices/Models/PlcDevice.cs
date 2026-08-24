namespace PlcServer.Devices.Models
{
    public class PlcDevice
    {
        public int PlcId { get; set; }
        
        public string? PlcName { get; set; }

        public string? DriverAssemblyName { get; set; }

        public string? DriverClassName { get; set; }

        public string? ConnectParameter { get; set; }
    }
}
