using System;
using System.Threading;
using System.Threading.Tasks;
using Wcs.Mjj;
using Wcs.LogTool;
using Wcs.ConfigTool;
using Wcs.RedisTool;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Wcs.SignalRTool;
using System.Collections.Generic;

namespace Wcs.Mjj
{
    /// <summary>
    /// 后台PLC变量监控以及心跳监控
    /// </summary>
    public class MjjMonitorJob : IHostedService, IDisposable
    {
        private readonly MjjManager _mjjManager;
        private readonly ILogger<MjjMonitorJob> _logger;
        private readonly IRedisClient _redisClient;
        private readonly IOptions<ConfigOptions> _options;
        private readonly HubMsgQHelper _hubHelper;

        private Timer mTimer;
        private int mDelayTime = 200;
        private MjjStatus mMjjStatusTemp;

        public MjjMonitorJob(
            MjjManager mjjManager, 
            ILogger<MjjMonitorJob> logger, 
            IRedisClient redisClient,
            IOptions<ConfigOptions> options,
            HubMsgQHelper hubHelper)
        {
            _mjjManager = mjjManager;
            _logger = logger;
            _options = options;
            _hubHelper = hubHelper;
            _redisClient = redisClient;
            _redisClient.Build(_options.Value.RedisConnStr, _options.Value.DefaultRedisNo);

            mMjjStatusTemp = new MjjStatus();
            mTimer = new Timer(DoWork, null, Timeout.Infinite, Timeout.Infinite);
        }

        public void Dispose()
        {
            mTimer.Dispose();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            mTimer.Change(3000, Timeout.Infinite);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            mTimer.Change(Timeout.Infinite, Timeout.Infinite);
            return Task.CompletedTask;
        }

        private void DoWork(object obj)
        {
            mTimer.Change(Timeout.Infinite, Timeout.Infinite);

            //Do something
            try
            {
                MjjStatus status = _mjjManager.GetMjjStatusAync().Result;
                if(status == null)
                {
                    mMjjStatusTemp = new MjjStatus();
                }
                else
                {
                    bool changed = !status.Equals(mMjjStatusTemp);
                    if (changed)
                    {
                        mMjjStatusTemp = status;
                        List<MjjStatusNmValMap> maps = new List<MjjStatusNmValMap>();
                        foreach(var pro in status.GetType().GetProperties())
                        {
                            maps.Add(new MjjStatusNmValMap(){
                                tagName = pro.Name,
                                tagValue = pro.GetValue(status).ToString()
                            });
                        }
                        _hubHelper.SendMessage(_options.Value.HubCliMethod_UpdateMjjStatus, maps);
                    }
                }
                
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
            }

            mTimer.Change(mDelayTime, Timeout.Infinite);
        }
    }
}