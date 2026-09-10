using AutoMapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace WarehouseManagement.StockTasks.Aggregates
{
    public class StockTask :   FullAuditedAggregateRoot<int>, IMultiTenant
    {
         /// <summary>
        /// 组盘ID 暂时未用
        /// </summary>
        public int? GoodsTemplateId { get; set; }
        /// <summary>
        /// 计划ID
        /// </summary>
        public int? PlanId { get; set; }
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
        public string MaterialBoxBarcode { get; set; }
        /// <summary>
        /// 档案盒满空标识
        /// 0，空，1满
        /// </summary>
        public string FullFlag { get; set; }
        /// <summary>
        /// 库位规格
        /// 暂时无用
        /// </summary>
        public string CellModel { get; set; }
        /// <summary>
        /// 开始库位ID
        /// </summary>
        public int StartCellId { get;  set; }
        /// <summary>
        /// 开始库位编码
        /// </summary>
        public string StartCellCode { get;  set; }
        /// <summary>
        /// 结束库位ID
        /// </summary>
        public int? EndCellId { get;  set; }
        /// <summary>
        /// 结束库位编码
        /// </summary>
        public string EndCellCode { get;  set; }
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
        /// 任务优先级
        /// </summary>
        public string TaskLevel { get; set; }
        /// <summary>
        /// 任务备注
        /// </summary>
        public string TaskRemark { get; set; }
        /// <summary>
        /// 任务确认时间
        /// </summary>
        public string TaskConfirmTime { get; set; }
        /// <summary>
        /// 重量
        /// </summary>
        public decimal SumWeight { get; set; }
        /// <summary>
        /// 租户ID
        /// </summary>
        public Guid? TenantId { get; set; }
        /// <summary>
        /// 料箱存储明细
        /// </summary>
        public List<StockTaskDetail> Details { get; private set; }
        
        /// <summary>
        /// 出入库任务表
        /// </summary>
        private StockTask()
        {
            TaskStatus = TaskStatus.WaitingExecute;
            Details = new List<StockTaskDetail>();
        }
        public StockTask(string manageTypeCode, string materialBoxBarcode,int startCellId, int endCellId,string startCellCode, string endCellCode)
        {
            //Id = id;
            TaskTypeCode = Enum.Parse<TaskType>(manageTypeCode);
            MaterialBoxBarcode = materialBoxBarcode;
            StartCellId = startCellId;
            EndCellId = endCellId;
            StartCellCode = startCellCode;
            EndCellCode = endCellCode;
            TaskStatus = TaskStatus.WaitingExecute;
            //SetAsCompleated("Completed");
            Details = new List<StockTaskDetail>();
        }
        
        public StockTask(string manageTypeCode,string materialBoxBarcode)
        {
            // 任务类型
            TaskTypeCode = Enum.Parse<TaskType>(manageTypeCode);
            // 物料信息条码
            MaterialBoxBarcode = materialBoxBarcode;
        }
        //出库任务
        public StockTask(string manageTypeCode, string materialBoxBarcode ,string startCellCode,int startCellId)
        {
            TaskTypeCode = Enum.Parse<TaskType>(manageTypeCode);
            MaterialBoxBarcode = materialBoxBarcode;
            StartCellCode = startCellCode;
            StartCellId = startCellId;
            Details = new List<StockTaskDetail>();
        }
        
        public StockTask(string refTaskCode, string manageTypeCode, string materialBoxBarcode, int startCellId, int endCellId, string startCellCode, string endCellCode)
        {
            //Id = id;
            TaskTypeCode = Enum.Parse<TaskType>(manageTypeCode);
            MaterialBoxBarcode = materialBoxBarcode;
            StartCellId = startCellId;
            EndCellId = endCellId;
            StartCellCode = startCellCode;
            EndCellCode = endCellCode;
            TaskStatus = TaskStatus.Executing;
            //SetAsCompleated("Completed");
            Details = new List<StockTaskDetail>();
        }
        public StockTask(TaskType taskTypeCode, int planId,string planTypeCode, string materialBoxBarcode, int startCellId, int endCellId, string startCellCode, string endCellCode)
        {
            //Id = id;
            TaskTypeCode = taskTypeCode;
            PlanId = planId;
            PlanTypeCode = planTypeCode;
            MaterialBoxBarcode = materialBoxBarcode;
            StartCellId = startCellId;
            EndCellId = endCellId;
            StartCellCode = startCellCode;
            EndCellCode = endCellCode;
            TaskStatus = TaskStatus.WaitingExecute;
            //SetAsCompleated("Completed");
            Details = new List<StockTaskDetail>();
        }

        public void Update(string manageTypeCode, string archiveBoxRfid, int startCellId, int endCellId, string startCellCode, string endCellCode)
        {
            TaskTypeCode = Enum.Parse<TaskType>(manageTypeCode);
            MaterialBoxBarcode = archiveBoxRfid;
            StartCellId = startCellId;
            EndCellId = endCellId;
            StartCellCode = startCellCode;
            EndCellCode = endCellCode;
        }

        public void SetStartCell(int startCellId, string startCellCode)
        {
            StartCellId = startCellId;
            StartCellCode = startCellCode;
        }
        public void SetEndCell(int endCellId, string endCellCode)
        {
            EndCellId = endCellId;
            EndCellCode= endCellCode;
        }
        
        public void SetAsCompleted()
        {
            TaskStatus = TaskStatus.Complete;
            //ADD an EVENT TO BE PUBLISHED
            //分布式事件发布
            //AddDistributedEvent(
            //    new StockTaskCompletedEto
            //    {
            //        StockTaskId = Id,
            //        ManageStatus = manageStatus
            //    }
            //);

            //ADD an EVENT TO BE PUBLISHED
            //本地事件发布
            AddLocalEvent(
                new StockTaskCompletedEto
                {
                    StockTaskId = Id,
                    ArchiveBoxRfid = MaterialBoxBarcode,
                    TaskTypeCode = TaskTypeCode,
                    StartCellId = (int)StartCellId,
                    EndCellId = (int)EndCellId,
                    ManageStatus = TaskStatus.Complete.ToString(),
                }
            );
        }
        public void SetManageStatus(TaskStatus taskStatus)
        {
            TaskStatus = taskStatus;
        }
        public void SetAsWaitingExecuted()
        {
            TaskStatus = TaskStatus.WaitingExecute;
            //ADD an EVENT TO BE PUBLISHED
            //分布式事件发布
            //AddDistributedEvent(
            //    new StockTaskCompletedEto
            //    {
            //        StockTaskId = Id,
            //        ManageStatus = manageStatus
            //    }
            //);

            //ADD an EVENT TO BE PUBLISHED
            //本地事件发布
            AddLocalEvent(
                new StockTaskCompletedEto
                {
                    StockTaskId = Id,
                    ArchiveBoxRfid = MaterialBoxBarcode,
                    TaskTypeCode = TaskTypeCode,
                    StartCellId = (int)StartCellId,
                    EndCellId = (int)EndCellId,
                    ManageStatus = TaskStatus.WaitingExecute.ToString(),
                }
            );
        }

        //public List<StockTaskDetailEto> GetStockTaskDetailEtos()
        //{
        //    List<StockTaskDetailEto> stockTaskDetails = new List<StockTaskDetailEto>();
        //    foreach (StockTaskDetail detail in Details)
        //    {
        //        stockTaskDetails.Add(new StockTaskDetailEto { StockTaskDetailId = detail.Id,
        //            StorageBoxDetailId = detail.StorageBoxDetailId,
        //            StockTaskId = detail.StockTaskId,
        //            StorageListQuantity = detail.StorageListQuantity,
        //            ManageListQuantity = detail.ManageListQuantity,
        //            GoodsId = (int)detail.GoodsId,
        //            PlanDetailId = detail.PlanDetailId,

        //        });
        //    }
        //    return stockTaskDetails;
        //}

        public void SetAsExecuting()
        {
            TaskStatus =  TaskStatus.Executing;
        }
        /// <summary>
        /// 设置任务为等待确认
        /// </summary>
        public void SetAsWaitingConfirm()
        {
            TaskStatus = TaskStatus.WaitingConfirm;
        }
        public void SetAsCancel()
        {
            TaskStatus = TaskStatus.Cancel;
            //ADD an EVENT TO BE PUBLISHED
            //本地事件发布
            AddLocalEvent(
                new StockTaskCanceledEto
                {
                    StockTaskId = Id,
                    ArchiveBoxRfid = MaterialBoxBarcode,
                    TaskTypeCode = TaskTypeCode,
                    StartCellId = StartCellId,
                    EndCellId = EndCellId,
                    ManageStatus = TaskStatus.Cancel.ToString(),
                }
            );
        }
        //借阅添加明细
        public void AddDetail(int archiveBoxDetailId,int archiveId,string username)
        {
            Details.Add(new StockTaskDetail(Id, archiveBoxDetailId , archiveId ,username));
        }
        public void AddDetail(int storageBoxDetailId, int planDetailId, int goodsId, string goodsBatchNo, decimal quantity,  string taskDetailRemark)
        {
            Details.Add(new StockTaskDetail(Id, storageBoxDetailId, planDetailId, goodsId, goodsBatchNo, quantity,  taskDetailRemark));
        }

        public void AddDetail(int storageBoxDetailId, int planDetailId, int goodsId, string goodsBatchNo, decimal quantity, string goodsProperty1, string taskDetailRemark)
        {
            Details.Add(new StockTaskDetail(Id, storageBoxDetailId, planDetailId, goodsId, goodsBatchNo, quantity, goodsProperty1, taskDetailRemark));
        }

        public void RemoveDetail(int stockTaskDetailId)
        {
            var detail = Details.FirstOrDefault(item => item.Id == stockTaskDetailId);
            if (null == detail)
            {
                //throw new DataDictionaryDomainException(message: "数据字典项不存在");
            }

            Details.Remove(detail);
        }

    }
}
