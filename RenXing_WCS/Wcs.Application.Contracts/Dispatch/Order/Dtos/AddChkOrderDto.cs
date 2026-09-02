using System.Collections.Generic;


using Volo.Abp.Application.Dtos;

namespace Wcs.Dispatch;

/// <summary>
/// 盘点订单
/// </summary>
public class AddCheckOrderDto : EntityDto
{
    /// <summary>
    /// 当前扫描段的唯一订单号。
    /// </summary>
    public string orderCode { get; set; } = string.Empty;    //调度订单Code，不可重复

    /// <summary>
    /// 整个盘点计划共享的查询码。WMS 使用该值一次查询所有扫描段结果。
    /// 未传时为兼容旧调用，WCS 使用 orderCode 作为查询码。
    /// </summary>
    public string queryCode { get; set; } = string.Empty;

    /// <summary>扫描段起始库位。</summary>
    public string startCellCode { get; set; } = string.Empty;

    /// <summary>扫描段终止库位；起终点必须位于同一排、同一层。</summary>
    public string endCellCode { get; set; } = string.Empty;

    /// <summary>调度优先级，数值越大越优先。</summary>
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
