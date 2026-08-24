using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using WarehouseManagement.StockTasks;
using WarehouseManagement.StockTasks.Aggregates;

namespace WarehouseManagement.EntityFrameworkCore.StockTasks
{
    public class EfCoreStockTaskRepository : EfCoreRepository<IWarehouseManagementDbContext, StockTask, int>, IStockTaskRepository
    {
        public EfCoreStockTaskRepository(IDbContextProvider<IWarehouseManagementDbContext> dbContextProvider) : base(dbContextProvider)
        {

        }
        public async Task<List<StockTask>> GetPagingListAsync(string filter = null, int maxResultCount = 10, int skipCount = 0, bool includeDetails = false, CancellationToken cancellationToken = default)
        {
            return await (await GetDbSetAsync())
                .IncludeDetails(includeDetails)
                .WhereIf(!filter.IsNullOrWhiteSpace(),
                    e => (e.ArchiveBoxRfid.Contains(filter)))
                .OrderByDescending(e => e.CreationTime)
                .PageBy(skipCount, maxResultCount)
                .ToListAsync(GetCancellationToken(cancellationToken));
        }

        public async Task<long> GetPagingCountAsync(string filter = null, CancellationToken cancellationToken = default)
        {
            return await (await GetDbSetAsync())
                .WhereIf(!filter.IsNullOrWhiteSpace(),
                    e => (e.ArchiveBoxRfid.Contains(filter)))
                .CountAsync(cancellationToken: cancellationToken);
        }
        public async Task<long> GetCountAsync(string filter = null, CancellationToken cancellationToken = default)
        {
            return await (await GetDbSetAsync())
                .WhereIf(!filter.IsNullOrWhiteSpace(), x => x.ArchiveBoxRfid.Contains(filter))
                .LongCountAsync(GetCancellationToken(cancellationToken));
        }

        public async Task<StockTask> FindByIdAsync(int id, bool includeDetails = true, CancellationToken cancellationToken = default)
        {
            return await(await GetDbSetAsync())
                .IncludeDetails(includeDetails)//包含明细
                .OrderBy(t => t.CreationTime)
                .FirstOrDefaultAsync(t => t.Id == id, GetCancellationToken(cancellationToken));
        }

        public async Task<StockTask> FindByBarcodeAsync(string storageBoxBarcode, bool includeDetails = true, CancellationToken cancellationToken = default)
        {
            return await(await GetDbSetAsync())
                .IncludeDetails(includeDetails)//包含明细
                .OrderBy(t => t.CreationTime)
                .FirstOrDefaultAsync(t => t.ArchiveBoxRfid == storageBoxBarcode, GetCancellationToken(cancellationToken));
        }


    }
}
