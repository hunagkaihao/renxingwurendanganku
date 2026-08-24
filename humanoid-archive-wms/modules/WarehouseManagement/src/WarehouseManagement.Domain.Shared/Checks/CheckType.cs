using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagement.Checks
{
    public enum CheckType
    {
        /// <summary>
        /// 循环盘点
        /// </summary>
        CircleCheck,
        /// <summary>
        /// 年度盘点
        /// </summary>
        AnnualCheck,
        /// <summary>
        /// 货柜档案盒盘点
        /// </summary>
        HgStockCheck,
        /// <summary>
        /// 按区域自动盘点
        /// </summary>
        AreaCodeAuto
    }
}
