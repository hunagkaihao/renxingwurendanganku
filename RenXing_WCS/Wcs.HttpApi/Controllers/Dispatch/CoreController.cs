using System.Threading.Tasks;
using Wcs.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace Wcs.Dispatch;

[Route("ecs/dispatch")]
[ApiController]
public class CoreController : WcsController, ICoreService
{
    private readonly ICoreService _coreService;

    public CoreController(ICoreService coreService)
    {
        _coreService = coreService;
    }

    [HttpGet("core/wcsStatus")]
    public async Task<string> GetDispatchSvrStateAsync()
    {
        return await _coreService.GetDispatchSvrStateAsync().ConfigureAwait(false);
    }

    [HttpPost("core/pause")]
    [MiddlewareFilter(typeof(ApiLogPipeline))]
    public ResponseDto PauseDispatcherSvr()
    {
        return _coreService.PauseDispatcherSvr();
    }

    [HttpPost("core/restart")]
    [MiddlewareFilter(typeof(ApiLogPipeline))]
    public ResponseDto RestartDispatcherSvr()
    {
        return _coreService.RestartDispatcherSvr();
    }

}