using System.ComponentModel.DataAnnotations;
using Volo.Abp.Domain.Entities;

namespace Wcs.Processes.Models;

/// <summary>
/// 物流过程上的节点定义，节点命令下发的前提记录在DispatchPrecondition中
/// </summary>
public class DispatchProcessStep : Entity<int>
{
    public int ProcessId { get; set; }

    public int Sequence { get; set; }

    [StringLength(50)]
    [Required]
    public string NodeCode { get; set; } = string.Empty;

    public int JobWorkerId { get; set; }

    public int JobCmdId { get; set; }

    /// <summary>
    /// 若当前节点是一个判断节点，当判断为true，跳转到此节点，若非判断节点，也按照此节点跳转
    /// </summary>
    public int NextTrueStep { get; set; }

    /// <summary>
    /// 若当前节点是一个判断节点，当判断为false，跳转到此节点
    /// </summary>
    /// <value></value>
    public int NextFalseStep { get; set; }

    [StringLength(500)]
    public string Describe { get; set; }

    public DispatchProcessStep()
    {

    }

    public DispatchProcessStep(int id)
    {
        Id = id;
    }
}
