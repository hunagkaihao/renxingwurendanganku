using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wcs.WMS
{
    public class TaskStatusDto
    {
        public string OrderCode { get; set; }= string.Empty;
        public string ExecState { get; set; } = string.Empty;
        public string ErrorInfo { get; set; }=string.Empty;
        public string HappenTime { get; set; }=string.Empty;
    }
}
