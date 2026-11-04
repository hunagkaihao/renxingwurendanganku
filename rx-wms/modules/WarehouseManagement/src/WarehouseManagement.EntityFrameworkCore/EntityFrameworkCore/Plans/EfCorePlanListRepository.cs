using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using WarehouseManagement.Plans;
using WarehouseManagement.Plans.Aggregates;

namespace WarehouseManagement.EntityFrameworkCore.Plans
{
    public class EfCorePlanListRepository : EfCoreRepository<IWarehouseManagementDbContext, PlanList, int>, IPlanListRepository
    {
        public EfCorePlanListRepository(IDbContextProvider<IWarehouseManagementDbContext> dbContextProvider) : base(dbContextProvider)
        {

        }
        public async Task<List<PlanList>> GetPagingListAsync(string filter = null, int maxResultCount = 10, int skipCount = 0,  CancellationToken cancellationToken = default)
        {
            return await (await GetDbSetAsync())
                .WhereIf(!filter.IsNullOrWhiteSpace(),
                    e => (e.GoodsCode.Contains(filter)))
                .OrderByDescending(e => e.CreationTime)
                .PageBy(skipCount, maxResultCount)
                .ToListAsync(GetCancellationToken(cancellationToken));
        }

        public async Task<long> GetPagingCountAsync(string filter = null, CancellationToken cancellationToken = default)
        {
            return await (await GetDbSetAsync())
                .WhereIf(!filter.IsNullOrWhiteSpace(),
                    e => (e.PlanBillNo.Contains(filter)))
                .CountAsync(cancellationToken: cancellationToken);
        }
        public async Task<long> GetCountAsync(string filter = null, CancellationToken cancellationToken = default)
        {
            return await (await GetDbSetAsync())
                .WhereIf(!filter.IsNullOrWhiteSpace(), x => x.PlanBillNo.Contains(filter))
                .LongCountAsync(GetCancellationToken(cancellationToken));
        }

        public async Task<PlanList> FindByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await(await GetDbSetAsync())
                .OrderBy(t => t.CreationTime)
                .FirstOrDefaultAsync(t => t.Id == id, GetCancellationToken(cancellationToken));
        }



    }
}
