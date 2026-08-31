using System.ComponentModel.DataAnnotations;
using Volo.Abp.Domain.Entities;

namespace Wcs.Jobs.Models;

/// <summary>
/// 实现IJobWorker
/// </summary>
public class DispatchJobWorker : Entity<int>
{
    [StringLength(50)]
    [Required]
    public string JobWorkerClassName { get; set; } = string.Empty;

    [StringLength(512)]
    public string Describe { get; set; }
}

