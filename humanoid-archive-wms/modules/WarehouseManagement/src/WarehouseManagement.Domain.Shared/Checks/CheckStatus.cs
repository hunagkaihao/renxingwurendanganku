using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagement.Checks
{
    public enum CheckStatus
    {
        /// <summary>
        /// 等待执行
        /// </summary>
        Waiting,
        /// <summary>
        /// 执行中
        /// </summary>
        Executing,
        /// <summary>
        /// 完成
        /// </summary>
        Complete,
        /// <summary>
        /// 审核完毕
        /// </summary>
        Finish
    }
}
