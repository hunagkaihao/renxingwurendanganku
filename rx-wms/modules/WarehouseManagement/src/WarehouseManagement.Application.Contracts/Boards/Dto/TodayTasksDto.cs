using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagement.Boards.Dto
{
    public class TodayTasksDto
    {
        public TodayTasksDto()
        { }
        //入库数量
        public int TodayInct { get; set; }
        //借阅数量
        public int TodayOutCt { get; set; }

    }
}
