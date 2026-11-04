using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace WarehouseManagement.StockTasks.Dto
{
    public class UpdateStockTaskCellDto
    {
        [Required]
        public int Id { get; set; }
        /// <summary>
        /// 开始库位ID
        /// </summary>
        public int StartCellId { get; set; }
        /// <summary>
        /// 结束库位ID
        /// </summary>
        public int EndCellId { get; set; }

    }
}
