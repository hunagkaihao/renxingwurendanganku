using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using WarehouseManagement.Goodss.Aggregates;

namespace WarehouseManagement.Material.Aggregates
{
    public class Material : FullAuditedAggregateRoot<int>, IMultiTenant
    {
        public Guid? TenantId { get; set; }
        public Material()
        {
            GoodsClassId = 1;
            //GoodsFlag = Enums.Achive_STATUS.Created.ToString();
        }
        public GoodsClass GoodsClass;
        public int GoodsClassId { get; set; }
        public int LogicId { get; set; }
        //RFIDId
        public String RfidId { get; set; }
        //物料编码
        public string MaterialCode { get; set; }
        //物料名称
        public string MaterialName { get; set; }
        /// <summary>
        /// 物料计量单位，例如 g、kg。
        /// </summary>
        public string MaterialUnit { get; set; }
        /// <summary>
        /// 有效期天数；历史数据允许为空，0 表示不设置有效期。
        /// </summary>
        public int? ValidityDays { get; set; }
        /// <summary>
        /// 创建物料的用户工号或外部系统用户标识。
        /// </summary>
        public string CreatorUserCode { get; set; }
        /// <summary>
        /// 物料创建时间；与数据库 datetime(6) 字段一致。
        /// </summary>
        public DateTime? MaterialCreateTime { get; set; }
        //档案年份
        public string MaterialConstProperty1 { get; set; }
        //档案密级
        public string MaterialConstProperty2 { get; set; }
        //档案材质
        public string MaterialConstProperty3 { get; set; }
        public string MaterialConstProperty4 { get; set; }
        public string MaterialConstProperty5 { get; set; }
        public string MaterialConstProperty6 { get; set; }
        public string MaterialConstProperty7 { get; set; }
        public string MaterialConstProperty8 { get; set; }
        /// <summary>
        /// 开始号
        /// </summary>
        public int StartNo { get; set; }
        /// <summary>
        /// 结束号
        /// </summary>
        public int EndNo { get; set; }
        public System.Decimal GoodsLimitUpperQuantity { get; set; }
        public System.Decimal GoodsLimitLowerQuantity { get; set; }
        public string GoodsRemark { get; set; }
        public int GoodsOrder { get; set; }
        public string GoodsFlag { get; set; }
        public string GoodsColor { get; set; }
        public string GoodsSpec { get; set; }
        public string GoodsWeight { get; set; }
        public string GoodsQgp { get; set; }
        public string GoodsAreaCode { get; set; }
        //CHECK_FREQUENCY 盘点周期（天） WARNING_TIME 保质期预警期（天） SLUGGISH_TIME 呆滞期（天）
        public System.Decimal CheckFrequency { get; set; }
        public System.Decimal WarningTime { get; set; }
        public System.Decimal SluggishTime { get; set; }


        public long? CreatorUserId { get; set; }
        public long? LastModifierUserId { get; set; }
        public long? DeleterUserId { get; set; }


        public string GoodsAJCode { get; set; }
        public string GoodsNo { get; set; }
        public string ClassCode { get; set; }
        public string Director { get; set; }
        public string ChenWendate { get; set; }
        public string Pages { get; set; }
        public string RetentionPeriod { get; set; }
        public string MaterialInDept { get; set; }
        public string MaterialInDate { get; set; }
        public string StorageRemark { get; set; }
        public string KuaiJiZhuTi { get; set; }
        public string Year { get; set; }
        /// <summary>
        /// 物料类型。
        /// </summary>
        public string MaterialType { get; set; }
        /// <summary>
        /// 密级
        /// </summary>
        public string SecretLevel { get; set; }
    }
}
