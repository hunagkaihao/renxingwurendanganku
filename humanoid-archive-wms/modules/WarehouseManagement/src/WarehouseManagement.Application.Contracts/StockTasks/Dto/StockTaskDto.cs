using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace WarehouseManagement.StockTasks.Dto
{
    public class StockTaskDto : AuditedEntityDto<int>
    {
        private ManageType _manageType;
        private ManageStatus _manageStatus;
        /// <summary>
        /// 任务类型
        /// </summary>
        public ManageType ManageTypeCode {
            get
            {
                return _manageType;
            }
            set
            {
                _manageType = value;
                ManageTypeCodeString = _manageType.ToString();
            }
        }
        public string ManageTypeCodeString { get; set; }
        /// <summary>
        /// 料箱条码
        /// </summary>
        public string StockBarcode { get; set; }
        public string ArchiveBoxRfid { get; set; }
        public string PlanTypeCode { get; set; }
        /// <summary>
        /// 开始库位ID
        /// </summary>
        public int StartCellId { get; set; }
        /// <summary>
        /// 开始库位编码
        /// </summary>
        public string StartCellCode { get; set; }
        /// <summary>
        /// 结束库位ID
        /// </summary>
        public int? EndCellId { get; set; }
        /// <summary>
        /// 结束库位编码
        /// </summary>
        public string EndCellCode { get; set; }
        /// <summary>
        /// 任务状态
        /// </summary>
        public ManageStatus ManageStatus { 
            get 
            {
                return _manageStatus;
            } 
            set 
            {
                _manageStatus = value;
                ManageStatusString = _manageStatus.ToString();
            } 
        }

        public string ManageStatusString { get; set; }
        /// <summary>
        /// 计划ID
        /// </summary>
        public int PlanId { get; set; }
        /// <summary>
        /// 任务编号
        /// </summary>
        public string ManageLaneWay { get; set; }

    }
}
