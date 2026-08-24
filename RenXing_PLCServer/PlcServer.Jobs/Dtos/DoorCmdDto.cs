namespace PlcServer.Jobs.Dtos
{
    public class DoorCmdDto
    {
        public ushort CmdVal { get; set; } = 0;
        public ushort TaskId { get; set; } = 0;
        public ushort Reserve1 { get; set; } = 0;
        public ushort Reserve2 { get; set; } = 0;
        public ushort Reserve3 { get; set; } = 0;
        public ushort Crc { get; set; } = 0;
    }
}