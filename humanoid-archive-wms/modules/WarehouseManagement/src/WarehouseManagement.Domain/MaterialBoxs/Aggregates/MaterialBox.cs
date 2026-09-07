using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace WarehouseManagement.MaterialBoxs.Aggregates
{
    public class MaterialBox : FullAuditedAggregateRoot<int>, IMultiTenant
    {
        private MaterialBox()
        {
            Details = new List<MaterialBoxDetail>();
        }
        public void Update(string materialBoxName, string stockBarcode)
        {
            MaterialBoxName = materialBoxName;
            StockBarcode = stockBarcode;
        }

        public MaterialBox(string materialBoxName, string stockBarcode)
        {
            //Id = id;
            MaterialBoxName = materialBoxName;
            StockBarcode = stockBarcode;
            Details = new List<MaterialBoxDetail>();
        }
        public void SetCell(int cellId)
        {
            CellId = cellId;
            Log.Warning($"Box:{this.MaterialBoxRfid} is SetCell Cell:{cellId}。Method：{System.Reflection.MethodBase.GetCurrentMethod().Name}");
        }
        //物料盒名称
        [Required]
        public string MaterialBoxName { get; set; }
        public string MaterialBoxRfid { get; set; }
        //物料盒编码
        public string StockBarcode { get; set; }
        //库存状态 0，空，1满
        public string FullFlag { get; set; }
        //物料盒备注
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
        public string MaterialInDate { get; set; }
        /// <summary>
        /// 归档部门
        /// </summary>
        public string MaterialInDept { get; set; }
        /// <summary>
        /// 移交人 归档人
        /// </summary>
        public string Material { get; set; }
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
        public List<MaterialBoxDetail> Details { get; private set; }

        public Guid? TenantId { get; set; }

        public void AddDetail(int boxId,int archiveId)
        {
            Details.Add(new MaterialBoxDetail(boxId, archiveId));
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
