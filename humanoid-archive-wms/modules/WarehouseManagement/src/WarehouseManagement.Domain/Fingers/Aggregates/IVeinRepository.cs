using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using WarehouseManagement.Faces.Aggregates;

namespace WarehouseManagement.Fingers.Aggregates
{
    public interface IVeinRepository : IRepository<Vein, int>
    {


        Task<List<Vein>> GetVeinsByUserId(
           string userId,
           bool includeDetails = true,
           CancellationToken cancellationToken = default);
        Task<Vein> GetVeinsByFingerId(
           string fingerId,
           bool includeDetails = true,
           CancellationToken cancellationToken = default);




    }
}
