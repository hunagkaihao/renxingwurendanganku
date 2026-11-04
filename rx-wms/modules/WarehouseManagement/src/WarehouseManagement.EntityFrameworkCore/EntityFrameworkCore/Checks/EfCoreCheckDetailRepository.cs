using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using WarehouseManagement.Checks;
using WarehouseManagement.Checks.Aggregates;

namespace WarehouseManagement.EntityFrameworkCore.Checks
{
    public class EfCoreCheckDetailRepository : EfCoreRepository<IWarehouseManagementDbContext, CheckDetail, int>, ICheckDetailRepository
    {
        public EfCoreCheckDetailRepository(IDbContextProvider<IWarehouseManagementDbContext> dbContextProvider) : base(dbContextProvider)
        {

        }
     }
}
