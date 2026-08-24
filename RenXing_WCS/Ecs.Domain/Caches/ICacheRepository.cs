using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Ecs.Caches.Models;
using Volo.Abp.Domain.Repositories;

namespace Ecs.Caches;

public interface ICacheRepository : IRepository<DispatchCache, int>
{
    public Task<DispatchCache> FindCacheByCachePosAsync(byte cachePos, CancellationToken cancelToken = default);
    public Task<List<DispatchCache>> FindCachesWithSpecsAsync(string specs, CancellationToken cancelToken = default);
}