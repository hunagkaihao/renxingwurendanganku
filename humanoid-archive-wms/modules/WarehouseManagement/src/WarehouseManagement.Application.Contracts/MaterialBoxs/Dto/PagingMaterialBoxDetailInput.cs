using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lion.AbpPro.Extension.Customs.Dtos;

namespace WarehouseManagement.MaterialBoxs.Dto
{
    public class PagingMaterialBoxDetailInput : PagingBase
    {
        public int MaterialBoxId { get; set; }
    }
}
