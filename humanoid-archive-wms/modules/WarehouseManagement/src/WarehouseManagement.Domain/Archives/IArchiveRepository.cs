using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using WarehouseManagement.Archives.Aggregates;

namespace WarehouseManagement.Archives
{
    public interface IArchiveRepository : IRepository<Archive, int>
    {
        Task<Archive> FindByIdAsync(
            int id,
            bool includeDetails = true,
            CancellationToken cancellationToken = default);
        Task<Archive> FindByRfidCodeAsync(
            string rfidCode,
            bool includeDetails = true,
            CancellationToken cancellationToken = default);
    }
}
