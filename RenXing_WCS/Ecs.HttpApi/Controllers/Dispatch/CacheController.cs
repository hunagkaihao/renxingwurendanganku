using System.Collections.Generic;
using System.Threading.Tasks;
using Ecs.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace Ecs.Dispatch;

[Route("ecs/dispatch")]
[ApiController]
public class CacheController : EcsController, ICacheService
{
    private readonly ICacheService _cacheService;

    public CacheController(ICacheService cacheService)
    {
        _cacheService = cacheService;
    }

    [HttpPost("cacheAdd")]
    public async Task<ResponseDto> AddCacheAsync(AddCacheDto cache)
    {
        return await _cacheService.AddCacheAsync(cache).ConfigureAwait(false);
    }

    [HttpPost("allCacheDel")]
    public async Task<ResponseDto> DelAllCachesAsync()
    {
        return await _cacheService.DelAllCachesAsync().ConfigureAwait(false);
    }

    [HttpGet("allCacheGet")]
    public async Task<List<CacheDto>> GetAllCachesAsync()
    {
        return await _cacheService.GetAllCachesAsync().ConfigureAwait(false);
    }
}