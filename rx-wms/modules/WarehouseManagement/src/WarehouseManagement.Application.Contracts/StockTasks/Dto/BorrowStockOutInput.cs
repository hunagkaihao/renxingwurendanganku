using System.ComponentModel.DataAnnotations;

namespace WarehouseManagement.StockTasks.Dto
{
    /// <summary>
    /// 临时借用出库请求。
    /// </summary>
    public class BorrowStockOutInput
    {
        /// <summary>
        /// 物料容器条码。
        /// </summary>
        [Required]
        public string MaterialCode { get; set; }

        /// <summary>
        /// 借用用途。
        /// </summary>
        [Required]
        public string BorrowPurpose { get; set; }

        /// <summary>
        /// 借用时长，单位为小时。
        /// </summary>
        [Range(1, int.MaxValue)]
        public int BorrowDurationHours { get; set; }
    }
}
