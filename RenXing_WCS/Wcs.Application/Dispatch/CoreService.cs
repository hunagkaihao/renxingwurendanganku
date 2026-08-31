using System;
using System.Threading.Tasks;
using Wcs.ConfigTool;
using Wcs.LogTool;
using Wcs.Notifiers;
using Wcs.RedisTool;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Wcs.Dispatch;

public class CoreService : WcsAppService, ICoreService
{
    private readonly NotifierManager _notifierManager;
    private readonly IRedisClient _ecsRedisClient;
    private readonly IOptions<ConfigOptions> _options;
    private readonly ILogger<CoreService> _logger;
    public CoreService(
        NotifierManager notifierManager,
        IOptions<ConfigOptions> options, 
        ILogger<CoreService> logger,
        IRedisClient redisClient)
    {
        _notifierManager = notifierManager;
        _options = options;
        _logger = logger;
        _ecsRedisClient = redisClient;
        _ecsRedisClient.Build(_options.Value.RedisConnStr, _options.Value.DefaultRedisNo);
    } 


    public ResponseDto PauseDispatcherSvr()
    {
        try
        {
            _notifierManager.NotifyDispatchSvr(WcsConsts.PauseDispatcherSvrNotifierName);
            return new ResponseDto(){ success = true, message = "" };
        }
        catch(Exception ex)
        {
            _logger.Error(ex.Message);
            return new ResponseDto() { success = false, message = ex.Message }; 
        }
    }

    public ResponseDto RestartDispatcherSvr()
    {
        try
        {
            _notifierManager.NotifyDispatchSvr(WcsConsts.RunDispatcherSvrNotifierName);
            return new ResponseDto(){ success = true, message = "" };
        }
        catch(Exception ex)
        {
            _logger.Error(ex.Message);
            return new ResponseDto() { success = false, message = ex.Message }; 
        }
    }

    public async Task<string> GetDispatchSvrStateAsync()
    {
        try
        {
            string state = await _ecsRedisClient.GetStringValueAsync(WcsConsts.DispatchSvrStateChannel).ConfigureAwait(false);
            if(state != "Running" && state != "Pause")
                state = "Unknown";

            return state;
        }
        catch(Exception ex)
        {
            _logger.Error(ex.Message);
            return "Unknown"; 
        }
    }

}