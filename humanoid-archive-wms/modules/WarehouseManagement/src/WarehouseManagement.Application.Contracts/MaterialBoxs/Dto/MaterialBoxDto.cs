using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace WarehouseManagement.MaterialBoxs.Dto
{
    public class MaterialBoxDto : AuditedEntityDto<int>
    {
        //物料盒名称
        [Required]
        public string MaterialBoxName { get; set; }
        //物料盒条码
        public string MaterialBoxBarcode { get; set; }
        public string MaterialBoxRfid { get; set; }
        //物料盒编码
        public string StockBarcode { get; set; }
        //库存状态 0，空，1满
        public string FullFlag { get; set; }
        //物料盒备注
        public string StorageRemark { get; set; }
        //库位Id
        public int CellId { get; set; }
        public string CellCode { get; set; }
        public long? CreatorUserId { get; set; }

        public long? DeleterUserId { get; set; }
        public DateTime? DeletionTime { get; set; }
        public bool IsDeleted { get; set; }
        public long? LastModifierUserId { get; set; }

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
        /// 尺寸
        /// </summary>
        public string CellModel { get; set; }
    }
}
