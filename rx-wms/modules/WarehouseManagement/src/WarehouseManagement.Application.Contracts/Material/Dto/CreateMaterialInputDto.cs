using System;

namespace WarehouseManagement.Material.Dto
{
    /// <summary>
    /// 创建基础物料的请求，仅包含基础信息；主键由系统生成。
    /// </summary>
    public class CreateMaterialInputDto
    {
        /// <summary>
        /// 物料码，不允许为空。
        /// </summary>
        public string MaterialCode { get; set; }

        /// <summary>
        /// 物料名称。
        /// </summary>
        public string MaterialName { get; set; }

        /// <summary>
        /// 物料类型。
        /// </summary>
        public string MaterialType { get; set; }

        /// <summary>
        /// 物料计量单位，例如 g、kg。
        /// </summary>
        public string MaterialUnit { get; set; }

        /// <summary>
        /// 有效期天数；允许为空，0 表示不设置有效期。
        /// </summary>
        public int? ValidityDays { get; set; }

        /// <summary>
        /// 创建物料的用户工号或外部系统用户标识。
        /// </summary>
        public string CreatorUserCode { get; set; }

        /// <summary>
        /// 物料创建时间，允许为空；对应数据库 datetime(6) 字段。
        /// </summary>
        public DateTime? MaterialCreateTime { get; set; }
    }
}
