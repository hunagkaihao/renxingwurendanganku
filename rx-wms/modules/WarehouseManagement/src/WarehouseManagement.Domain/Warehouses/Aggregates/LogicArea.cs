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
    public class LogicArea :   FullAuditedAggregateRoot<int>, IMultiTenant
    {
        /// <summary>
        /// 仓库基础信息表
        /// </summary>
        private LogicArea()
        {
        }
        public LogicArea(int warehouseId,string logicAreaCode, string logicAreaName)
        {
            WarehouseId = warehouseId;
            LogicAreaCode = logicAreaCode;
            LogicAreaName = logicAreaName;
            //LogicAreaType = logicAreaType;


        }

        public void Update(int warehouseId, string logicAreaCode, string logicAreaName)
        {
            WarehouseId = warehouseId;
            LogicAreaCode = logicAreaCode;
            LogicAreaName = logicAreaName;
            //LogicAreaType = logicAreaType;
        }

        /// <summary>
        /// 仓库ID
        /// </summary>
        public int WarehouseId { get; set; }
        /// <summary>
        /// 逻辑分区编码
        /// </summary>
        public string LogicAreaCode { get; set; }
        /// <summary>
        /// 逻辑分区名称
        /// </summary>
        public string LogicAreaName { get; set; }
        /// <summary>
        /// 仓库标记
        /// </summary>
        public string LogicAreaFlag { get; set; }
        ///// <summary>
        ///// 仓库类型
        ///// </summary>
        //public LogicAreaType LogicAreaType { get; set; }
        /// <summary>
        /// 仓库备注
        /// </summary>
        public string LogicAreaRemark { get; set; }
        /// <summary>
        /// 排序号
        /// </summary>
        public string LogicAreaOrder { get; set; }
        /// <summary>
        /// 逻辑分区分组
        /// </summary>
        public string LogicAreaGroup { get; set; }
        /// <summary>
        /// 租户ID
        /// </summary>
        public Guid? TenantId { get; set; }




    }
}
