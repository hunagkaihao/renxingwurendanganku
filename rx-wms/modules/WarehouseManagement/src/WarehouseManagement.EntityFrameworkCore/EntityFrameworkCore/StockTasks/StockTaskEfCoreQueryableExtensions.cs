using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseManagement.StockTasks.Aggregates;

namespace WarehouseManagement.EntityFrameworkCore.StockTasks
{
    public static class StockTaskEfCoreQueryableExtensions
    {
        public static IQueryable<StockTask> IncludeDetails(this IQueryable<StockTask> queryable,
            bool include = true)
        {
            if (!include)
            {
                return queryable;
            }

            return queryable.Include(x => x.Details);//leixd 主要针对明细表，一对多对象
        }
    }
}
