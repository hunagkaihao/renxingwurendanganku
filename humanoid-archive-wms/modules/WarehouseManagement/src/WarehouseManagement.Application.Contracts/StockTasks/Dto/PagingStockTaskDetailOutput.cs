using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace WarehouseManagement.StockTasks.Dto
{
    public class PagingStockTaskDetailOutput : EntityDto<int>
    {
        //任务ID
        public int StockTaskId { get; set; }
        /// <summary>
        /// 档案盒ID
        /// </summary>
        public int StorageBoxDetailId { get; set; }
        /// <summary>
        /// 物料ID
        /// </summary>
        public int GoodsId { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public System.Decimal ManageListQuantity { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string TaskDetailRemark { get; set; }

    }
}
