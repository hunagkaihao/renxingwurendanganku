using System.Linq;
using WarehouseManagement.CheckHiss.Aggregates;

namespace WarehouseManagement.EntityFrameworkCore.CheckHiss
{
    public static class CheckHisEfCoreQueryableExtensions
    {
        public static IQueryable<CheckHis> IncludeDetails(this IQueryable<CheckHis> queryable,
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
