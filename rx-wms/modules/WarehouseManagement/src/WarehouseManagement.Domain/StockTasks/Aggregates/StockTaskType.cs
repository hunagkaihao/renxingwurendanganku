using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;


namespace WarehouseManagement.StockTasks.Aggregates
{
    public class StockTaskType :  Entity<int>
    {
        /// <summary>
        /// 出入库任务表
        /// </summary>
        private StockTaskType()
        {

        }

        public string ManageTypeCode { get; set; }
        //任务类型名称
        public string ManageTypeName { get; set; }
        public string ManageTypeInout { get; set; }
        public string ManageTypeGroup { get; set; }
        public string ManageTypeClass { get; set; }
        public int ManageTypeOrder { get; set; }
        public string ManageTypeFlag { get; set; }

    }
}
