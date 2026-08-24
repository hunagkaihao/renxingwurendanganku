using Volo.Abp.Application.Dtos;

namespace Ecs.Dispatch;

public class ForceDoneDto : EntityDto
{
    public string orderCode { get; set; } = string.Empty;
}