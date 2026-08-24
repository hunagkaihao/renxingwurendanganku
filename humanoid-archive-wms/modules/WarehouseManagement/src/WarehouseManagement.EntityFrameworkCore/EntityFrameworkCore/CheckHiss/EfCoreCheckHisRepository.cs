using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using WarehouseManagement.CheckHiss;
using WarehouseManagement.CheckHiss.Aggregates;

namespace WarehouseManagement.EntityFrameworkCore.CheckHiss
{
    public class EfCoreCheckHisRepository : EfCoreRepository<IWarehouseManagementDbContext,CheckHis,int>, ICheckHisRepository
    {
        public EfCoreCheckHisRepository(IDbContextProvider<IWarehouseManagementDbContext> dbContextProvider) : base(dbContextProvider)
        {

        }
    }
}
