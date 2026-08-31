using Volo.Abp.Domain.Entities;

namespace Wcs.Jobs.Models;

public class DispatchJobId : Entity<int>
{
    public int JobId { get; set; }
}