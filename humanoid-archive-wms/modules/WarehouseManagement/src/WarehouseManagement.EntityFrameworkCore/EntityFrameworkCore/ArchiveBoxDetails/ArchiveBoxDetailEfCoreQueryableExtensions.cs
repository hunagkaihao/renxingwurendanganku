using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseManagement.ArchiveBoxs.Aggregates;

namespace WarehouseManagement.EntityFrameworkCore.ArchiveBoxDetails
{
    public static class ArchiveBoxDetailEfCoreQueryableExtensions
    {
        public static IQueryable<ArchiveBoxDetail> IncludeDetails(this IQueryable<ArchiveBoxDetail> queryable,
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
