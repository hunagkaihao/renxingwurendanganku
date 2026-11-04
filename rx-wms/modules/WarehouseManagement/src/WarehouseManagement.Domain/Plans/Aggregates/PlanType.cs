using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;


namespace WarehouseManagement.Plans.Aggregates
{
    public class PlanType :  Entity<int>
    {
        /// <summary>
        /// 计划单据类型表
        /// </summary>
        private PlanType()
        {

        }

        //计划类型编码
        [Required]
        public string PlanTypeCode { get; set; }
        //计划类型名称
        [Required]
        public string PlanTypeName { get; set; }
        public PlanTypeGroup PlanTypeGroup { get; set; }
        public PlanTypeInout PlanTypeInout { get; set; }
        public string PlanTypeRemark { get; set; }
        public int PlanTypeOrder { get; set; }
        public string PlanTypeFlag { get; set; }
        public string PlanTypeClass { get; set; }
        public string ManageTypeCode { get; set; }
        public int CanPause { get; set; }

    }
}
