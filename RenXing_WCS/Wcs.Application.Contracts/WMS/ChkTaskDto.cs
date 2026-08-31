using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wcs.WMS
{
    public class ChkTaskDto
    {
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        public string CheckTaskStatus { get; set; } = "0";
    } 
}
