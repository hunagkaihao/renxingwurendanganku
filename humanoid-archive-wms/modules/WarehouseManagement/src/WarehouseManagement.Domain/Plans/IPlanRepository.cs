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
    public interface IPlanRepository : IRepository<Plan, int>
    {
        Task<Plan> FindByIdAsync(
            int id,
            bool includeDetails = true,
            CancellationToken cancellationToken = default);
        Task<Plan> FindByBillNoAsync(
    string billNo,
    bool includeDetails = true,
    CancellationToken cancellationToken = default);


        Task<List<Plan>> GetPagingListAsync(
            string filter = null,
            int maxResultCount = 10,
            int skipCount = 0,
            bool includeDetails = false,
            CancellationToken cancellationToken = default);

        Task<long> GetPagingCountAsync(string filter = null,
            CancellationToken cancellationToken = default);
    }
}
