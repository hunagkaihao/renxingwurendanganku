using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using WarehouseManagement.ArchiveBoxs.Aggregates;
using WarehouseManagement.ArchiveBoxs;
using System.Threading;
using WarehouseManagement.Faces.Aggregates;
using Microsoft.EntityFrameworkCore;

namespace WarehouseManagement.EntityFrameworkCore.Faces
{
    public class EfCoreFaceRepository : EfCoreRepository<IWarehouseManagementDbContext, Face, int>, IFaceRepository
    {

        public EfCoreFaceRepository(IDbContextProvider<IWarehouseManagementDbContext> dbContextProvider) : base(dbContextProvider)
        {

        }

        public async Task<Face> FindByIdAsync(string UserId, bool includeDetails = true, CancellationToken cancellationToken = default)
        {
            return await(await GetDbSetAsync())
                .OrderBy(t => t.CreationTime)
                .IncludeDetails(includeDetails)//包含明细
                .FirstOrDefaultAsync(t => t.UserId == UserId, GetCancellationToken(cancellationToken));

        }
    }
}
