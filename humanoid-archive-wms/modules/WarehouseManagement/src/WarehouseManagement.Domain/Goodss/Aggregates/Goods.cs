using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace WarehouseManagement.Goodss.Aggregates
{
    public class Goods : FullAuditedAggregateRoot<int>, IMultiTenant
    {

        private Goods()
        { }
        public Goods(string goodsCode, string goodsName, string goodsSpec, string goodsBand, string goodsUnits)
        {
            //Id = id;
            SetProperties(goodsCode, goodsName, goodsSpec, goodsBand, goodsUnits);
            GoodsStatus = GoodsStatus.Enable;
        }

        public Goods(string goodsCode, string goodsName, string goodsSpec, string goodsBand)
        {
            //Id = id;
            SetProperties(goodsCode, goodsName, goodsSpec, goodsBand, null);
            GoodsStatus = GoodsStatus.Enable;
        }

        public Goods(string matCode, string ownerCode, string matText, string matUnit,double groWet,string matTypCode,string matGrpCode
            , string abcFlag, double minStkQty, double maxStkQty, string picUrl, string abolishFlag
            , string matStr1, string matStr2, string matStr3, string matStr4, string matStr5, string matStr6
            , string matStr7, string matStr8, string matStr9
            , string validateFlag, string validateRule, int validatePeriod, int expireWarnTime, int outPriorTime
            )
        {
            GoodsCode = matCode;
            OwnerCode = ownerCode;
            GoodsName = matText;
            GoodsUnits = matUnit;
            GroWet = groWet;
            MatTypCode = matTypCode;
            MatGrpCode = matGrpCode;
            AbcFlag = abcFlag;
            MinStkQty = minStkQty;
            MaxStkQty = maxStkQty;
            PicUrl = picUrl;
            AbolishFlag = abolishFlag;
            GoodsConstProperty1 = matStr1;
            GoodsConstProperty2 = matStr2;
            GoodsConstProperty3 = matStr3;
            GoodsConstProperty4 = matStr4;
            GoodsConstProperty5 = matStr5;
            GoodsConstProperty6 = matStr6;
            GoodsConstProperty7 = matStr7;
            GoodsConstProperty8 = matStr8;
            GoodsConstProperty9 = matStr9;
            ValidateFlag = validateFlag;
            ValidateRule = validateRule;
            ValidatePeriod = validatePeriod;
            ExpireWarnTime = expireWarnTime;
            OutPriorTime = outPriorTime;

        }
        public void SetProperties(string goodsCode, string goodsName, string goodsSpec, string goodsBand, string goodsUnits)
        {
            GoodsCode = goodsCode;
            GoodsName= goodsName;
            GoodsUnits = goodsUnits.IsNullOrEmpty()?"PCS": goodsUnits;
            GoodsSpec = goodsSpec;
            GoodsConstProperty1 = goodsBand;
            
        }
        public void Update(string goodsCode, string goodsName, string goodsSpec, string goodsBand, string goodsUnits)
        {
            SetProperties(goodsCode, goodsName, goodsSpec, goodsBand, goodsUnits);
        }

        public void UpdateGoodsStatus(GoodsStatus goodsStatus)
        {
            GoodsStatus = goodsStatus;
        }

        public Guid? TenantId { get; set; }
        /// <summary>
        /// 物料类别ID
        /// </summary>
        public int GoodsClassId { get; set; }
        /// <summary>
        /// 仪器状态
        /// </summary>
        public GoodsStatus GoodsStatus { get; set; }
        /// <summary>
        /// 逻辑区ID
        /// </summary>
        public int LogicId { get; set; }
        //RFIDId
        public String RfidId { get; set; }
        /// <summary>
        /// 物料编码  MatCode
        /// </summary>
        public string GoodsCode { get; set; }
        /// <summary>
        /// 物料名称 MatText
        /// </summary>
        public string GoodsName { get; set; }
        /// <summary>
        /// 物料规格
        /// </summary>
        public string GoodsSpec { get; set; }
        /// <summary>
        /// 物料单位 MatUnit （默认为 EA）
        /// </summary>
        public string GoodsUnits { get; set; } = "EA";
        /// <summary>
        /// 自定义 1 MatStr1
        /// </summary>
        public string GoodsConstProperty1 { get; set; }
        public string GoodsConstProperty2 { get; set; }
        public string GoodsConstProperty3 { get; set; }
        public string GoodsConstProperty4 { get; set; }
        public string GoodsConstProperty5 { get; set; }
        public string GoodsConstProperty6 { get; set; }
        public string GoodsConstProperty7 { get; set; }
        public string GoodsConstProperty8 { get; set; }
        public string GoodsConstProperty9 { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string GoodsRemark { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int GoodsOrder { get; set; }
        /// <summary>
        /// 物料标记
        /// </summary>
        public string GoodsFlag { get; set; }
        /// <summary>
        /// 物料助记色
        /// </summary>
        public string GoodsColor { get; set; }
        /// <summary>
        /// 物料重量 GroWet 物料毛重（单位 g，默认0.001）
        /// </summary>
        public double GroWet { get; set; } = 0.001;
        /// <summary>
        /// 物料区域编码
        /// </summary>
        public string GoodsAreaCode { get; set; }
        //CHECK_FREQUENCY 盘点周期（天） WARNING_TIME 保质期预警期（天） SLUGGISH_TIME 呆滞期（天）
        /// <summary>
        /// 物料盘点周期
        /// </summary>
        public System.Decimal CheckFrequency { get; set; }
        /// <summary>
        /// 货主编号（PK 主键）
        /// </summary>
        [StringLength(32)]
        public string OwnerCode { get; set; }
        /// <summary>
        /// 物料类型编号
        /// </summary>
        [StringLength(16)]
        public string MatTypCode { get; set; }
        /// <summary>
        /// 物料组编号
        /// </summary>
        [StringLength(16)]
        public string MatGrpCode { get; set; }
        /// <summary>
        /// ABC 标识
        /// </summary>
        [StringLength(16)]
        public string AbcFlag { get; set; }
        /// <summary>
        /// 最小安全库存（按主单位，        默认 1）
        /// </summary>
        public double MinStkQty { get; set; } = 1;
        /// <summary>
        /// 最大安全库存（按主单位，        默认 1）
        /// </summary>
        public double MaxStkQty { get; set; } = 1;

        /// <summary>
        /// 物料图片 url
        /// </summary>
        [StringLength(256)]
        public string PicUrl { get; set; }
        /// <summary>
        /// 作废标识（0-否，1-是，默认0）
        /// </summary>
        [StringLength(1)]
        public string AbolishFlag { get; set; } = "0";
        #region 有效期管理
        /// <summary>
        /// 有效期管控（0-不启用，1- 启用，默认为 0）
        /// </summary>
        [StringLength(1)]
        public string ValidateFlag { get; set; } = "0";
        /// <summary>
        /// 有效期计算规则（1-生产日期，2-入库日期，3-失效日期，默认为 2）
        /// </summary>
        [StringLength(1)]
        public string ValidateRule { get; set; } = "2";
        /// <summary>
        /// 有效期（单位：天，4 位整数，默认为 9999）
        /// </summary>
        [MaxLength(4)]
        public int ValidatePeriod { get; set; } = 9999;
        /// <summary>
        /// 过期预警时间（单位：天，4 位整数，默认为 0）
        /// </summary>
        [MaxLength(4)]
        public int ExpireWarnTime { get; set; } = 0;
        /// <summary>
        /// 出库提前效期（单位：天，4位整数，默认为 0）
        /// </summary>
        [MaxLength(4)]
        public int OutPriorTime { get; set; } = 0;
        #endregion
    }
}
