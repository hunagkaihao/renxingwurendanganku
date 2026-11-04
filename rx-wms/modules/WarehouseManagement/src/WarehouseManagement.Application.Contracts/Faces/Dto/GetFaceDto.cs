using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagement.Faces.Dto
{
    public class face 
    {
        public string UserId { get; set; }
        public string ImageDate { get; set; }
    }



    public class GetFaceDto
    {

        public bool Success { get; set; }

        public face Face { get; set; }  
        public string Error { get; set; }


    }
}
