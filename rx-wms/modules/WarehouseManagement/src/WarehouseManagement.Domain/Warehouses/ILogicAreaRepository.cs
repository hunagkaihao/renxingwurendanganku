using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using WarehouseManagement.Warehouses.Aggregates;

namespace WarehouseManagement.Warehouses
{
    public interface ILogicAreaRepository : IRepository<LogicArea, int>
    {
        Task<LogicArea> FindByIdAsync(
            int id,
            bool includeDetails = false,
            CancellationToken cancellationToken = default);

        Task<LogicArea> FindByNameAsync(
            string name,
            bool includeDetails = false,
            CancellationToken cancellationToken = default);

        Task<LogicArea> FindByCodeAsync(
    string name,
    bool includeDetails = false,
    CancellationToken cancellationToken = default);

        Task<List<LogicArea>> GetPagingListAsync(
            string filter = null,
            int maxResultCount = 10,
            int skipCount = 0,
            bool includeDetails = false,
            CancellationToken cancellationToken = default);

        Task<long> GetPagingCountAsync(string filter = null,
            CancellationToken cancellationToken = default);

    }
}
