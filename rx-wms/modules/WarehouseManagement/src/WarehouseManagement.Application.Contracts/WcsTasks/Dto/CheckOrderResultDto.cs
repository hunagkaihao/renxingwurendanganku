using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace WarehouseManagement.WcsTasks.Dto
{
    public class CheckOrderResultDto
    {
        public string QueryCode { get; set; }
        public string OrderCode { get; set; }
        public string CellCode { get; set; }

        /// <summary>
        /// 仅供 WCS 虚拟模式构造与账面一致的盘点回传；真实 WCS 查询不会传输该字段。
        /// </summary>
        [JsonIgnore]
        public string SimulationExpectedPlateCode { get; set; }
    }
}
