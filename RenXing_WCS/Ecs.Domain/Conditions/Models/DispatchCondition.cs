using System.ComponentModel.DataAnnotations;
using Volo.Abp;
using Volo.Abp.Domain.Entities;

namespace Ecs.Conditions.Models;

/// <summary>
/// 物流过程下每个节点的工作前提
/// </summary>
public class DispatchCondition : Entity<int>
{
    private DispatchCondition()
    {
    }

    internal DispatchCondition(string conditionName, string conditionSrc, string describe)
    {
        ConditionName = Check.NotNullOrWhiteSpace(conditionName, nameof(conditionName));
        ConditionSrc = Check.NotNullOrWhiteSpace(conditionSrc, nameof(conditionSrc));
        Describe = describe;
    }

    /// <summary>
    /// 条件名称
    /// </summary>
    /// <value></value>
    [Required]
    [StringLength(50)]
    public string ConditionName { get; private set; }

    /// <summary>
    /// 条件变量来源，如，PLC，MJJ等
    /// </summary>
    /// <value></value>
    [Required]
    [StringLength(50)]
    public string ConditionSrc { get; private set; }

    /// <summary>
    /// 描述
    /// </summary>
    [StringLength(512)]
    public string Describe { get; private set; }
}