namespace WarehouseManagement.WcsTasks.Dto;

/// <summary>
/// WMS 向 WCS 下发的单个连续盘点扫描段。
/// 一个扫描段必须位于同一排、同一层，WCS/PLC 在起终点之间连续扫码。
/// </summary>
public class WcsCheckOrderRequestDto
{
    /// <summary>扫描段唯一订单号。</summary>
    public string OrderCode { get; set; }

    /// <summary>整个盘点计划共享的查询码。</summary>
    public string QueryCode { get; set; }

    /// <summary>扫描段起始库位。</summary>
    public string StartCellCode { get; set; }

    /// <summary>扫描段终止库位。</summary>
    public string EndCellCode { get; set; }

    /// <summary>调度优先级。</summary>
    public int Priority { get; set; }
}
