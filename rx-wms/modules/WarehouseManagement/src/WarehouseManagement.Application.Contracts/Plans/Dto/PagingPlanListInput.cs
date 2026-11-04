using Lion.AbpPro.Extension.Customs.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace WarehouseManagement.Plans.Dto
{
    public class PagingPlanListInput : PagingBase
    {
        /// <summary>
        /// 关键字
        /// </summary>
        public string Filter { get; set; }
        /// <summary>
        /// 计划编号
        /// </summary>
        public string PlanCode { get; set; }
        /// <summary>
        /// 计划类型
        /// </summary>
        public string PlanTypeCode { get; set; }
    }
}
