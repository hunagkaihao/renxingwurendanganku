using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using WarehouseManagement.TaskHiss;
using WarehouseManagement.TaskHiss.Aggregates;

namespace WarehouseManagement.EntityFrameworkCore.TaskHiss
{
    public class EfCoreTaskHisDetailRepository : EfCoreRepository<IWarehouseManagementDbContext, TaskHisDetail, int>, ITaskHisDetailRepository
    {
        public EfCoreTaskHisDetailRepository(IDbContextProvider<IWarehouseManagementDbContext> dbContextProvider) : base(dbContextProvider)
        {

        }
     }
}
