using Volo.Abp.Application.Dtos;

namespace Wcs.Dispatch;

public class CheckOrderResultDto : EntityDto
{
    public string orderCode { get; set; } = string.Empty;
    public string cellCode { get; set; } = string.Empty;
    public string plateCode { get; set; } = string.Empty;
}