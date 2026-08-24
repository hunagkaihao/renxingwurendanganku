using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagement.StockTasks.Dto
{
    public class OutCellTrue
    {
        public string BoxCode { get; set; }

        public string StartCellCode { get; set; }
        
        public List<PagingStockTaskDetailOutput> Output{ get; set; }
}
}
