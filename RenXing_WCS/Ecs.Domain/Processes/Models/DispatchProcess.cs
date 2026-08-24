using System.ComponentModel.DataAnnotations;
using Volo.Abp.Domain.Entities;

namespace Ecs.Processes.Models;

/// <summary>
/// 物流过程，通过起始点和终止点确定过程，每个DispatchTask包含一个DispatchPath，
/// 每个DispatchPath包含一系列Node，每个Node包含执行命令和执行前提
/// </summary>
public class DispatchProcess : Entity<int>
{
    [StringLength(50)]
    [Required]
    public string StartNodeCode { get; set; }

    [StringLength(50)]
    [Required]
    public string EndNodeCode { get; set; }

    public DispatchProcess()
    {
        StartNodeCode = string.Empty;
        EndNodeCode = string.Empty;
    }

    public DispatchProcess(int id, string startNodeCode, string endNodeCode)
    {
        Id = id;
        StartNodeCode = startNodeCode;
        EndNodeCode = endNodeCode;
    }
}
