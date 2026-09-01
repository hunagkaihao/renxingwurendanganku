using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseManagement.WcsTasks;

namespace WarehouseManagement.WcsTasks.Dto
{
    public class ResultStatesDto
    {
        public string OrderCode { get; set; }
        /// <summary>
        /// 任务生命周期状态，库存处理应根据此字段判断。
        /// </summary>
        public WcsTaskStatus Status { get; set; }

        /// <summary>
        /// 当前中文执行工步，仅用于展示和诊断。
        /// </summary>
        public string ExecState { get; set; }
        public string ErrorInfo { get; set; }
        public string HappenTime { get; set; }


    }
}
