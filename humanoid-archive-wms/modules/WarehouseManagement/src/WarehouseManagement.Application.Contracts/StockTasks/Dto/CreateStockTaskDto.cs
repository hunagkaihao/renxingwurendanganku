using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace WarehouseManagement.StockTasks.Dto
{
    public class CreateStockTaskDto
    {
        /// <summary>
        /// 任务类型
        /// </summary>
        public string ManageTypeCode { get; set; }
        /// <summary>
        /// 料箱ID
        /// </summary>
        public int StorageBoxId { get; set; }
        /// <summary>
        /// 料箱条码
        /// </summary>
        public string StockBarcode { get; set; }
        /// <summary>
        /// 料箱ID
        /// </summary>
        public int ArchiveBoxId { get; set; }
        /// <summary>
        /// 料箱条码
        /// </summary>
        public string ArchiveCode { get; set; }
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
        /// 任务状态
        /// </summary>
        public String ManageStatus { get; set; }

    }
}
