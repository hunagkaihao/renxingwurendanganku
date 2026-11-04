using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using WarehouseManagement.CheckHiss.Aggregates;
using WarehouseManagement.CheckHiss;
using Volo.Abp.EntityFrameworkCore;

namespace WarehouseManagement.EntityFrameworkCore.CheckHiss
{
    public class EfCoreCheckDetailHisRepository : EfCoreRepository<IWarehouseManagementDbContext, CheckDetailHis, int>, ICheckDetailHisRepository
    {
        public EfCoreCheckDetailHisRepository(IDbContextProvider<IWarehouseManagementDbContext> dbContextProvider) : base(dbContextProvider)
        {

        }
    }
}
