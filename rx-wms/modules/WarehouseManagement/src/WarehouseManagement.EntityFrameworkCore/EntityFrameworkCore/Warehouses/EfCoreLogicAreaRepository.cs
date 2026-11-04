using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using WarehouseManagement.Warehouses.Aggregates;
using WarehouseManagement.Warehouses;

namespace WarehouseManagement.EntityFrameworkCore.Warehouses
{
    public class EfCoreLogicAreaRepository : EfCoreRepository<IWarehouseManagementDbContext, LogicArea, int>, ILogicAreaRepository
    {
        public EfCoreLogicAreaRepository(IDbContextProvider<IWarehouseManagementDbContext> dbContextProvider) : base(dbContextProvider)
        {

        }
        public async Task<List<LogicArea>> GetPagingListAsync(string filter = null, int maxResultCount = 10, int skipCount = 0, bool includeDetails = false, CancellationToken cancellationToken = default)
        {
            return await (await GetDbSetAsync())
                //.IncludeDetails(includeDetails)
                .WhereIf(!filter.IsNullOrWhiteSpace(),
                    e => (e.LogicAreaName.Contains(filter)))
                .OrderByDescending(e => e.CreationTime)
                .PageBy(skipCount, maxResultCount)
                .ToListAsync(GetCancellationToken(cancellationToken));
        }

        public async Task<long> GetPagingCountAsync(string filter = null, CancellationToken cancellationToken = default)
        {
            return await (await GetDbSetAsync())
                .WhereIf(!filter.IsNullOrWhiteSpace(),
                    e => (e.LogicAreaName.Contains(filter)))
                .CountAsync(cancellationToken: cancellationToken);
        }


        public async Task<long> GetCountAsync(string filter = null, CancellationToken cancellationToken = default)
        {
            return await (await GetDbSetAsync())
                .WhereIf(!filter.IsNullOrWhiteSpace(), x => x.LogicAreaName.Contains(filter))
                .LongCountAsync(GetCancellationToken(cancellationToken));
        }

        public async Task<LogicArea> FindByIdAsync(int id, bool includeDetails = false, CancellationToken cancellationToken = default)
        {
            return await(await GetDbSetAsync())
                //.IncludeDetails(includeDetails)//包含明细
                .OrderBy(t => t.CreationTime)
                .FirstOrDefaultAsync(t => t.Id == id, GetCancellationToken(cancellationToken));
        }

        public async Task<LogicArea> FindByNameAsync(string goodsName, bool includeDetails = false, CancellationToken cancellationToken = default)
        {
            return await(await GetDbSetAsync())
                //.IncludeDetails(includeDetails)//包含明细
                .OrderBy(t => t.CreationTime)
                .FirstOrDefaultAsync(t => t.LogicAreaName == goodsName, GetCancellationToken(cancellationToken));
        }

        public async Task<LogicArea> FindByCodeAsync(string cellCode, bool includeDetails = false, CancellationToken cancellationToken = default)
        {
            return await (await GetDbSetAsync())
                //.IncludeDetails(includeDetails)//包含明细
                .OrderBy(t => t.CreationTime)
                .FirstOrDefaultAsync(t => t.LogicAreaCode == cellCode, GetCancellationToken(cancellationToken));
        }
        

    }
}
