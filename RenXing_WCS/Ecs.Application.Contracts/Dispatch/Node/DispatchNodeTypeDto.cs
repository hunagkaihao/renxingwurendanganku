using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace Ecs.Dispatch;

/// <summary>
/// 设备节点类型
/// </summary>
public class DispatchNodeTypeDto : EntityDto<int>
{
    public string NodeTypeName { get; set; } = string.Empty;

    public string NodeTypeCode { get; set; } = string.Empty;

    public string Describe { get; set; } = string.Empty; 
}