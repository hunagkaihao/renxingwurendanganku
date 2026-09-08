using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lion.AbpPro.Extension.Customs.Dtos;

namespace WarehouseManagement.Material.Dto
{
    public class PagingMaterialListInput : PagingBase
    {
        public string Filter { get; set; }
    }
}
