using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace WarehouseManagement.CheckHiss.Dto
{
    public class CheckDetailHisDto : AuditedEntityDto<int>
    {

        /// <summary>
        /// 关联的盘点任务 ID（StockTask.Id）
        /// </summary>
        public int TaskId { get; set; }
        /// <summary>
        /// 盘点备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 内部账面条码快照，仅用于持久化和物料条码回退，不在 pageDetail 响应中输出。
        /// </summary>
        [JsonIgnore]
        public string StockBarcode { get; set; }
        /// <summary>
        /// 货物id
        /// </summary>
        [JsonIgnore]
        public int GoodsId { get; set; }
        /// <summary>
        /// 物料条码：优先返回 WCS/PLC 实扫条码；无实扫条码时返回账面物料条码。
        /// </summary>
        public string MaterialBoxBarcode { get; set; }
        /// <summary>
        /// 库位名称
        /// </summary>
        public string CellName { get; set; }
        /// <summary>
        /// 供应商
        /// </summary>
        [JsonIgnore]
        public string Supplier { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        [JsonIgnore]
        public decimal Account { get; set; }
        /// <summary>
        /// 实际数量1
        /// </summary>
        [JsonIgnore]
        public decimal RealAmount_1 { get; set; }
        /// <summary>
        /// 实际数量2
        /// </summary>
        [JsonIgnore]
        public decimal RealAmount_2 { get; set; }
        /// <summary>
        /// 利润损失数量
        /// </summary>
        [JsonIgnore]
        public decimal ProfitLossAmount { get; set; }
        /// <summary>
        /// 盘点id
        /// </summary>
        public int CheckId { get; set; }
        /// <summary>
        /// 盘点人
        /// </summary>
        public string Checker { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public string BeginTime { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public string FinishTime { get; set; }
        /// <summary>
        /// 完成标志位
        /// </summary>
        public int CompleteFlag { get; set; }
        /// <summary>
        /// 内部实扫条码快照，仅用于持久化和物料条码优先取值，不在 pageDetail 响应中输出。
        /// </summary>
        [JsonIgnore]
        public string BoxBarcode { get; set; }
        /// <summary>
        /// 核实标志位
        /// </summary>
        public int VerifyFlag { get; set; }
        /// <summary>
        /// 核实数量
        /// </summary>
        [JsonIgnore]
        public decimal VerifyAmount { get; set; }
        /// <summary>
        /// 核实结束时间
        /// </summary>
        public string VerifyFinishTime { get; set; }
        /// <summary>
        /// 核实用户
        /// </summary>
        public string VerifyUser { get; set; }
        /// <summary>
        /// 最新修改用户id
        /// </summary>
        public long? LastModifierUserId { get; set; }
        /// <summary>
        /// 删除用户id
        /// </summary>
        public long? DeleterUserId { get; set; }
        /// <summary>
        /// 创建用户id
        /// </summary>
        public long? CreatorUserId { get; set; }
        /// <summary>
        /// 租赁id
        /// </summary>
        [JsonIgnore]
        public Guid? TenantId { get; set; }
    }
}
