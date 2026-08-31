using Volo.Abp.Application.Dtos;

namespace Wcs.Dispatch;

public class JobWorkerDto : EntityDto<int>
{
    public string JobWorkerClassName { get; set; }
    public string Describe { get; set; }
}