using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseManagement.Goodss.Aggregates;

namespace WarehouseManagement.EntityFrameworkCore.Goodss
{
    public static class GoodsEfCoreQueryableExtensions
    {
        public static IQueryable<Goods> IncludeDetails(this IQueryable<Goods> queryable,
            bool include = true)
        {
            if (!include)
            {
                return queryable;
            }

            return queryable.Include(x => x.GoodsName);//leixd 主要针对明细表，一对多对象
        }
    }
}
