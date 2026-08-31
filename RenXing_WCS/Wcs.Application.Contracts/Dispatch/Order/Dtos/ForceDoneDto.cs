using Volo.Abp.Application.Dtos;

namespace Wcs.Dispatch;

public class ForceDoneDto : EntityDto
{
    public string orderCode { get; set; } = string.Empty;
}