using Volo.Abp.Application.Dtos;

namespace Wcs.Dispatch;

public class CancelOrderDto : EntityDto
{
    public string orderCode { get; set; } = string.Empty;
}