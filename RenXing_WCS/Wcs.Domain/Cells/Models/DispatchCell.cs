using System.ComponentModel.DataAnnotations;
using Volo.Abp;
using Volo.Abp.Domain.Entities;

namespace Wcs.Cells.Models;

/// <summary>
/// 约定：
/// 面对密集架上显示屏，最左侧为排1
/// 靠近密集架显示屏为列1
/// 最底层为层1
/// </summary>
public class DispatchCell : Entity<int>
{
    /// <summary>
    /// 库位所属的库Id
    /// </summary>
    /// <value></value>
    public int WarehouseId { get; private set; }
    /// <summary>
    /// 库位码
    /// </summary>
    /// <value></value>
    [StringLength(50)]
    [Required]
    public string CellCode { get; private set; } = string.Empty;
    /// <summary>
    /// 库位名称
    /// </summary>
    /// <value></value>
    [StringLength(50)]
    [Required]
    public string CellName { get; private set; } = string.Empty;
    /// <summary>
    /// 密集架排
    /// </summary>
    /// <value></value>
    public int Row { get; private set; }
    /// <summary>
    /// 密集架列
    /// </summary>
    /// <value></value>
    public int Col { get; private set; }
    /// <summary>
    /// 密集架层
    /// </summary>
    /// <value></value>
    public int Layer { get; private set; }
    /// <summary>
    /// Plc用的排
    /// </summary>
    /// <value></value>
    public int RowForPlc { get; set; }
    /// <summary>
    /// Plc用的层
    /// </summary>
    /// <value></value>
    public int LayerForPlc { get; set; }
    /// <summary>
    /// Plc用的密集架的节号
    /// </summary>
    /// <value></value>
    public int SectNoForPlc { get; private set; }
    /// <summary>
    /// Plc用的一节中的列号
    /// </summary>
    /// <value></value>
    public int ColNoInSectForPlc { get; private set; }
    /// <summary>
    /// 库位规格
    /// </summary>
    [StringLength(50)]
    [Required]
    public string CellSpecs { get; private set; }
    /// <summary>
    /// 该库位对应的设备节点
    /// </summary>
    /// <value></value>
    [StringLength(50)]
    [Required]
    public string RelativeNode { get; private set; } = string.Empty;

    private DispatchCell()
    {

    }

    internal DispatchCell(
        int wareHouseId,
        int row,
        int col,
        int layer,
        int rowForPlc,
        int layerForPlc,
        int sectNoForPlc,
        int colInSectForPlc,
        string specs, string
        relativeNode)
    {
        WarehouseId = Check.Positive(wareHouseId, nameof(wareHouseId));
        Row = Check.Range(row, nameof(row), 1, 99);
        Col = Check.Range(col, nameof(col), 1, 999);
        Layer = Check.Range(layer, nameof(layer), 1, 99);
        RowForPlc = Check.Range(rowForPlc, nameof(rowForPlc), 1, 99);
        LayerForPlc = Check.Range(layerForPlc, nameof(layerForPlc), 1, 99);
        SectNoForPlc = Check.Range(sectNoForPlc, nameof(sectNoForPlc), 1, 99);
        ColNoInSectForPlc = Check.Range(colInSectForPlc, nameof(colInSectForPlc), 1, 999);
        RelativeNode = Check.NotNullOrEmpty(relativeNode, nameof(relativeNode));
        CellSpecs = Check.NotNullOrEmpty(specs, nameof(specs));
        CellCode = $"{row:D2}-{col:D3}-{layer:D2}";
        CellName = $"{row:D2}排-{col:D3}列-{layer:D2}层";
    }
}