using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp;
using Volo.Abp.Domain.Entities;

namespace Ecs.Orders.Models;

public class DispatchChkOrderRslt : Entity<int>
{
    private DispatchChkOrderRslt()
    {

    }

    public DispatchChkOrderRslt(string orderCode, string cellCode, string plateCode, string queryCode = "")
    {
        Check.NotNullOrEmpty(orderCode, nameof(orderCode));
        Check.NotNullOrEmpty(cellCode, nameof(cellCode));
        Check.NotNullOrEmpty(plateCode, nameof(plateCode));
        string[] sects = cellCode.Split("-");
        if (sects.Length != 3)
            throw new Exception($"仓位{cellCode}格式不正确，应为xx-xx-xx");
        if (!int.TryParse(sects[0], out int row))
            throw new Exception($"仓位{cellCode}的排信息{sects[0]}不正确，无法转换为整数");
        if (!int.TryParse(sects[1], out int col))
            throw new Exception($"仓位{cellCode}的列信息{sects[1]}不正确，无法转换为整数");
        if (!int.TryParse(sects[2], out int layer))
            throw new Exception($"仓位{cellCode}的层信息{sects[2]}不正确，无法转换为整数");
        if (row <= 0)
            throw new Exception($"仓位{cellCode}的排信息{sects[0]}不正确，应大于0");
        if (col <= 0)
            throw new Exception($"仓位{cellCode}的列信息{sects[1]}不正确，应大于0");
        if (layer <= 0)
            throw new Exception($"仓位{cellCode}的层信息{sects[2]}不正确，应大于0");
        if (plateCode != "waiting" && plateCode != "empty" && plateCode != "error" && !int.TryParse(plateCode, out int iPlateCode))
            throw new Exception($"档案盒条码可取值为waiting、 empty、 error或整数，当前值为{plateCode}，不符合要求");

        OrderCode = orderCode;
        CellCode = cellCode;
        PlateCode = plateCode;
        QueryCode = queryCode;
    }

    [StringLength(50)]
    [Required]
    public string OrderCode { get; set; } = string.Empty;    //所属的盘点订单Code

    [StringLength(50)]
    [Required]
    public string CellCode { get; set; } = string.Empty;   //盘点的其中一个库位号

    [StringLength(50)]
    [Required]
    public string PlateCode { get; set; } = string.Empty;   //库位中的档案盒号，没有盒子反馈empty，检测失败反馈error，尚未检查默认waiting

    [StringLength(50)]
    [Required]
    public string QueryCode { get; set; } = string.Empty;   //盘点结果查询码
}