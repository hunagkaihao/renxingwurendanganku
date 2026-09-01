using Wcs.WMS;

namespace Wcs.Dispatch;

public class OrderStateDto
{
    public string orderCode { get; set; } = string.Empty;
    /// <summary>
    /// 任务生命周期状态，供 WMS 做业务判断。
    /// </summary>
    public WcsTaskStatus status { get; set; } = WcsTaskStatus.Unknown;

    /// <summary>
    /// 当前中文执行工步，仅用于展示和诊断。
    /// </summary>
    public string execState { get; set; } = string.Empty;
    public string errorInfo { get; set; } = string.Empty;
    public string happenTime { get; set; } = string.Empty;
}
