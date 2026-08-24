using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using WarehouseManagement.StockTasks.Aggregates;

namespace WarehouseManagement.TaskHiss.Aggregates
{
    public class TaskHisDetail : FullAuditedAggregateRoot<int>, IMultiTenant
    {
        /// <summary>
        /// 出入库任务明细表
        /// </summary>
        private TaskHisDetail()
        {
        }
        public TaskHisDetail(int taskHisId, StockTaskDetail stockTaskDetail)
        {
            TaskHisId = taskHisId;
            PlanDetailId = stockTaskDetail.PlanDetailId;
            StockTaskId = stockTaskDetail.StockTaskId;
            StorageBoxDetailId = stockTaskDetail.StorageBoxDetailId;
            GoodsId = (int)stockTaskDetail.GoodsId;
            BoxBarcode = stockTaskDetail.BoxBarcode;
            StorageBoxDetailQuantity = stockTaskDetail.StorageListQuantity;
            TaskDetailQuantity = stockTaskDetail.ManageListQuantity;
            TaskHisDetailQuantity = stockTaskDetail.ManageListQuantity;
            TaskDetailHisRemark = stockTaskDetail.TaskDetailRemark;
            GoodsBatchNo = stockTaskDetail.GoodsBatchNo;
            BackFlag = stockTaskDetail.BackFlag;
            Borrower = stockTaskDetail.Borrower;
            BorrowerDate = stockTaskDetail.BorrowerDate;
            ReturnerId = stockTaskDetail.ReturnerId;
            ReturnerDate = stockTaskDetail.ReturnerDate;
        }

        public int TaskHisId { get; set; }
        /// <summary>
        /// 仓储明细ID
        /// </summary>
        public int StorageBoxDetailId { get; set; }
        //计划明细ID
        public int? PlanDetailId { get; set; }
        //任务ID
        public int StockTaskId { get; set; }
        //物料ID
        public int GoodsId { get; set; }
        /// <summary>
        /// 物料箱条码
        /// </summary>
        public string BoxBarcode { get; set; }
        public decimal StorageBoxDetailQuantity { get; set; }
        //数量默认1
        public System.Decimal TaskHisDetailQuantity { get; set; }
        //数量默认1
        public System.Decimal TaskDetailQuantity { get; set; }
        //备注
        public string TaskDetailHisRemark { get; set; }

        public string Borrower { get; set; }
        public DateTime BorrowerDate { get; set; }
        public string ReturnerId { get; set; }
        public DateTime ReturnerDate { get; set; }

        /// <summary>
        /// 退库物料  退库物料 0  非退库物料 1
        /// 暂时未发现用途
        /// </summary>
        public int BackFlag { get; set; }
        public string GoodsProperty1 { get; set; }
        public string GoodsProperty2 { get; set; }
        public string GoodsProperty3 { get; set; }
        public string GoodsProperty4 { get; set; }
        public string GoodsProperty5 { get; set; }
        public string GoodsProperty6 { get; set; }
        public string GoodsProperty7 { get; set; }
        public string GoodsProperty8 { get; set; }
        public string EntryTime { get; set; }
        public string ProductionTime { get; set; }
        public string ArrivalDate { get; set; }
        public string Supplier { get; set; }
        public string GoodsBatchNo { get; set; }
        public string InspectResult { get; set; }
        /// <summary>
        /// 租户ID
        /// </summary>
        public Guid? TenantId { get; set; }





    }
}
