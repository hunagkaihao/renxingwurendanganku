using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.EventBus;
using Volo.Abp.MultiTenancy;

namespace WarehouseManagement.StockTasks.Aggregates
{
    public class StockTaskDetailEto
    {
        public int StockTaskDetailId { get; set; }
        /// <summary>
        /// 仓储明细ID
        /// </summary>
        public int StorageBoxDetailId { get; set; }
        //计划明细ID
        public int? PlanDetailId { get; set; }
        //任务ID
        public int StockTaskId { get; set; }
        //物料ID
        public int GoodsId { get; set; }
        //数量默认1
        public decimal StorageListQuantity { get; set; }
        //数量默认1
        public System.Decimal ManageListQuantity { get; set; }
        //备注
        public string TaskDetailRemark { get; set; }
        /// <summary>
        /// 物料箱条码
        /// </summary>
        public string BoxBarcode { get; set; }
        /// <summary>
        /// 退库物料  退库物料 0  非退库物料 1
        /// 暂时未发现用途
        /// </summary>
        public int BackFlag { get; set; }

    }
}
