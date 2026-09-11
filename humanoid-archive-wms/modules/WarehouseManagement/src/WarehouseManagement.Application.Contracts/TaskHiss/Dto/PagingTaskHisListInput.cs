using Lion.AbpPro.Extension.Customs.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace WarehouseManagement.TaskHiss.Dto
{
    public class PagingTaskHisListInput : PagingBase
    {
        /// <summary>
        /// 关键字
        /// </summary>
        public string Filter { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime StartCreationTime { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime EndCreationTime { get; set; }
        /// <summary>
        /// 任务状态
        /// </summary>
        public String TaskStatus { get; set; }
    }
}
