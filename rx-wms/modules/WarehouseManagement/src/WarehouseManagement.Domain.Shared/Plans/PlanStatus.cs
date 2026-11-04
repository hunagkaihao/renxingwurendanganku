using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagement.Plans
{
    public enum PlanStatus
    {
        /// <summary>
        /// 待下发
        /// </summary>
        Waiting,
        /// <summary>
        /// 执行中
        /// </summary>
        Executing,
        /// <summary>
        /// 已完成
        /// </summary>
        Finish,
        /// <summary>
        /// 暂停中
        /// </summary>
        Pause,
        /// <summary>
        /// 已取消
        /// </summary>
        Cancel
    }
}
