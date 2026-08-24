using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagement.Cells
{
    public enum CellStatus
    {
        /// <summary>
        /// 满货
        /// </summary>
        Full,
        /// <summary>
        /// 有货
        /// </summary>
        Have,
        /// <summary>
        /// 无货
        /// </summary>
        Nohave,
        /// <summary>
        /// 空档案盒
        /// </summary>
        Pallet,
        /// <summary>
        /// 异常货位
        /// </summary>
        Exception,

        /// <summary>
        /// 禁用
        /// </summary>
        Forbiden
    }
}
