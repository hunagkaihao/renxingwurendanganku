using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseManagement.TaskHiss.Aggregates;

namespace WarehouseManagement.EntityFrameworkCore.TaskHiss
{
    public static class TaskHisEfCoreQueryableExtensions
    {
        public static IQueryable<TaskHis> IncludeDetails(this IQueryable<TaskHis> queryable,
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
