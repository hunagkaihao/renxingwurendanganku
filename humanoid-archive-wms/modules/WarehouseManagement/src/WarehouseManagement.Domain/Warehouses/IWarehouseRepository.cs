using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using WarehouseManagement.Warehouses.Aggregates;

namespace WarehouseManagement.Warehouses
{
    public interface IWarehouseRepository : IRepository<Warehouse, int>
    {
        Task<Warehouse> FindByIdAsync(
            int id,
            bool includeDetails = false,
            CancellationToken cancellationToken = default);

        Task<Warehouse> FindByNameAsync(
            string name,
            bool includeDetails = false,
            CancellationToken cancellationToken = default);

        Task<Warehouse> FindByCodeAsync(
    string name,
    bool includeDetails = false,
    CancellationToken cancellationToken = default);

        Task<List<Warehouse>> GetPagingListAsync(
            string filter = null,
            int maxResultCount = 10,
            int skipCount = 0,
            bool includeDetails = false,
            CancellationToken cancellationToken = default);

        Task<long> GetPagingCountAsync(string filter = null,
            CancellationToken cancellationToken = default);


    }
}
