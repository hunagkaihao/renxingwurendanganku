using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Wcs.LogTool;
using Microsoft.Extensions.Logging;

namespace Wcs.Station;

public class StationService : WcsAppService, IStationService
{
    private readonly ILogger<StationService> _logger;

    private readonly StationManager _stationManager;

    public StationService(
        ILogger<StationService> logger,
        StationManager stationManager)
    {
        _logger = logger;
        _stationManager = stationManager;
    }
    
    //[RemoteService(false)]
    public async Task<List<StationInfoDto>> GetInformationsAsync(string stationCode)
    {
        try
        {
            List<StationInfo> stationInfos = await _stationManager.GetAllStationInfoAsync(stationCode).ConfigureAwait(false);
            return ObjectMapper.Map<List<StationInfo>, List<StationInfoDto>>(stationInfos);
        }
        catch(Exception e)
        {
            _logger.Error(e.Message);
            return new List<StationInfoDto>();
        }
    }
}