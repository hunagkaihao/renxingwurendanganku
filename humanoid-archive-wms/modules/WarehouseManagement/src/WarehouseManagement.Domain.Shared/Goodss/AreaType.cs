using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagement.Goodss
{
    //仪器位置状态
    public enum AreaType
    {
        /// <summary>
        /// 受理区
        /// </summary>
        CounterArea,
        /// <summary>
        /// 受理缓存区
        /// </summary>
        CounterCacheArea,
        /// <summary>
        /// 待检区
        /// </summary>
        CounterWaitingArea,
        /// <summary>
        /// 楼层等待区
        /// </summary>
        FloorWaitingArea,
    }
}
