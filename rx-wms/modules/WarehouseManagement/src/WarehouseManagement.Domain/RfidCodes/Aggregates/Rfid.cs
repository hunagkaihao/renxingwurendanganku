using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace WarehouseManagement.RfidCodes.Aggregates
{
    public class Rfid : FullAuditedAggregateRoot<int>, IMultiTenant
    {
        private Rfid()
        {

        }
        public Guid? TenantId { get; set; }
        [Required]
        public int RfidTypeCode { get; set; }
        [Required]
        public string RfidCode { get; set; }
        //标签状态  默认是0 1为不可用
        public string Status { get; set; }
        //打印状态  默认是0
        public string PrintStatus { get; set; }
        //写卡状态  默认是0
        public string WriteStatus { get; set; }
        /// <summary>
        /// 关联档案盒ID
        /// </summary>
        public int BoxId { get; set; }

        public Rfid(string rfidCode , int rfidTypeCode)
        {
            RfidCode = rfidCode;
            RfidTypeCode = rfidTypeCode;
            Status = "0";
            PrintStatus = "0";
            WriteStatus = "0";
        }
    }
}
