using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagement.Plans
{
    public enum PlanTypeGroup
    {
        /// <summary>
        /// 仓储类
        /// </summary>
        StoreGroup = 1,
        /// <summary>
        /// 工位类
        /// </summary>
        WorkStationGroup = 2,
        /// <summary>
        /// 生产装配类
        /// </summary>
        ProduceGroup = 3
    }
}
