using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lion.AbpPro.Extension.Customs.Dtos;

namespace WarehouseManagement.MaterialBoxs.Dto
{
    public class PagingMaterialBoxListInput : PagingBase
    {
        /// <summary>
        /// 关键字
        /// </summary>
        public string Filter { get; set; }

        /// <summary>
        /// 创建时间起点（含）。
        /// </summary>
        public DateTime? StartCreationTime { get; set; }

        /// <summary>
        /// 创建时间终点（含）。
        /// </summary>
        public DateTime? EndCreationTime { get; set; }

    }
}
