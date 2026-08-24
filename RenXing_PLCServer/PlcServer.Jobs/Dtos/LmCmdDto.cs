namespace PlcServer.Jobs.Dtos
{
    public class LmCmdDto
    {
        public ushort CmdVal { get; set; } = 0;
        public ushort TaskId { get; set; } = 0;
        public ushort RowVal { get; set; } = 0;
        public ushort ColVal { get; set; } = 0;
        public ushort LayerVal { get; set; } = 0;
        public ushort CacheNo { get; set; } = 0;
        public ushort DoorNo { get; set; } = 0;
        public ushort Reserve1 { get; set; } = 0;
        public ushort Reserve2 { get; set; } = 0;
        public ushort Reserve3 { get; set; } = 0;
        public ushort BarcodeH { get; set; } = 0;
        public ushort BarcodeL { get; set; } = 0;
        public ushort Crc { get; set; } = 0;
    }
}