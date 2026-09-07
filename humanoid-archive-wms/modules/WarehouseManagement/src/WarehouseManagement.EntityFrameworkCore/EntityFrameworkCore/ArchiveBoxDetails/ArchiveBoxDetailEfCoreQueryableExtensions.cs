using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseManagement.MaterialBoxs.Aggregates;

namespace WarehouseManagement.EntityFrameworkCore.ArchiveBoxDetails
{
    public static class ArchiveBoxDetailEfCoreQueryableExtensions
    {
        public static IQueryable<MaterialBoxDetail> IncludeDetails(this IQueryable<MaterialBoxDetail> queryable,
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
