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

        /// <summary>
        /// 现场扫描状态；WMS 必须结合冻结账面快照生成最终盘点结论。
        /// </summary>
        public WcsCheckCellStatus status { get; set; } = WcsCheckCellStatus.Unknown;

        /// <summary>PLC/扫码器读取的现场实际条码。</summary>
        public string plateCode { get; set; }
    }
}
