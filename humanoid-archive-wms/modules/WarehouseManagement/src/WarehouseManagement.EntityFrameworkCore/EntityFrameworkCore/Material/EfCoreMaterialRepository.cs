using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using WarehouseManagement.Material;
using MaterialAggregate = WarehouseManagement.Material.Aggregates.Material;

namespace WarehouseManagement.EntityFrameworkCore.Material
{
    public class EfCoreMaterialRepository : EfCoreRepository<IWarehouseManagementDbContext, MaterialAggregate, int>, IMaterialRepository
    {
        public EfCoreMaterialRepository(IDbContextProvider<IWarehouseManagementDbContext> dbContextProvider) : base(dbContextProvider)
        {

        }
        public async Task<MaterialAggregate> FindByIdAsync(int Id, bool includeDetails = true, CancellationToken cancellationToken = default)
        {
            return await (await GetDbSetAsync())
                .OrderBy(t => t.CreationTime)
                .FirstOrDefaultAsync(t => t.Id == Id, GetCancellationToken(cancellationToken));
        }
        public async Task<MaterialAggregate> FindByRfidCodeAsync(string rfidCode, bool includeDetails = true, CancellationToken cancellationToken = default)
        {
            return await (await GetDbSetAsync())
                .OrderBy(t => t.CreationTime)
                .FirstOrDefaultAsync(t => t.RfidId == rfidCode, GetCancellationToken(cancellationToken));
        }
    }
}
