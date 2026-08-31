using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;

namespace Wcs.Dispatch;

/// <summary>
/// 设备节点命令
/// </summary>
public class DispatchNodeCmdDto : EntityDto<int>
{
    public string NodeTypeCode { get; set; } = string.Empty;

    public string NodeCmdName { get; set; } = string.Empty;

    public int NodeCmdValue { get; set; }
}