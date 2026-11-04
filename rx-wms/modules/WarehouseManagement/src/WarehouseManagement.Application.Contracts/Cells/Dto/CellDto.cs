using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace WarehouseManagement.Cells.Dto
{
    public class CellDto : AuditedEntityDto<int>
    {
        public string CellCode { get; set; }
        public string CellName { get; set; }
        public string CellType { get; set; }
        public string CellModel { get; set; }
        public string MaterialCode { get; set; }
        /// <summary>
        /// 库存查询返回的在库实物条码，用于区分同种物料的不同实物。
        /// 其他库位接口未填充时为空。
        /// </summary>
        public string MaterialBarcode { get; set; }
        public string DeviceCode { get; set; }
        public int Cell_x { get; set; }
        public int Cell_y { get; set; }
        public int Cell_z { get; set; }
        public string CellStatus { get; set; }
        public string RunStatus { get; set; }

    }
}
