using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Volo.Abp;
using Wcs.LogTool;
using Wcs.Caches;
using Wcs.Caches.Models;

namespace Wcs.Dispatch;

public class CacheService : WcsAppService, ICacheService
{
    private readonly ILogger<CacheService> _logger;
    private readonly CacheManager _cacheManager;
    private readonly ICacheRepository _cacheRepository;
    
    public CacheService(
        CacheManager cacheManager,
        ICacheRepository cacheRepository,
        ILogger<CacheService> logger)
    {
        _cacheManager = cacheManager;
        _cacheRepository = cacheRepository;
        _logger = logger;
    }

    public async Task<ResponseDto> AddCacheAsync(AddCacheDto cache)
    {
        try
        {
            DispatchCache cacheExist = await _cacheRepository.FindCacheByCachePosAsync(cache.CachePos).ConfigureAwait(false);
            if(cacheExist != null)
                return new ResponseDto(){ success = false, message = $"缓存位{cache.CachePos}已经存在"};
            
            DispatchCache cacheToAdd = await _cacheManager.CreateCacheAsync(cache.CachePos, cache.DASpecs).ConfigureAwait(false);
            if(cacheToAdd == null)
                return new ResponseDto(){ success = false, message = $"创建缓存失败" };

            await _cacheRepository.InsertAsync(cacheToAdd).ConfigureAwait(false);
            return new ResponseDto() { success = true, message = "添加成功" };
        }
        catch(Exception ex)
        {
            _logger.Error(ex.Message);
            return new ResponseDto() { success = false, message = ex.Message };
        }
    }

    public async Task<ResponseDto> DelAllCachesAsync()
    {
        try
        {
            await _cacheRepository.DeleteAsync(o => o.Id > 0).ConfigureAwait(false);
            return new ResponseDto() { success = true, message = "删除成功" };
        }
        catch(Exception ex)
        {
            _logger.Error(ex.Message);
            return new ResponseDto() { success = false, message = ex.Message };
        }
    }

    public async Task<List<CacheDto>> GetAllCachesAsync()
    {
        try
        {
            await Task.Delay(1).ConfigureAwait(false);
            _cacheManager.GetAllCaches(out List<DispatchCache> caches);
            return ObjectMapper.Map<List<DispatchCache>, List<CacheDto>>(caches);
        }
        catch(Exception ex)
        {
            _logger.Error(ex.Message);
            return new List<CacheDto>();
        }
    }
}