using Volo.Abp.Application.Dtos;

namespace Ecs.Dispatch;

public class CancelOrderDto : EntityDto
{
    public string orderCode { get; set; } = string.Empty;
}