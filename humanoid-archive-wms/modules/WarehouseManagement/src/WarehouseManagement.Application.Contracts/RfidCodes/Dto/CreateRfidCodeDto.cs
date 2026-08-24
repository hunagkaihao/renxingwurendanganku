using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagement.RfidCodes.Dto
{
    public class CreateRfidCodeDto
    {
        //[Required]
        public int RfidTypeCode { get; set; }
        //[Required]
        public string RfidCode { get; set; }
        public int Id { get; set; }
    }
}
