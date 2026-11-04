using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace WarehouseManagement.Plans.Dto
{
    public class PlanDetailDto : AuditedEntityDto<int>
    {
        /// <summary>
        /// 物料编码
        /// </summary>
        public string GoodsCode { get; set; }
        /// <summary>
        /// 物料批号
        /// </summary>
        public string GoodsBatchNo { get; set; }
        /// <summary>
        /// 计划数量
        /// </summary>
        public Decimal PlanListQty { get; set; }
        public string PlanListRemark { get; set; }

    }
}
