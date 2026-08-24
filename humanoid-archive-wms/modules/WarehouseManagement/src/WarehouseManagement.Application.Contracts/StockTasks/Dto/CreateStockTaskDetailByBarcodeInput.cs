using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagement.StockTasks.Dto
{
    public class CreateStockTaskDetailByBarcodeInput
    {
        [Required]
        /// <summary>
        /// 档案盒ID
        /// </summary>
        public string StorageBoxBarCode { get; set; }
        [Required]
        /// <summary>
        /// 任务ID
        /// </summary>
        public int StockTaskId { get; set; }
        //数量默认1
        public System.Decimal ManageListQuantity { get; set; }
    }
}
