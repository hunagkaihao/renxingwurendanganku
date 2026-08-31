using Wcs.ConfigTool;
using Wcs.RedisTool;
using Wcs.LogTool;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using Volo.Abp.DependencyInjection;
using Wcs.Errors;

namespace Wcs.Notifiers;

public class NotifierManager : ISingletonDependency
{
    private readonly ILogger<ErrorManager> _logger;
    private readonly IOptions<ConfigOptions> _options;
    private readonly IRedisClient _ecsRedisClient;

    public NotifierManager(
        ILogger<ErrorManager> logger,
        IOptions<ConfigOptions> options,
        IRedisClient redisClient)
    {
        _logger = logger;
        _options = options;
        _ecsRedisClient = redisClient;
        _ecsRedisClient.Build(_options.Value.RedisConnStr, _options.Value.DefaultRedisNo);
    }

    public void NotifyDispatchSvr(string notifierName)
    {
        try
        {
            string notifierVal = _ecsRedisClient.GetHashValue(WcsConsts.DispatchSvrNotifyChannel, notifierName);
            if (notifierVal == null)
                _ecsRedisClient.SetHashValue(WcsConsts.DispatchSvrNotifyChannel, notifierName, "1");
            else
            {
                if (!int.TryParse(notifierVal, out int val))
                    _ecsRedisClient.SetHashValue(WcsConsts.DispatchSvrNotifyChannel, notifierName, "1");
                else
                {
                    val++;
                    if (val == int.MaxValue)
                        val = 1;
                    _ecsRedisClient.SetHashValue(WcsConsts.DispatchSvrNotifyChannel, notifierName, val.ToString());
                }
            }
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
        }
    }

    public bool? IsNotifierValChanged(string notifierName)
    {
        try
        {
            string notifierVal = _ecsRedisClient.GetHashValue(WcsConsts.DispatchSvrNotifyChannel, notifierName);
            string notifierTempVal = _ecsRedisClient.GetHashValue(WcsConsts.DispatchSvrNotifyTempChannel, notifierName);
            if (notifierVal != notifierTempVal)
            {
                _ecsRedisClient.SetHashValue(WcsConsts.DispatchSvrNotifyTempChannel, notifierName, notifierVal);
                return true;
            }
            return false;
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            return null;
        }
    }

    public void NotifyDispatchSvrWithPara(string notifierName, string para)
    {
        try
        {
            string notifierVal = _ecsRedisClient.GetHashValue(WcsConsts.DispatchSvrNotifyWithParaChannel, notifierName);
            if (notifierVal == null)
                _ecsRedisClient.SetHashValue(WcsConsts.DispatchSvrNotifyWithParaChannel, notifierName, $"1@#${para}");
            else
            {
                string[] sections = notifierVal.Split("@#$");

                if (sections.Length != 2)
                    _ecsRedisClient.SetHashValue(WcsConsts.DispatchSvrNotifyWithParaChannel, notifierName, $"1@#${para}");
                else if (!int.TryParse(sections[0], out int val))
                    _ecsRedisClient.SetHashValue(WcsConsts.DispatchSvrNotifyWithParaChannel, notifierName, $"1@#${para}");
                else
                {
                    val++;
                    if (val == int.MaxValue)
                        val = 1;
                    _ecsRedisClient.SetHashValue(WcsConsts.DispatchSvrNotifyWithParaChannel, notifierName, $"{val}@#${para}");
                }
            }
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
        }
    }

    public bool? IsNotifierValWithParaChanged(string notifierName, out string para)
    {
        try
        {
            para = string.Empty;

            string notifierVal = _ecsRedisClient.GetHashValue(WcsConsts.DispatchSvrNotifyWithParaChannel, notifierName);
            string notifierTempVal = _ecsRedisClient.GetHashValue(WcsConsts.DispatchSvrNotifyTempWithParaChannel, notifierName);

            if (notifierVal == null)
                return false;

            string[] sections = notifierVal.Split("@#$");
            if (sections.Length != 2)
                return false;

            int val = -1;
            if (!int.TryParse(sections[0], out val))
                return false;

            if (val.ToString() != notifierTempVal)
            {
                para = sections[1];
                _ecsRedisClient.SetHashValue(WcsConsts.DispatchSvrNotifyTempWithParaChannel, notifierName, val.ToString());
                return true;
            }
            return false;
        }
        catch (Exception ex)
        {
            para = string.Empty;
            _logger.Error(ex.Message);
            return null;
        }
    }
}