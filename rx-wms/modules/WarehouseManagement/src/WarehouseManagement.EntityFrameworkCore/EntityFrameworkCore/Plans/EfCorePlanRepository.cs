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
    public class EfCorePlanRepository : EfCoreRepository<IWarehouseManagementDbContext, Plan, int>, IPlanRepository
    {
        public EfCorePlanRepository(IDbContextProvider<IWarehouseManagementDbContext> dbContextProvider) : base(dbContextProvider)
        {

        }
        public async Task<List<Plan>> GetPagingListAsync(string filter = null, int maxResultCount = 10, int skipCount = 0, bool includeDetails = false, CancellationToken cancellationToken = default)
        {
            return await (await GetDbSetAsync())
                .IncludeDetails(includeDetails)
                .WhereIf(!filter.IsNullOrWhiteSpace(),
                    e => (e.PlanBillNo.Contains(filter)))
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

        public async Task<Plan> FindByIdAsync(int id, bool includeDetails = true, CancellationToken cancellationToken = default)
        {
            return await(await GetDbSetAsync())
                .IncludeDetails(includeDetails)//包含明细
                .OrderBy(t => t.CreationTime)
                .FirstOrDefaultAsync(t => t.Id == id, GetCancellationToken(cancellationToken));
        }

        public async Task<Plan> FindByBillNoAsync(string billNo, bool includeDetails = true, CancellationToken cancellationToken = default)
        {
            return await (await GetDbSetAsync())
                .IncludeDetails(includeDetails)//包含明细
                .OrderBy(t => t.CreationTime)
                .FirstOrDefaultAsync(t => t.PlanBillNo == billNo, GetCancellationToken(cancellationToken));
        }


    }
}
