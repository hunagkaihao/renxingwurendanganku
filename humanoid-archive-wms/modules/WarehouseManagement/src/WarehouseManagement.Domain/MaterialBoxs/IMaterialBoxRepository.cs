using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using WarehouseManagement.MaterialBoxs.Aggregates;

namespace WarehouseManagement.MaterialBoxs
{
    public interface IMaterialBoxRepository : IRepository<MaterialBox, int>
    {
        Task<MaterialBox> FindByIdAsync(
            int id,
            bool includeDetails = true,
            CancellationToken cancellationToken = default);
        
        Task<MaterialBox> FindByBoxNameAsync(
            string name,
            bool includeDetails = true,
            CancellationToken cancellationToken = default);

        Task<MaterialBox> FindByArchiveBoxcodeAsync(
            string archiveBoxBarcode,
            bool includeDetails = true,
            CancellationToken cancellationToken = default);

        Task<MaterialBox> FindByRfidCodeAsync(
            string rfidCode,
            bool includeDetails = true,
            CancellationToken cancellationToken = default);

        Task<MaterialBox> FindByCellIdAsync(
            int cellId,
            bool includeDetails = true,
            CancellationToken cancellationToken = default);
    }
}
