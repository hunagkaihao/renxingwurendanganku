using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WarehouseManagement.RfidCodes.Aggregates;
using WarehouseManagement.StockTasks.Aggregates;

namespace WarehouseManagement.EntityFrameworkCore.RfidCodes
{
    public static class RfidEfCoreQueryableExtensions
    {
        public static IQueryable<Rfid> IncludeDetails(this IQueryable<Rfid> queryable,
            bool include = true)
        {
            if (!include)
            {
                return queryable;
            }

            return queryable;
        }
    }
}
