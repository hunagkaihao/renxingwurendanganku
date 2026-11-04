using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace WarehouseManagement.StockTasks.Dto
{
    public class CreateStockTaskDto
    {
        /// <summary>
        /// 物料ID
        /// </summary>
        public int MaterialBoxId { get; set; }
        /// <summary>
        /// 物料条码
        /// </summary>
        [Required]
        public string MaterialCode { get; set; }
        /// <summary>
        /// 物料名称
        /// </summary>
        public string MaterialName { get; set; }
        /// <summary>
        /// 物料类型/库位规格，例如 ML、LL
        /// </summary>
        [Required]
        public string MaterialType { get; set; }
        /// <summary>
        /// 物料单位，例如 g、kg
        /// </summary>
        public string MaterialUnit { get; set; }
        /// <summary>
        /// 有效期（天）
        /// </summary>
        [Range(0, int.MaxValue)]
        public int ValidityDays { get; set; }
        /// <summary>
        /// 创建用户 ID（外部系统字符串标识）
        /// </summary>
        public string CreatorUserCode { get; set; }
        /// <summary>
        /// 创建时间，格式：yyyy-MM-dd HH:mm:ss
        /// </summary>
        [Required]
        public string MaterialCreateTime { get; set; }
        /// <summary>
        /// 开始库位ID
        /// </summary>
        public int StartCellId { get; set; }
        /// <summary>
        /// 开始库位编码
        /// </summary>
        public string StartCellCode { get; set; }
        /// <summary>
        /// 结束库位ID
        /// </summary>
        public int EndCellId { get; set; }
        /// <summary>
        /// 结束库位编码
        /// </summary>
        public string EndCellCode { get; set; }
        /// <summary>
        /// 任务类型
        /// </summary>
        public string TaskTypeCode { get; set; }
        /// <summary>
        /// 任务状态
        /// </summary>
        public String TaskStatus { get; set; }

    }
}
