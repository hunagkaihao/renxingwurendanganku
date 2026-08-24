using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagement.WcsTasks.Dto
{
    public class StockOrderCreateDto
    {
        public StockOrderCreateDto(string orderCode, string plateCode, string startNode, string endNode, int priority)
        {
            OrderCode = orderCode;
            PlateCode = plateCode;
            StartNode = startNode;
            EndNode = endNode;
            Priority = priority;
        }
        public StockOrderCreateDto(){

        }
        /// <summary>
        /// 订单编号，唯一值
        /// </summary>
        public string OrderCode { get; set; }
        /// <summary>
        /// 档案盒编号
        /// </summary>
        public string PlateCode { get; set; }
        /// <summary>
        /// 档案盒起点设备码，可以是库位，也可以是取档口等
        /// </summary>
        public string StartNode { get; set; }
        /// <summary>
        /// 档案盒终点设备码，可以是库位，也可以是取档口等
        /// </summary>
        public string EndNode { get; set; }
        /// <summary>
        /// 执行优先级，值越大，优先级越高
        /// </summary>
        public int Priority { get; set; }
    }
}
