using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseManagement.Cells;

namespace WarehouseManagement.EntityFrameworkCore.Cells
{
    public static class CellEfCoreQueryableExtensions
    {
        public static IQueryable<Cell> IncludeDetails(this IQueryable<Cell> queryable,
            bool include = true)
        {
            if (!include)
            {
                return queryable;
            }

            return queryable.Include(x => x.CellName);//leixd 主要针对明细表，一对多对象
        }
    }
}
