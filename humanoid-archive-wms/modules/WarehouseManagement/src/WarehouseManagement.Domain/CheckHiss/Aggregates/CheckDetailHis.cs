using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace WarehouseManagement.CheckHiss.Aggregates
{
    public class CheckDetailHis : FullAuditedAggregateRoot<int>, IMultiTenant
    {
        private CheckDetailHis()
        {

        }
        /// <summary>
        /// 租户ID
        /// </summary>
        public Guid? TenantId { get; set; }
        //计划Id
        public int CheckId { get; set; }
        //管理任务ID
        public int ManageId { get; set; }
        //盘点备注
        public string Remark { get; set; }
        public string StockBarcode { get; set; }
        //库位名称
        public string CellName { get; set; }
        public int GoodsId { get; set; }
        public string Supplier { get; set; }
        public decimal Account { get; set; }
        public decimal RealAmount_1 { get; set; }
        public decimal RealAmount_2 { get; set; }
        public decimal ProfitLossAmount { get; set; }
        public string Checker { get; set; }
        public string BeginTime { get; set; }
        public string FinishTime { get; set; }
        public string VerifyFinishTime { get; set; }
        public int CompleteFlag { get; set; }
        public string BoxBarcode { get; set; }

        public int VerifyFlag { get; set; }
        public decimal VerifyAmount { get; set; }
        public string VerifyUser { get; set; }
        public long? LastModifierUserId { get; set; }
        public long? DeleterUserId { get; set; }
        public long? CreatorUserId { get; set; }

    }
}
