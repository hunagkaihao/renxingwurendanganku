using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using WarehouseManagement.ArchiveBoxs;
using WarehouseManagement.ArchiveBoxs.Aggregates;

namespace WarehouseManagement.EntityFrameworkCore.ArchiveBoxs
{
    public class EfCoreArchiveBoxRepository : EfCoreRepository<IWarehouseManagementDbContext, ArchiveBox, int>, IArchiveBoxRepository
    {
        public EfCoreArchiveBoxRepository(IDbContextProvider<IWarehouseManagementDbContext> dbContextProvider) : base(dbContextProvider)
        {

        }
        public async Task<List<ArchiveBox>> GetPagingListAsync(string filter = null, int maxResultCount = 10, int skipCount = 0, bool includeDetails = false, CancellationToken cancellationToken = default)
        {
            return await (await GetDbSetAsync())
                .IncludeDetails(includeDetails)
                .WhereIf(!filter.IsNullOrWhiteSpace(),
                    e => (e.ArchiveBoxName.Contains(filter)))
                .OrderByDescending(e => e.CreationTime)
                .PageBy(skipCount, maxResultCount)
                .ToListAsync(GetCancellationToken(cancellationToken));
        }

        public async Task<long> GetPagingCountAsync(string filter = null, CancellationToken cancellationToken = default)
        {
            return await (await GetDbSetAsync())
                .WhereIf(!filter.IsNullOrWhiteSpace(),
                    e => (e.ArchiveBoxName.Contains(filter)))
                .CountAsync(cancellationToken: cancellationToken);
        }
        public async Task<long> GetCountAsync(string filter = null, CancellationToken cancellationToken = default)
        {
            return await (await GetDbSetAsync())
                .WhereIf(!filter.IsNullOrWhiteSpace(), x => x.ArchiveBoxName.Contains(filter))
                .LongCountAsync(GetCancellationToken(cancellationToken));
        }

        public async Task<ArchiveBox> FindByIdAsync(int id, bool includeDetails = true, CancellationToken cancellationToken = default)
        {
            return await (await GetDbSetAsync())
                .IncludeDetails(includeDetails)//包含明细
                .OrderBy(t => t.CreationTime)
                .FirstOrDefaultAsync(t => t.Id == id, GetCancellationToken(cancellationToken));
        }

        public async Task<ArchiveBox> FindByBoxNameAsync(string boxName, bool includeDetails = true, CancellationToken cancellationToken = default)
        {
            return await (await GetDbSetAsync())
                .IncludeDetails(includeDetails)//包含明细
                .OrderBy(t => t.CreationTime)
                .FirstOrDefaultAsync(t => t.ArchiveBoxName == boxName, GetCancellationToken(cancellationToken));
        }

        public async Task<ArchiveBox> FindByBoxBarcodeAsync(string archiveBoxBarcode, bool includeDetails = true, CancellationToken cancellationToken = default)
        {
            return await (await GetDbSetAsync())
                .IncludeDetails(includeDetails)//包含明细
                .OrderBy(t => t.CreationTime)
                .FirstOrDefaultAsync(t => t.ArchiveBoxName == archiveBoxBarcode, GetCancellationToken(cancellationToken));
        }

        public async Task<ArchiveBox> FindByRfidCodeAsync(string rfidCode, bool includeDetails = true, CancellationToken cancellationToken = default)
        {
            return await (await GetDbSetAsync())
                .IncludeDetails(includeDetails)//包含明细
                .OrderBy(t => t.CreationTime)
                .FirstOrDefaultAsync(t => t.ArchiveBoxRfid == rfidCode, GetCancellationToken(cancellationToken));
        }

        public async Task<ArchiveBox> FindByCellIdAsync(int cellId, bool includeDetails = true, CancellationToken cancellationToken = default)
        {
            return await (await GetDbSetAsync())
                .IncludeDetails(includeDetails)//包含明细
                .OrderBy(t => t.CreationTime)
                .FirstOrDefaultAsync(t => t.CellId == cellId, GetCancellationToken(cancellationToken));
        }

        public async Task<ArchiveBox> FindByArchiveBoxcodeAsync(string archiveBoxBarcode, bool includeDetails = true, CancellationToken cancellationToken = default)
        {
            return await(await GetDbSetAsync())
                .IncludeDetails(includeDetails)//包含明细
                .OrderBy(t => t.CreationTime)
                .FirstOrDefaultAsync(t => t.ArchiveBoxRfid == archiveBoxBarcode, GetCancellationToken(cancellationToken));
        }
    }
}
