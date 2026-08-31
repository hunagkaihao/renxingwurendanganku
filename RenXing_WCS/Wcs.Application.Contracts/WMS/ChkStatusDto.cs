using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wcs.WMS
{
    public class ChkStatusDto
    {
        public string orderCode { get; set; }= string.Empty;
        public string execState { get; set; } = string.Empty;
        public string errorInfo { get; set; }=string.Empty;
        public string happenTime { get; set; }=string.Empty;
        public ResultsDto resultsDto { get; set; }=new ResultsDto();
    }

    public class ResultsDto
    {
        public List<Cell> cells { get; set; } = new List<Cell>();
    }
    public class Cell
    {
        public string orderCode { get; set; }
        public string cellCode { get; set; }
        public string plateCode { get; set; }
    }
}
