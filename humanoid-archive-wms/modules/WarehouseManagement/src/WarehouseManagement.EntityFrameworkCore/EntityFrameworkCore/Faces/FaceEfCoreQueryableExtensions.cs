using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseManagement.ArchiveBoxs.Aggregates;
using WarehouseManagement.Faces.Aggregates;

namespace WarehouseManagement.EntityFrameworkCore.Faces
{
    public static class FaceEfCoreQueryableExtensions   
    {
        public static IQueryable<Face> IncludeDetails(this IQueryable<Face> queryable,
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
