using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;

namespace WarehouseManagement.StockTasks.Dto
{
    [Table("ManageMain")]
    public class ManageMain : Entity
    {
        public ManageMain()
        { }
        public int GoodsTemplateId { get; set; }
        //计划Id
        public int PlanId { get; set; }
        //计划类型代码
        public string PlanTypeCode { get; set; }
        //任务类型编码
        public string ManageTypeCode { get; set; }
        //任务状态
        public String ManageStatus { get; set; }
        //档案盒Rfid号
        public string ArchiveBoxRfid { get; set; }
        //档案盒编码
        public string StockBarcode { get; set; }
        public string FullFlag { get; set; }
        public string CellModel { get; set; }
        //开始库位
        public int StartCellId { get; set; }
        //结束库位
        public int EndCellId { get; set; }
        public string ManageOperator { get; set; }
        //任务开始时间
        public string ManageBeginTime { get; set; }
        //任务完成时间
        public string ManageEndTime { get; set; }
        public string ManageLevel { get; set; }
        //任务备注
        public string ManageRemark { get; set; }
        public string ManageConfirmTime { get; set; }
        public string ManageLaneWay { get; set; }
        public decimal SumWeight { get; set; }
        public long? LastModifierUserId { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public long? DeleterUserId { get; set; }
        public DateTime? DeletionTime { get; set; }
        public bool IsDeleted { get; set; }
        public long? CreatorUserId { get; set; }
        public DateTime CreationTime { get; set; }

        public override object[] GetKeys()
        {
            throw new NotImplementedException();
        }
    }
}
