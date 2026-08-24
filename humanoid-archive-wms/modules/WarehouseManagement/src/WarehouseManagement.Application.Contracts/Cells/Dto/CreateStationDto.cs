using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace WarehouseManagement.Cells.Dto
{
    public class CreateStationDto
    {
        private const string CellRegex = @"^\d{5}$";
        [Required]
        [RegularExpression(CellRegex)]//库位编码验证
        public string CellCode { get; set; }
        public string CellType { get; set; }
        public string CellName { get; set; }

        public int WarehouseId { get; set; }

    }
}
