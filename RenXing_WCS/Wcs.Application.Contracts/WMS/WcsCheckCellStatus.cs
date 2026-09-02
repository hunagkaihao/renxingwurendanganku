using System.Text.Json.Serialization;

namespace Wcs.WMS;

/// <summary>
/// WCS 现场盘点采集状态。
/// 该枚举只描述 PLC/扫码器采集事实，不代表 WMS 的盘盈、盘亏或错位结论。
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum WcsCheckCellStatus
{
    /// <summary>未知状态。</summary>
    Unknown = 0,

    /// <summary>库位已进入盘点范围，但 PLC 尚未完成扫描。</summary>
    Waiting = 10,

    /// <summary>PLC/扫码器正在扫描当前库位。</summary>
    Scanning = 20,

    /// <summary>二维码读取成功，PlateCode 为现场实际条码。</summary>
    Scanned = 30,

    /// <summary>现场确认库位为空。</summary>
    Empty = 40,

    /// <summary>二维码存在但无法识别，或扫码过程失败。</summary>
    ScanError = 50,

    /// <summary>机械定位、通讯或其他设备执行异常。</summary>
    DeviceError = 60
}
