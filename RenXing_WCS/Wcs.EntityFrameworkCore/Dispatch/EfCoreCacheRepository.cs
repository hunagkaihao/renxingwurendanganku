using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Wcs.Caches;
using Wcs.Caches.Models;
using Wcs.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Wms.EntityFrameworkCore.Repositories.ArchiveBoxs
{
    public class EfCoreCacheRepository : EfCoreRepository<WcsDbContext, DispatchCache, int>, ICacheRepository
    {
        public EfCoreCacheRepository(IDbContextProvider<WcsDbContext> dbContextProvider) : base(dbContextProvider)
        {

        }

        public async Task<DispatchCache> FindCacheByCachePosAsync(byte cachePos, CancellationToken cancelToken = default)
        {
            var dbSet = await GetDbSetAsync().ConfigureAwait(false);
            var caches = await dbSet.AsNoTracking().Where(o => o.CachePos == cachePos).ToListAsync(cancelToken);
            if(caches.Count > 1)
                throw new Exception($"{cachePos}号位的缓存数量不止1个，数据错误");
            if(caches.Count == 1) return caches[0];
            else return null;
        }

        public async Task<List<DispatchCache>> FindCachesWithSpecsAsync(string specs, CancellationToken cancelToken = default)
        {
            var dbSet = await GetDbSetAsync().ConfigureAwait(false);
            return await dbSet.AsNoTracking().Where(o => o.Specs == specs).ToListAsync(cancelToken);
        }
    }
}