using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace WarehouseManagement.Plans.Dto
{
    public class PlanDto : AuditedEntityDto<int>
    {
        /// <summary>
        /// 计划编号
        /// </summary>
        public string PlanCode { get; set; }
        public PlanExecuteType PlanExecuteType { get; set; }
        /// <summary>
        /// 计划类型
        /// </summary>
        public string PlanTypeCode { get; set; }
        /// <summary>
        /// 单据编号 orderNum
        /// </summary>
        public string PlanBillNo { get; set; }
        /// <summary>
        /// 计划单据日期  dateCrtText
        /// </summary>
        public string PlanBillDate { get; set; }
        /// <summary>
        /// 计划状态
        /// </summary>
        public PlanStatus PlanStatus { get; set; }
        //计划区域
        public string AreaCode { get; set; }

    }
}
