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
    public class EfCoreWarehouseRepository : EfCoreRepository<IWarehouseManagementDbContext, Warehouse, int>, IWarehouseRepository
    {
        public EfCoreWarehouseRepository(IDbContextProvider<IWarehouseManagementDbContext> dbContextProvider) : base(dbContextProvider)
        {

        }
        public async Task<List<Warehouse>> GetPagingListAsync(string filter = null, int maxResultCount = 10, int skipCount = 0, bool includeDetails = false, CancellationToken cancellationToken = default)
        {
            return await (await GetDbSetAsync())
                //.IncludeDetails(includeDetails)
                .WhereIf(!filter.IsNullOrWhiteSpace(),
                    e => (e.WarehouseName.Contains(filter)))
                .OrderByDescending(e => e.CreationTime)
                .PageBy(skipCount, maxResultCount)
                .ToListAsync(GetCancellationToken(cancellationToken));
        }

        public async Task<long> GetPagingCountAsync(string filter = null, CancellationToken cancellationToken = default)
        {
            return await (await GetDbSetAsync())
                .WhereIf(!filter.IsNullOrWhiteSpace(),
                    e => (e.WarehouseName.Contains(filter)))
                .CountAsync(cancellationToken: cancellationToken);
        }


        public async Task<long> GetCountAsync(string filter = null, CancellationToken cancellationToken = default)
        {
            return await (await GetDbSetAsync())
                .WhereIf(!filter.IsNullOrWhiteSpace(), x => x.WarehouseName.Contains(filter))
                .LongCountAsync(GetCancellationToken(cancellationToken));
        }

        public async Task<Warehouse> FindByIdAsync(int id, bool includeDetails = false, CancellationToken cancellationToken = default)
        {
            return await(await GetDbSetAsync())
                //.IncludeDetails(includeDetails)//包含明细
                .OrderBy(t => t.CreationTime)
                .FirstOrDefaultAsync(t => t.Id == id, GetCancellationToken(cancellationToken));
        }

        public async Task<Warehouse> FindByNameAsync(string goodsName, bool includeDetails = false, CancellationToken cancellationToken = default)
        {
            return await(await GetDbSetAsync())
                //.IncludeDetails(includeDetails)//包含明细
                .OrderBy(t => t.CreationTime)
                .FirstOrDefaultAsync(t => t.WarehouseName == goodsName, GetCancellationToken(cancellationToken));
        }

        public async Task<Warehouse> FindByCodeAsync(string cellCode, bool includeDetails = false, CancellationToken cancellationToken = default)
        {
            return await (await GetDbSetAsync())
                //.IncludeDetails(includeDetails)//包含明细
                .OrderBy(t => t.CreationTime)
                .FirstOrDefaultAsync(t => t.WarehouseCode == cellCode, GetCancellationToken(cancellationToken));
        }
        

    }
}
