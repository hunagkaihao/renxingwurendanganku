namespace WarehouseManagement.StockTasks.Dto
{
    /// <summary>
    /// 创建出库任务的请求参数。
    /// 出库任务从已有物料容器读取物料属性，因此无需提交入库建档字段。
    /// </summary>
    public class CreateStockOutTaskDto
    {
        /// <summary>
        /// 物料容器 ID。
        /// </summary>
        public int MaterialBoxId { get; set; }

        /// <summary>
        /// 物料容器条码。未提供容器 ID 时使用此字段查询。
        /// </summary>
        public string MaterialCode { get; set; }
    }
}
