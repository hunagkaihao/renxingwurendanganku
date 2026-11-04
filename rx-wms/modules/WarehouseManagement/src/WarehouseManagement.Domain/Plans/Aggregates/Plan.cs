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
    public class Plan:   FullAuditedAggregateRoot<int>, IMultiTenant
    {
        /// <summary>
        /// 出入库任务表
        /// </summary>
        private Plan()
        {
            Details = new List<PlanList>();
        }
        public Plan(string planTypeCode, string planBillNo, string planBillDate, string planCreater,
    int planPriority, int planExecuteType, string planRemark)
        {
            //Id = id;
            SetProperties(planTypeCode, planBillNo, planBillDate, planCreater, planPriority, planExecuteType, planRemark);
            //SetAsCompleated("Completed");
            //后期考虑用流水号
            PlanCode = "P" + DateTime.Now.ToString("yyyyMMddHHmmss");
            PlanStatus = PlanStatus.Waiting;
            PlanCreateTime = DateTime.Now.ToString();
            Details = new List<PlanList>();
        }
        public Plan(string planTypeCode, string areaCode)
        {
            PlanTypeCode = planTypeCode;
            AreaCode = areaCode;
            //后期考虑用流水号
            PlanCode = "P" + DateTime.Now.ToString("yyyyMMddHHmmss");
            PlanStatus = PlanStatus.Waiting;
            PlanCreateTime = DateTime.Now.ToString();
        }

        public void Update(string planTypeCode, string planBillNo, string planBillDate, string planCreater,
    int planPriority, int planExecuteType, string planRemark)
        {
            SetProperties(planTypeCode, planBillNo, planBillDate, planCreater, planPriority, planExecuteType, planRemark);
        }

        public void SetProperties(string planTypeCode, string planBillNo, string planBillDate, string planCreater,
    int planPriority, int planExecuteType, string planRemark)
        {
            PlanTypeCode = planTypeCode;
            PlanBillNo = planBillNo;
            PlanBillDate = planBillDate;
            PlanCreater = planCreater;
            PlanPriority = (PlanPriority)planPriority;
            PlanExecuteType = (PlanExecuteType)planExecuteType;
            PlanRemark = planRemark;

        }
        public string AreaCode { get; set; }

        public int PlanRelativeId { get; set; }
        /// <summary>
        /// 计划编号 orderNum
        /// </summary>
        [Required]
        public string PlanCode { get; set; }
        /// <summary>
        /// 计划类型 orderType
        /// </summary>
        [Required]
        public string PlanTypeCode { get; set; }
        /// <summary>
        /// 计划创建时间 
        /// </summary>
        public string PlanCreateTime { get; set; }
        /// <summary>
        /// 计划开始时间
        /// </summary>
        public string PlanBeginTime { get; set; }
        /// <summary>
        /// 计划结束时间
        /// </summary>
        public string PlanEndTime { get; set; }
        /// <summary>
        /// 单据编号 orderNum
        /// </summary>
        public string PlanBillNo { get; set; }
        /// <summary>
        /// 计划单据日期  dateCrtText
        /// </summary>
        public string PlanBillDate { get; set; }
        /// <summary>
        /// 计划状态
        /// </summary>
        public PlanStatus PlanStatus { get; set; }
        /// <summary>
        /// 计划创建人 userCrt
        /// </summary>
        public string PlanCreater { get; set; }
        public string PlanFromDept { get; set; }
        public string PlanToDept { get; set; }
        public string PlanFromUser { get; set; }
        public string PlanToUser { get; set; }
        /// <summary>
        /// 计划确认时间
        /// </summary>
        public string PlanConfirmTime { get; set; }
        /// <summary>
        /// 确认人
        /// </summary>
        public string PlanConfirmUser { get; set; }
        /// <summary>
        /// 优先级
        /// </summary>
        public PlanPriority PlanPriority { get; set; }
        /// <summary>
        /// 计划标识 priority
        /// </summary>
        public string PlanFlag { get; set; }
        /// <summary>
        /// 执行类型（自动下达/手动下达）
        /// </summary>
        public PlanExecuteType PlanExecuteType { get; set; }
        public string PlanRemark { get; set; }

        public string PlanExparam1 { get; set; }
        public string PlanExparam2 { get; set; }
        /// <summary>
        /// 单据头自定义 1
        /// </summary>
        [StringLength(64)]
        public string HdDefineStr1 { get; set; }
        /// <summary>
        /// 单据头自定义 2
        /// </summary>
        [StringLength(64)]
        public string HdDefineStr2 { get; set; }
        /// <summary>
        /// 单据头自定义 3
        /// </summary>
        [StringLength(64)]
        public string HdDefineStr3 { get; set; }
        /// <summary>
        /// 单据头自定义 4
        /// </summary>
        [StringLength(64)]
        public string HdDefineStr4 { get; set; }
        /// <summary>
        /// 单据头自定义 5
        /// </summary>
        [StringLength(64)]
        public string HdDefineStr5 { get; set; }

        /// <summary>
        /// 租户ID
        /// </summary>
        public Guid? TenantId { get; set; }
        public List<PlanList> Details { get; private set; }

        public void AddDetail(int planId, string planBillNo, int planListPriority,
            int goodsId, string goodsCode, string goodsBatchNo,
            Decimal planListQty,
            string planListRemark)
        {
            Details.Add(new PlanList(planId, planBillNo, planListPriority, goodsId, goodsCode, goodsBatchNo, planListQty, planListRemark));
        }
        public void AddDetail(int planId, string planBillNo, int planListPriority, int goodsId, string goodsCode, string goodsBatchNo,
    Decimal planListQty, string planListRemark, string warehouseCode
            , string orderItem, string orderKeyThird, string ownerCode, string matUnit, string traceCode, string orderSubType, int priority
, string dateExpectIntoText, string dateGenText, string dateExpireText, string packFormat, string qualityStatus
, string batchNum, string batchAttr07, string batchAttr08, string batchAttr09, string batchAttr10, string batchAttr11, string batchAttr12
            , string batchAttr13, string batchAttr14, string batchAttr15, string orderStr1, string orderStr2
            , string orderStr3, string orderStr4, string orderStr5)
        {
            Details.Add(new PlanList(planId, planBillNo, planListPriority, goodsId, goodsCode, goodsBatchNo, planListQty, planListRemark
               , warehouseCode, orderItem,orderKeyThird,  ownerCode, matUnit, traceCode,  orderSubType, priority
,  dateExpectIntoText,  dateGenText,  dateExpireText,  packFormat,  qualityStatus
,  batchNum,  batchAttr07,  batchAttr08,  batchAttr09,  batchAttr10,  batchAttr11,  batchAttr12
            ,  batchAttr13,  batchAttr14,  batchAttr15,  orderStr1,  orderStr2
            ,  orderStr3,  orderStr4,  orderStr5));
        }

        public void AddDetail(int planId, string planBillNo, int planListPriority, int goodsId, string goodsCode, string goodsBatchNo,
Decimal planListQty, string planListRemark, string warehouseCode
    , string orderItem, string orderKeyThird, string ownerCode, string matUnit, string traceCode, string orderSubType, int priority
, string dateExpectIntoText, string dateGenText, string dateExpireText, string packFormat, string qualityStatus
, string batchNum, string batchAttr07, string batchAttr08, string batchAttr09, string batchAttr10, string batchAttr11, string batchAttr12
    , string batchAttr13, string batchAttr14, string batchAttr15, string orderStr1, string orderStr2
    , string orderStr3, string orderStr4, string orderStr5, string orderStr6, string orderStr7, string orderStr8
            , string orderStr9, string orderStr10, string boxCode
            , string waveCode, string consigneeCode, string carrierCode)
        {
            Details.Add(new PlanList(planId, planBillNo, planListPriority, goodsId, goodsCode, goodsBatchNo, planListQty, planListRemark
               , warehouseCode, orderItem, orderKeyThird, ownerCode, matUnit, traceCode, orderSubType, priority
, dateExpectIntoText, dateGenText, dateExpireText, packFormat, qualityStatus
, batchNum, batchAttr07, batchAttr08, batchAttr09, batchAttr10, batchAttr11, batchAttr12
            , batchAttr13, batchAttr14, batchAttr15, orderStr1, orderStr2
            , orderStr3, orderStr4, orderStr5, orderStr6, orderStr7, orderStr8, orderStr9, orderStr10,boxCode, waveCode, consigneeCode, carrierCode));
        }
        public void RemoveDetail(int PlanDetailId)
        {
            var detail = Details.FirstOrDefault(item => item.Id == PlanDetailId);
            if (null == detail)
            {
                //throw new DataDictionaryDomainException(message: "数据字典项不存在");
            }

            Details.Remove(detail);
        }

    }
}
