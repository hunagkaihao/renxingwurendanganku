using System.Collections.Generic;
using System.Threading.Tasks;
using Ecs.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace Ecs.Dispatch;

[Route("ecs/dispatch")]
[ApiController]
public class ConditionController : EcsController, IConditionService
{
    private readonly IConditionService _conditionService;

    public ConditionController(IConditionService conditionService)
    {
        _conditionService = conditionService;
    }

    [HttpPost("conditionSeed")]
    [MiddlewareFilter(typeof(ApiLogPipeline))]
    public async Task<ResponseDto> ConditionSeedsAsync()
    {
        return await _conditionService.ConditionSeedsAsync().ConfigureAwait(false);
    }

    [HttpPost("allConditionDel")]
    public async Task<ResponseDto> DelAllConditionsAsync()
    {
        return await _conditionService.DelAllConditionsAsync().ConfigureAwait(false);
    }

    [HttpGet("allConditionGet")]
    public async Task<List<ConditionDto>> GetAllContitionsAsync()
    {
        return await _conditionService.GetAllContitionsAsync().ConfigureAwait(false);
    }
}