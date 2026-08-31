using Volo.Abp.Domain.Entities;

namespace Wcs.Tasks.Models;

public class DispatchTaskId : Entity<int>
{
    public int TaskId { get; set; }
}