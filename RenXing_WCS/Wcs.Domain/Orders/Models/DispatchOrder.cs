using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Wcs.Dispatch;
using Volo.Abp;
using Volo.Abp.Domain.Entities;

namespace Wcs.Orders.Models;

public class DispatchOrder : Entity<int>
{
    private DispatchOrder()
    {

    }

    public DispatchOrder(string orderCode, EnumDispatchOrderType orderType, string startNode, string endNode, int priority)
    {
        Check.NotNullOrEmpty(orderCode, nameof(orderCode));
        Check.NotNullOrEmpty(startNode, nameof(startNode));
        Check.NotNullOrEmpty(endNode, nameof(endNode));
        Check.Positive(priority, nameof(priority));
        OrderCode = orderCode;
        OrderType = orderType;
        StartNode = startNode;
        EndNode = endNode;
        Priority = priority;
        PlateCode = string.Empty;
        PlateSpecs = string.Empty;
        State = EnumDispatchOrderState.Created;
        CanOpenDoorImmediate = false;
        HasError = false;
        ExecStep = "等待执行";
        ExecInfo = string.Empty;
        ExecUpdateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }

    public void SetPlate(string plateCode, string plateSpecs)
    {
        if (OrderType == EnumDispatchOrderType.CheckDown) //盘点订单不需要指定档案盒
            return;
        if (!int.TryParse(plateCode, out int iPlateCode))
            throw new Exception($"档案盒条码为{PlateCode}，包含非数字字符，应只包含数字字符");

        if (string.IsNullOrEmpty(plateSpecs))
            throw new Exception($"档案盒规格不能为空");

        PlateCode = plateCode;
        PlateSpecs = plateSpecs;
    }

    public void SetOrderState(EnumDispatchOrderState state)
    {
        State = state;
    }

    public void SetCanOpenDoorImmediate(bool canOpen)
    {
        CanOpenDoorImmediate = canOpen;
    }

    public void SetExecInfo(string execInfo, bool hasError)
    {
        ExecInfo = execInfo;
        HasError = hasError;
        ExecUpdateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }

    public void SetExecStep(string execStep)
    {
        ExecStep = execStep;
        ExecUpdateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }

    /// <summary>
    /// 输出盘点库位号集合
    /// </summary>
    /// <returns></returns>
    public List<string> OutputCellCodesToChk()
    {
        if (OrderType != EnumDispatchOrderType.CheckDown)
            return new List<string>();

        string[] sects = StartNode.Split("-");
        if (sects.Count() != 3)
            throw new Exception($"类型为CheckDown的订单的起点为库位，格式为xx-xx-xx，但当前订单起点为{StartNode}，格式不正确");

        if (!int.TryParse(sects[0], out int startRow))
            throw new Exception($"类型为CheckDown的订单的起点为库位，当前订单起点为{StartNode}，其中排信息为{sects[0]}，无法转换成整数");

        if (!int.TryParse(sects[1], out int startCol))
            throw new Exception($"类型为CheckDown的订单的起点为库位，当前订单起点为{StartNode}，其中列信息为{sects[1]}，无法转换成整数");

        if (!int.TryParse(sects[2], out int startLayer))
            throw new Exception($"类型为CheckDown的订单的起点为库位，当前订单起点为{StartNode}，其中层信息为{sects[2]}，无法转换成整数");

        sects = EndNode.Split("-");
        if (sects.Count() != 3)
            throw new Exception($"类型为CheckDown的订单的终点为库位，格式为xx-xx-xx，但当前订单终点为{EndNode}，格式不正确");

        if (!int.TryParse(sects[0], out int endRow))
            throw new Exception($"类型为CheckDown的订单的终点为库位，当前订单终点为{EndNode}，其中排信息为{sects[0]}，无法转换成整数");

        if (!int.TryParse(sects[1], out int endCol))
            throw new Exception($"类型为CheckDown的订单的终点为库位，当前订单终点为{EndNode}，其中列信息为{sects[1]}，无法转换成整数");

        if (!int.TryParse(sects[2], out int endLayer))
            throw new Exception($"类型为CheckDown的订单的终点为库位，当前订单终点为{EndNode}，其中层信息为{sects[2]}，无法转换成整数");

        if (startRow <= 0)
            throw new Exception($"类型为CheckDown的订单的起点为库位，当前订单起点为{StartNode}，其中排信息为{sects[0]}，应大于0");

        if (startCol <= 0)
            throw new Exception($"类型为CheckDown的订单的起点为库位，当前订单起点为{StartNode}，其中列信息为{sects[1]}，应大于0");

        if (startLayer <= 0)
            throw new Exception($"类型为CheckDown的订单的起点为库位，当前订单起点为{StartNode}，其中层信息为{sects[2]}，应大于0");

        if (endRow <= 0)
            throw new Exception($"类型为CheckDown的订单的终点为库位，当前订单终点为{EndNode}，其中排信息为{sects[0]}，应大于0");

        if (endCol <= 0)
            throw new Exception($"类型为CheckDown的订单的终点为库位，当前订单终点为{EndNode}，其中列信息为{sects[1]}，应大于0");

        if (endLayer <= 0)
            throw new Exception($"类型为CheckDown的订单的终点为库位，当前订单终点为{EndNode}，其中层信息为{sects[2]}，应大于0");

        if (startRow != endRow)
            throw new Exception($"类型为CheckDown的订单的起点和终点应在同一排，但该订单起点排为{startRow}，终点排为{endRow}，不一致");

        if (startLayer != endLayer)
            throw new Exception($"类型为CheckDown的订单的起点和终点应在同一层，但该订单起点层为{startLayer}，终点层为{endLayer}，不一致");

        int minCol = startCol <= endCol ? startCol : endCol;
        int maxCol = startCol <= endCol ? endCol : startCol;
        List<string> result = new List<string>();
        for (int col = minCol; col <= maxCol; col++)
        {
            result.Add($"{startRow:D2}-{col:D3}-{startLayer:D2}");
        }
        return result;
    }

    /// <summary>
    /// 对调度订单的有效性验证
    /// </summary>
    /// <param name="failedReason"></param>
    /// <returns></returns>
    public bool Validate(out string failedReason)
    {
        failedReason = string.Empty;

        Check.NotNullOrEmpty(OrderCode, nameof(OrderCode));

        if (OrderType == EnumDispatchOrderType.StockIn)
        {
            if (!int.TryParse(StartNode, out int iStartNode))
                failedReason = $"类型为StockIn的订单的起始点非库位，可转换为整数，但当前订单起始点为{StartNode}，无法转换成整数";

            string[] sects = EndNode.Split("-");
            if (sects.Count() != 3)
            {
                failedReason = $"类型为StockIn的订单的终点为库位，格式为xx-xx-xx，但当前订单终点为{EndNode}，格式不正确";
                return false;
            }

            if (!int.TryParse(sects[0], out int row))
            {
                failedReason = $"类型为StockIn的订单的终点为库位，当前订单终点为{EndNode}，其中排信息为{sects[0]}，无法转换成整数";
                return false;
            }

            if (!int.TryParse(sects[1], out int col))
            {
                failedReason = $"类型为StockIn的订单的终点为库位，当前订单终点为{EndNode}，其中列信息为{sects[1]}，无法转换成整数";
                return false;
            }

            if (!int.TryParse(sects[2], out int layer))
            {
                failedReason = $"类型为StockIn的订单的终点为库位，当前订单终点为{EndNode}，其中层信息为{sects[2]}，无法转换成整数";
                return false;
            }

            if (row <= 0)
            {
                failedReason = $"类型为StockIn的订单的终点为库位，当前订单终点为{EndNode}，其中排信息为{sects[0]}，应大于0";
                return false;
            }

            if (col <= 0)
            {
                failedReason = $"类型为StockIn的订单的终点为库位，当前订单终点为{EndNode}，其中列信息为{sects[1]}，应大于0";
                return false;
            }

            if (layer <= 0)
            {
                failedReason = $"类型为StockIn的订单的终点为库位，当前订单终点为{EndNode}，其中层信息为{sects[2]}，应大于0";
                return false;
            }

            if (!int.TryParse(PlateCode, out int iPlateCode))
            {
                failedReason = $"当前类型为StockIn的订单的档案盒条码为{PlateCode}，包含非数字字符，应只包含数字字符";
                return false;
            }

            if (string.IsNullOrEmpty(PlateSpecs))
            {
                failedReason = $"当前类型为StockIn的订单的档案盒规格为空";
                return false;
            }
        }
        else if (OrderType == EnumDispatchOrderType.StockOut)
        {
            if (!int.TryParse(EndNode, out int iEndNode))
            {
                failedReason = $"类型为StockOut的订单的终点非库位，可转换为整数，但当前订单终点为{EndNode}，无法转换成整数";
                return false;
            }

            string[] sects = StartNode.Split("-");
            if (sects.Count() != 3)
            {
                failedReason = $"类型为StockOut的订单的起点为库位，格式为xx-xx-xx，但当前订单起点为{StartNode}，格式不正确";
                return false;
            }

            if (!int.TryParse(sects[0], out int row))
            {
                failedReason = $"类型为StockOut的订单的起点为库位，当前订单起点为{StartNode}，其中排信息为{sects[0]}，无法转换成整数";
                return false;
            }

            if (!int.TryParse(sects[1], out int col))
            {
                failedReason = $"类型为StockOut的订单的起点为库位，当前订单起点为{StartNode}，其中列信息为{sects[1]}，无法转换成整数";
                return false;
            }

            if (!int.TryParse(sects[2], out int layer))
            {
                failedReason = $"类型为StockOut的订单的起点为库位，当前订单起点为{StartNode}，其中层信息为{sects[2]}，无法转换成整数";
                return false;
            }

            if (row <= 0)
            {
                failedReason = $"类型为StockOut的订单的起点为库位，当前订单起点为{StartNode}，其中排信息为{sects[0]}，应大于0";
                return false;
            }

            if (col <= 0)
            {
                failedReason = $"类型为StockOut的订单的起点为库位，当前订单起点为{StartNode}，其中列信息为{sects[1]}，应大于0";
                return false;
            }

            if (layer <= 0)
            {
                failedReason = $"类型为StockOut的订单的起点为库位，当前订单起点为{StartNode}，其中层信息为{sects[2]}，应大于0";
                return false;
            }            

            if (!int.TryParse(PlateCode, out int iPlateCode))
            {
                failedReason = $"当前类型为StockOut的订单的档案盒条码为{PlateCode}，包含非数字字符，应只包含数字字符";
                return false;
            }

            if (string.IsNullOrEmpty(PlateSpecs))
            {
                failedReason = $"当前类型为StockOut的订单的档案盒规格为空";
                return false;
            }
        }
        else if (OrderType == EnumDispatchOrderType.Move)
        {
            string[] sects = StartNode.Split("-");
            if (sects.Count() != 3)
            {
                failedReason = $"类型为Move的订单的起点为库位，格式为xx-xx-xx，但当前订单起点为{StartNode}，格式不正确";
                return false;
            }

            if (!int.TryParse(sects[0], out int row))
            {
                failedReason = $"类型为Move的订单的起点为库位，当前订单起点为{StartNode}，其中排信息为{sects[0]}，无法转换成整数";
                return false;
            }

            if (!int.TryParse(sects[1], out int col))
            {
                failedReason = $"类型为Move的订单的起点为库位，当前订单起点为{StartNode}，其中列信息为{sects[1]}，无法转换成整数";
                return false;
            }

            if (!int.TryParse(sects[2], out int layer))
            {
                failedReason = $"类型为Move的订单的起点为库位，当前订单起点为{StartNode}，其中层信息为{sects[2]}，无法转换成整数";
                return false;
            }

            if (row <= 0)
            {
                failedReason = $"类型为Move的订单的起点为库位，当前订单起点为{StartNode}，其中排信息为{sects[0]}，应大于0";
                return false;
            }

            if (col <= 0)
            {
                failedReason = $"类型为Move的订单的起点为库位，当前订单起点为{StartNode}，其中列信息为{sects[1]}，应大于0";
                return false;
            }

            if (layer <= 0)
            {
                failedReason = $"类型为Move的订单的起点为库位，当前订单起点为{StartNode}，其中层信息为{sects[2]}，应大于0";
                return false;
            }

            sects = EndNode.Split("-");
            if (sects.Count() != 3)
            {
                failedReason = $"类型为Move的订单的终点为库位，格式为xx-xx-xx，但当前订单终点为{EndNode}，格式不正确";
                return false;
            }

            if (!int.TryParse(sects[0], out row))
            {
                failedReason = $"类型为Move的订单的终点为库位，当前订单终点为{EndNode}，其中排信息为{sects[0]}，无法转换成整数";
                return false;
            }

            if (!int.TryParse(sects[1], out col))
            {
                failedReason = $"类型为Move的订单的终点为库位，当前订单终点为{EndNode}，其中列信息为{sects[1]}，无法转换成整数";
                return false;
            }

            if (!int.TryParse(sects[2], out layer))
            {
                failedReason = $"类型为Move的订单的终点为库位，当前订单终点为{EndNode}，其中层信息为{sects[2]}，无法转换成整数";
                return false;
            }

            if (row <= 0)
            {
                failedReason = $"类型为Move的订单的终点为库位，当前订单终点为{EndNode}，其中排信息为{sects[0]}，应大于0";
                return false;
            }

            if (col <= 0)
            {
                failedReason = $"类型为Move的订单的终点为库位，当前订单终点为{EndNode}，其中列信息为{sects[1]}，应大于0";
                return false;
            }

            if (layer <= 0)
            {
                failedReason = $"类型为Move的订单的终点为库位，当前订单终点为{EndNode}，其中层信息为{sects[2]}，应大于0";
                return false;
            }

            if (!int.TryParse(PlateCode, out int iPlateCode))
            {
                failedReason = $"当前类型为Move的订单的档案盒条码为{PlateCode}，包含非数字字符，应只包含数字字符";
                return false;
            }

            if (string.IsNullOrEmpty(PlateSpecs))
            {
                failedReason = $"当前类型为Move的订单的档案盒规格为空";
                return false;
            }
        }
        else
        {
            string[] sects = StartNode.Split("-");
            if (sects.Count() != 3)
            {
                failedReason = $"类型为CheckDown的订单的起点为库位，格式为xx-xx-xx，但当前订单起点为{StartNode}，格式不正确";
                return false;
            }

            if (!int.TryParse(sects[0], out int startRow))
            {
                failedReason = $"类型为CheckDown的订单的起点为库位，当前订单起点为{StartNode}，其中排信息为{sects[0]}，无法转换成整数";
                return false;
            }

            if (!int.TryParse(sects[1], out int startCol))
            {
                failedReason = $"类型为CheckDown的订单的起点为库位，当前订单起点为{StartNode}，其中列信息为{sects[1]}，无法转换成整数";
                return false;
            }

            if (!int.TryParse(sects[2], out int startLayer))
            {
                failedReason = $"类型为CheckDown的订单的起点为库位，当前订单起点为{StartNode}，其中层信息为{sects[2]}，无法转换成整数";
                return false;
            }

            sects = EndNode.Split("-");
            if (sects.Count() != 3)
            {
                failedReason = $"类型为CheckDown的订单的终点为库位，格式为xx-xx-xx，但当前订单终点为{EndNode}，格式不正确";
                return false;
            }

            if (!int.TryParse(sects[0], out int endRow))
            {
                failedReason = $"类型为CheckDown的订单的终点为库位，当前订单终点为{EndNode}，其中排信息为{sects[0]}，无法转换成整数";
                return false;
            }

            if (!int.TryParse(sects[1], out int endCol))
            {
                failedReason = $"类型为CheckDown的订单的终点为库位，当前订单终点为{EndNode}，其中列信息为{sects[1]}，无法转换成整数";
                return false;
            }

            if (!int.TryParse(sects[2], out int endLayer))
            {
                failedReason = $"类型为CheckDown的订单的终点为库位，当前订单终点为{EndNode}，其中层信息为{sects[2]}，无法转换成整数";
                return false;
            }

            if (startRow <= 0)
            {
                failedReason = $"类型为CheckDown的订单的起点为库位，当前订单起点为{StartNode}，其中排信息为{sects[0]}，应大于0";
                return false;
            }

            if (startCol <= 0)
            {
                failedReason = $"类型为CheckDown的订单的起点为库位，当前订单起点为{StartNode}，其中列信息为{sects[1]}，应大于0";
                return false;
            }

            if (startLayer <= 0)
            {
                failedReason = $"类型为CheckDown的订单的起点为库位，当前订单起点为{StartNode}，其中层信息为{sects[2]}，应大于0";
                return false;
            }

            if (endRow <= 0)
            {
                failedReason = $"类型为CheckDown的订单的终点为库位，当前订单终点为{EndNode}，其中排信息为{sects[0]}，应大于0";
                return false;
            }

            if (endCol <= 0)
            {
                failedReason = $"类型为CheckDown的订单的终点为库位，当前订单终点为{EndNode}，其中列信息为{sects[1]}，应大于0";
                return false;
            }

            if (endLayer <= 0)
            {
                failedReason = $"类型为CheckDown的订单的终点为库位，当前订单终点为{EndNode}，其中层信息为{sects[2]}，应大于0";
                return false;
            }

            if (startRow != endRow)
            {
                failedReason = $"类型为CheckDown的订单的起点和终点应在同一排，但该订单起点排为{startRow}，终点排为{endRow}，不一致";
                return false;
            }

            if (startLayer != endLayer)
            {
                failedReason = $"类型为CheckDown的订单的起点和终点应在同一层，但该订单起点层为{startLayer}，终点层为{endLayer}，不一致";
                return false;
            }
        }
        return true;
    }

    [StringLength(50)]
    [Required]
    public string OrderCode { get; private set; } = string.Empty;    //调度订单Code，不可重复

    [StringLength(50)]
    [Required]
    public string PlateCode { get; private set; } = string.Empty;   //托盘或物料承载物条码

    [StringLength(50)]
    [Required]
    public string PlateSpecs { get; private set; } = string.Empty;     //托盘或物料承载物规格

    [StringLength(50)]
    [Required]
    public string StartNode { get; private set; } = string.Empty;   //物流起点

    [StringLength(50)]
    [Required]
    public string EndNode { get; private set; } = string.Empty;     //物流终点

    public EnumDispatchOrderState State { get; private set; }

    public EnumDispatchOrderType OrderType { get; private set; } //订单类型

    public bool? LastCheckOrder { get; set; } //是否为最后一个盘点任务

    public bool CanOpenDoorImmediate { get; private set; } //该任务是否可以直接开门

    public int Priority { get; private set; }

    [StringLength(50)]
    public string CreateTime { get; private set; }

    [StringLength(512)]
    public string ExecStep { get; private set; } = string.Empty;

    [StringLength(1024)]
    public string ExecInfo { get; private set; } = string.Empty;

    [StringLength(50)]
    public string ExecUpdateTime { get; private set; } = string.Empty;

    public bool HasError { get; private set; } = false;
}