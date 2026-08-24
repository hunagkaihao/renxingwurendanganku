using AutoMapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace WarehouseManagement.Warehouses.Aggregates
{
    public class Warehouse :   FullAuditedAggregateRoot<int>, IMultiTenant
    {
        /// <summary>
        /// 仓库基础信息表
        /// </summary>
        private Warehouse()
        {
        }
        public Warehouse(string warehouseCode, string warehouseName, WarehouseType warehouseType)
        {
            WarehouseCode = warehouseCode;
            WarehouseName = warehouseName;
            WarehouseType = warehouseType;

        }

        public void Update(string warehouseCode, string warehouseName, WarehouseType warehouseType)
        {
            WarehouseCode = warehouseCode;
            WarehouseName = warehouseName;
            WarehouseType = warehouseType;
        }
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
        /// <summary>
        /// 租户ID
        /// </summary>
        public Guid? TenantId { get; set; }




    }
}
