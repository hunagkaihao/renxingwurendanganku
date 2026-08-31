using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;

namespace Wcs.Dispatch;

/// <summary>
/// 设备节点
/// </summary>
public class DispatchNodeDto : EntityDto<int>
{
    public string NodeCode { get; set; } = string.Empty;

    public string NodeName { get; set; } = string.Empty;

    public string NodeTypeCode { get; set; } = string.Empty;

    public string DASpecs { get; set; } = string.Empty;  //档案盒规格，若兼容每种档案盒，填入any

    public string CmdTagName { get; set; } = string.Empty; //该Node接收设备指令的地址

    public string ResponseTagName { get; set; } = string.Empty; //该Node执行设备指令后的反馈地址

    public int TaskIdOwnIt { get; set; }

    public EnumDispatchNodeState NodeState { get; set; }
}