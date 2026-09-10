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
        public void Update(string materialBoxName, string materialBoxBarcode)
        {
            MaterialBoxName = materialBoxName;
            MaterialBoxBarcode = materialBoxBarcode;
        }

        public MaterialBox(string materialBoxName, string materialBoxBarcode)
        {
            MaterialBoxName = materialBoxName;
            MaterialBoxBarcode = materialBoxBarcode;
            Details = new List<MaterialBoxDetail>();
        }
        public void SetCell(int cellId)
        {
            CellId = cellId;
            Log.Warning($"Box:{this.MaterialBoxBarcode} is SetCell Cell:{cellId}。Method：{System.Reflection.MethodBase.GetCurrentMethod().Name}");
        }
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
        
        /// <summary>
        /// 物料盒名称
        /// </summary>
        [Required]
        public string MaterialBoxName { get; set; }
        /// <summary>
        /// 物料盒条码
        /// </summary>
        public string MaterialBoxBarcode { get; set; }
        /// <summary>
        /// 库存状态 0，空，1满
        /// </summary>
        public string FullFlag { get; set; }
        /// <summary>
        /// 物料盒备注
        /// </summary>
        public string StorageRemark { get; set; }
        /// <summary>
        /// 库位Id
        /// </summary>
        public int CellId { get; set; }
        /// <summary>
        /// 创建用户Id
        /// </summary>
        public long? CreatorUserId { get; set; }
        /// <summary>
        /// 删除用户Id
        /// </summary>
        public long? DeleterUserId { get; set; }
        /// <summary>
        /// 最新修改用户Id
        /// </summary>
        public long? LastModifierUserId { get; set; }
        /// <summary>
        /// 物料产量属性1
        /// </summary>
        public string MaterialUnit { get; set; }
        /// <summary>
        /// 物料产量属性2
        /// </summary>
        public string GoodsConstProperty2 { get; set; }
        /// <summary>
        /// 物料产量属性3
        /// </summary>
        public string GoodsConstProperty3 { get; set; }
        /// <summary>
        /// 物料产量属性4
        /// </summary>
        public string GoodsConstProperty4 { get; set; }
        /// <summary>
        /// 物料产量属性5
        /// </summary>
        public string GoodsConstProperty5 { get; set; }
        /// <summary>
        /// 物料产量属性6
        /// </summary>
        public string GoodsConstProperty6 { get; set; }
        /// <summary>
        /// 物料产量属性7
        /// </summary>
        public string GoodsConstProperty7 { get; set; }
        /// <summary>
        /// 物料产量属性8
        /// </summary>
        public string GoodsConstProperty8 { get; set; }
        /// <summary>
        /// 物料产量属性9
        /// </summary>
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
        public string MaterialPeople { get; set; }
        /// <summary>
        /// 责任人
        /// </summary>
        public string Director { get; set; }
        /// <summary>
        /// 保管期限
        /// </summary>
        public string RetentionPeriod { get; set; }
        /// <summary>
        /// 类别
        /// </summary>
        public string ClassType { get; set; }
        /// <summary>
        /// 物料规格
        /// </summary>
        public string CellModel { get; set; }
        /// <summary>
        /// 物料容器存储明细
        /// </summary>
        public List<MaterialBoxDetail> Details { get; private set; }
        /// <summary>
        /// 租赁 id
        /// </summary>
        public Guid? TenantId { get; set; }

    }
}
