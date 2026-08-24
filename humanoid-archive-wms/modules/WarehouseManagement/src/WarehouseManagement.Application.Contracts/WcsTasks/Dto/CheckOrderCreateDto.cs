using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagement.WcsTasks.Dto
{
    public class CheckOrderCreateDto
    {
        public List<OrderDto> Orders { get; set; }

        public int Priority { get; set; }


    }
}
