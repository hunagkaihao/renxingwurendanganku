using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using Volo.Abp.Application.Dtos;

namespace WarehouseManagement.StockTasks.Dto
{
    public class StockTaskDto : AuditedEntityDto<int>
    {
        private TaskType _taskType;
        private TaskStatus _taskStatus;
        
        /// <summary>
        /// 任务类型编码
        /// </summary>
        public TaskType TaskTypeCode {
            get
            {
                return _taskType;
            }
            set
            {
                _taskType = value;
                TaskTypeCodeString = _taskType.ToString();
            }
        }
        /// <summary>
        /// 任务类型字符串
        /// </summary>
        public string TaskTypeCodeString { get; set; }
        /// <summary>
        /// 任务状态编号
        /// </summary>
        public TaskStatus TaskStatus { 
            get 
            {
                return _taskStatus;
            } 
            set 
            {
                _taskStatus = value;
                TaskStatusString = _taskStatus.ToString();
            } 
        }
        /// <summary>
        /// 任务状态字符串
        /// </summary>
        public string TaskStatusString { get; set; }
        /// <summary>
        /// 内部任务映射使用的实物条码；对外统一通过 MaterialBarcode 返回。
        /// </summary>
        [JsonIgnore]
        public string MaterialBoxBarcode { get; set; }
        /// <summary>
        /// 客户填写的实物条码；仅有基础物料码的旧预约返回空值。
        /// </summary>
        public string MaterialBarcode { get; set; }
        /// <summary>
        /// 实物关联的基础物料编码，同种物料可共用该编码。
        /// </summary>
        public string MaterialCode { get; set; }
        /// <summary>
        /// 计划ID
        /// </summary>
        public int PlanId { get; set; }
        /// <summary>
        /// 计划任务类型
        /// </summary>
        public string PlanTypeCode { get; set; }
        /// <summary>
        /// 开始库位ID
        /// </summary>
        public int StartCellId { get; set; }
        /// <summary>
        /// 开始库位编码
        /// </summary>
        public string StartCellCode { get; set; }
        /// <summary>
        /// 结束库位ID
        /// </summary>
        public int? EndCellId { get; set; }
        /// <summary>
        /// 结束库位编码
        /// </summary>
        public string EndCellCode { get; set; }
        
    }
}
