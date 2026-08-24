using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;


namespace WarehouseManagement.CheckHiss.Aggregates
{
    public class CheckHis : FullAuditedAggregateRoot<int>, IMultiTenant
    {
        private CheckHis()
        {

        }
        public Guid? TenantId { get; set; }
        public string CheckCode { get; set; }
        public string CheckType { get; set; }
        public string GoodsCode { get; set; }
        public string BatchNo { get; set; }
        public string AreaCode { get; set; }
        public string Supplier { get; set; }
        public string CreateTime { get; set; }
        public string CheckStatus { get; set; }
        public string BeginTime { get; set; }
        public string FinishTime { get; set; }
        public string VerifyFinishTime { get; set; }
        public int AccuracyFlag { get; set; }
        public long? LastModifierUserId { get; set; }
        public long? DeleterUserId { get; set; }
        public long? CreatorUserId { get; set; }
    }
}
