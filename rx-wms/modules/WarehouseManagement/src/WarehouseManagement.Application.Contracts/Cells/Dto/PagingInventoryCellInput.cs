using Lion.AbpPro.Extension.Customs.Dtos;

namespace WarehouseManagement.Cells.Dto
{
    /// <summary>
    /// 第三方库存分页查询参数。
    /// </summary>
    public class PagingInventoryCellInput : PagingBase
    {
        /// <summary>
        /// 库位名称或库位编码关键字。
        /// </summary>
        public string Filter { get; set; }
    }
}
