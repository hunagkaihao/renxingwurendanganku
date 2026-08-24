using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;
using WarehouseManagement.StockTasks;

namespace WarehouseManagement.TaskHiss.Dto
{
    public class TaskHisDto : AuditedEntityDto<int>
    {
        /// <summary>
        /// 任务类型
        /// </summary>
        public ManageType ManageTypeCode { get; set; }
        /// <summary>
        /// 料箱条码
        /// </summary>
        public string StockBarcode { get; set; }
        /// <summary>
        /// 开始库位ID
        /// </summary>
        public int? StartCellId { get; set; }
        /// <summary>
        /// 开始库位编码
        /// </summary>
        public string StartCellPosition { get; set; }
        /// <summary>
        /// 结束库位ID
        /// </summary>
        public int? EndCellId { get; set; }
        /// <summary>
        /// 结束库位编码
        /// </summary>
        public string EndCellPosition { get; set; }
        /// <summary>
        /// 任务状态
        /// </summary>
        public ManageStatus ManageStatus { get; set; }
        /// <summary>
        /// 任务编号
        /// </summary>
        public string ManageLaneWay { get; set; }

    }
}
