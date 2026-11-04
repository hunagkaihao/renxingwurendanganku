using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using WarehouseManagement.Goodss.Aggregates;

namespace WarehouseManagement.Goodss
{
    public interface IGoodsRepository : IRepository<Goods, int>
    {
        Task<Goods> FindByIdAsync(
            int id,
            bool includeDetails = false,
            CancellationToken cancellationToken = default);

        Task<Goods> FindByNameAsync(
            string name,
            bool includeDetails = false,
            CancellationToken cancellationToken = default);

        Task<Goods> FindByCodeAsync(
    string goodsCode,
    bool includeDetails = false,
    CancellationToken cancellationToken = default);

        Task<List<Goods>> GetPagingListAsync(
            string filter = null,
            string goodsCode = null, string goodsSpec = null,
            int maxResultCount = 10,
            int skipCount = 0,
            bool includeDetails = false,
            CancellationToken cancellationToken = default);

        Task<List<Goods>> GetSelectOptionsAsync(
    string goodsName,
    string goodsSpec,
    int maxResultCount = 20,
    int skipCount = 0,
    bool includeDetails = false,
    CancellationToken cancellationToken = default);

        Task<long> GetPagingCountAsync(string filter = null, string goodsCode = null, string goodsSpec = null,
            CancellationToken cancellationToken = default);

    }
}
