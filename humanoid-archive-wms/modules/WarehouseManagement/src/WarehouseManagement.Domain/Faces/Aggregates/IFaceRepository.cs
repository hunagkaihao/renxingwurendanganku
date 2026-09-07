using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using WarehouseManagement.MaterialBoxs.Aggregates;
using WarehouseManagement.Material.Aggregates;

namespace WarehouseManagement.Faces.Aggregates
{
    public interface  IFaceRepository : IRepository<Face, int>
    {
        Task<Face> FindByIdAsync(
            string UserId,
            bool includeDetails = true,
            CancellationToken cancellationToken = default);


    }
}
