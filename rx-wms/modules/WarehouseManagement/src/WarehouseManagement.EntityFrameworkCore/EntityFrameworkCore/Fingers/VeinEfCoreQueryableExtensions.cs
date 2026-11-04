using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseManagement.Faces.Aggregates;
using WarehouseManagement.Fingers.Aggregates;

namespace WarehouseManagement.EntityFrameworkCore.Fingers
{
    public static class VeinEfCoreQueryableExtensions
    {
        public static IQueryable<Vein> IncludeDetails(this IQueryable<Vein> queryable,
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
