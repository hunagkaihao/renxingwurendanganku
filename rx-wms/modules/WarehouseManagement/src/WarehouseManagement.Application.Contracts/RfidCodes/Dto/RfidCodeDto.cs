using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace WarehouseManagement.RfidCodes.Dto
{
    public class RfidCodeDto : AuditedEntityDto<int>
    {
        public int RfidTypeCode { get; set; }
        public string RfidCode { get; set; }
        public string Status { get; set; }
        //打印状态  默认是0
        public string PrintStatus { get; set; }
        //写卡状态  默认是0
        public string WriteStatus { get; set; }
        /// <summary>
        /// 关联档案盒ID
        /// </summary>
        public int BoxId { get; set; }
    }
}
