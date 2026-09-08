using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using MaterialAggregate = WarehouseManagement.Material.Aggregates.Material;

namespace WarehouseManagement.Material
{
    public interface IMaterialRepository : IRepository<MaterialAggregate, int>
    {
        Task<MaterialAggregate> FindByIdAsync(
            int id,
            bool includeDetails = true,
            CancellationToken cancellationToken = default);
        Task<MaterialAggregate> FindByRfidCodeAsync(
            string rfidCode,
            bool includeDetails = true,
            CancellationToken cancellationToken = default);
    }
}
