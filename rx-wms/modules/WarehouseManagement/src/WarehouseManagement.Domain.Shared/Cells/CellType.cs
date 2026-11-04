using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagement.Cells
{
    public enum CellType
    {
        /// <summary>
        /// 货位
        /// </summary>
        Cell,
        /// <summary>
        /// CTU库位
        /// </summary>
        CTUCell,
        /// <summary>
        /// 分拨墙
        /// </summary>
        WallCell,
        /// <summary>
        /// 站台/输送台
        /// </summary>
        Station,
        /// <summary>
        /// 异常站台/工位
        /// </summary>
        ErrorStation,
        /// <summary>
        /// 生产工位
        /// </summary>
        WorkStation
    }
}
