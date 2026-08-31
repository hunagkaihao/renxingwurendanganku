using System;
using Wcs.PlcTool;
using Wcs.RedisTool;
using Wcs.LogTool;
using Wcs.ConfigTool;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using Wcs.Conditions.Models;
using Volo.Abp.Domain.Repositories;
using System.Linq;

namespace Wcs.Conditions
{
    /// <summary>
    /// 调度任务执行条件监控任务
    /// </summary>
    public class ConditionMonitorJob : IHostedService, IDisposable
    {
        private readonly PlcHelper _plcHelper;
        private readonly ConditionManager _conditionManager;
        private readonly IRepository<DispatchCondition, int> _conditionRepository;
        private readonly ILogger<ConditionMonitorJob> _logger;
        private readonly IOptions<ConfigOptions> _options;
        private readonly IRedisClient _ecsRedisClient;

        private readonly string mConditionChannel = WcsConsts.DispatchConditionChannel;

        public ConditionMonitorJob(
            PlcHelper plcHelper,
            ConditionManager conditionManager,
            IRepository<DispatchCondition, int> conditionRepository,
            ILogger<ConditionMonitorJob> logger,
            IOptions<ConfigOptions> options,
            IRedisClient redisClient)
        {
            _plcHelper = plcHelper;
            _conditionManager = conditionManager;
            _conditionRepository = conditionRepository;
            _logger = logger;
            _options = options;
            _ecsRedisClient = redisClient;
            _ecsRedisClient.Build(_options.Value.RedisConnStr, _options.Value.DefaultRedisNo);
        }

        public void Dispose()
        {

        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Task.Run(async () =>
            {
                await Task.Delay(3000).ConfigureAwait(false);
                await ConditionCycleUpdate().ConfigureAwait(false); ;
            });
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private class CondTag
        {
            public string TagOwner { get; set; } = string.Empty;
            public string TagName { get; set; } = string.Empty;
        }

        private async Task ConditionCycleUpdate()
        {
            List<CondTag> plcTags = new List<CondTag>(); //来自PLC前提条件
            List<CondTag> otherTags = new List<CondTag>(); //来自除PLC外的其它设备的前提条件

            try
            {
                await _ecsRedisClient.RemoveKeyAsync(mConditionChannel).ConfigureAwait(false); //删除后重新初始化

                List<DispatchCondition> items = await _conditionRepository.GetListAsync().ConfigureAwait(false);
                if (items == null || items.Count == 0)
                    return;

                items = items.OrderBy(o => o.Id).ToList();
                foreach (DispatchCondition item in items)  //搜索属于PLC变量的前提，完成变量订阅
                {
                    string _conditionName = item.ConditionName;
                    string _conditionSrc = item.ConditionSrc;
                    if (_conditionSrc == "PLC")
                    {
                        string[] sections = _conditionName.Split(".");
                        if (sections.Length != 2)
                        {
                            _logger.Error($"名为{_conditionName}的前提PLC变量不存在，PLC变量名应为 plcName.tagName");
                            continue;
                        }

                        if (true != _plcHelper.IsPlcTagExist(sections[0], sections[1]))
                        {
                            _logger.Error($"名为{_conditionName}的前提PLC变量不存在");
                            continue;
                        }

                        CondTag plcTag = new CondTag()
                        {
                            TagOwner = sections[0],
                            TagName = sections[1]
                        };
                        plcTags.Add(plcTag);
                    }
                    else
                    {
                        CondTag otherTag = new CondTag()
                        {
                            TagOwner = _conditionSrc,
                            TagName = _conditionName
                        };
                        otherTags.Add(otherTag);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
            }

            await Task.Run(async () =>
            {
                while (true)
                {
                    await Task.Delay(500).ConfigureAwait(false);

                    try
                    {
                        int plcTagCount = plcTags.Count;
                        if (plcTagCount > 0)
                        {
                            Task<PlcTagValue>[] plcValTasks = new Task<PlcTagValue>[plcTagCount];
                            for (int i = 0; i < plcTagCount; i++)
                            {
                                plcValTasks[i] = _plcHelper.ReadPlcTagAsync(plcTags[i].TagOwner, plcTags[i].TagName);
                            }
                            Task.WaitAll(plcValTasks);

                            Task[] tasks = new Task[plcTagCount];
                            for (int i = 0; i < plcTagCount; i++)
                            {
                                PlcTagValue tagVal = plcValTasks[i].Result;
                                if (tagVal == null || tagVal.Quality == EnumQuality.Bad)
                                    tasks[i] = _conditionManager.UpdateConditionAsync($"{plcTags[i].TagOwner}.{plcTags[i].TagName}", "error");
                                else
                                    tasks[i] = _conditionManager.UpdateConditionAsync($"{plcTags[i].TagOwner}.{plcTags[i].TagName}", tagVal.Value);
                            }
                            Task.WaitAll(tasks);
                        }

                        int otherTagCount = otherTags.Count;
                        if (otherTagCount > 0)
                        {
                            Task<string>[] valTasks = new Task<string>[otherTagCount];
                            for (int i = 0; i < otherTagCount; i++)
                            {
                                valTasks[i] = _conditionManager.ReadCondValFromDeviceCacheAsync(otherTags[i].TagOwner, otherTags[i].TagName);
                            }
                            Task.WaitAll(valTasks);

                            Task[] tasks = new Task[otherTagCount];
                            for (int i = 0; i < otherTagCount; i++)
                            {
                                tasks[i] = _conditionManager.UpdateConditionAsync(otherTags[i].TagName, valTasks[i].Result);
                            }
                            Task.WaitAll(tasks);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex.Message);
                    }
                }
            });
        }
    }
}