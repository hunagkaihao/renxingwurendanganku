using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lion.AbpPro.Extension.Customs.Dtos;

namespace WarehouseManagement.ArchiveBoxs.Dto
{
    public class PagingArchiveBoxDetailInput : PagingBase
    {
        public int ArchiveBoxId { get; set; }
    }
}
