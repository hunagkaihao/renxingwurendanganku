using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Wcs.LogTool;
using Microsoft.Extensions.Logging;

namespace Wcs.Log;

public class LogService : WcsAppService, ILogService
{
    private readonly ILogger<LogService> _logger;

    public LogService(ILogger<LogService> logger)
    {
        _logger = logger;
    }

    public List<LogDto> query(string msgSnip, string grade, int logCntMax = 2000)
    {
        try
        {
            List<SqliteLogItem> logs = SqliteLogHelper.GetLogItems(msgSnip, grade, logCntMax);
            var ret = ObjectMapper.Map<List<SqliteLogItem>, List<LogDto>>(logs);
            return ret;
        }
        catch(Exception ex)
        {
            _logger.Error(ex.Message);
            return new List<LogDto>();
        }
    }
}