using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagement.Warehouses
{
    public enum WarehouseType
    {
        /// <summary>
        /// 立库 = 数据库主键ID
        /// </summary>
        LK = 1,
        /// <summary>
        /// 平库
        /// </summary>
        PK = 2,
        /// <summary>
        /// CTU库
        /// </summary>
        CTU = 3,
        /// <summary>
        /// 叉车库
        /// </summary>
        FK=4,
        /// <summary>
        /// 接驳
        /// </summary>
        JB = 5

    }
}
