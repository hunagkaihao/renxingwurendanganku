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
        public string MaterialUnit { get; set; }
        public string RetentionPeriod { get; set; }
        public string MaterialPeople { get; set; }
        public DateTime CreationTime { get; set; }
    }
}
