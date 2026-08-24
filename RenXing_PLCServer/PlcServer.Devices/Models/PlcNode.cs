namespace PlcServer.Devices.Models
{
    public class PlcNode
    {
        public int NodeId { get; set; }

        public string? NodeName { get; set; }

        public string? NodeAddr { get; set; }

        public string? NodeType { get; set; }

        public string? NodeAccess { get; set; }

        public int IsPublish { get; set; }

        public string? PlcName { get; set; }

        public string? Remark { get; set; }
    }
}
