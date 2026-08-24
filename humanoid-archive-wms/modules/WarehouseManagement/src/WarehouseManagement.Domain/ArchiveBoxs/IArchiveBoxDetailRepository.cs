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
    public interface IArchiveBoxDetailRepository : IRepository<ArchiveBoxDetail, int>
    {
        Task<ArchiveBoxDetail> FindByArchiveIdAsync(
            int id,
            bool includeDetails = true,
            CancellationToken cancellationToken = default);
    }
}
