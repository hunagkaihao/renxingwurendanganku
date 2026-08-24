using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using WarehouseManagement.StockTasks;
using WarehouseManagement.StockTasks.Aggregates;

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
            ManageTypeCode = stockTask.ManageTypeCode;
            ManageStatus=stockTask.ManageStatus;
            StartCellPosition = stockTask.StartCellCode;
            EndCellPosition = stockTask.EndCellCode;
            StockBarcode =stockTask.ArchiveBoxRfid;
            Details = new List<TaskHisDetail>();
            foreach (StockTaskDetail detail in stockTaskDetails)
            {
                Details.Add(new TaskHisDetail(Id, detail));
            }

        }

        //public void Update(string manageTypeCode, string stockBarcode)
        //{
        //    ManageTypeCode = manageTypeCode;
        //    StockBarcode = stockBarcode;
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
        public ManageType ManageTypeCode { get; set; }
        /// <summary>
        /// 任务状态
        /// </summary>
        public ManageStatus ManageStatus { get; set; }
        /// <summary>
        /// 料箱条码
        /// </summary>
        public string StockBarcode { get; set; }
        //开始库位
        public string StartCellPosition { get; set; }
        //结束库位
        public string EndCellPosition { get; set; }
        /// <summary>
        /// 任务操作者
        /// </summary>
        public string ManageOperator { get; set; }
        /// <summary>
        /// 任务开始时间
        /// </summary>
        public string ManageBeginTime { get; set; }
        /// <summary>
        /// 任务完成时间
        /// </summary>
        public string ManageEndTime { get; set; }
        /// <summary>
        /// 任务确认时间
        /// </summary>
        public string ManageConfirmTime { get; set; }
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
