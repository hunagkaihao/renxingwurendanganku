using System.Collections.Concurrent;
using ClientApi.Vein.Dto;

namespace ClientApi.Vein;

/// <summary>在当前设备进程内保存短期录入进度，读取不访问 SDK，不阻塞正在采集的请求。</summary>
internal static class VeinRegistrationProgressStore
{
    /// <summary>按随机操作标识隔离的不可变快照，避免读取到字段更新一半的状态。</summary>
    private static readonly ConcurrentDictionary<Guid, VeinRegistrationProgressDto> Entries = new();
    /// <summary>操作快照最多保留十五分钟，不作为持久化的录入结果。</summary>
    private static readonly TimeSpan Retention = TimeSpan.FromMinutes(15);

    /// <summary>创建本次进度并清理旧记录；重复操作标识被拒绝，不覆盖正在进行的采集。</summary>
    public static bool TryStart(Guid operationId, string userId)
    {
        foreach (var item in Entries)
            if (DateTimeOffset.UtcNow - item.Value.UpdatedAt > Retention) Entries.TryRemove(item.Key, out _);
        return Entries.TryAdd(operationId, new VeinRegistrationProgressDto { OperationId = operationId, UserId = userId });
    }

    /// <summary>获取未过期的当前快照；不存在或已过期时返回空。</summary>
    public static VeinRegistrationProgressDto? Get(Guid operationId)
    {
        if (!Entries.TryGetValue(operationId, out var progress)) return null;
        if (DateTimeOffset.UtcNow - progress.UpdatedAt <= Retention) return progress;
        Entries.TryRemove(operationId, out _);
        return null;
    }

    /// <summary>发布实际采集阶段，旧接口未提供操作标识时保持原行为。</summary>
    public static void Update(Guid? operationId, string stage, string message, int completedSamples,
        bool isComplete = false, bool? success = null, int? sdkCode = null)
    {
        if (!operationId.HasValue || !Entries.TryGetValue(operationId.Value, out var previous)) return;
        Entries[operationId.Value] = new VeinRegistrationProgressDto
        {
            OperationId = previous.OperationId, UserId = previous.UserId,
            Stage = stage, Message = message, CompletedSamples = completedSamples,
            IsComplete = isComplete, Success = success, SdkCode = sdkCode
        };
    }
}
