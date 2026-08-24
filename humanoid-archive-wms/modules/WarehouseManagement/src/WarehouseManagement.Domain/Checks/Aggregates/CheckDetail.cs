using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using WarehouseManagement.StockTasks.Aggregates;

namespace WarehouseManagement.Checks.Aggregates
{
    public class CheckDetail : FullAuditedAggregateRoot<int>, IMultiTenant
    {
        /// <summary>
        /// 出入库任务明细表
        /// </summary>
        private CheckDetail()
        {
        }
        public CheckDetail(int checkId)
        {
            CheckId = checkId;
        }
        public CheckDetail(int checkId, string stockBarcode, string cellName
            , int goodsId, decimal account)
        {
            CheckId = checkId;
            StockBarcode = stockBarcode;
            CellName = cellName;
            GoodsId = goodsId;
            Account = account;
            BeginTime = DateTime.Now.ToString();
            CompleteFlag = 0;
        }

        /// <summary>
        /// 盘点计划ID
        /// </summary>
        public int CheckId { get; set; }
        /// <summary>
        /// 管理任务ID
        /// </summary>
        public int ManageId { get; set; }
        /// <summary>
        /// 档案盒条码
        /// </summary>
        public string StockBarcode { get; set; }
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
        /// <summary>
        /// 租户ID
        /// </summary>
        public Guid? TenantId { get; set; }





    }
}
