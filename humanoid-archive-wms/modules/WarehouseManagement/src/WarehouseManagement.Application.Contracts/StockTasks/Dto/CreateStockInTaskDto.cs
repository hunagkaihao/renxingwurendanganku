using System.ComponentModel.DataAnnotations;

namespace WarehouseManagement.StockTasks.Dto
{
    /// <summary>
    /// 创建入库预约任务的请求参数。
    /// 物料名称、类型、单位和有效期由服务端按物料码从基础物料信息中读取，不允许由客户端传入。
    /// </summary>
    public class CreateStockInTaskDto
    {
        /// <summary>
        /// 基础物料编码。
        /// </summary>
        [Required]
        public string MaterialCode { get; set; }

        /// <summary>
        /// 预约创建用户的外部标识。
        /// </summary>
        public string CreatorUserCode { get; set; }

        /// <summary>
        /// 预约创建时间，格式为 yyyy-MM-dd HH:mm:ss。
        /// </summary>
        [Required]
        public string MaterialCreateTime { get; set; }

        /// <summary>
        /// 入库任务起始点位主键；供一体机或外部系统指定物料进入系统的位置。
        /// </summary>
        public int StartCellId { get; set; }

        /// <summary>
        /// 入库任务起始点位编码。
        /// </summary>
        public string StartCellCode { get; set; }

        /// <summary>
        /// 指定目标库位的主键；未指定时由后续流程分配。
        /// </summary>
        public int EndCellId { get; set; }

        /// <summary>
        /// 指定目标库位编码，仅用于客户端传递和任务展示。
        /// </summary>
        public string EndCellCode { get; set; }
    }
}
