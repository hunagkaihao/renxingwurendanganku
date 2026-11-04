using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using WarehouseManagement.Faces.Aggregates;
using WarehouseManagement.Fingers.Aggregates;

namespace WarehouseManagement.EntityFrameworkCore.Fingers
{
    public class EfCoreVeinRepository : EfCoreRepository<IWarehouseManagementDbContext, Vein, int>, IVeinRepository
    {
        public EfCoreVeinRepository(IDbContextProvider<IWarehouseManagementDbContext> dbContextProvider) : base(dbContextProvider)
        {

        }


        public async Task<List<Vein>> GetVeinsByUserId(string userId, bool includeDetails = true, CancellationToken cancellationToken = default)
        {


            return await (await GetDbSetAsync())
                      .OrderByDescending(o => o.CreationTime)
                      .IncludeDetails(includeDetails)
                      //包含明细
                      .Where(t => t.UserId == userId)
                     .ToListAsync(GetCancellationToken(cancellationToken));


        }

        public async Task<Vein> GetVeinsByFingerId(string fingerId, bool includeDetails = true, CancellationToken cancellationToken = default)
        {
            return (await GetDbSetAsync())
                      .OrderByDescending(o => o.CreationTime)
                      .IncludeDetails(includeDetails)
                      //包含明细
                     .FirstOrDefault(t => t.FingerId == fingerId);
        }

    }
}
