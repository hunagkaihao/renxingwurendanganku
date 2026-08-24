using System;
using System.Threading.Tasks;
using Ecs.ConfigTool;
using Ecs.LogTool;
using Ecs.Notifiers;
using Ecs.RedisTool;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Ecs.Dispatch;

public class CoreService : EcsAppService, ICoreService
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
            _notifierManager.NotifyDispatchSvr(EcsConsts.PauseDispatcherSvrNotifierName);
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
            _notifierManager.NotifyDispatchSvr(EcsConsts.RunDispatcherSvrNotifierName);
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
            string state = await _ecsRedisClient.GetStringValueAsync(EcsConsts.DispatchSvrStateChannel).ConfigureAwait(false);
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