namespace WarehouseManagement.StockTasks.Dto
{
    /// <summary>
    /// 客户端物料出库任务下发请求
    /// </summary>
    public class ClientOutCellInput
    {
        /// <summary>
        /// 物料码，即物料容器条码。与库位至少填写一个。
        /// </summary>
        public string MaterialCode { get; set; }

        /// <summary>
        /// 库位编码或库位名称。与物料码至少填写一个。
        /// </summary>
        public string CellCode { get; set; }
    }
}
