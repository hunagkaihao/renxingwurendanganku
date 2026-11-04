using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using WarehouseManagement.StockTasks.Aggregates;

namespace WarehouseManagement.StockTasks
{
    public interface IStockTaskDetailRepository : IRepository<StockTaskDetail, int>
    {

    }
}
