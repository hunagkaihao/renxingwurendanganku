using System;

namespace WarehouseManagement.MaterialBoxs.Dto
{
    /// <summary>
    /// 物料容器分页查询返回模型。
    /// </summary>
    public class MaterialBoxPageDto
    {
        public int Id { get; set; }
        public string MaterialCode { get; set; }
        public string MaterialName { get; set; }
        public string MaterialType { get; set; }
        public string MaterialUnit { get; set; }
        public string RetentionPeriod { get; set; }
        public string MaterialPeople { get; set; }
        public DateTime CreationTime { get; set; }
    }
}
