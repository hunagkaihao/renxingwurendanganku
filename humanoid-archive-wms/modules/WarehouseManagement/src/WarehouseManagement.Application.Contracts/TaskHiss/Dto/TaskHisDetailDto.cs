using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace WarehouseManagement.TaskHiss.Dto
{
    public class TaskHisDetailDto : AuditedEntityDto<int>
    {
        /// <summary>
        /// 档案盒、料箱条码
        /// </summary>
        public string StockBarcode { get; set; }
        /// <summary>
        /// 物料编码
        /// </summary>
        public string GoodsCode { get; set; }
        /// <summary>
        /// 物料名称
        /// </summary>
        public string GoodsName { get; set; }
        public string GoodsSpec { get; set; }
        public string GoodsBand { get; set; }
        /// <summary>
        /// 批号
        /// </summary>
        public string GoodsBatchNo { get; set; }
        /// <summary>
        /// 物料单位
        /// </summary>
        public string GoodsUnits { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public System.Decimal Quantity { get; set; }

    }
}
