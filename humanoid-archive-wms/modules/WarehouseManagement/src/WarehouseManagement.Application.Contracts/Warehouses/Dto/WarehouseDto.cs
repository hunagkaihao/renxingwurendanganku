using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace WarehouseManagement.Warehouses.Dto
{
    public class WarehouseDto : AuditedEntityDto<int>
    {
        /// <summary>
        /// 仓库编码
        /// </summary>
        public string WarehouseCode { get; set; }
        /// <summary>
        /// 仓库名称
        /// </summary>
        public string WarehouseName { get; set; }
        /// <summary>
        /// 仓库标记
        /// </summary>
        public string WarehouseFlag { get; set; }
        /// <summary>
        /// 仓库类型
        /// </summary>
        public WarehouseType WarehouseType { get; set; }

        /// <summary>
        /// 仓库备注
        /// </summary>
        public string WarehouseRemark { get; set; }
        /// <summary>
        /// 排序号
        /// </summary>
        public string WarehouseOrder { get; set; }

    }
}
