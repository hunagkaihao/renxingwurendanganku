using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace WarehouseManagement.Plans.Aggregates
{
    public class PlanList :   FullAuditedAggregateRoot<int>, IMultiTenant
    {
        /// <summary>
        /// 出入库任务明细表
        /// </summary>
        private PlanList()
        {
        }
        public PlanList(int planId, string planBillNo, int planListPriority, 
            int goodsId, string goodsCode, string goodsBatchNo,
            Decimal planListQty,
            string planListRemark)
        {
            SetProperties(planId,planBillNo, planListPriority,goodsId, goodsCode,goodsBatchNo,planListQty,planListRemark);
            WarehouseCode = "W001";
            PlanListCreateQty = 0;
            PlanListExecuteQty = 0;
            PlanListFinishedQty = 0;
            PlanListStatus = PlanListStatus.Waiting;

        }
        public PlanList(int planId, string planBillNo, int planListPriority, int goodsId, string goodsCode, string goodsBatchNo,
    Decimal planListQty,    string planListRemark, string warehouseCode
            , string orderItem,string orderKeyThird, string ownerCode, string matUnit,string traceCode, string orderSubType, int priority
, string dateExpectIntoText, string dateGenText, string dateExpireText, string packFormat, string qualityStatus
, string batchNum, string batchAttr07, string batchAttr08, string batchAttr09, string batchAttr10, string batchAttr11, string batchAttr12
            , string batchAttr13, string batchAttr14, string batchAttr15, string orderStr1, string orderStr2
            ,string orderStr3,string orderStr4,string orderStr5)
        {
            SetProperties(planId, planBillNo, planListPriority, goodsId, goodsCode, goodsBatchNo, planListQty, planListRemark);
            WarehouseCode = warehouseCode;
            PlanListCreateQty = 0;
            PlanListExecuteQty = 0;
            PlanListFinishedQty = 0;
            PlanListStatus = PlanListStatus.Waiting;
            OrderItem = orderItem;
            OrderKeyThird = orderKeyThird;
            OwnerCode = ownerCode;
            MatUnit = matUnit;
            TraceCode = traceCode;
            OrderSubType = orderSubType;
            Priority = priority;
            DateExpectIntoText = dateExpectIntoText;
            DateGenText = dateGenText;
            DateExpireText = dateExpireText;
            PackFormat = packFormat;
            QualityStatus = qualityStatus;
            BatchNum = batchNum;
            BatchAttr07 = batchAttr07;
            BatchAttr08 = batchAttr08;
            BatchAttr09 = batchAttr09;
            BatchAttr10 = batchAttr10;
            BatchAttr11 = batchAttr11;
            BatchAttr12 = batchAttr12;
            BatchAttr13 = batchAttr13;
            BatchAttr14 = batchAttr14;
            BatchAttr15 = batchAttr15;
            OrderStr1 = orderStr1;
            OrderStr2 = orderStr2;
            OrderStr3 = orderStr3;
            OrderStr4 = orderStr4;
            OrderStr5 = orderStr5;

        }
        public PlanList(int planId, string planBillNo, int planListPriority, int goodsId, string goodsCode, string goodsBatchNo,
Decimal planListQty, string planListRemark, string warehouseCode
    , string orderItem, string orderKeyThird, string ownerCode, string matUnit, string traceCode, string orderSubType, int priority
, string dateExpectIntoText, string dateGenText, string dateExpireText, string packFormat, string qualityStatus
, string batchNum, string batchAttr07, string batchAttr08, string batchAttr09, string batchAttr10, string batchAttr11, string batchAttr12
    , string batchAttr13, string batchAttr14, string batchAttr15, string orderStr1, string orderStr2
    , string orderStr3, string orderStr4, string orderStr5
            , string orderStr6, string orderStr7, string orderStr8
            , string orderStr9, string orderStr10, string boxCode
            , string waveCode, string consigneeCode, string carrierCode)
        {
            SetProperties(planId, planBillNo, planListPriority, goodsId, goodsCode, goodsBatchNo, planListQty, planListRemark);
            WarehouseCode = warehouseCode;
            PlanListCreateQty = 0;
            PlanListExecuteQty = 0;
            PlanListFinishedQty = 0;
            PlanListStatus = PlanListStatus.Waiting;
            OrderItem = orderItem;
            OrderKeyThird = orderKeyThird;
            OwnerCode = ownerCode;
            MatUnit = matUnit;
            TraceCode = traceCode;
            OrderSubType = orderSubType;
            Priority = priority;
            DateExpectIntoText = dateExpectIntoText;
            DateGenText = dateGenText;
            DateExpireText = dateExpireText;
            PackFormat = packFormat;
            QualityStatus = qualityStatus;
            BatchNum = batchNum;
            BatchAttr07 = batchAttr07;
            BatchAttr08 = batchAttr08;
            BatchAttr09 = batchAttr09;
            BatchAttr10 = batchAttr10;
            BatchAttr11 = batchAttr11;
            BatchAttr12 = batchAttr12;
            BatchAttr13 = batchAttr13;
            BatchAttr14 = batchAttr14;
            BatchAttr15 = batchAttr15;
            OrderStr1 = orderStr1;
            OrderStr2 = orderStr2;
            OrderStr3 = orderStr3;
            OrderStr4 = orderStr4;
            OrderStr5 = orderStr5;
            OrderStr6 = orderStr6;
            OrderStr7 = orderStr7;
            OrderStr8 = orderStr8;
            OrderStr9 = orderStr9;
            OrderStr10 = orderStr10;
            BoxCode = boxCode;
            WaveCode = waveCode;
            ConsigneeCode = consigneeCode;
            CarrierCode = carrierCode;

        }

        public void Update(int planId, string planBillNo, int planListPriority,
            int goodsId, string goodsCode, string goodsBatchNo,
            Decimal planListQty,
            string planListRemark)
        {
            SetProperties(planId, planBillNo, planListPriority, goodsId, goodsCode, goodsBatchNo, planListQty, planListRemark);

        }
        public void SetProperties(int planId,string planBillNo, int planListPriority, 
            int goodsId, string goodsCode, string goodsBatchNo, 
            Decimal planListQty, 
            string planListRemark)
        {
            PlanId = planId;
            PlanBillNo = planBillNo;
            PlanListPriority = planListPriority;
            GoodsId =goodsId;
            GoodsCode=goodsCode;
            GoodsBatchNo = goodsBatchNo;
            PlanListQty = planListQty;
            PlanListRemark = planListRemark;
        }

        [Required]
        public int PlanId { get; set; }
        /// <summary>
        /// 仓库编号 whCode
        /// </summary>
        [Required]
        public string WarehouseCode { get; set; }
        /// <summary>
        /// 物料ID
        /// </summary>
        [Required]
        public int GoodsId { get; set; }
        /// <summary>
        /// 物料编码
        /// </summary>
        public string GoodsCode { get; set; }
        public string SupplierCode { get; set; }
        /// <summary>
        /// 计划数量
        /// </summary>
        public Decimal PlanListQty { get; set; }
        /// <summary>
        /// WMS任务创建数量
        /// </summary>
        public Decimal PlanListCreateQty { get; set; }
        /// <summary>
        /// WCS执行数量
        /// </summary>
        public Decimal PlanListExecuteQty { get; set; }
        /// <summary>
        /// 执行完成数量
        /// </summary>
        public Decimal PlanListFinishedQty { get; set; }
        public int PlanListPriority { get; set; }
        public PlanListStatus PlanListStatus { get; set; }
        /// <summary>
        /// 物料批号
        /// </summary>
        public string GoodsBatchNo { get; set; }
        public decimal PlanListLackQty { get; set; }
        public string ProductDate { get; set; }
        public string ArrivalDate { get; set; }
        public string InspectResult { get; set; }
        public string DeliverDate { get; set; }
        public string PlanListSendDept { get; set; }
        public string PlanListSendUser { get; set; }
        public string PlanListSendTime { get; set; }
        public string PlanListRemark { get; set; }
        public string PlanBillNo { get; set; }
        /// <summary>
        /// 租户ID
        /// </summary>
        public Guid? TenantId { get; set; }
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
