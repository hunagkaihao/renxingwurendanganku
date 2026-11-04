using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagement.Plans
{
    public class PlanListDto
    {
        /// <summary>
        /// 物料编码
        /// </summary>
        public string GoodsCode { get; set; }
        /// <summary>
        /// 物料批号
        /// </summary>
        public string GoodsBatchNo { get; set; }
        /// <summary>
        /// 计划数量
        /// </summary>
        public Decimal PlanListQty { get; set; }
        public string WarehouseCode { get; set; }
        public string PlanListRemark { get; set; }
        #region 海康出入库单据
        /// <summary>
        /// 单据项目（PK 主键）
        /// </summary>
        [StringLength(32)]
        public string OrderItem { get; set; }
        /// <summary>
        /// 单据子项（PK 主键，默认为 0，备用字 段）
        /// </summary>
        [StringLength(32)]
        public string OrderKeyThird { get; set; } = "0";
        /// <summary>
        /// 货主编号
        /// </summary>
        [StringLength(16)]
        public string OwnerCode { get; set; }
        /// <summary>
        /// 单位
        /// </summary>
        [StringLength(16)]
        public string MatUnit { get; set; }
        /// <summary>
        /// 容器号/档案盒号/跟踪号/LPN
        /// </summary>
        [StringLength(16)]
        public string TraceCode { get; set; }
        /// <summary>
        /// 单据子类
        /// </summary>
        [StringLength(16)]
        public string OrderSubType { get; set; }
        /// <summary>
        /// 优先级（1-人工处理，2-超时，默认普通 3
        /// </summary>
        public int Priority { get; set; }
        /// <summary>
        /// 预期到货时间（格式“yyyy-MM-dd  HH:mm:ss”，为空则默认为推送时的系统时间）
        /// </summary>
        [StringLength(32)]
        public string DateExpectIntoText { get; set; }
        /// <summary>
        /// 批次属性 01（生产日期，默认为推送时的系统时间，格式是 yyyy-MM-dd）
        /// </summary>
        [StringLength(32)]
        public string DateGenText { get; set; }
        /// <summary>
        /// 批次属性 03（失效日期，默认为生产日期+有效期，格式是 yyyy-MM-dd）
        /// </summary>
        [StringLength(32)]
        public string DateExpireText { get; set; }
        /// <summary>
        /// 批次属性 04（包装规格，1/S/M/L，S 为小包装数量，M 为中包装数量，L 为大
        /// 包装数量，如 1/20/200/0，默认从物料包装中获取）
        /// </summary>
        [StringLength(32)]
        public string PackFormat { get; set; }
        /// <summary>
        /// 批次属性 05（质检状态，F-合格，B-不良，S-冻结，X-质检，默认为 F）
        /// </summary>
        [StringLength(64)]
        public string QualityStatus { get; set; } = "F";
        /// <summary>
        /// 批次属性 06（外部批次）
        /// </summary>
        [StringLength(64)]
        public string BatchNum { get; set; }
        /// <summary>
        /// 批次属性 07
        /// </summary>
        [StringLength(64)]
        public string BatchAttr07 { get; set; }
        /// <summary>
        /// 批次属性 08
        /// </summary>
        [StringLength(64)]
        public string BatchAttr08 { get; set; }
        /// <summary>
        /// 批次属性 09
        /// </summary>
        [StringLength(64)]
        public string BatchAttr09 { get; set; }
        /// <summary>
        /// 批次属性 10
        /// </summary>
        [StringLength(64)]
        public string BatchAttr10 { get; set; }
        /// <summary>
        /// 批次属性 11
        /// </summary>
        [StringLength(64)]
        public string BatchAttr11 { get; set; }
        /// <summary>
        /// 批次属性 12
        /// </summary>
        [StringLength(64)]
        public string BatchAttr12 { get; set; }
        /// <summary>
        /// 批次属性 13
        /// </summary>
        [StringLength(64)]
        public string BatchAttr13 { get; set; }
        /// <summary>
        /// 批次属性 14
        /// </summary>
        [StringLength(64)]
        public string BatchAttr14 { get; set; }
        /// <summary>
        /// 批次属性 15
        /// </summary>
        [StringLength(64)]
        public string BatchAttr15 { get; set; }
        /// <summary>
        /// 自定义 1
        /// </summary>
        [StringLength(64)]
        public string OrderStr1 { get; set; }
        /// <summary>
        /// 自定义 2
        /// </summary>
        [StringLength(64)]
        public string OrderStr2 { get; set; }
        /// <summary>
        /// 自定义 3
        /// </summary>
        [StringLength(64)]
        public string OrderStr3 { get; set; }
        /// <summary>
        /// 自定义 4
        /// </summary>
        [StringLength(64)]
        public string OrderStr4 { get; set; }
        /// <summary>
        /// 自定义 5
        /// </summary>
        [StringLength(64)]
        public string OrderStr5 { get; set; }
        /// <summary>
        /// 自定义 6
        /// </summary>
        [StringLength(64)]
        public string OrderStr6 { get; set; }
        /// <summary>
        /// 自定义 7
        /// </summary>
        [StringLength(64)]
        public string OrderStr7 { get; set; }
        /// <summary>
        /// 自定义 8
        /// </summary>
        [StringLength(64)]
        public string OrderStr8 { get; set; }
        /// <summary>
        /// 自定义 9
        /// </summary>
        [StringLength(64)]
        public string OrderStr9 { get; set; }
        /// <summary>
        /// 自定义 10
        /// </summary>
        [StringLength(64)]
        public string OrderStr10 { get; set; }
        /// <summary>
        /// 箱号（多个以英文逗号隔开）
        /// </summary>
        public string BoxCode { get; set; }
        /// <summary>
        /// 波次编号
        /// </summary>
        [StringLength(32)]
        public string WaveCode { get; set; }
        /// <summary>
        /// 收货人
        /// </summary>
        [StringLength(32)]
        public string ConsigneeCode { get; set; }
        /// <summary>
        /// 承运商
        /// </summary>
        [StringLength(32)]
        public string CarrierCode { get; set; }
        #endregion
    }
}
