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
    public interface IArchiveBoxDetailRepository : IRepository<MaterialBoxDetail, int>
    {
        Task<MaterialBoxDetail> FindByArchiveIdAsync(
            int id,
            bool includeDetails = true,
            CancellationToken cancellationToken = default);
    }
}
