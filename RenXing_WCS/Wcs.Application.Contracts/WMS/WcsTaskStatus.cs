using System.Text.Json.Serialization;

namespace Wcs.WMS;

/// <summary>
/// WCS 与 WMS 之间传输的任务生命周期状态。
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum WcsTaskStatus
{
    /// <summary>未知或无法识别的状态。</summary>
    Unknown = 0,

    /// <summary>WCS 已受理任务，任务可能正在排队或等待资源。</summary>
    Accepted = 10,

    /// <summary>WCS 已获得执行资源并开始执行任务。</summary>
    Executing = 20,

    /// <summary>WCS 已正常完成任务。</summary>
    Completed = 30,

    /// <summary>WCS 已取消任务。</summary>
    Canceled = 40,

    /// <summary>WCS 已强制结束任务。</summary>
    ForceCompleted = 50
}
