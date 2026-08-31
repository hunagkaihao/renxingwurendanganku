using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wcs.Dispatch
{
    public enum EnumDispatchDeviceState
    {
        /// <summary>
        /// 入库中
        /// </summary>
        StockIn = 1,
        
        /// <summary>
        /// 入库调拨中
        /// </summary>
        StockInMove = 2,
        
        /// <summary>
        /// 出库中
        /// </summary>
        StockOut = 3,

        /// <summary>
        /// 出库调拨中
        /// </summary>
        StockOutMove = 4,

        /// <summary>
        /// 盘点中
        /// </summary>
        Inventory = 5,

        /// <summary>
        /// 批量入库中
        /// </summary>
        BatchStockIn = 6,

        /// <summary>
        /// 空闲中
        /// </summary>
        Idle = 7,

        /// <summary>
        /// 故障中
        /// </summary>
        Error = 8
    }
}
