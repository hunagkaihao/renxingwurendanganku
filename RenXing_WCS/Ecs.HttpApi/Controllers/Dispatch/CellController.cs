using System.Threading.Tasks;
using Ecs.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace Ecs.Dispatch;

[Route("ecs/dispatch")]
[ApiController]
public class CellController : EcsController, ICellService
{
    private readonly ICellService _cellService;

    public CellController(ICellService cellService)
    {
        _cellService = cellService;
    }

    [HttpPost("cellsClear")]
    [MiddlewareFilter(typeof(ApiLogPipeline))]
    public async Task<ResponseDto> CellsAllClearAsync()
    {
        return await _cellService.CellsAllClearAsync().ConfigureAwait(false);
    }

    [HttpPost("cellSeed")]
    [MiddlewareFilter(typeof(ApiLogPipeline))]
    public async Task<ResponseDto> CellSeedsAsync(AddCellsDto cellsDto)
    {
        return await _cellService.CellSeedsAsync(cellsDto).ConfigureAwait(false);
    }
}