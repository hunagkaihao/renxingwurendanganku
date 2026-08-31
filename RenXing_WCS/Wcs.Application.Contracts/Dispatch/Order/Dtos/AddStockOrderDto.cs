using Volo.Abp.Application.Dtos;

namespace Wcs.Dispatch;

/// <summary>
/// 出入库订单
/// </summary>
public class AddStockOrderDto : EntityDto
{
    public string orderCode { get; set; } = string.Empty;    //调度订单Code，不可重复

    public string plateCode { get; set; } = string.Empty;   //托盘或物料承载物条码

    public string startNode { get; set; } = string.Empty;   //物流起点

    public string endNode { get; set; } = string.Empty;     //物流终点

    public int priority { get; set; }
}
