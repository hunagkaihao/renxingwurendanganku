using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace WarehouseManagement.Warehouses.Dto
{
    public class CreateWarehouseAreaDto
    {
        public int WarehouseId { get; set; }
        /// <summary>
        /// 仓库编码
        /// </summary>
        public string WarehouseAreaCode { get; set; }
        /// <summary>
        /// 仓库名称
        /// </summary>
        public string WarehouseAreaName { get; set; }
        /// <summary>
        /// 仓库类型
        /// </summary>
        public string WarehouseAreaType { get; set; }

    }
}
