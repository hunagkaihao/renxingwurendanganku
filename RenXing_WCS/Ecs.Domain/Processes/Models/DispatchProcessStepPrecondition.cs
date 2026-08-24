using System.ComponentModel.DataAnnotations;
using Volo.Abp.Domain.Entities;

namespace Ecs.Processes.Models;

/// <summary>
/// 调度过程每个命令的工作前提
/// </summary>
public class DispatchProcessStepPrecondition : Entity<int>
{
    public int ProcessId { get; set; }

    public int Sequence { get; set; }

    [StringLength(50)]
    [Required]
    public string ConditionName { get; set; } = string.Empty;

    [StringLength(50)]
    [Required]
    public string ConditionValue { get; set; } = string.Empty;

    [StringLength(200)]
    public string Describe { get; set; }
}
