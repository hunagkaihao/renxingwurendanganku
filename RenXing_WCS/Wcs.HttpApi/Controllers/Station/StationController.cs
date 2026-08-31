using System.Collections.Generic;
using System.Threading.Tasks;
using Wcs.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace Wcs.Station;

[Route("wcs/station")]
[ApiController]
public class StationController : WcsController, IStationService
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