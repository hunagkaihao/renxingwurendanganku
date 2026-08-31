using System.ComponentModel.DataAnnotations;
using Volo.Abp;
using Volo.Abp.Domain.Entities;

namespace Wcs.Nodes.Models;

/// <summary>
/// 设备节点命令
/// </summary>
public class DispatchNodeCmd : Entity<int>
{
    [StringLength(50)]
    [Required]
    public string NodeTypeCode { get; set; } = string.Empty;

    [StringLength(50)]
    [Required]
    public string NodeCmdName { get; set; } = string.Empty;

    [Required]
    public int NodeCmdValue { get; set; }

    public DispatchNodeCmd() { }

    public DispatchNodeCmd(string nodeTypeCode, string nodeCmdName, int nodeCmdValue)
    {
        try
        {
            NodeTypeCode = Check.NotNullOrEmpty(nodeTypeCode, nameof(nodeTypeCode), 50, 1);
            NodeCmdName = Check.NotNullOrEmpty(nodeCmdName, nameof(nodeCmdName), 50, 1);
            NodeCmdValue = Check.Positive(nodeCmdValue, nameof(nodeCmdValue));
        }
        catch
        {
            NodeTypeCode = string.Empty;
            NodeCmdName = string.Empty;
            NodeCmdValue = 0;
        }
    }
}