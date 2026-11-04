using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using WarehouseManagement.Plans.Aggregates;

namespace WarehouseManagement.Plans
{
    public interface IPlanListRepository : IRepository<PlanList, int>
    {
        Task<PlanList> FindByIdAsync(
            int id,
            CancellationToken cancellationToken = default);


        Task<List<PlanList>> GetPagingListAsync(
            string filter = null,
            int maxResultCount = 10,
            int skipCount = 0,
            CancellationToken cancellationToken = default);

        Task<long> GetPagingCountAsync(string filter = null,
            CancellationToken cancellationToken = default);
    }
}
