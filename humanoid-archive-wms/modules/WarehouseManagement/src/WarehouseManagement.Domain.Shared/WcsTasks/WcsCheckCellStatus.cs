namespace WarehouseManagement.WcsTasks;

/// <summary>
/// WCS 现场盘点采集状态。
/// 该状态是现场事实，WMS 必须结合冻结的账面快照生成最终盘点结论。
/// </summary>
public enum WcsCheckCellStatus
{
    /// <summary>未知状态。</summary>
    Unknown = 0,

    /// <summary>等待 PLC/扫码器扫描。</summary>
    Waiting = 10,

    /// <summary>PLC/扫码器正在扫描。</summary>
    Scanning = 20,

    /// <summary>二维码读取成功。</summary>
    Scanned = 30,

    /// <summary>现场库位为空。</summary>
    Empty = 40,

    /// <summary>二维码扫描失败。</summary>
    ScanError = 50,

    /// <summary>设备或定位异常。</summary>
    DeviceError = 60
}
