using System.Collections.Generic;
using System.Threading.Tasks;
using Wcs.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace Wcs.Dispatch;

[Route("ecs/dahSpec")]
[ApiController]
public class DahSpecController : WcsController, IDahSpecService
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