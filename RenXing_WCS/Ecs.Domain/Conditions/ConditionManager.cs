using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Volo.Abp.Domain.Repositories;
using Ecs.LogTool;
using Ecs.RedisTool;
using System;
using System.Linq;
using Microsoft.Extensions.Options;
using Ecs.ConfigTool;
using Volo.Abp.DependencyInjection;
using Ecs.Conditions.Models;
using StackExchange.Redis;

namespace Ecs.Conditions;

public class ConditionManager : ISingletonDependency
{
    private readonly IRepository<DispatchCondition, int> _conditionRepository;
    private readonly ILogger<ConditionManager> _logger;
    private readonly IOptions<ConfigOptions> _options;
    private readonly IRedisClient _ecsRedisClient;

    public ConditionManager(
        IRepository<DispatchCondition, int> conditionRepository,
        IRedisClient redisClient,
        ILogger<ConditionManager> logger,
        IOptions<ConfigOptions> options)
    {
        _conditionRepository = conditionRepository;
        _logger = logger;
        _options = options;
        _ecsRedisClient = redisClient;
        _ecsRedisClient.Build(_options.Value.RedisConnStr, _options.Value.DefaultRedisNo);
    }

    public async Task<DispatchCondition> CreateDispatchCondition(string conditionName, string conditionSrc, string describe)
    {
        var conditions = await _conditionRepository.GetListAsync(o => o.ConditionName == conditionName).ConfigureAwait(false);
        if (conditions.Count > 0)
            throw new Exception($"名为{conditionName}的调度条件已经存在");

        return new DispatchCondition(conditionName, conditionSrc, describe);
    }

    public async Task<string> ReadCondValFromDeviceCacheAsync(string channelName, string fieldName)
    {
        try
        {
            string ret = await _ecsRedisClient.GetHashValueAsync(channelName, fieldName).ConfigureAwait(false);
            return ret;
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            return null;
        }
    }

    public async Task UpdateConditionAsync(string conditionName, string conditionValue)
    {
        try
        {
            await _ecsRedisClient.SetHashValueAsync(EcsConsts.DispatchConditionChannel, conditionName, conditionValue).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
        }
    }

    public async Task<string> GetConditionValueAsync(string conditionName)
    {
        try
        {
            return await _ecsRedisClient.GetHashValueAsync(EcsConsts.DispatchConditionChannel, conditionName).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            return null;
        }
    }


}