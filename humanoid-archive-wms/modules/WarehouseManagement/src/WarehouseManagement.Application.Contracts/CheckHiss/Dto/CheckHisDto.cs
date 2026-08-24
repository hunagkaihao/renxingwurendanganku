using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace WarehouseManagement.CheckHiss.Dto
{
    public class CheckHisDto: AuditedEntityDto<int>
    {
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
