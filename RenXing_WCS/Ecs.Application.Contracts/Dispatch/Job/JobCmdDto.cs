using Volo.Abp.Application.Dtos;

namespace Ecs.Dispatch;

public class JobCmdDto : EntityDto<int>
{
    public string JobCmdClassName { get; set; }
    public string Describe { get; set; }
}