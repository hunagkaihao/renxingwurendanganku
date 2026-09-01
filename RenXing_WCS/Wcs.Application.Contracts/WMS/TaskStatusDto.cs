using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wcs.WMS
{
    public class TaskStatusDto
    {
        public string OrderCode { get; set; }= string.Empty;
        /// <summary>
        /// 任务生命周期状态，WMS 应根据此字段处理库存。
        /// </summary>
        public WcsTaskStatus Status { get; set; } = WcsTaskStatus.Unknown;

        /// <summary>
        /// 当前中文执行工步，仅用于展示和诊断，不用于库存判断。
        /// </summary>
        public string ExecutionStep { get; set; } = string.Empty;
        public string ErrorInfo { get; set; }=string.Empty;
        public string HappenTime { get; set; }=string.Empty;
    }
}
