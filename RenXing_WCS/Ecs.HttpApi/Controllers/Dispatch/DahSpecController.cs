using System.Collections.Generic;
using System.Threading.Tasks;
using Ecs.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace Ecs.Dispatch;

[Route("ecs/dahSpec")]
[ApiController]
public class DahSpecController : EcsController, IDahSpecService
{
    private readonly IDahSpecService _dahSpecService;

    public DahSpecController(IDahSpecService dahSpecService)
    {
        _dahSpecService = dahSpecService;
    }

    [HttpPost("specAdd")]
    public async Task<ResponseDto> AddDahSpecAsync(AddDahSpecDto spec)
    {
        return await _dahSpecService.AddDahSpecAsync(spec).ConfigureAwait(false);
    }

    [HttpPost("allSpecDel")]
    public async Task<ResponseDto> DelAllDahSpecsAsync()
    {
        return await _dahSpecService.DelAllDahSpecsAsync().ConfigureAwait(false);
    }

    [HttpGet("allSpecGet")]
    public async Task<List<DahSpecDto>> GetAllDahSpecsAsync()
    {
        return await _dahSpecService.GetAllDahSpecsAsync().ConfigureAwait(false);
    }
}