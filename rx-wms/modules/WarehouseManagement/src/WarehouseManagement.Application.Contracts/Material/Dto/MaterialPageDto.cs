using Volo.Abp.Application.Dtos;

namespace WarehouseManagement.Material.Dto
{
    /// <summary>
    /// 基础物料分页查询结果，包含基础信息及审计信息。
    /// </summary>
    public class MaterialPageDto : AuditedEntityDto<int>
    {
        /// <summary>
        /// 物料名称。
        /// </summary>
        public string MaterialName { get; set; }

        /// <summary>
        /// 物料码。
        /// </summary>
        public string MaterialCode { get; set; }

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
        /// 物料备注。
        /// </summary>
        public string MaterialRemark { get; set; }

        /// <summary>
        /// 物料类型。
        /// </summary>
        public string MaterialType { get; set; }
    }
}
