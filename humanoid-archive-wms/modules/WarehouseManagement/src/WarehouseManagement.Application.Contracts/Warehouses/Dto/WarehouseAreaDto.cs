using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace WarehouseManagement.Warehouses.Dto
{
    public class WarehouseAreaDto : AuditedEntityDto<int>
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
        /// 仓库标记
        /// </summary>
        public string WarehouseAreaFlag { get; set; }
        /// <summary>
        /// 仓库类型
        /// </summary>
        public string WarehouseAreaType { get; set; }

        /// <summary>
        /// 仓库备注
        /// </summary>
        public string WarehouseAreaRemark { get; set; }
        /// <summary>
        /// 排序号
        /// </summary>
        public string WarehouseAreaOrder { get; set; }

    }
}
