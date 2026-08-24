using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagement.ArchiveBoxs.Dto
{
    public class ArchiveBoxDetailDto
    {
        public int ArchiveBoxId { get; set; }

        public int ArchiveId { get; set; }
        public string ArchiveName { get; set; }
        public string ArchiveCode { get; set; }
        public int PlanListId { get; set; }
        public System.Decimal StorageListQuantity { get; set; }
        
        public string EntryTime { get; set; }
        public string UpdateTime { get; set; }
        public string StorageListRemark { get; set; }
        public string BoxBarcode { get; set; }
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
        public DateTime CreationTime { get; set; }
        public long? LastModifierUserId { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public long? DeleterUserId { get; set; }
        public DateTime? DeletionTime { get; set; }
        public bool IsDeleted { get; set; }
    }
}
