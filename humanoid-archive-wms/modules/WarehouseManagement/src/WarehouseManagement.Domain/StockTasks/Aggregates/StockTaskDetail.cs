using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using WarehouseManagement.ArchiveBoxs.Aggregates;
using WarehouseManagement.Goodss.Aggregates;
using WarehouseManagement.Plans.Aggregates;

namespace WarehouseManagement.StockTasks.Aggregates
{
    public class StockTaskDetail :   FullAuditedAggregateRoot<int>, IMultiTenant
    {
        /// <summary>
        /// 出入库任务明细表
        /// </summary>
        private StockTaskDetail()
        {
        }

        public StockTaskDetail(int goodsId)
        {
            GoodsId = goodsId;
        }
        public StockTaskDetail(int stockTaskId,int archivedetailId, int archiveId,string username)
        {
            StockTaskId = stockTaskId;
            GoodsId = archiveId;
            StorageBoxDetailId = archivedetailId;
            //BoxBarcode = archiveBoxDetail.ArchiveBoxRfid;
            Borrower = username;
        }
        public StockTaskDetail(int stockTaskId, int storageBoxDetailId, int planDetailId, int goodsId, string goodsBatchNo, decimal manageListQuantity, string goodsProperty1, string taskDetailRemark = null)
        {
            StockTaskId = stockTaskId;
            StorageBoxDetailId = storageBoxDetailId;
            PlanDetailId = planDetailId;
            GoodsId = goodsId;
            ManageListQuantity = manageListQuantity;
            GoodsBatchNo = goodsBatchNo;
            TaskDetailRemark = taskDetailRemark;
            GoodsProperty1 = goodsProperty1;
        }
        public StockTaskDetail(int stockTaskId, int storageBoxDetailId, int planDetailId, int goodsId, string goodsBatchNo, decimal manageListQuantity, string taskDetailRemark=null)
        {
            StockTaskId = stockTaskId;
            StorageBoxDetailId = storageBoxDetailId;
            PlanDetailId = planDetailId;
            GoodsId = goodsId;
            ManageListQuantity = manageListQuantity;
            GoodsBatchNo = goodsBatchNo;
            TaskDetailRemark = taskDetailRemark;

        }

        public void Update(int stockTaskId,int storageBoxDetailId, int planDetailId, int goodsId, string goodsBatchNo, decimal manageListQuantity, string taskDetailRemark = null)
        {
            StockTaskId = stockTaskId;
            StorageBoxDetailId = storageBoxDetailId;
            PlanDetailId = planDetailId;
            GoodsId = goodsId;
            GoodsBatchNo = goodsBatchNo;
            ManageListQuantity = manageListQuantity;
            TaskDetailRemark = taskDetailRemark;

        }

        public void UpdatemanageListQuantity(int stockTaskId, decimal manageListQuantity)
        {
            StockTaskId = stockTaskId;

            ManageListQuantity = manageListQuantity;
        }
        /// <summary>
        /// 仓储明细ID
        /// </summary>
        public int StorageBoxDetailId { get; set; }
        //计划明细ID
        public int? PlanDetailId { get; set; }
        //任务ID
        public int StockTaskId { get; set; }
        //物料ID
        public int? GoodsId { get; set; }
        //数量默认1
        public decimal StorageListQuantity { get; set; }
        //数量默认1
        public System.Decimal ManageListQuantity { get; set; }
        //备注
        public string TaskDetailRemark { get; set; }
        /// <summary>
        /// 物料箱条码
        /// </summary>
        public string BoxBarcode { get; set; }
        /// <summary>
        /// 退库物料  退库物料 0  非退库物料 1
        /// 暂时未发现用途
        /// </summary>
        public int BackFlag { get; set; }
        /// <summary>
        /// 租户ID
        /// </summary>
        public Guid? TenantId { get; set; }
        public string GoodsProperty1 { get; set; }
        public string Borrower { get; set; }
        public DateTime BorrowerDate { get; set; }
        public string ReturnerId { get; set; }
        public DateTime ReturnerDate { get; set; }
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

        public ManageStatus StorageListStatus { get; set; }




    }
}
