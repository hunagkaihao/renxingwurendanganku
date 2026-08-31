using System;
using Wcs.ConfigTool;
using Wcs.RedisTool;
using Wcs.LogTool;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using System.Collections.Generic;
using Volo.Abp.DependencyInjection;

namespace Wcs.Errors;

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
            _ecsRedisClient.SetHashValue(WcsConsts.DispatchSvrErrChannel, errMark, errInfo);
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
            await _ecsRedisClient.SetHashValueAsync(WcsConsts.DispatchSvrErrChannel, errMark, errInfo);
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
            _ecsRedisClient.RemoveHashFields(WcsConsts.DispatchSvrErrChannel, new string[] { infoMark });
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
            await _ecsRedisClient.RemoveHashFieldsAsync(WcsConsts.DispatchSvrErrChannel, new string[] { infoMark });
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
            _ecsRedisClient.RemoveKey(WcsConsts.DispatchSvrErrChannel);
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
            await _ecsRedisClient.RemoveKeyAsync(WcsConsts.DispatchSvrErrChannel).ConfigureAwait(false);
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
            KeyValuePair<string, string>[] pairs = _ecsRedisClient.GetAllHashFieldValuePairs(WcsConsts.DispatchSvrErrChannel);
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
            KeyValuePair<string, string>[] pairs = await _ecsRedisClient.GetAllHashFieldValuePairsAsync(WcsConsts.DispatchSvrErrChannel);
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