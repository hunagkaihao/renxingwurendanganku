using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseManagement.Archives.Aggregates;

namespace WarehouseManagement.EntityFrameworkCore.Archives
{
    public static class ArchiveEfCoreQueryableExtensions
    {
        public static IQueryable<Archive> IncludeDetails(this IQueryable<Archive> queryable,
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
