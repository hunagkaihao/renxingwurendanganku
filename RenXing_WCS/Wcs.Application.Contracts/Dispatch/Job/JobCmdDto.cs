using Volo.Abp.Application.Dtos;

namespace Wcs.Dispatch;

public class JobCmdDto : EntityDto<int>
{
    public string JobCmdClassName { get; set; }
    public string Describe { get; set; }
}