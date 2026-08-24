using System.Threading.Tasks;
using Ecs.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace Ecs.Dispatch;

[Route("ecs/dispatch")]
[ApiController]
public class ProcessController : EcsController, IProcessService
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