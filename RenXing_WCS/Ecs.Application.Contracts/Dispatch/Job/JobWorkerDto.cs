using Volo.Abp.Application.Dtos;

namespace Ecs.Dispatch;

public class JobWorkerDto : EntityDto<int>
{
    public string JobWorkerClassName { get; set; }
    public string Describe { get; set; }
}