using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace WarehouseManagement.Plans.Dto
{
    public class CreatePlanDto
    {
        /// <summary>
        /// 计划类型
        /// </summary>
        public string PlanTypeCode { get; set; }
        /// <summary>
        /// 计划单据编号
        /// </summary>
        public string PlanBillNo { get; set; }
        /// <summary>
        /// 计划单据日期
        /// </summary>
        public string PlanBillDate { get; set; }
        /// <summary>
        /// 计划创建人
        /// </summary>
        public string PlanCreater { get; set; }
        /// <summary>
        /// 优先级
        /// </summary>
        public int PlanPriority { get; set; }
        /// <summary>
        /// 执行类型（自动下达/手动下达）
        /// </summary>
        public int PlanExecuteType { get; set; }
        /// <summary>
        /// 计划备注
        /// </summary>
        public string PlanRemark { get; set; }
        /// <summary>
        /// 计划区域
        /// </summary>
        public string AreaCode { get; set; }


    }
}
