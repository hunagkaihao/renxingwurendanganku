using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace WarehouseManagement.Plans.Dto
{
    public class UpdatePlanDto
    {
        [Required]
        public int Id { get; set; }
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
