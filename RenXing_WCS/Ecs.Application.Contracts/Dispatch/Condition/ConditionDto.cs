using Volo.Abp.Application.Dtos;

namespace Ecs.Dispatch;

public class ConditionDto : EntityDto<int>
{
    /// <summary>
    /// 条件名称
    /// </summary>
    /// <value></value>
    public string ConditionName { get; set; } = string.Empty;

    /// <summary>
    /// 条件变量来源，如，PLC，MJJ等
    /// </summary>
    /// <value></value>
    public string ConditionSrc { get; set; } = string.Empty;

    public string Describe { get; set; }
}