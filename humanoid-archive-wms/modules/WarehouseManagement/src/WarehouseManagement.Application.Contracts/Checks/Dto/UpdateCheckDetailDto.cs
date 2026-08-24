using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace WarehouseManagement.Checks.Dto
{
    public class UpdateCheckDetailDto
    {
        [Required]
        public int Id { get; set; }
        public decimal RealAmount_1 { get; set; }


    }
}
