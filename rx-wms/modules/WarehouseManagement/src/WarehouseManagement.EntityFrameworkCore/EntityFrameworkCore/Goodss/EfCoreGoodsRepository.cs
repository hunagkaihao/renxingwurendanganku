using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using WarehouseManagement.Goodss;
using WarehouseManagement.Goodss.Aggregates;

namespace WarehouseManagement.EntityFrameworkCore.Goodss
{
    public class EfCoreGoodsRepository : EfCoreRepository<IWarehouseManagementDbContext, Goods, int>, IGoodsRepository
    {
        public EfCoreGoodsRepository(IDbContextProvider<IWarehouseManagementDbContext> dbContextProvider) : base(dbContextProvider)
        {

        }
        public async Task<List<Goods>> GetPagingListAsync(string filter = null,
            string goodsCode = null, string goodsSpec = null, 
            int maxResultCount = 10, int skipCount = 0, bool includeDetails = false, CancellationToken cancellationToken = default)
        {
            return await (await GetDbSetAsync())
                .IncludeDetails(includeDetails)
                .WhereIf(!filter.IsNullOrWhiteSpace(),
                    e => (e.GoodsName.Contains(filter)))
                .WhereIf(!goodsCode.IsNullOrWhiteSpace(),
                    e => (e.GoodsCode.Contains(goodsCode)))
                .WhereIf(!goodsSpec.IsNullOrWhiteSpace(),
                    e => (e.GoodsSpec.Contains(goodsSpec)))
                .OrderByDescending(e => e.CreationTime)
                .PageBy(skipCount, maxResultCount)
                .ToListAsync(GetCancellationToken(cancellationToken));
        }
        public async Task<List<Goods>> GetSelectOptionsAsync(
                string goodsName,
                string goodsSpec,
                int maxResultCount = 20,
                int skipCount = 0,
                bool includeDetails = false,
                CancellationToken cancellationToken = default)
        {
                        return await (await GetDbSetAsync())
                .IncludeDetails(includeDetails)
                .WhereIf(!goodsName.IsNullOrWhiteSpace(),
                    e => (e.GoodsName.Contains(goodsName)))
                    .WhereIf(!goodsSpec.IsNullOrWhiteSpace(),
                    e => (e.GoodsSpec.Contains(goodsSpec)))
                .OrderByDescending(e => e.CreationTime)
                .PageBy(skipCount, maxResultCount)
                .ToListAsync(GetCancellationToken(cancellationToken));
        }

        public async Task<long> GetPagingCountAsync(string filter = null,
            string goodsCode = null, string goodsSpec = null, 
            CancellationToken cancellationToken = default)
        {
            return await (await GetDbSetAsync())
                .WhereIf(!filter.IsNullOrWhiteSpace(),
                    e => (e.GoodsName.Contains(filter)))
                .WhereIf(!goodsCode.IsNullOrWhiteSpace(),
                    e => (e.GoodsCode.Contains(goodsCode)))
                .WhereIf(!goodsSpec.IsNullOrWhiteSpace(),
                    e => (e.GoodsSpec.Contains(goodsSpec)))
                .CountAsync(cancellationToken: cancellationToken);
        }
        public async Task<long> GetCountAsync(string filter = null, CancellationToken cancellationToken = default)
        {
            return await (await GetDbSetAsync())
                .WhereIf(!filter.IsNullOrWhiteSpace(), x => x.GoodsName.Contains(filter))
                .LongCountAsync(GetCancellationToken(cancellationToken));
        }

        public async Task<Goods> FindByIdAsync(int id, bool includeDetails = false, CancellationToken cancellationToken = default)
        {
            return await(await GetDbSetAsync())
                //.IncludeDetails(includeDetails)//包含明细
                .OrderBy(t => t.CreationTime)
                .FirstOrDefaultAsync(t => t.Id == id, GetCancellationToken(cancellationToken));
        }

        public async Task<Goods> FindByNameAsync(string goodsName, bool includeDetails = false, CancellationToken cancellationToken = default)
        {
            return await(await GetDbSetAsync())
                .IncludeDetails(includeDetails)//包含明细
                .OrderBy(t => t.CreationTime)
                .FirstOrDefaultAsync(t => t.GoodsName == goodsName, GetCancellationToken(cancellationToken));
        }

        public async Task<Goods> FindByCodeAsync(string goodsCode, bool includeDetails = false, CancellationToken cancellationToken = default)
        {
            return await (await GetDbSetAsync())
                .IncludeDetails(includeDetails)//包含明细
                .OrderBy(t => t.CreationTime)
                .FirstOrDefaultAsync(t => t.GoodsCode == goodsCode, GetCancellationToken(cancellationToken));
        }

    }
}
