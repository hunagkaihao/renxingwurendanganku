using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Wcs.Dispatch;
using Volo.Abp.Domain.Entities;

namespace Wcs.Jobs.Models;

public class DispatchJob : Entity<int>
{
    public int TaskId { get; set; }

    [StringLength(50)]
    [Required]
    public string OrderCode { get; set; } = string.Empty;

    public int ProcessId { get; set; }

    public int ProcessSequence { get; set; }

    [StringLength(50)]
    [Required]
    public string NodeCode { get; set; } = string.Empty;

    public int JobCmdId { get; set; }

    public int JobWorkerId { get; set; }

    public int NextTrueStep { get; set; }

    public int NextFalseStep { get; set; }

    public EnumDispatchJobState State { get; set; }

    public int Priority { get; set; }

    [StringLength(50)]
    public string CreateTime { get; set; } = string.Empty;

    public DispatchJob()
    {
    }

    public DispatchJob(int id)
    {
        Id = id;
    }

    public DispatchJob(DispatchJob other)
    {
        Id = other.Id;
        TaskId = other.TaskId;
        OrderCode = other.OrderCode;
        ProcessId = other.ProcessId;
        ProcessSequence = other.ProcessSequence;
        NextFalseStep = other.NextFalseStep;
        NextTrueStep = other.NextTrueStep;
        NodeCode = other.NodeCode;
        JobCmdId = other.JobCmdId;
        JobWorkerId = other.JobWorkerId;
        State = other.State;
        Priority = other.Priority;
        CreateTime = other.CreateTime;

    }
}