using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagement.StockTasks.Dto
{
    public class PickOutDto
    {
        //档案盒Id
        [Required]
        public int ArchiveBoxId { get; set; }
        //档案Id
        [Required]
        public int ArchiveId { get; set; }
        //用户Id
        [Required]
        public int Userid { get; set; }
    }
}
