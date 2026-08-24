using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace Ecs.Dispatch;

/// <summary>
/// 出入库订单s
/// </summary>
public class AddStockOrdersDto : EntityDto
{
    public List<AddStockOrderDto> stockOrders { get; set; } = new List<AddStockOrderDto>();
}
