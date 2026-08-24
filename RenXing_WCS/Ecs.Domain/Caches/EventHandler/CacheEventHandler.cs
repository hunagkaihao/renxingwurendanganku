using System;
using System.Threading.Tasks;
using Ecs.Caches;
using Ecs.Caches.Etos;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus;
using Volo.Abp.Uow;

namespace Ecs.Caches.EventHandler;

public class CacheEventsHandler :
    ILocalEventHandler<ReleaseCacheOccupiedEvent>,
    ITransientDependency

{
    private readonly CacheManager _cacheManager;

    public CacheEventsHandler(
        CacheManager cacheManager)
    {
        _cacheManager = cacheManager;
    }

    public virtual async Task HandleEventAsync(ReleaseCacheOccupiedEvent eventData)
    {
        await Task.Delay(1).ConfigureAwait(false);

        int taskIdOwnCache = eventData.TaskIdOwnCache;
        bool ret = _cacheManager.ReleaseCache(taskIdOwnCache);
        if (!ret)
            throw new Exception($"{nameof(ReleaseCacheOccupiedEvent)}事件执行失败");
    }
}