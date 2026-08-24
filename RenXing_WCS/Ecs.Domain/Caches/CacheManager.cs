using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Volo.Abp;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Ecs.LogTool;
using Volo.Abp.Uow;
using System.Collections.Generic;
using System.Linq;
using Ecs.Dispatch;
using Ecs.Caches.Models;
using Ecs.DahSpecss.Models;

namespace Ecs.Caches;

public class CacheManager : ISingletonDependency
{
    private readonly ILogger<CacheManager> _logger;
    private readonly IRepository<DahSpecs, int> _dahSpecsRepository;
    private readonly ICacheRepository _cacheRepository;
    private readonly IUnitOfWorkManager _unitOfWorkManager;
    private readonly object mLocker = new object();

    public CacheManager(
        IRepository<DahSpecs, int> dahSpecsRepository,
        ICacheRepository cacheRepository,
        IUnitOfWorkManager unitOfWorkManager,
        ILogger<CacheManager> logger)
    {
        _dahSpecsRepository = dahSpecsRepository;
        _cacheRepository = cacheRepository;
        _unitOfWorkManager = unitOfWorkManager;
        _logger = logger;
    }

    /// <summary>
    /// 创建缓存的实例
    /// </summary>
    /// <param name="cachePos"></param>
    /// <param name="cacheSpecs"></param>
    /// <returns></returns>
    public async Task<DispatchCache> CreateCacheAsync(byte cachePos, string cacheSpecs)
    {
        try
        {
            var specsList = await _dahSpecsRepository.GetListAsync(o => o.SpecCode == cacheSpecs).ConfigureAwait(false);
            if (specsList.Count == 0)
                throw new Exception($"规格{cacheSpecs}不存在");

            return new DispatchCache(cachePos, cacheSpecs);
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            return null;
        }
    }

    /// <summary>
    /// 查询被调度任务占用的缓存
    /// </summary>
    /// <param name="taskId"></param>
    /// <returns>false：发生错误，查询失败，true：查询成功</returns>
    public bool GetCacheByTaskId(int taskId, out DispatchCache cache)
    {
        lock (mLocker)
        {
            try
            {
                cache = null;
                var getTask = _cacheRepository.GetListAsync(o => o.TaskIdOwnIt == taskId);
                var caches = getTask.GetAwaiter().GetResult();
                if (caches.Count == 0) return true;

                if (caches.Count > 1)
                    throw new Exception($"被任务号{taskId}占用的缓存多于1个");

                cache = caches[0];
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                cache = null;
                return false;
            }
        }
    }

    /// <summary>
    /// 查询所有的缓存，按Id升序排列，未查询到数据，返回空集合
    /// </summary>
    /// <param name="caches"></param>
    /// <returns>true：查询成功，false：查询失败</returns>
    public bool GetAllCaches(out List<DispatchCache> caches)
    {
        lock (mLocker)
        {
            try
            {
                caches = new List<DispatchCache>();

                var getTask = _cacheRepository.GetListAsync();
                caches = getTask.GetAwaiter().GetResult().OrderBy(o => o.Id).ToList();
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                caches = new List<DispatchCache>();
                return false;
            }
        }
    }

    /// <summary>
    /// 查询指定规格的缓存，并占用此缓存
    /// </summary>
    /// <param name="daSpecs"></param>
    /// <param name="taskId"></param>
    /// <returns></returns>
    public DispatchCache GetFirstIdleCacheWithSpecsAndOccupyIt(string daSpecs, int taskId)
    {
        lock (mLocker)
        {
            try
            {
                Check.Positive(taskId, nameof(taskId));

                var gTask = _cacheRepository.GetListAsync(o => o.TaskIdOwnIt == taskId);
                var cs = gTask.GetAwaiter().GetResult();
                if (cs.Count > 0) //已经存在被该任务号占用的缓存，则返回此缓存
                    return cs[0];

                var getTask = _cacheRepository.GetListAsync(o => o.Specs == daSpecs && o.TaskIdOwnIt == -1);
                var caches = getTask.GetAwaiter().GetResult();
                if (caches.Count == 0) return null;

                caches[0].TaskIdOwnIt = taskId;
                var updateTask = _cacheRepository.UpdateAsync(caches[0], true);

                return updateTask.GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                return null;
            }
        }
    }

    /// <summary>
    /// 释放指定位置的缓存，释放成功返回true，失败返回false
    /// </summary>
    /// <param name="cachePos"></param>
    /// <returns></returns>
    public bool ReleaseCache(byte cachePos)
    {
        lock (mLocker)
        {
            try
            {
                var getTask = _cacheRepository.GetListAsync(o => o.CachePos == cachePos);
                var caches = getTask.GetAwaiter().GetResult();
                if (caches.Count == 0) throw new Exception($"不存在缓存码为{cachePos}的缓存");
                if (caches.Count > 1) throw new Exception($"缓存位为{cachePos}的缓存多于1个");

                caches[0].TaskIdOwnIt = -1;
                _cacheRepository.UpdateAsync(caches[0], true).Wait();

                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                return false;
            }
        }
    }

    /// <summary>
    /// 释放被指定任务占用的缓存，释放成功返回true，失败返回false
    /// </summary>
    /// <param name="taskIdOccupy"></param>
    /// <returns></returns>
    public bool ReleaseCache(int taskIdOccupy)
    {
        lock (mLocker)
        {
            try
            {
                // 2. 查询被指定任务占用的所有缓存
                var getTask = _cacheRepository.GetListAsync(o => o.TaskIdOwnIt == taskIdOccupy);
                var caches = getTask.GetAwaiter().GetResult();

                // 3. 如果没有占用缓存，直接返回成功
                if (caches.Count == 0)
                    return true;

                // 4. 使用事务处理缓存释放
                using (var unit = _unitOfWorkManager.Begin(isTransactional: true))
                {
                    // 5. 遍历所有被占用的缓存
                    foreach (var cache in caches)
                    {
                        // 6. 将缓存的任务ID设置为-1，表示未被占用
                        cache.TaskIdOwnIt = -1;
                        // 7. 更新缓存记录
                        _cacheRepository.UpdateAsync(cache, true).Wait();
                    }
                    // 8. 提交事务
                    unit.CompleteAsync().Wait();
                }

                return true;
            }
            catch (Exception ex)
            {
                // 9. 错误处理
                _logger.Error(ex.Message);
                return false;
            }
        }
    }
}