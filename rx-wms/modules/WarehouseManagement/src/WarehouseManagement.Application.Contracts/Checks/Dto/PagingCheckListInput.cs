using Lion.AbpPro.Extension.Customs.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace WarehouseManagement.Checks.Dto
{
    public class PagingCheckListInput : PagingBase
    {
        /// <summary>
        /// 关键字
        /// </summary>
        public string Filter { get; set; }
        public DateTime StartCreationTime { get; set; }

        public DateTime EndCreationTime { get; set; }
        /// <summary>
        /// 任务状态
        /// </summary>
        public String ManageStatus { get; set; }
    }
}
