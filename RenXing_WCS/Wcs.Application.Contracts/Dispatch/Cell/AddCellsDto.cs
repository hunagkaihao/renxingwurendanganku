using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace Wcs.Dispatch;

public class AddCellsDto : EntityDto
{
    /// <summary>
    /// 库位所属的库Id
    /// </summary>
    /// <value></value>
    public int warehouseId { get; set; }
    /// <summary>
    /// 增加的排号
    /// </summary>
    /// <value></value>
    public int rowNo { get; set; }
    /// <summary>
    /// 增加的列数量
    /// </summary>
    /// <value></value>
    public int colCnt { get; set; }
    /// <summary>
    /// 起始层
    /// </summary>
    /// <value></value>
    public int startLayerNo { get; set; }
    /// <summary>
    /// 增加的层数量
    /// </summary>
    /// <value></value>
    public int layerCnt { get; set; }
    /// <summary>
    /// Plc内定义的排
    /// </summary>
    /// <value></value>
    public int rowNoForPlc { get; set; }
    /// <summary>
    /// wms的层方向和Plc的层方向是否一致
    /// </summary>
    /// <value></value>
    public bool layerConsistence { get; set; }
    /// <summary>
    /// wms的列放向与Plc定义的节方向是否一致
    /// </summary>
    /// <value></value>
    public bool sectConsistence { get; set; }
    /// <summary>
    /// wms的列方向与Plc定义的节内列方向是否一致
    /// </summary>
    /// <value></value>
    public bool colConsistence { get; set; }
    /// <summary>
    /// 各节中的列数
    /// </summary>
    /// <value></value>
    public List<int> colCntInSect { get; set; }
    /// <summary>
    /// 库位规格，全部档案盒都兼容填写any
    /// </summary>
    /// <value></value>
    public string cellSpecs { get; set; }
    /// <summary>
    /// 该库位对应的设备节点
    /// </summary>
    /// <value></value>
    public string relativeNode { get; set; }
}