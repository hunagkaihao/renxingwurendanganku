using System.Collections.Generic;
using System.Threading.Tasks;
using Wcs.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace Wcs.PlcMonitor;

[Route("wcs/plc")]
[ApiController]
public class PlcMonitorController : WcsController, IPlcMonitorService
{
    private readonly IPlcMonitorService _plcMonitorService;

    public PlcMonitorController(
        IPlcMonitorService plcMonitorService)
    {
        _plcMonitorService = plcMonitorService;
    }

    [HttpGet("plcMonitor")]
    public async Task<List<MonitorDto>> GetMonitorsAsync()
    {
        return await _plcMonitorService.GetMonitorsAsync().ConfigureAwait(false);
    }
}