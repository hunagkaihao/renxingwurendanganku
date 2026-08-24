using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using WarehouseManagement.Checks.Aggregates;

namespace WarehouseManagement.Checks
{
    public interface ICheckRepository : IRepository<Check, int>
    {
        Task<Check> FindByIdAsync(
            int id,
            bool includeDetails = true,
            CancellationToken cancellationToken = default);
        Task<Check> FindByCheckCodeAsync(
            string id,
            bool includeDetails = true,
            CancellationToken cancellationToken = default);

        Task<List<Check>> GetPagingListAsync(
            string filter = null,
            int maxResultCount = 10,
            int skipCount = 0,
            bool includeDetails = false,
            CancellationToken cancellationToken = default);

        Task<long> GetPagingCountAsync(string filter = null,
            CancellationToken cancellationToken = default);
    }
}
