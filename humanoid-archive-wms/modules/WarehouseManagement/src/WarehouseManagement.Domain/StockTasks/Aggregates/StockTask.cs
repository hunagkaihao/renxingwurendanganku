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
        /// 出入库任务表
        /// </summary>
        private StockTask()
        {
            ManageStatus = ManageStatus.WaitingExecute;
            Details = new List<StockTaskDetail>();
        }
        public StockTask(string manageTypeCode, string archiveBoxRfid,int startCellId, int endCellId,string startCellCode, string endCellCode)
        {
            //Id = id;
            ManageTypeCode = Enum.Parse<ManageType>(manageTypeCode);
            ArchiveBoxRfid = archiveBoxRfid;
            StartCellId = startCellId;
            EndCellId = endCellId;
            StartCellCode = startCellCode;
            EndCellCode = endCellCode;
            ManageStatus = ManageStatus.WaitingExecute;
            //SetAsCompleated("Completed");
            Details = new List<StockTaskDetail>();
        }
        public StockTask(string manageTypeCode,string archiveBoxRfid)
        {
            ManageTypeCode = Enum.Parse<ManageType>(manageTypeCode);
            ArchiveBoxRfid = archiveBoxRfid;
        }
        //出库任务
        public StockTask(string manageTypeCode, string archiveBoxRfid ,string startCellCode,int startCellId)
        {
            ManageTypeCode = Enum.Parse<ManageType>(manageTypeCode);
            ArchiveBoxRfid = archiveBoxRfid;
            StartCellCode = startCellCode;
            StartCellId = startCellId;
            Details = new List<StockTaskDetail>();
        }


        public StockTask(string refTaskCode, string manageTypeCode, string archiveBoxRfid, int startCellId, int endCellId, string startCellCode, string endCellCode)
        {
            //Id = id;
            ManageLaneWay = refTaskCode;//记录MES的任务编号
            ManageTypeCode = Enum.Parse<ManageType>(manageTypeCode);
            ArchiveBoxRfid = archiveBoxRfid;
            StartCellId = startCellId;
            EndCellId = endCellId;
            StartCellCode = startCellCode;
            EndCellCode = endCellCode;
            ManageStatus = ManageStatus.Executing;
            //SetAsCompleated("Completed");
            Details = new List<StockTaskDetail>();
        }
        public StockTask(ManageType manageTypeCode, int planId,string planTypeCode, string archiveBoxRfid, int startCellId, int endCellId, string startCellCode, string endCellCode)
        {
            //Id = id;
            ManageTypeCode = manageTypeCode;
            PlanId = planId;
            PlanTypeCode = planTypeCode;
            ArchiveBoxRfid = archiveBoxRfid;
            StartCellId = startCellId;
            EndCellId = endCellId;
            StartCellCode = startCellCode;
            EndCellCode = endCellCode;
            ManageStatus = ManageStatus.WaitingExecute;
            //SetAsCompleated("Completed");
            Details = new List<StockTaskDetail>();
        }

        public void Update(string manageTypeCode, string archiveBoxRfid, int startCellId, int endCellId, string startCellCode, string endCellCode)
        {
            ManageTypeCode = Enum.Parse<ManageType>(manageTypeCode);
            ArchiveBoxRfid = archiveBoxRfid;
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
        public ManageType ManageTypeCode { get; set; }
        /// <summary>
        /// 任务状态
        /// </summary>
        public ManageStatus ManageStatus { get; set; }
        /// <summary>
        /// 料箱条码
        /// </summary>
        public string ArchiveBoxRfid { get; set; }
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
        /// 任务优先级
        /// </summary>
        public string ManageLevel { get; set; }
        /// <summary>
        /// 任务备注
        /// </summary>
        public string ManageRemark { get; set; }
        /// <summary>
        /// 任务确认时间
        /// </summary>
        public string ManageConfirmTime { get; set; }
        /// <summary>
        /// 任务巷道
        /// </summary>
        public string ManageLaneWay { get; set; }
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

        public void SetAsCompleted()
        {
            ManageStatus = ManageStatus.Complete;
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
                    ArchiveBoxRfid = ArchiveBoxRfid,
                    ManageTypeCode = ManageTypeCode,
                    StartCellId = (int)StartCellId,
                    EndCellId = (int)EndCellId,
                    ManageStatus = ManageStatus.Complete.ToString(),
                }
            );
        }
        public void SetManageStatus(ManageStatus manageStatus)
        {
            ManageStatus = manageStatus;
        }
        public void SetAsWaitingExecuted()
        {
            ManageStatus = ManageStatus.WaitingExecute;
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
                    ArchiveBoxRfid = ArchiveBoxRfid,
                    ManageTypeCode = ManageTypeCode,
                    StartCellId = (int)StartCellId,
                    EndCellId = (int)EndCellId,
                    ManageStatus = ManageStatus.WaitingExecute.ToString(),
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
            ManageStatus =  ManageStatus.Executing;
        }
        /// <summary>
        /// 设置任务为等待确认
        /// </summary>
        public void SetAsWaitingConfirm()
        {
            ManageStatus = ManageStatus.WaitingConfirm;
        }
        public void SetAsCancel()
        {
            ManageStatus = ManageStatus.Cancel;
            //ADD an EVENT TO BE PUBLISHED
            //本地事件发布
            AddLocalEvent(
                new StockTaskCanceledEto
                {
                    StockTaskId = Id,
                    ArchiveBoxRfid = ArchiveBoxRfid,
                    ManageTypeCode = ManageTypeCode,
                    StartCellId = StartCellId,
                    EndCellId = EndCellId,
                    ManageStatus = ManageStatus.Cancel.ToString(),
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
