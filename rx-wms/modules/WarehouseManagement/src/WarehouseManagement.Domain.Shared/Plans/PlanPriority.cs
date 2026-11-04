using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagement.Plans
{
    public enum PlanPriority
    {
        /// <summary>
        /// 滞后
        /// </summary>
        Delay = 0,
        /// <summary>
        /// 正常
        /// </summary>
        Normal = 1,
        /// <summary>
        /// 紧急
        /// </summary>
        Urgent = 2
    }
}
