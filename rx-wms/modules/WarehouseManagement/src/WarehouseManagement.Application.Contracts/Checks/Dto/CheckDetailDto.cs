using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace WarehouseManagement.Checks.Dto
{
    public class CheckDetailDto : AuditedEntityDto<int>
    {
        /// <summary>
        /// 档案盒、料箱条码
        /// </summary>
        public string StockBarcode { get; set; }
        /// <summary>
        /// 物料编码
        /// </summary>
        public string GoodsCode { get; set; }
        /// <summary>
        /// 物料名称
        /// </summary>
        public string GoodsName { get; set; }
        /// <summary>
        /// 盘点计划ID
        /// </summary>
        public int CheckId { get; set; }
        /// <summary>
        /// 管理任务ID
        /// </summary>
        public int ManageId { get; set; }
        /// <summary>
        /// 库位名称
        /// </summary>
        public string CellName { get; set; }
        /// <summary>
        /// 物料ID
        /// </summary>
        public int GoodsId { get; set; }
        public string Supplier { get; set; }
        /// <summary>
        /// 账目数量
        /// </summary>
        public decimal Account { get; set; }
        /// <summary>
        /// 实盘量1
        /// </summary>
        public decimal RealAmount_1 { get; set; }
        /// <summary>
        /// 实盘量2
        /// </summary>
        public decimal RealAmount_2 { get; set; }
        /// <summary>
        /// 盈亏量
        /// </summary>
        public decimal ProfitLossAmount { get; set; }
        /// <summary>
        /// 盘点员姓名
        /// </summary>
        public string Checker { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public string BeginTime { get; set; }
        /// <summary>
        /// 完成时间
        /// </summary>
        public string FinishTime { get; set; }
        /// <summary>
        /// 审核时间
        /// </summary>
        public string VerifyFinishTime { get; set; }
        /// <summary>
        /// 完成标记
        /// </summary>
        public int CompleteFlag { get; set; }
        /// <summary>
        /// 箱条码
        /// </summary>
        public string BoxBarcode { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

    }
}
