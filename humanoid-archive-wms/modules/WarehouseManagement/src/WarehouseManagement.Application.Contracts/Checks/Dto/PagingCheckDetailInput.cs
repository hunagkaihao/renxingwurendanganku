using Lion.AbpPro.Extension.Customs.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace WarehouseManagement.Checks.Dto
{
    public class PagingCheckDetailInput : PagingBase
    {
        public int CheckId { get; set; }
        public string Filter { get; set; }
        public string StockBarcode { get; set; }
        
    }
}
