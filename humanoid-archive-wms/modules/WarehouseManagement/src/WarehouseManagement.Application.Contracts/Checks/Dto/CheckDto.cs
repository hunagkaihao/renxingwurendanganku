using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace WarehouseManagement.Checks.Dto
{
    public class CheckDto : AuditedEntityDto<int>
    {
        /// <summary>
        /// 盘点编号
        /// </summary>
        public string CheckCode { get; set; }
        /// <summary>
        /// 盘点类型
        /// </summary>
        public CheckType CheckType { get; set; }
        public string GoodsCode { get; set; }
        public string BatchNo { get; set; }
        /// <summary>
        /// 区域编码
        /// </summary>
        public string AreaCode { get; set; }
        public string Supplier { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public string CreateTime { get; set; }
        /// <summary>
        /// 盘点执行状态
        /// </summary>
        public CheckStatus CheckStatus { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public string BeginTime { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public string FinishTime { get; set; }
        /// <summary>
        /// 审核时间
        /// </summary>
        public string VerifyFinishTime { get; set; }
        /// <summary>
        /// 准确性标识
        /// </summary>
        public int AccuracyFlag { get; set; }

    }
}
