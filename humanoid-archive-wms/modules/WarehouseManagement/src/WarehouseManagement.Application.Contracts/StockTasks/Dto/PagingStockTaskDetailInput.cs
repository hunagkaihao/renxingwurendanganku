using Lion.AbpPro.Extension.Customs.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace WarehouseManagement.StockTasks.Dto
{
    public class PagingStockTaskDetailInput : PagingBase
    {
        public int StockTaskId { get; set; }

        public int ArchiveId { get; set; }
        public string Filter { get; set; }
    }
}
