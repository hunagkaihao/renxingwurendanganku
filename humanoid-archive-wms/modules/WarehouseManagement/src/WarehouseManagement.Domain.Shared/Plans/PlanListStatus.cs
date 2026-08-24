using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagement.Plans
{
    public enum PlanListStatus
    {
        /// <summary>
        /// 待下发（计划创建，还未生成任务）
        /// </summary>
        Waiting,
        /// <summary>
        /// 待下达（任务生成，还未下达给WCS）
        /// </summary>
        WaitingSend,
        /// <summary>
        /// 待执行（已经下达给WCS，WCS还未分解）
        /// </summary>
        WaitingExecute,
        /// <summary>
        /// 执行中（堆垛机已经在执行）
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
