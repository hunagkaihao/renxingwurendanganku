using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace WarehouseManagement.Cells.Dto
{
    public class PagingCellOutput : EntityDto<int>
    {
        public string CellCode { get; set; }
        public string CellName { get; set; }
        public string CellType { get; set; }
        public string DeviceCode { get; set; }
        public int Cell_z { get; set; }
        public int Cell_x { get; set; }
        public int Cell_y { get; set; }

    }
}
