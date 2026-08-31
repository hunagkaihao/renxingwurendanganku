using Wcs.ConfigTool;
using Wcs.RedisTool;
using Wcs.LogTool;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Volo.Abp.Domain.Services;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Wcs.Station;

public class StationManager : IDomainService
{
    private readonly ILogger<StationManager> _logger;
    private readonly IRedisClient _redisClient;
    private readonly IOptions<ConfigOptions> _options;


    public StationManager(
        ILogger<StationManager> logger, 
        IRedisClient redisClient,
        IOptions<ConfigOptions> options)
    {
        _logger = logger;
        _options = options;
        _redisClient = redisClient;
        _redisClient.Build(options.Value.RedisConnStr, options.Value.DefaultRedisNo);

        try
        {   
            //从Redis删除所有站点通知器数据
            string[] notifierKeys = _redisClient.GetHashFields(WcsConsts.StationNotifierChannelName);
            string[] notifierTempKeys = _redisClient.GetHashFields(WcsConsts.StationNotifierTmpChannelName);
            _redisClient.RemoveHashFields(WcsConsts.StationNotifierChannelName, notifierKeys);
            _redisClient.RemoveHashFields(WcsConsts.StationNotifierTmpChannelName, notifierTempKeys);
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
        }
    }

    /// <summary>
    /// 通知站点
    /// </summary>
    /// <param name="notifierName"></param>
    public void NotifyStation(string notifierName)
    {
        try
        {
            string notifierVal = _redisClient.GetHashValue(WcsConsts.StationNotifierChannelName, notifierName);
            if (notifierVal == null)
                _redisClient.SetHashValue(WcsConsts.StationNotifierChannelName, notifierName, "1");
            else
            {
                if (!int.TryParse(notifierVal, out int val))
                    _redisClient.SetHashValue(WcsConsts.StationNotifierChannelName, notifierName, "1");
                else
                {
                    val++;
                    if (val == int.MaxValue)
                        val = 1;
                    _redisClient.SetHashValue(WcsConsts.StationNotifierChannelName, notifierName, val.ToString());
                }
            }
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
        }
    }

    /// <summary>
    /// 判断站点通知器是否收到通知
    /// </summary>
    /// <param name="notifierName"></param>
    /// <returns>true：收到通知，false：未收到通知，null：发生错误</returns>
    public bool? IsNotifierValChanged(string notifierName)
    {
        try
        {
            string notifierVal = _redisClient.GetHashValue(WcsConsts.StationNotifierChannelName, notifierName);
            string notifierTempVal = _redisClient.GetHashValue(WcsConsts.StationNotifierTmpChannelName, notifierName);
            if (notifierVal != notifierTempVal)
            {
                _redisClient.SetHashValue(WcsConsts.StationNotifierTmpChannelName, notifierName, notifierVal);
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

    /// <summary>
    /// 更新站点信息
    /// </summary>
    /// <param name="stationCode"></param>
    /// <param name="infoMark"></param>
    /// <param name="info"></param>
    public void UpdateStationInfo(string stationCode, string infoMark, string info)
    {
        try
        {
            _redisClient.SetHashValue($"{stationCode}.RealTime", infoMark, info);
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
        }
    }

    /// <summary>
    /// 异步更新站点信息
    /// </summary>
    /// <param name="stationCode"></param>
    /// <param name="infoMark"></param>
    /// <param name="info"></param>
    /// <returns></returns>
    public async Task UpdateStationInfoAsync(string stationCode, string infoMark, string info)
    {
        try
        {
            await _redisClient.SetHashValueAsync($"{stationCode}.RealTime", infoMark, info);
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
        }
    }

    /// <summary>
    /// 获取站点信息
    /// </summary>
    /// <param name="stationCode"></param>
    /// <param name="infoMark"></param>
    /// <returns>若发生错误返回null</returns>
    public StationInfo GetStationInfo(string stationCode, string infoMark)
    {
        try
        {
            string info = _redisClient.GetHashValue($"{stationCode}.RealTime", infoMark);
            StationInfo staInfo = new StationInfo(){
                StaInfoName = $"{stationCode}.{infoMark}",
                StaInformation = info
            };
            return staInfo;
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            return null;
        }
    }

    /// <summary>
    /// 异步获取站点信息
    /// </summary>
    /// <param name="stationCode"></param>
    /// <param name="infoMark"></param>
    /// <returns>若发生错误返回null</returns>
    public async Task<StationInfo> GetStationInfoAsync(string stationCode, string infoMark)
    {
        try
        {
            string info = await _redisClient.GetHashValueAsync($"{stationCode}.RealTime", infoMark);
            StationInfo staInfo = new StationInfo(){
                StaInfoName = $"{stationCode}.{infoMark}",
                StaInformation = info
            };
            return staInfo;
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            return null;
        }
    }

    public List<StationInfo> GetAllStationInfo(string stationCode)
    {
        try
        {
            KeyValuePair<string, string>[] pairs = _redisClient.GetAllHashFieldValuePairs($"{stationCode}.RealTime");
            if (pairs.Length == 0)
                return new List<StationInfo>();

            List<StationInfo> staInfos = new List<StationInfo>();
            foreach(var pair in pairs)
            {
                StationInfo staInfo = new StationInfo()
                {
                    StaInfoName = $"{stationCode}.{pair.Key}",
                    StaInformation = pair.Value
                };
                staInfos.Add(staInfo);
            }
            return staInfos;
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            return null;
        }
    }

    public async Task<List<StationInfo>> GetAllStationInfoAsync(string stationCode)
    {
        try
        {
            KeyValuePair<string, string>[] pairs = await _redisClient.GetAllHashFieldValuePairsAsync($"{stationCode}.RealTime");
            if (pairs.Length == 0)
                return new List<StationInfo>();

            List<StationInfo> staInfos = new List<StationInfo>();
            foreach(var pair in pairs)
            {
                StationInfo staInfo = new StationInfo()
                {
                    StaInfoName = $"{stationCode}.{pair.Key}",
                    StaInformation = pair.Value
                };
                staInfos.Add(staInfo);
            }
            return staInfos;
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            return null;
        }
    }
}
