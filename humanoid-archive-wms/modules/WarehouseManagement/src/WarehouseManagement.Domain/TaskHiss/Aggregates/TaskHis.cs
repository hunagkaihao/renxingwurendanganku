using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using WarehouseManagement.StockTasks;
using WarehouseManagement.StockTasks.Aggregates;
using TaskStatus = WarehouseManagement.StockTasks.TaskStatus;

namespace WarehouseManagement.TaskHiss.Aggregates
{
    public class TaskHis : FullAuditedAggregateRoot<int>, IMultiTenant
    {
        /// <summary>
        /// 出入库记录表
        /// </summary>
        private TaskHis()
        {
            Details = new List<TaskHisDetail>();
        }
        public TaskHis(StockTask stockTask, List<StockTaskDetail> stockTaskDetails)
        {
            //Id = id;
            StockTaskId = stockTask.Id;
            PlanTypeCode = stockTask.PlanTypeCode;
            TaskTypeCode = stockTask.TaskTypeCode;
            TaskStatus=stockTask.TaskStatus;
            StartCellPosition = stockTask.StartCellCode;
            EndCellPosition = stockTask.EndCellCode;
            MaterialBarcode = stockTask.MaterialBoxBarcode;
            Details = new List<TaskHisDetail>();
            foreach (StockTaskDetail detail in stockTaskDetails)
            {
                Details.Add(new TaskHisDetail(Id, detail));
            }

        }

        //public void Update(string manageTypeCode, string materialBarcode)
        //{
        //    ManageTypeCode = manageTypeCode;
        //    MaterialBarcode = materialBarcode;
        //}
        public int StockTaskId { get; set; }
        /// <summary>
        /// 计划ID
        /// </summary>

        public string PlanCode { get; set; }
        /// <summary>
        /// 计划类型
        /// </summary>
        public string PlanTypeCode { get; set; }
        /// <summary>
        /// 任务类型
        /// </summary>
        public TaskType TaskTypeCode { get; set; }
        /// <summary>
        /// 任务状态
        /// </summary>
        public TaskStatus TaskStatus { get; set; }
        /// <summary>
        /// 料箱条码
        /// </summary>
        public string MaterialBarcode { get; set; }
        //开始库位
        public string StartCellPosition { get; set; }
        //结束库位
        public string EndCellPosition { get; set; }
        /// <summary>
        /// 任务操作者
        /// </summary>
        public string TaskOperator { get; set; }
        /// <summary>
        /// 任务开始时间
        /// </summary>
        public string TaskBeginTime { get; set; }
        /// <summary>
        /// 任务完成时间
        /// </summary>
        public string TaskEndTime { get; set; }
        /// <summary>
        /// 任务确认时间
        /// </summary>
        public string TaskConfirmTime { get; set; }
        /// <summary>
        /// 任务备注
        /// </summary>
        public string TaskHisRemark { get; set; }
        /// <summary>
        /// 租户ID
        /// </summary>
        public Guid? TenantId { get; set; }
        /// <summary>
        /// 料箱存储明细
        /// </summary>
        public List<TaskHisDetail> Details { get; private set; }


        public void AddDetail(int taskHisDetailId, StockTaskDetail stockTaskDetail)
        {
            if (Details.Any(e => e.Id == taskHisDetailId))
            {
                //throw new DataDictionaryDomainException(message: "数据字典项已存在");
            }
            Details.Add(new TaskHisDetail(Id,stockTaskDetail));
        }

        public void RemoveDetail(int taskHisDetailId)
        {
            var detail = Details.FirstOrDefault(item => item.Id == taskHisDetailId);
            if (null == detail)
            {
                //throw new DataDictionaryDomainException(message: "数据字典项不存在");
            }

            Details.Remove(detail);
        }

    }
}
