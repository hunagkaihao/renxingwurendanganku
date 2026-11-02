namespace ClientApi.Vein.Dto;

/// <summary>一次指静脉六次采集的进度快照，不包含特征模板或图片。</summary>
public sealed class VeinRegistrationProgressDto
{
    /// <summary>本次操作随机标识，与注册请求一一对应。</summary>
    public Guid OperationId { get; init; }
    /// <summary>注册请求所属的业务用户编号，供 WMS 校验查询范围。</summary>
    public string UserId { get; init; } = "";
    /// <summary>采集阶段：preparing、place、capturing、remove、writing、completed 或 failed。</summary>
    public string Stage { get; init; } = "preparing";
    /// <summary>当前应当展示给操作者的提示。</summary>
    public string Message { get; init; } = "正在准备采集，请使用同一根手指完成六次录入。";
    /// <summary>已经成功采集的特征次数，范围 0～6。</summary>
    public int CompletedSamples { get; init; }
    /// <summary>本次需要采集的总次数，固定为 6。</summary>
    public int TotalSamples { get; init; } = 6;
    /// <summary>本地设备录入是否已经结束；不代表 WMS 账号绑定已完成。</summary>
    public bool IsComplete { get; init; }
    /// <summary>设备录入成功为 true，失败为 false，采集中为空。</summary>
    public bool? Success { get; init; }
    /// <summary>采集失败时的厂商 SDK 错误码，没有错误码时为空。</summary>
    public int? SdkCode { get; init; }
    /// <summary>最后更新时间，用于清理过期操作，使用 UTC。</summary>
    public DateTimeOffset UpdatedAt { get; init; } = DateTimeOffset.UtcNow;
}
