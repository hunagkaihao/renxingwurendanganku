using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagement.WcsTasks.Dto
{
    public class CheckOrderResultDto
    {
        public string QueryCode { get; set; }
        public string OrderCode { get; set; }
        public string CellCode { get; set; }
    }
}
