using System;

namespace WarehouseManagement.Goodss.Dto
{
    /// <summary>
    /// 用于RFID标签类型下拉选项的DTO
    /// </summary>
    public class GoodsTypeSelectDto
    {
        /// <summary>
        /// 物品类型ID（存储值）
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 物品编码（用于显示和匹配）
        /// </summary>
        public string GoodsCode { get; set; }

        /// <summary>
        /// 物品名称（显示标签）
        /// </summary>
        public string GoodsName { get; set; }

        /// <summary>
        /// 显示文本：编码 - 名称
        /// </summary>
        public string Label => string.IsNullOrEmpty(GoodsCode)
            ? GoodsName
            : $"{GoodsCode} - {GoodsName}";

        /// <summary>
        /// 值：使用ID
        /// </summary>
        public int Value => Id;
    }
}
