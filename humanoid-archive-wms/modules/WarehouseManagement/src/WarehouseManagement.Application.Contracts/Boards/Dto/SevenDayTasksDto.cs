using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagement.Boards.Dto
{
    public class SevenDayTasksDto
    {
        public SevenDayTasksDto(){
         
        }
        public int TotalCount { get; set; }

        public List<string> Keys { get; set; }
        public List<int> Value { get; set; }
        public List<int> Invalue { get; set; }
        public List<int> Outvalue { get; set; }
    }
}
