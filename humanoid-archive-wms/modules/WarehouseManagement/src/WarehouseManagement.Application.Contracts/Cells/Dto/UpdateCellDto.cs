using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace WarehouseManagement.Cells.Dto
{
    public class UpdateCellDto
    {
        [Required]
        public int Id { get; set; }
        public string CellCode { get; set; }
        public string CellType { get; set; }
        public string CellName { get; set; }
        public int WarehouseId { get; set; }
    }
}
