using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lion.AbpPro.Extension.Customs.Dtos;

namespace WarehouseManagement.CheckHiss.Dto
{
    public class PagingCheckDetailHisDto : PagingBase
    {
        public int CheckId { get; set; }

    }
}
