namespace ClientApi.Vein.Dto;

/// <summary>指静脉设备执行结果，不包含生物特征数据。</summary>
public sealed class VeinOperationResultDto(bool success, string msg, int? fingerId = null,
    int completedSamples = 0, int? sdkCode = null)
{
    /// <summary>仅模板写入成功或匹配到有效编号时为 true。</summary>
    public bool Success { get; } = success;
    /// <summary>执行结果或失败阶段的说明。</summary>
    public string Msg { get; } = msg;
    /// <summary>成功写入或识别的设备模板编号；失败时为空。</summary>
    public int? FingerId { get; } = fingerId;
    /// <summary>注册时成功采集的样本数，范围 0～6；识别时为 0。</summary>
    public int CompletedSamples { get; } = completedSamples;
    /// <summary>失败时厂商 SDK 原始错误码；等待超时或未就绪时为空。</summary>
    public int? SdkCode { get; } = sdkCode;
}
