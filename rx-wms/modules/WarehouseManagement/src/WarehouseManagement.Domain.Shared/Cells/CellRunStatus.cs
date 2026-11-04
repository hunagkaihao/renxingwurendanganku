using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagement.Cells
{
    public enum CellRunStatus
    {
        /// <summary>
        /// 禁用
        /// </summary>
        Disable,
        /// <summary>
        /// 待用
        /// </summary>
        Enable,
        /// <summary>
        /// 运行
        /// </summary>
        Run,
        /// <summary>
        /// 选定
        /// </summary>
        Selected
    }
}
