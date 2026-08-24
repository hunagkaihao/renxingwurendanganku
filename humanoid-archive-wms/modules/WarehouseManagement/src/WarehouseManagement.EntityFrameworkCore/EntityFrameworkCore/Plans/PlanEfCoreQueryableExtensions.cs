using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseManagement.Plans.Aggregates;

namespace WarehouseManagement.EntityFrameworkCore.Plans
{
    public static class PlanEfCoreQueryableExtensions
    {
        public static IQueryable<Plan> IncludeDetails(this IQueryable<Plan> queryable,
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
