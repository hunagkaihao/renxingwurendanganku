using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagement.StockTasks
{
    /// <summary>
    /// 任务类型编码，显式指定数值以保持接口编码稳定。
    /// </summary>
    public enum TaskType
    {
        /// <summary>
        /// 物料入库
        /// </summary>
        NPFullStockIn = 0,
        /// <summary>
        /// 物料出库
        /// </summary>
        NPSortStockOut = 1,
        /// <summary>
        /// 物料借用出库
        /// </summary>
        HPSortStockOut = 2,
        /// <summary>
        /// 计划组盘入库
        /// </summary>
        HPFullStockIn = 3,
        /// <summary>
        /// 无计划补盘入库
        /// </summary>
        NPSupllyStockIn = 4,
        /// <summary>
        /// 计划补盘入库
        /// </summary>
        HPSupplyStockIn = 5,
        /// <summary>
        /// 实档案盒上架
        /// </summary>
        FullSotckUp = 6,
        /// <summary>
        /// 实档案盒下架
        /// </summary>
        FullStockDown = 7,
        /// <summary>
        /// 空档案盒入库
        /// </summary>
        EmptyStockIn = 8,
        /// <summary>
        /// 空档案盒出库
        /// </summary>
        EmptyStockOut = 9,
        /// <summary>
        /// 封存品出库
        /// </summary>
        SealedGoodsDown = 10,
        /// <summary>
        /// 无计划拣选出库
        /// </summary>
        NpFullStockOut = 11,
        /// <summary>
        /// 批量入库
        /// </summary>
        HPBatchStockIn = 12,
        /// <summary>
        /// 批量出库
        /// </summary>
        HPBatchStockOut = 13,
        /// <summary>
        /// 自动盘点下架
        /// </summary>
        HpAnnualCheckDown = 14,
        /// <summary>
        /// 盘盈入库
        /// </summary>
        SurplusIn = 15,
        /// <summary>
        /// 盘亏出库
        /// </summary>
        LossOut = 16,
        
    }
}
