using System.Collections.Generic;
using System.Threading.Tasks;
using Ecs.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace Ecs.Station;

[Route("ecs/station")]
[ApiController]
public class StationController : EcsController, IStationService
{
    private readonly IStationService _stationService;

    public StationController(IStationService stationService)
    {
        _stationService = stationService;
    }

    [HttpGet("stationInfo")]
    public async Task<List<StationInfoDto>> GetInformationsAsync(string stationCode)
    {
        return await _stationService.GetInformationsAsync(stationCode).ConfigureAwait(false);
    }
}