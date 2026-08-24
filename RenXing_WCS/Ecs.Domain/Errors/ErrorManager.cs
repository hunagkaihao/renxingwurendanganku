using System;
using Ecs.ConfigTool;
using Ecs.RedisTool;
using Ecs.LogTool;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using System.Collections.Generic;
using Volo.Abp.DependencyInjection;

namespace Ecs.Errors;

public class ErrorManager : ISingletonDependency
{
    private readonly ILogger<ErrorManager> _logger;
    private readonly IOptions<ConfigOptions> _options;
    private readonly IRedisClient _ecsRedisClient;

    public ErrorManager(
        ILogger<ErrorManager> logger,
        IOptions<ConfigOptions> options,
        IRedisClient redisClient)
    {
        _logger = logger;
        _options = options;
        _ecsRedisClient = redisClient;
        _ecsRedisClient.Build(_options.Value.RedisConnStr, _options.Value.DefaultRedisNo);
    }


    public void UpdateErrInfoOfDispatchSvr(string errMark, string errInfo)
    {
        try
        {
            errInfo = $"{errInfo}, {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}";
            _ecsRedisClient.SetHashValue(EcsConsts.DispatchSvrErrChannel, errMark, errInfo);
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
        }
    }

    public async Task UpdateErrInfoOfDispatchSvrAsync(string errMark, string errInfo)
    {
        try
        {
            errInfo = $"{errInfo}, {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}";
            await _ecsRedisClient.SetHashValueAsync(EcsConsts.DispatchSvrErrChannel, errMark, errInfo);
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
        }
    }

    public void RemoveErrInfoOfDispatchSvr(string infoMark)
    {
        try
        {
            _ecsRedisClient.RemoveHashFields(EcsConsts.DispatchSvrErrChannel, new string[] { infoMark });
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
        }
    }

    public async Task RemoveErrInfoOfDispatchSvrAsync(string infoMark)
    {
        try
        {
            await _ecsRedisClient.RemoveHashFieldsAsync(EcsConsts.DispatchSvrErrChannel, new string[] { infoMark });
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
        }
    }

    public void RemoveAllErrInfoOfDispatchSvr()
    {
        try
        {
            _ecsRedisClient.RemoveKey(EcsConsts.DispatchSvrErrChannel);
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
        }
    }

    public async Task RemoveAllErrInfoOfDispatchSvrAsync()
    {
        try
        {
            await _ecsRedisClient.RemoveKeyAsync(EcsConsts.DispatchSvrErrChannel).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
        }
    }

    public List<DispatchSvrErr> GetAllErrInfoOfDispatchSvr()
    {
        try
        {
            KeyValuePair<string, string>[] pairs = _ecsRedisClient.GetAllHashFieldValuePairs(EcsConsts.DispatchSvrErrChannel);
            if (pairs.Length == 0)
                return new List<DispatchSvrErr>();

            List<DispatchSvrErr> errs = new List<DispatchSvrErr>();
            foreach (var pair in pairs)
            {
                errs.Add(new DispatchSvrErr() { ErrorMark = pair.Key ?? "", ErrorInfo = pair.Value ?? "" });
            }
            return errs;
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            return new List<DispatchSvrErr>();
        }
    }

    public async Task<List<DispatchSvrErr>> GetAllErrInfoOfDispatchSvrAsync()
    {
        try
        {
            KeyValuePair<string, string>[] pairs = await _ecsRedisClient.GetAllHashFieldValuePairsAsync(EcsConsts.DispatchSvrErrChannel);
            if (pairs.Length == 0)
                return new List<DispatchSvrErr>();

            List<DispatchSvrErr> errs = new List<DispatchSvrErr>();
            foreach (var pair in pairs)
            {
                errs.Add(new DispatchSvrErr() { ErrorMark = pair.Key ?? "", ErrorInfo = pair.Value ?? "" });
            }
            return errs;
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            return new List<DispatchSvrErr>();
        }
    }
}