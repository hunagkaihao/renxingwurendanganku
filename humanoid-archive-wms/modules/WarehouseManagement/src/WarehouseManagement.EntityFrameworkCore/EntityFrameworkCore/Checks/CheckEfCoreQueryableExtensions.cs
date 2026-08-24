using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseManagement.Checks.Aggregates;

namespace WarehouseManagement.EntityFrameworkCore.Checks
{
    public static class CheckEfCoreQueryableExtensions
    {
        public static IQueryable<Check> IncludeDetails(this IQueryable<Check> queryable,
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
