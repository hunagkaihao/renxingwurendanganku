using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;

namespace WarehouseManagement.MaterialBoxs.Aggregates
{
    public class MaterialBoxDetail : FullAuditedAggregateRoot<int>
    {
        private MaterialBoxDetail()
        {

        }

        public MaterialBoxDetail(int MaterialBoxId, int MaterialId)
        {
            this.MaterialBoxId = MaterialBoxId;
            this.MaterialId = MaterialId;
        }
        public int MaterialBoxId { get; set; }
        public int PlanListId { get; set; }
        public System.Decimal StorageListQuantity { get; set; }
        public int MaterialId { get; set; }
        public string EntryTime { get; set; }
        public string UpdateTime { get; set; }
        public string StorageListRemark { get; set; }
        public string ArchiveBoxRfid { get; set; }
        public string GoodsProperty1 { get; set; }
        public string GoodsProperty2 { get; set; }
        public string GoodsProperty3 { get; set; }
        public string GoodsProperty4 { get; set; }
        public string GoodsProperty5 { get; set; }
        public string GoodsProperty6 { get; set; }
        public string GoodsProperty7 { get; set; }
        public string GoodsProperty8 { get; set; }
        public int BackFlag { get; set; }
        public string GoodsBatchNo { get; set; }
        public string ProductionTime { get; set; }
        public string InspectResult { get; set; }
        public string Supplier { get; set; }
        public string ArrivalDate { get; set; }
        public int StorageListStatus { get; set; }
        public long? CreatorUserId { get; set; }
        public long? LastModifierUserId { get; set; }
        public long? DeleterUserId { get; set; }

    }
}
