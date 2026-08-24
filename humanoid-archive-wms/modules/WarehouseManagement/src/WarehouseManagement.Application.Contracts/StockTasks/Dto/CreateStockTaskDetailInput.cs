using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagement.StockTasks.Dto
{
    public class CreateStockTaskDetailInput
    {
        [Required]
        /// <summary>
        /// 任务ID
        /// </summary>
        public int StockTaskId { get; set; }
        /// <summary>
        /// 仓储明细ID
        /// </summary>
        public int StorageBoxDetailId { get; set; }
        /// <summary>
        /// 计划明细ID
        /// </summary>
        public int PlanListId { get; set; }
        public string GoodsBatchNo { get; set; }
        //数量默认1
        public System.Decimal ManageListQuantity { get; set; }
        //备注
        public string TaskDetailRemark { get; set; }
    }
}
