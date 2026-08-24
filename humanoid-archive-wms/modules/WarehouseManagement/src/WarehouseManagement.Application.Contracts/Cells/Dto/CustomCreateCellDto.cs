using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace WarehouseManagement.Cells.Dto
{
    public class CustomCreateCellDto
    {
        [Required]
        public string CellCode { get; set; }
        public string CellType { get; set; }
        public string CellName { get; set; }
        public string DeviceCode { get; set; }
        public int Cell_z { get; set; }
        public int Cell_x { get; set; }
        public int Cell_y { get; set; }

        public string CellGroup { get; set; }
        public string CellStorageType { get; set; }
        public string CustomCode { get; set; }
        public string CellModel { get; set; }
        public int WarehouseId { get; set; }

    }
}
