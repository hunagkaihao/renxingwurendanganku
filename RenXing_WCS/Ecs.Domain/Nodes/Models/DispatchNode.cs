using System.ComponentModel.DataAnnotations;
using Ecs.Dispatch;
using Volo.Abp.Domain.Entities;

namespace Ecs.Nodes.Models;

/// <summary>
/// 设备节点
/// </summary>
public class DispatchNode : Entity<int>
{
    [StringLength(50)]
    [Required]
    public string NodeCode { get; set; } = string.Empty;

    [StringLength(50)]
    [Required]
    public string NodeName { get; set; } = string.Empty;

    [StringLength(50)]
    [Required]
    public string NodeTypeCode { get; set; } = string.Empty;

    [StringLength(50)]
    [Required]
    public string DASpecs { get; set; } = string.Empty;  //档案盒规格，若兼容每种档案盒，填入any

    [StringLength(50)]
    [Required]
    public string CmdTagName { get; set; } = string.Empty; //该Node接收设备指令的地址

    [StringLength(50)]
    [Required]
    public string ResponseTagName { get; set; } = string.Empty; //该Node执行设备指令后的反馈地址

    public int TaskIdOwnIt { get; set; }

    public EnumDispatchNodeState NodeState { get; set; }

    public DispatchNode() { }

    public DispatchNode(int id)
    {
        Id = id;
    }
}