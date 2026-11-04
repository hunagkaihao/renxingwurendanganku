using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using WarehouseManagement.TaskHiss.Aggregates;

namespace WarehouseManagement.TaskHiss
{
    public interface ITaskHisRepository : IRepository<TaskHis, int>
    {
        Task<TaskHis> FindByIdAsync(
            int id,
            bool includeDetails = true,
            CancellationToken cancellationToken = default);

        Task<List<TaskHis>> GetPagingListAsync(
            string filter = null,
            int maxResultCount = 10,
            int skipCount = 0,
            bool includeDetails = false,
            CancellationToken cancellationToken = default);

        Task<long> GetPagingCountAsync(string filter = null,
            CancellationToken cancellationToken = default);
    }
}
