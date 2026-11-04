using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using WarehouseManagement.RfidCodes;
using WarehouseManagement.RfidCodes.Aggregates;

namespace WarehouseManagement.EntityFrameworkCore.RfidCodes
{
    public class EFCoreRfidRepository : EfCoreRepository<IWarehouseManagementDbContext, Rfid, int>, IRfidRepository
    {
        public EFCoreRfidRepository(IDbContextProvider<IWarehouseManagementDbContext> dbContextProvider) : base(dbContextProvider)
        {

        }

        public async Task<Rfid> FindByIdAsync(int id, bool includeDetails = true, CancellationToken cancellationToken = default)
        {
            return await(await GetDbSetAsync())
                .OrderBy(t => t.CreationTime)
                .FirstOrDefaultAsync(t => t.Id == id, GetCancellationToken(cancellationToken));
        }
    }
}
