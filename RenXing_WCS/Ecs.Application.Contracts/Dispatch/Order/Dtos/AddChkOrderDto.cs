using System.Collections.Generic;


using Volo.Abp.Application.Dtos;

namespace Ecs.Dispatch;

/// <summary>
/// 盘点订单
/// </summary>
public class AddCheckOrderDto : EntityDto
{
    
    public string orderCode { get; set; } = string.Empty;    //调度订单Code，不可重复
    public string startCellCode { get; set; } = string.Empty;
    public string endCellCode { get; set; } = string.Empty;
    public int priority { get; set; }
}
public class CheckOrder
{
    public string id { get; set; } = string.Empty;    //调度订单Code，不可重复
    public string startCellCode { get; set; } = string.Empty;
    public string endCellCode { get; set; } = string.Empty;
    public int priorityLevel { get; set; } = 1;
}
/// <summary>
/// 盘点订单列表
/// </summary>
// public class AddCheckOrdersDto : EntityDto
// {
//     public List<AddCheckOrderDto> orders { get; set; } = new List<AddCheckOrderDto>();

//     public int priority { get; set; }
// }