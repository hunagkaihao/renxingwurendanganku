using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using WarehouseManagement.ArchiveBoxs;
using WarehouseManagement.ArchiveBoxs.Aggregates;

namespace WarehouseManagement.EntityFrameworkCore.ArchiveBoxDetails
{
    public class EfCoreArchiveBoxDetailRepository : EfCoreRepository<IWarehouseManagementDbContext, ArchiveBoxDetail, int>
        , IArchiveBoxDetailRepository
    {
        public EfCoreArchiveBoxDetailRepository(IDbContextProvider<IWarehouseManagementDbContext> dbContextProvider) : base(dbContextProvider)
        {

        }

        public async Task<ArchiveBoxDetail> FindByArchiveIdAsync(int id, bool includeDetails = true, CancellationToken cancellationToken = default)
        {
            return await(await GetDbSetAsync())
                .OrderBy(t => t.CreationTime)
                .FirstOrDefaultAsync(t => t.ArchiveId == id, GetCancellationToken(cancellationToken));
        }
    }
}
