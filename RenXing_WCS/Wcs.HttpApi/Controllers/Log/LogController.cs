using System.Collections.Generic;
using Wcs.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace Wcs.Log;

[Route("wcs/log")]
[ApiController]
public class LogController : WcsController, ILogService
{
    private readonly ILogService _logServece;
    public LogController(ILogService logService)
    {
        _logServece = logService;    
    }

    [HttpGet("query")]
    public List<LogDto> query(string msgSnip, string grade, int logCntMax = 2000)
    {
        return _logServece.query(msgSnip, grade, logCntMax);
    }
}