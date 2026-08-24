using Volo.Abp.Domain.Entities;

namespace Ecs.Jobs.Models;

public class DispatchJobId : Entity<int>
{
    public int JobId { get; set; }
}