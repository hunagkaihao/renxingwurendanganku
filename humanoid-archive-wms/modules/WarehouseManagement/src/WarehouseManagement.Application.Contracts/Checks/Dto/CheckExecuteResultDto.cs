namespace WarehouseManagement.Checks.Dto
{
    /// <summary>
    /// 盘点计划下达结果，供调用方关联计划和 WCS 批次；不代表盘点已完成。
    /// </summary>
    public class CheckExecuteResultDto
    {
        /// <summary>
        /// WCS 已接收任务且计划已进入执行中时为 true；失败通过 HTTP 错误响应返回。
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// 盘点计划下达结果说明。
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 本次下达的盘点计划主键。
        /// </summary>
        public int CheckId { get; set; }

        /// <summary>
        /// 本次下达的盘点计划编号。
        /// </summary>
        public string CheckCode { get; set; }

        /// <summary>
        /// WCS 返回并保存到计划中的任务批次号，沿用 WCS 返回值。
        /// </summary>
        public string BatchNo { get; set; }
    }
}
