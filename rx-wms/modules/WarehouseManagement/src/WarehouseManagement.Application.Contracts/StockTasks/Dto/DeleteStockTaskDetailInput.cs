using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagement.StockTasks.Dto
{
    public class DeleteStockTaskDetailInput
    {
        public int Id { get; set; }

        public int StockTaskId { get; set; }
    }
}
