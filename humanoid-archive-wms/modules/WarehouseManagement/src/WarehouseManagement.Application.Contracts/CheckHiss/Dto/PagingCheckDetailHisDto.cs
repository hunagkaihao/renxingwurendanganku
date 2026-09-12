using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lion.AbpPro.Extension.Customs.Dtos;

namespace WarehouseManagement.CheckHiss.Dto
{
    public class PagingCheckDetailHisDto : PagingBase
    {
        /// <summary>
        /// 盘点历史明细创建时间筛选起点（含）。
        /// </summary>
        public DateTime? StartCreationTime { get; set; }

        /// <summary>
        /// 盘点历史明细创建时间筛选终点（含）。
        /// </summary>
        public DateTime? EndCreationTime { get; set; }
    }
}
