using System.ComponentModel.DataAnnotations;
using Volo.Abp.Domain.Entities;

namespace Wcs.Processes.Models;

/// <summary>
/// 调度过程中每个命令需要占用的Node
/// </summary>
public class DispatchProcessStepResource : Entity<int>
{
    public int ProcessId { get; set; }

    public int Sequence { get; set; }

    [StringLength(500)]
    [Required]
    public string Resource { get; set; } = string.Empty; //NodeCode名称集合，以半角逗号分隔
}
