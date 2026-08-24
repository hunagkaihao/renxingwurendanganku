using Volo.Abp.Application.Dtos;

namespace Ecs.Dispatch;

public class AddProcessDto : EntityDto
{
    public string ProcessTemplateName { get; set; }

    public int ProcessId { get; set; }

    public string StartNodeCode { get; set; }

    public string EndNodeCode { get; set; }
}