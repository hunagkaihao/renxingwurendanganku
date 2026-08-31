using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Wcs.Dispatch;

public interface ICacheService : IApplicationService
{
    public Task<ResponseDto> AddCacheAsync(AddCacheDto cache);

    public Task<ResponseDto> DelAllCachesAsync();

    public Task<List<CacheDto>> GetAllCachesAsync();
}