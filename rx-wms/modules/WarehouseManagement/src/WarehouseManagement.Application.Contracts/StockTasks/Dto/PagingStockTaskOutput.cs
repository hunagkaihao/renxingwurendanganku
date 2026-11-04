using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace WarehouseManagement.StockTasks.Dto
{
    public class PagingStockTaskOutput : EntityDto<int>
    {
        /// <summary>
        /// 档案盒、料箱条码
        /// </summary>
        public string StockTaskBarcode { get; set; }

    }
}
