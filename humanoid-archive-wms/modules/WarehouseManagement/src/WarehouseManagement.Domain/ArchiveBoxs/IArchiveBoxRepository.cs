using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using WarehouseManagement.ArchiveBoxs.Aggregates;

namespace WarehouseManagement.ArchiveBoxs
{
    public interface IArchiveBoxRepository : IRepository<ArchiveBox, int>
    {
        Task<ArchiveBox> FindByIdAsync(
            int id,
            bool includeDetails = true,
            CancellationToken cancellationToken = default);
        
        Task<ArchiveBox> FindByBoxNameAsync(
            string name,
            bool includeDetails = true,
            CancellationToken cancellationToken = default);

        Task<ArchiveBox> FindByArchiveBoxcodeAsync(
            string archiveBoxBarcode,
            bool includeDetails = true,
            CancellationToken cancellationToken = default);

        Task<ArchiveBox> FindByRfidCodeAsync(
            string rfidCode,
            bool includeDetails = true,
            CancellationToken cancellationToken = default);

        Task<ArchiveBox> FindByCellIdAsync(
            int cellId,
            bool includeDetails = true,
            CancellationToken cancellationToken = default);
    }
}
