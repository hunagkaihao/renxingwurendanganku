using Volo.Abp.Application.Dtos;

namespace Wcs.Dispatch;

public class DispatchCellDto : EntityDto<int>
{
    /// <summary>
    /// 库位所属的库Id
    /// </summary>
    /// <value></value>
    public int WarehouseId { get; set; }
    /// <summary>
    /// 库位码
    /// </summary>
    /// <value></value>
    public string CellCode { get; set; }
    /// <summary>
    /// 库位名称
    /// </summary>
    /// <value></value>
    public string CellName { get; set; }
    /// <summary>
    /// 密集架列
    /// </summary>
    /// <value></value>
    public int Row { get; set; }
    /// <summary>
    /// 密集架节
    /// </summary>
    /// <value></value>
    public int Col { get; set; }
    /// <summary>
    /// 密集架层
    /// </summary>
    /// <value></value>
    public int Layer { get; set; }
    /// <summary>
    /// 库位规格，兼容所有档案盒，则填any
    /// </summary>
    public string CellSpecs { get; set; }
    /// <summary>
    /// 该库位对应的设备节点
    /// </summary>
    /// <value></value>
    public string RelativeNode { get; set; }
}