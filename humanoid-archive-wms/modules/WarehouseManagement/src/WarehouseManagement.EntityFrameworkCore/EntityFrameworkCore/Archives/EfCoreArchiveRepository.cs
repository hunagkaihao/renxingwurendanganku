using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using WarehouseManagement.Archives;
using WarehouseManagement.Archives.Aggregates;

namespace WarehouseManagement.EntityFrameworkCore.Archives
{
    public class EfCoreArchiveRepository : EfCoreRepository<IWarehouseManagementDbContext, Archive ,int>, IArchiveRepository
    {
        public EfCoreArchiveRepository(IDbContextProvider<IWarehouseManagementDbContext> dbContextProvider) : base(dbContextProvider)
        {

        }
        public async Task<Archive> FindByIdAsync(int Id, bool includeDetails = true, CancellationToken cancellationToken = default)
        {
            return await (await GetDbSetAsync())
                .OrderBy(t => t.CreationTime)
                .FirstOrDefaultAsync(t => t.Id == Id, GetCancellationToken(cancellationToken));
        }
        public async Task<Archive> FindByRfidCodeAsync(string rfidCode, bool includeDetails = true, CancellationToken cancellationToken = default)
        {
            return await (await GetDbSetAsync())
                .OrderBy(t => t.CreationTime)
                .FirstOrDefaultAsync(t => t.RfidId == rfidCode, GetCancellationToken(cancellationToken));
        }
    }
}
