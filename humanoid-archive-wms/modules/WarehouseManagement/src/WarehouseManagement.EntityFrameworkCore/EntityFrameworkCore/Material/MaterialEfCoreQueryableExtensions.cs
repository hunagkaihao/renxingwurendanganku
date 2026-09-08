using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MaterialAggregate = WarehouseManagement.Material.Aggregates.Material;

namespace WarehouseManagement.EntityFrameworkCore.Material
{
    public static class MaterialEfCoreQueryableExtensions
    {
        public static IQueryable<MaterialAggregate> IncludeDetails(this IQueryable<MaterialAggregate> queryable,
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
