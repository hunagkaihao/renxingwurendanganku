using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlcServer.Jobs.Dtos
{
    public class MoverCmdDto
    {
        public ushort CmdVal { get; set; } = 0;
        public ushort TaskId { get; set; } = 0;
        public ushort Reserve1 { get; set; } = 0;
        public ushort Crc { get; set; } = 0;
    }
}
