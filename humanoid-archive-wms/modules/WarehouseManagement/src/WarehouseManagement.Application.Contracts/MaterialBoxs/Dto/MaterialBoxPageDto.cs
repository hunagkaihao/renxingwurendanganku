using System;

namespace WarehouseManagement.MaterialBoxs.Dto
{
    /// <summary>
    /// 物料容器分页查询返回模型。
    /// </summary>
    public class MaterialBoxPageDto
    {
        public int Id { get; set; }
        public string MaterialBoxBarcode { get; set; }
        public string MaterialBoxName { get; set; }
        public string CellModel { get; set; }
        /// <summary>
        /// 当前在库库位编码；仅在物料容器已分配库位时返回。
        /// </summary>
        public string CellCode { get; set; }
        public string MaterialUnit { get; set; }
        public string RetentionPeriod { get; set; }
        public string MaterialPeople { get; set; }
        public string MaterialInDate { get; set; }
        public DateTime? MaterialOutTime { get; set; }
        public DateTime CreationTime { get; set; }
    }
}
