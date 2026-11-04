using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace WarehouseManagement.Checks.Dto
{
    /// <summary>
    /// 创建待下达的区域盘点计划，不会立即执行设备任务。
    /// </summary>
    public class CreateCheckDto
    {
        /// <summary>
        /// 盘点区域编码，格式为排-列-层（x-y-z），每段为非负整数。
        /// 0 表示该方向全部库位：0-0-0 为全库，1-0-0 为第 1 排，
        /// 1-0-2 为第 1 排第 2 层，1-2-3 为单个库位。
        /// </summary>
        public string AreaCode { get; set; }

    }
}
