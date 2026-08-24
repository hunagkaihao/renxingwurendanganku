using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace WarehouseManagement.Checks.Aggregates
{
    public class Check : FullAuditedAggregateRoot<int>, IMultiTenant
    {
        /// <summary>
        /// 出入库记录表
        /// </summary>
        private Check()
        {
            Details = new List<CheckDetail>();
        }
        public Check(string areaCode)
        {
            //CheckCode = checkCode;
            CheckCode = "C" + DateTime.Now.ToString("yyyyMMddHHmmss");
            AreaCode = areaCode;
            CheckType = CheckType.AreaCodeAuto;
            CheckStatus = CheckStatus.Waiting;
            CreateTime = DateTime.Now.ToString();
            Details = new List<CheckDetail>();
        }
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
        /// <summary>
        /// 盘点计划明细
        /// </summary>
        public List<CheckDetail> Details { get; private set; }

        public Guid? TenantId { get; set; }

        public void AddDetail(int CheckDetailId)
        {
            if (Details.Any(e => e.Id == CheckDetailId))
            {
                //throw new DataDictionaryDomainException(message: "数据字典项已存在");
            }
            Details.Add(new CheckDetail(Id));
        }

        public void RemoveDetail(int CheckDetailId)
        {
            var detail = Details.FirstOrDefault(item => item.Id == CheckDetailId);
            if (null == detail)
            {
                //throw new DataDictionaryDomainException(message: "数据字典项不存在");
            }

            Details.Remove(detail);
        }

    }
}
