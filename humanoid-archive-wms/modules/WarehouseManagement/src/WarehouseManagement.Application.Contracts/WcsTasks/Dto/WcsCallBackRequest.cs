using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseManagement.WcsTasks;

namespace WarehouseManagement.WcsTasks.Dto
{
    public class WcsCallBackRequest
    {
        public string OrderCode { get; set; }

        /// <summary>
        /// 任务生命周期状态，WMS 根据此字段处理库存。
        /// </summary>
        public WcsTaskStatus Status { get; set; }

        /// <summary>
        /// 当前中文执行工步，仅用于展示和诊断。
        /// </summary>
        public string ExecutionStep { get; set; }
        public string ErrorInfo { get; set; }
        public string HappenTime { get; set; }
        public string PlateCode { get; set; }
    }
}
