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
    public class StockTaskCompletedEto
    {
        public int StockTaskId { get; set; }

        public string ArchiveBoxRfid { get; set; }
        public ManageType ManageTypeCode { get; set; }
        public int StartCellId { get; set; }
        public int EndCellId { get; set; }
        public String ManageStatus { get; set; }

    }
}
