using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace WarehouseManagement.StockTasks.Dto
{
    public class DaysSumDto
    {
        /// <summary>
        /// 入库总数
        /// </summary>
        public int[] StockInSums { get; set; }
        /// <summary>
        /// 出库总数
        /// </summary>
        public int[] StockOutSums { get; set; }
        /// <summary>
        /// 日期
        /// </summary>
        public int[] Days { get; set; }

    }
}
