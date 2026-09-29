namespace WarehouseManagement.StockTasks.Dto
{
    /// <summary>
    /// 客户端物料出库任务下发请求
    /// </summary>
    public class ClientOutCellInput
    {
        /// <summary>
        /// 物料条码，用于区分同种物料的不同实物。与库位至少填写一个。
        /// </summary>
        public string MaterialBarcode { get; set; }

        /// <summary>
        /// 库位编码或库位名称。与物料条码至少填写一个。
        /// </summary>
        public string CellCode { get; set; }
    }
}
