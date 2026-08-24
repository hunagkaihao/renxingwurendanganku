using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagement.Plans
{
    public enum PlanTypeInout
    {
        /// <summary>
        /// 入库
        /// </summary>
        In = 1,
        /// <summary>
        /// 出库
        /// </summary>
        Out = 2,
        /// <summary>
        /// 移库
        /// </summary>
        Move = 3,
        /// <summary>
        /// 拣选，回流
        /// </summary>
        Sort = 4
    }
}
