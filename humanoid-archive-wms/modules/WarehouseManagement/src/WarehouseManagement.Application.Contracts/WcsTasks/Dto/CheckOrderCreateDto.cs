using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagement.WcsTasks.Dto
{
    public class CheckOrderCreateDto
    {
        /// <summary>
        /// 整个盘点计划共享的结果查询码。
        /// </summary>
        public string QueryCode { get; set; }

        /// <summary>
        /// WMS 根据全库或分区库位生成的连续扫描段。
        /// </summary>
        public List<OrderDto> Orders { get; set; }

        /// <summary>盘点计划调度优先级。</summary>
        public int Priority { get; set; }


    }
}
