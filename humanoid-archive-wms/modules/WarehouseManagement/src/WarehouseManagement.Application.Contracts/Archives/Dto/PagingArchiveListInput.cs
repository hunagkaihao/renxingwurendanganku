using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lion.AbpPro.Extension.Customs.Dtos;

namespace WarehouseManagement.Archives.Dto
{
    public class PagingArchiveListInput : PagingBase
    {
        public string Filter { get; set; }
    }
}
