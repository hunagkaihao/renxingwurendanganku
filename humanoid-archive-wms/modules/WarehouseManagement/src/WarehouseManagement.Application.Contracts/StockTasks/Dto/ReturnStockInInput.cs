using System.ComponentModel.DataAnnotations;

namespace WarehouseManagement.StockTasks.Dto
{
    /// <summary>
    /// 临时借用归还入库请求。
    /// </summary>
    public class ReturnStockInInput
    {
        /// <summary>
        /// 物料容器条码。
        /// </summary>
        [Required]
        public string MaterialCode { get; set; }
    }
}
