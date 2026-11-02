namespace ClientApi.Configuration;

/// <summary>指静脉设备启动时的连接配置；串口参数由 YAML 提供。</summary>
public sealed class VeinConnectionSettings
{
    /// <summary>是否先装载串口设备；false 时沿用 SDK 的设备枚举流程。</summary>
    public bool UseSerialPort { get; init; } = true;
    /// <summary>串口名称，如 COM3；串口连接时必需。</summary>
    public string ComName { get; init; } = string.Empty;
    /// <summary>串口波特率，串口连接时必须为正整数。</summary>
    public int ComRate { get; init; }
    /// <summary>厂商 SDK 协议号；串口连接时必须显式配置，具体取值按设备要求。</summary>
    public int? ComProtocol { get; init; }

    /// <summary>在调用设备 SDK 前验证所选连接方式所需参数。</summary>
    public void Validate()
    {
        if (!UseSerialPort) return;
        if (string.IsNullOrWhiteSpace(ComName) || ComRate <= 0 || ComProtocol == null)
            throw new InvalidOperationException("串口连接需要配置 Vein:ComName、正数 Vein:ComRate 和 Vein:ComProtocol。");
    }
}
