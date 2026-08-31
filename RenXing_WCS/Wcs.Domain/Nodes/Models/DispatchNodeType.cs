using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Volo.Abp;
using Volo.Abp.Domain.Entities;

namespace Wcs.Nodes.Models;

/// <summary>
/// 设备节点类型
/// </summary>
public class DispatchNodeType : Entity<int>
{
    [StringLength(50)]
    [Required]
    public string NodeTypeName { get; set; } = string.Empty;

    [StringLength(50)]
    [Required]
    public string NodeTypeCode { get; set; } = string.Empty;

    [StringLength(500)]
    public string Describe { get; set; } = string.Empty;

    public DispatchNodeType() { }

    public DispatchNodeType(string typeName, string typeCode, string describe)
    {
        try
        {
            NodeTypeName = Check.NotNullOrEmpty(typeName, nameof(typeName), 50, 1);
            NodeTypeCode = Check.NotNullOrEmpty(typeCode, nameof(typeCode), 50, 1);
            Describe = Check.NotNullOrEmpty(describe, nameof(describe), 500, 1);
        }
        catch
        {
            NodeTypeName = string.Empty;
            NodeTypeCode = string.Empty;
            Describe = string.Empty;
        }
    }
}