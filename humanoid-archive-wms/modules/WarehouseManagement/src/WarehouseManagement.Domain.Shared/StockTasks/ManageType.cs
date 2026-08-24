using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagement.StockTasks
{
    public enum ManageType
    {
        /// <summary>
        /// 档案盒入库
        /// </summary>
        NPFullStockIn,
        /// <summary>
        /// 计划组盘入库
        /// </summary>
        HPFullStockIn,
        /// <summary>
        /// 档案盒出库
        /// </summary>
        NPSortStockOut,
        /// <summary>
        /// 档案借阅出库
        /// </summary>
        HPSortStockOut,
        /// <summary>
        /// 无计划补盘入库
        /// </summary>
        NPSupllyStockIn,
        /// <summary>
        /// 计划补盘入库
        /// </summary>
        HPSupplyStockIn,
        /// <summary>
        /// 实档案盒上架
        /// </summary>
        FullSotckUp,
        /// <summary>
        /// 实档案盒下架
        /// </summary>
        FullStockDown,
        /// <summary>
        /// 空档案盒入库
        /// </summary>
        EmptyStockIn,
        /// <summary>
        /// 空档案盒出库
        /// </summary>
        EmptyStockOut,

        /// <summary>
        /// 封存品出库
        /// </summary>
        SealedGoodsDown,

        /// <summary>
        /// 无计划拣选出库
        /// </summary>
        NpFullStockOut,
        /// <summary>
        /// 批量入库
        /// </summary>
        HPBatchStockIn,
        /// <summary>
        /// 自动盘点下架
        /// </summary>
        HpAnnualCheckDown,
        /// <summary>
        /// 盘盈入库
        /// </summary>
        SurplusIn,
        /// <summary>
        /// 盘亏出库
        /// </summary>
        LossOut,


    }
}
