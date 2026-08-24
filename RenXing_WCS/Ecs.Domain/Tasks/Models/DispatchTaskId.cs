using Volo.Abp.Domain.Entities;

namespace Ecs.Tasks.Models;

public class DispatchTaskId : Entity<int>
{
    public int TaskId { get; set; }
}