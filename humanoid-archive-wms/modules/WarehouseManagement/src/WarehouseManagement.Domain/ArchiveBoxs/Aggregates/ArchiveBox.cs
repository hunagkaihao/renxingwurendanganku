using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace WarehouseManagement.ArchiveBoxs.Aggregates
{
    public class ArchiveBox : FullAuditedAggregateRoot<int>, IMultiTenant
    {
        private ArchiveBox()
        {
            Details = new List<ArchiveBoxDetail>();
        }
        public void Update(string archiveBoxName, string stockBarcode)
        {
            ArchiveBoxName = archiveBoxName;
            StockBarcode = stockBarcode;
        }

        public ArchiveBox(string archiveBoxName, string stockBarcode)
        {
            //Id = id;
            ArchiveBoxName = archiveBoxName;
            StockBarcode = stockBarcode;
            Details = new List<ArchiveBoxDetail>();
        }
        public void SetCell(int cellId)
        {
            CellId = cellId;
            Log.Warning($"Box:{this.ArchiveBoxRfid} is SetCell Cell:{cellId}。Method：{System.Reflection.MethodBase.GetCurrentMethod().Name}");
        }
        //档案盒名称
        [Required]
        public string ArchiveBoxName { get; set; }
        public string ArchiveBoxRfid { get; set; }
        //档案盒编码
        public string StockBarcode { get; set; }
        //库存状态 0，空，1满
        public string FullFlag { get; set; }
        //档案盒备注
        public string StorageRemark { get; set; }
        //库位Id
        public int CellId { get; set; }
        public long? CreatorUserId { get; set; }
        public long? DeleterUserId { get; set; }
        public long? LastModifierUserId { get; set; }
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
        /// 年度
        /// </summary>
        public int Year { get; set; }
        /// <summary>
        /// 密级
        /// </summary>
        public string SecretLevel { get; set; }
        /// <summary>
        /// 页数
        /// </summary>
        public string Pages { get; set; }
        /// <summary>
        /// 分类编号
        /// </summary>
        public string ClassCode { get; set; }
        /// <summary>
        /// 分类名称
        /// </summary>
        public string ClassName { get; set; }
        /// <summary>
        /// 归档日期
        /// </summary>
        public string AchieveInDate { get; set; }
        /// <summary>
        /// 归档部门
        /// </summary>
        public string AchieveInDept { get; set; }
        /// <summary>
        /// 移交人 归档人
        /// </summary>
        public string Achiever { get; set; }
        /// <summary>
        /// 责任人
        /// </summary>
        public string Director { get; set; }
        /// <summary>
        /// 目录号
        /// </summary>
        public string CatalogNo { get; set; }
        /// <summary>
        /// 保管期限
        /// </summary>
        public string RetentionPeriod { get; set; }
        /// <summary>
        /// 类别
        /// </summary>
        public string ClassType { get; set; }
        /// <summary>
        /// 尺寸
        /// </summary>
        public string CellModel { get; set; }
        /// <summary>
        /// 档案盒存储明细
        /// </summary>
        public List<ArchiveBoxDetail> Details { get; private set; }

        public Guid? TenantId { get; set; }

        public void AddDetail(int boxId,int archiveId)
        {
            Details.Add(new ArchiveBoxDetail(boxId, archiveId));
        }
        public void RemoveDetail(int storageBoxDetailId)
        {
            var detail = Details.FirstOrDefault(item => item.Id == storageBoxDetailId);
            if (null == detail)
            {
                return;
            }

            Details.Remove(detail);
        }
    }
}
