using System.ComponentModel.DataAnnotations;

namespace WarehouseManagement.Cells.Dto
{
    public class BindMaterialToCellDto
    {
        [Required]
        public int CellId { get; set; }

        [Required]
        public string MaterialCode { get; set; }
    }
}
