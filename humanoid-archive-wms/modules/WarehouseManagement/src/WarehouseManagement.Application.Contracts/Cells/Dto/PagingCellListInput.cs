using Lion.AbpPro.Extension.Customs.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace WarehouseManagement.Cells.Dto
{
    public class PagingCellListInput : PagingBase
    {
        /// <summary>
        /// 关键字
        /// </summary>
        public string Filter { get; set; }

        public int CellZ { get; set; }
        /// <summary>
        /// 仓库ID
        /// </summary>
        public int WarehouseId { get; set; }
        /// <summary>
        /// 库位类型
        /// </summary>
        public string CellType { get; set; }
    }
}
