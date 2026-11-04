using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using WarehouseManagement.RfidCodes.Aggregates;

namespace WarehouseManagement.RfidCodes
{
    public interface IRfidRepository : IRepository<Rfid, int>
    {
        Task<Rfid> FindByIdAsync(
            int id,
            bool includeDetails = true,
            CancellationToken cancellationToken = default);
    }
}
