using System.Threading.Tasks;
using Wcs.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace Wcs.Dispatch;

[Route("ecs/dispatch")]
[ApiController]
public class ProcessController : WcsController, IProcessService
{
    private readonly IProcessService _processService;

    public ProcessController(IProcessService pathService)
    {
        _processService = pathService;
    }

    [HttpPost("process/processAllDel")]
    public async Task<ResponseDto> DelAllProcessesAsync()
    {
        return await _processService.DelAllProcessesAsync().ConfigureAwait(false);
    }

    [HttpPost("process/processSeed")]
    [MiddlewareFilter(typeof(ApiLogPipeline))]
    public async Task<ResponseDto> ProcessSeedAsync(AddProcessDto process)
    {
        return await _processService.ProcessSeedAsync(process).ConfigureAwait(false);
    }
}