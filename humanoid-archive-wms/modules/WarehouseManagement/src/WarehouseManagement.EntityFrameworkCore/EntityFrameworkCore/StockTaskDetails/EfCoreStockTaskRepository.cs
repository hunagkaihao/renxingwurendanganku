using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using WarehouseManagement.StockTasks;
using WarehouseManagement.StockTasks.Aggregates;

namespace WarehouseManagement.EntityFrameworkCore.StockTaskDetails
{
    public class EfCoreStockTaskDetailRepository : EfCoreRepository<IWarehouseManagementDbContext, StockTaskDetail, int>, IStockTaskDetailRepository
    {
        public EfCoreStockTaskDetailRepository(IDbContextProvider<IWarehouseManagementDbContext> dbContextProvider) : base(dbContextProvider)
        {

        }

    }
}
