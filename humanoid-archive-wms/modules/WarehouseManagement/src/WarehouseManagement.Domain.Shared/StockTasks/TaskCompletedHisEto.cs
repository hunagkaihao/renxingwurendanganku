using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.EventBus;
using Volo.Abp.MultiTenancy;

namespace WarehouseManagement.StockTasks.Aggregates
{
    [EventName("MyApp.StockTask.Completed")]
    public class TaskCompletedHisEto
    {
        /// <summary>
        /// 任务ID
        /// </summary>
        public int StockTaskId;
    }
}
