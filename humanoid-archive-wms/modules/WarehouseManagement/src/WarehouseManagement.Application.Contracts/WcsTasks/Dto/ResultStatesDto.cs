using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagement.WcsTasks.Dto
{
    public class ResultStatesDto
    {
        public string OrderCode { get; set; }
        public string ExecState { get; set; }
        public string ErrorInfo { get; set; }
        public string HappenTime { get; set; }


    }
}
