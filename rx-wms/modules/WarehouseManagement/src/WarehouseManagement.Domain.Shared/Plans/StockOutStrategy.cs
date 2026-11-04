using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagement.Plans
{
    /// <summary>
    /// 出库策略
    /// </summary>
    public enum StockOutStrategy
    {
        /// <summary>
        /// 先进先出
        /// </summary>
        FIFO,
        /// <summary>
        /// 后进先出
        /// </summary>
        LIFO,
        /// <summary>
        /// 到期先出， 系统会根据产品的到期日期的先后顺序分发产品。
        /// </summary>
        FEFO
    }
}
