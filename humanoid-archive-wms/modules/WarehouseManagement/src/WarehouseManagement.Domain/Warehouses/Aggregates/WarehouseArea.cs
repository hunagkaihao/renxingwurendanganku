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
    public class WarehouseArea :   FullAuditedAggregateRoot<int>, IMultiTenant
    {
        /// <summary>
        /// 仓库基础信息表
        /// </summary>
        private WarehouseArea()
        {
        }
        public WarehouseArea(int warehouseId,string warehouseAreaCode, string warehouseAreaName, WarehouseAreaType warehouseAreaType)
        {
            WarehouseId = warehouseId;
            WarehouseAreaCode = warehouseAreaCode;
            WarehouseAreaName = warehouseAreaName;
            WarehouseAreaType = warehouseAreaType;


        }

        public void Update(int warehouseId, string warehouseAreaCode, string warehouseAreaName, WarehouseAreaType warehouseAreaType)
        {
            WarehouseId = warehouseId;
            WarehouseAreaCode = warehouseAreaCode;
            WarehouseAreaName = warehouseAreaName;
            WarehouseAreaType = warehouseAreaType;
        }

        /// <summary>
        /// 仓库ID
        /// </summary>
        public int WarehouseId { get; set; }
        /// <summary>
        /// 仓库分区编码
        /// </summary>
        public string WarehouseAreaCode { get; set; }
        /// <summary>
        /// 仓库分区名称
        /// </summary>
        public string WarehouseAreaName { get; set; }
        /// <summary>
        /// 仓库标记
        /// </summary>
        public string WarehouseAreaFlag { get; set; }
        /// <summary>
        /// 仓库类型
        /// </summary>
        public WarehouseAreaType WarehouseAreaType { get; set; }
        /// <summary>
        /// 仓库备注
        /// </summary>
        public string WarehouseAreaRemark { get; set; }
        /// <summary>
        /// 排序号
        /// </summary>
        public string WarehouseAreaOrder { get; set; }
        /// <summary>
        /// 仓库分区分组
        /// </summary>
        public string WarehouseAreaGroup { get; set; }
        /// <summary>
        /// 租户ID
        /// </summary>
        public Guid? TenantId { get; set; }




    }
}
