namespace WarehouseManagement.Material.Dto
{
    /// <summary>
    /// 创建基础物料的返回结果，仅包含已保存的主键和基础信息。
    /// </summary>
    public class CreateMaterialResultDto
    {
        /// <summary>
        /// 创建成功后由数据库生成的物料主键。
        /// </summary>
        public int Id { get; set; }

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
        /// 物料创建时间，格式为 yyyy-MM-dd HH:mm:ss（24 小时制），允许为空。
        /// </summary>
        public string MaterialCreateTime { get; set; }
    }
}
