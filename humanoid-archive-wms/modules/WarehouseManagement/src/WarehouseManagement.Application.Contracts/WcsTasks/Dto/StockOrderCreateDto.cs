using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagement.WcsTasks.Dto
{
    public class StockOrderCreateDto
    {
        public StockOrderCreateDto(string orderCode, string plateCode, string startNode, string endNode,
            string taskType, int priority)
        {
            OrderCode = orderCode;
            PlateCode = plateCode;
            StartNode = startNode;
            EndNode = endNode;
            TaskType = taskType;
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
        /// WMS 传给 WCS 的任务类型。当前 WCS 接收但仍按起点和终点推导实际类型。
        /// </summary>
        public string TaskType { get; set; }
        /// <summary>
        /// 执行优先级，值越大，优先级越高
        /// </summary>
        public int Priority { get; set; }
    }
}
