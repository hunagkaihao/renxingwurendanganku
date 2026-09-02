using Volo.Abp.Application.Dtos;
using Wcs.WMS;

namespace Wcs.Dispatch;

public class CheckOrderResultDto : EntityDto
{
    public string orderCode { get; set; } = string.Empty;
    public string cellCode { get; set; } = string.Empty;

    /// <summary>
    /// WCS 现场采集状态。WMS 不得仅凭 PlateCode 是否为空判断扫描是否完成。
    /// </summary>
    public WcsCheckCellStatus status { get; set; } = WcsCheckCellStatus.Unknown;

    /// <summary>
    /// PLC/扫码器现场读取的实际档案盒条码；该值不是 WMS 账面绑定值。
    /// </summary>
    public string plateCode { get; set; } = string.Empty;
}
