using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseManagement.ArchiveBoxs.Aggregates;

namespace WarehouseManagement.EntityFrameworkCore.ArchiveBoxs
{
    public static class ArchiveBoxEfCoreQueryableExtensions
    {
        public static IQueryable<ArchiveBox> IncludeDetails(this IQueryable<ArchiveBox> queryable,
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
