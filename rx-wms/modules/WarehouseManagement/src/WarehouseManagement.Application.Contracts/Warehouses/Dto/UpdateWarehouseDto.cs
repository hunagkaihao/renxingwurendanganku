using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace WarehouseManagement.Warehouses.Dto
{
    public class UpdateWarehouseDto
    {
        [Required]
        public int Id { get; set; }
        /// <summary>
        /// 仓库编码
        /// </summary>
        public string WarehouseCode { get; set; }
        /// <summary>
        /// 仓库名称
        /// </summary>
        public string WarehouseName { get; set; }
        /// <summary>
        /// 仓库类型
        /// </summary>
        public string WarehouseType { get; set; }

    }
}
