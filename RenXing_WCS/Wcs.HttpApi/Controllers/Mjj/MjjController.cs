using System.Collections.Generic;
using System.Threading.Tasks;
using Wcs.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace Wcs.Mjj;

[Route("wcs/mjj")]
[ApiController]
public class MjjController : WcsController, IMjjService
{
    private readonly IMjjService _mjjService;

    public MjjController(IMjjService mjjService)
    {
        _mjjService = mjjService;
    }

    [HttpGet("mjjStatus")]
    public async Task<MjjStatusDto> GetStatusAsync()
    {
        return await _mjjService.GetStatusAsync().ConfigureAwait(false);
    }

    [HttpGet("mjjStatusOfNmValMapList")]
    public async Task<List<MjjStatusNmValMapDto>> GetStatusInNmValMapFormAsync()
    {
        return await _mjjService.GetStatusInNmValMapFormAsync().ConfigureAwait(false);
    }

    [HttpPost("moveLeft")]
    [MiddlewareFilter(typeof(ApiLogPipeline))]
    public async Task<ResponseDto> MoveLeftAsync(byte colNo)
    {
        return await _mjjService.MoveLeftAsync(colNo).ConfigureAwait(false);
    }

    [HttpPost("moveRight")]
    [MiddlewareFilter(typeof(ApiLogPipeline))]
    public async Task<ResponseDto> MoveRightAsync(byte colNo)
    {
        return await _mjjService.MoveRightAsync(colNo).ConfigureAwait(false);
    }

    [HttpPost("open")]
    [MiddlewareFilter(typeof(ApiLogPipeline))]
    public async Task<ResponseDto> OpenAsync(MjjOpenDto para)
    {
        return await _mjjService.OpenAsync(para).ConfigureAwait(false);
    }

    [HttpPost("reset")]
    [MiddlewareFilter(typeof(ApiLogPipeline))]
    public async Task<ResponseDto> ResetAsync()
    {
        return await _mjjService.ResetAsync().ConfigureAwait(false);
    }
}