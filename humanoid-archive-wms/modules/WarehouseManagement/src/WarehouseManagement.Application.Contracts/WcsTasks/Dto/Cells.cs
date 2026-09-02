using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseManagement.WcsTasks;

namespace WarehouseManagement.WcsTasks.Dto
{
    public class Cells
    {
        public string OrderCode { get; set; }
        public string CellCode { get; set; }

        /// <summary>
        /// WCS 现场采集状态，WMS 根据该状态区分等待、空库位、扫码成功和异常。
        /// </summary>
        public WcsCheckCellStatus Status { get; set; }

        /// <summary>
        /// PLC/扫码器读取的现场实际条码，不是 WMS 账面库存值。
        /// </summary>
        public string PlateCode { get; set; }
    }
}
