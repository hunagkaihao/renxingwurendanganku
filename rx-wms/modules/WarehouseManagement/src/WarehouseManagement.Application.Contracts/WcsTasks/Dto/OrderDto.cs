using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagement.WcsTasks.Dto
{
    public class OrderDto
    {
        /// <summary>扫描段唯一订单号。</summary>
        public string OrderCode { get; set; }

        /// <summary>兼容旧的单库位调用；新流程优先使用 StartCellCode。</summary>
        public string CellCode { get; set; }

        /// <summary>连续扫描段起始库位。</summary>
        public string StartCellCode { get; set; }

        /// <summary>连续扫描段终止库位。</summary>
        public string EndCellCode { get; set; }

        /// <summary>扫描段在整条盘点路线中的执行顺序。</summary>
        public int Sequence { get; set; }

        public OrderDto(){

        }
    }
}
