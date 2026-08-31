using System;
using System.Collections.Generic;
using System.Threading;
using Wcs.Caches;
using Wcs.Caches.Models;
using Wcs.Cells;
using Wcs.Cells.Models;
using Wcs.Conditions;
using Wcs.ConfigTool;
using Wcs.DahSpecss;
using Wcs.DahSpecss.Models;
using Wcs.Jobs.Models;
using Wcs.Mjj;
using Wcs.Nodes;
using Wcs.Nodes.Models;
using Wcs.Orders;
using Wcs.Orders.Models;
using Wcs.PlcTool;
using Wcs.Processes;
using Wcs.Processes.Models;
using Wcs.Tasks;
using Wcs.Tasks.Models;
using Microsoft.Extensions.Options;
using Volo.Abp.DependencyInjection;

namespace Wcs.Jobs.JobCmds;

public class JobCmdHelper : ISingletonDependency
{
    private readonly IOptions<ConfigOptions> _options;
    private readonly OrderManager _orderManager;
    private readonly TaskManager _taskManager;
    private readonly ICellRepository _cellRepository;
    private readonly IDahSpecsRepository _dahSpecsRepository;
    private readonly CacheManager _cacheManager;
    private readonly NodeManager _nodeManager;
    private readonly ConditionManager _conditionManager;
    private readonly MjjManager _mjjManager;
    private readonly ProcessManager _processManager;
    private readonly PlcHelper _plcHelper;

    public JobCmdHelper(
        IOptions<ConfigOptions> options,
        OrderManager orderManager,
        TaskManager taskManager,
        ICellRepository cellRepository,
        IDahSpecsRepository dahSpecsRepository,
        CacheManager cacheManager,
        NodeManager nodeManager,
        ConditionManager conditionManager,
        MjjManager mjjManager,
        ProcessManager processManager,
        PlcHelper plcHelper)
    {
        _options = options;
        _orderManager = orderManager;
        _taskManager = taskManager;
        _cellRepository = cellRepository;
        _dahSpecsRepository = dahSpecsRepository;
        _cacheManager = cacheManager;
        _nodeManager = nodeManager;
        _conditionManager = conditionManager;
        _mjjManager = mjjManager;
        _processManager = processManager;
        _plcHelper = plcHelper;
    }

    /// <summary>
    /// 龙门到指定的取档口取放档案时，相应的密集架避让配置是否正确，并输出正确的避让列和左右值
    /// </summary>
    /// <param name="doorCode">取档口</param>
    /// <param name="mjjCol">密集架避让列</param>
    /// <param name="zyNo">密集架避让列左右值</param>
    /// <param name="failedReason">获取失败时的原因</param>
    /// <returns>true：获取成功，false：获取失败</returns>
    public bool GetMjjAvoidLmPosAboutDoorCode(string doorCode, out byte mjjCol, out byte zyNo, out string failedReason)
    {
        mjjCol = 255;
        zyNo = 255;
        failedReason = string.Empty;

        List<MjjAvoidPos> avoidPositions = _options.Value.MjjAvoidLmPos;
        foreach (MjjAvoidPos pos in avoidPositions)
        {
            if (pos.LmTarget == doorCode)
            {
                mjjCol = pos.MjjAvoidCol;
                zyNo = pos.MjjAvoidZY;
                break;
            }
        }

        if (mjjCol == 255 && zyNo == 255)
        {
            failedReason = $"未找到取档口{doorCode}对应的密集架避让位置配置";
            return false;
        }

        int mjjColCnt = _options.Value.MjjColCnt; //密集架列数
        if (mjjCol < 1 || mjjCol > mjjColCnt)
        {
            failedReason = $"密集架避让位置配置错误，避让列为{mjjCol}，不在有效范围1~{mjjColCnt}内";
            return false;
        }

        if (zyNo != 1 && zyNo != 2)
        {
            failedReason = $"密集架避让位置配置错误，避让列左右值为{zyNo}，不在有效范围1~2内";
            return false;
        }

        string fixCol = _options.Value.MjjFixColPos.ToLower(); //固定列位置
        if (fixCol == "left" && mjjCol == 1 && zyNo == 1 || fixCol == "right" && mjjCol == mjjColCnt && zyNo == 2)
        {
            failedReason = $"密集架避让位置配置错误，避让位在非中间固定的固定列外侧，密集架无法移动来避让龙门，若避让位确实在固定列外侧，请配置密集架不需要避让龙门";
            return false;
        }
        return true;
    }

    /// <summary>
    /// 查询指定Job所在调度任务的龙门避让位
    /// </summary>
    /// <param name="job"></param>
    /// <param name="mjjCol"></param>
    /// <param name="zyNo"></param>
    /// <param name="failedReason"></param>
    /// <returns></returns>
    public bool GetMjjAvoidLmPosAboutJob(DispatchJob job, out byte mjjCol, out byte zyNo, out string failedReason)
    {
        mjjCol = 255;
        zyNo = 255;
        failedReason = string.Empty;

        DispatchProcess process = _processManager.GetDispatchProcessAsync(job.ProcessId).Result;
        if (process == null)
        {
            failedReason = $"当前job所属过程信息查询失败";
            return false;
        }

        if (process.StartNodeCode.StartsWith("12") && process.EndNodeCode.StartsWith("12"))
        {
            failedReason = $"当前job所属过程起止点都是取档口，无法确定避让位置";
            return false;
        }

        string doorNode = string.Empty;
        if (process.StartNodeCode.StartsWith("12"))
            doorNode = process.StartNodeCode;
        if (process.EndNodeCode.StartsWith("12"))
            doorNode = process.EndNodeCode;

        if (doorNode == string.Empty)
        {
            failedReason = $"当前job所属过程起止点都不是取档口，无法确定避让位置";
            return false;
        }

        List<MjjAvoidPos> avoidPositions = _options.Value.MjjAvoidLmPos;
        foreach (MjjAvoidPos pos in avoidPositions)
        {
            if (pos.LmTarget == doorNode)
            {
                mjjCol = pos.MjjAvoidCol;
                zyNo = pos.MjjAvoidZY;
                break;
            }
        }

        if (mjjCol == 255 && mjjCol == 255)
        {
            failedReason = $"未找到取档口{doorNode}对应的密集架避让位置配置";
            return false;
        }

        int mjjColCnt = _options.Value.MjjColCnt;
        if (mjjCol < 1 || mjjCol > mjjColCnt)
        {
            failedReason = $"密集架避让位置配置错误，目标列为{mjjCol}，不在有效范围1~{mjjColCnt}内";
            return false;
        }

        if (zyNo != 1 && zyNo != 2)
        {
            failedReason = $"密集架避让位置配置错误，避让列左右值为{zyNo}，不在有效范围1~2内";
            return false;
        }

        string fixCol = _options.Value.MjjFixColPos.ToLower(); //固定列位置
        if (fixCol == "left" && mjjCol == 1 && zyNo == 1 || fixCol == "right" && mjjCol == mjjColCnt && zyNo == 2)
        {
            failedReason = $"密集架避让位置配置错误，避让位在非中间固定的固定列外侧，密集架无法移动来避让龙门，若避让位确实在固定列外侧，请配置密集架不需要避让龙门";
            return false;
        }

        return true;
    }

    /// <summary>
    /// 获取入库任务目标库位的电气定义的排列层及取档口
    /// </summary>
    /// <param name="taskId"></param>
    /// <param name="row"></param>
    /// <param name="layer"></param>
    /// <param name="sectNo"></param>
    /// <param name="colNoInSect"></param>
    /// <param name="doorNo"></param>
    /// <param name="failedReason"></param>
    /// <returns></returns>
    public bool GetPlcCellXYZOfStockInTask(
        int taskId,
        out ushort row,
        out ushort layer,
        out ushort sectNo,
        out ushort colNoInSect,
        out int cellSpecsValue,
        out ushort doorNo,
        out string failedReason)
    {
        row = 0;
        layer = 0;
        sectNo = 0;
        colNoInSect = 0;
        cellSpecsValue = 0;
        doorNo = 0;
        failedReason = string.Empty;

        try
        {
            DispatchTask task = _taskManager.GetDispatchTaskByTaskIdAsync(taskId).Result;
            if (task == null)
            {
                failedReason = $"根据调度任务Id({taskId})查询调度任务信息失败";
                return false;
            }

            string startNodeCode = task.StartNode;  //入库任务，起始设备不是库位
            string endNodeCode = task.EndNode;      //入库任务，终止设备为库位

            if (!ushort.TryParse(startNodeCode, out doorNo))
            {
                failedReason = $"入库任务的起始设备非库位，设备码可以转换成整型，但该任务起始设备为{startNodeCode}，无法转换成整型";
                return false;
            }

            string[] sections = endNodeCode.Split("-");
            if (sections.Length != 3)
            {
                failedReason = $"入库任务的终止设备为库位，设备码格式应为zz-xx-yy，但该任务终止设备为{endNodeCode}，格式错误";
                return false;
            }

            if (!ushort.TryParse(sections[0], out ushort r))
            {
                failedReason = $"入库任务目标库位为{endNodeCode}，参数排{sections[0]}无法转换成整数";
                return false;
            }

            if (!ushort.TryParse(sections[1], out ushort c))
            {
                failedReason = $"入库任务目标库位为{endNodeCode}，参数列{sections[1]}无法转换成整数";
                return false;
            }

            if (!ushort.TryParse(sections[2], out ushort l))
            {
                failedReason = $"入库任务目标库位为{endNodeCode}，参数层{sections[2]}无法转换成整数";
                return false;
            }

            DispatchCell cell = _cellRepository.FindByWmsCellXYZAsync(r, c, l).Result;
            if (cell == null)
            {
                failedReason = $"入库任务{taskId}的目标库位为{endNodeCode}，但此库位未定义";
                return false;
            }

            DahSpecs specs = _dahSpecsRepository.FindBySpecsCodeAsync(cell.CellSpecs).Result;
            if (specs == null)
            {
                failedReason = $"入库任务{taskId}的目标库位的规格{cell.CellSpecs}不存在";
                return false;
            }

            row = (ushort)cell.RowForPlc;
            layer = (ushort)cell.LayerForPlc;
            sectNo = (ushort)cell.SectNoForPlc;
            colNoInSect = (ushort)cell.ColNoInSectForPlc;
            cellSpecsValue = specs.SpecValue;

            return true;
        }
        catch (Exception ex)
        {
            failedReason = $"查询入库任务的PLC目标库位失败：{ex.Message}";
            return false;
        }
    }

    /// <summary>
    /// 获取移库任务起始库位的电气定义的排列层及取档口
    /// </summary>
    /// <param name="taskId"></param>
    /// <param name="row"></param>
    /// <param name="layer"></param>
    /// <param name="sectNo"></param>
    /// <param name="colNoInSect"></param>
    /// <param name="doorNo"></param>
    /// <param name="failedReason"></param>
    /// <returns></returns>
    public bool GetSrcPlcCellXYZOfMoveTask(
        int taskId,
        out ushort row,
        out ushort layer,
        out ushort sectNo,
        out ushort colNoInSect,
        out int cellSpecsValue,
        out string failedReason)
    {
        row = 0;
        layer = 0;
        sectNo = 0;
        colNoInSect = 0;
        cellSpecsValue = 0;
        failedReason = string.Empty;

        try
        {
            DispatchTask task = _taskManager.GetDispatchTaskByTaskIdAsync(taskId).Result;
            if (task == null)
            {
                failedReason = $"根据调度任务Id({taskId})查询调度任务信息失败";
                return false;
            }

            string startNodeCode = task.StartNode;  //移库任务，起始设备是库位
            string endNodeCode = task.EndNode;      //移库任务，终止设备是库位

            string[] sections = startNodeCode.Split("-");
            if (sections.Length != 3)
            {
                failedReason = $"移库任务的起始设备为库位，设备码格式应为zz-xx-yy，但该任务起始设备为{startNodeCode}，格式错误";
                return false;
            }

            if (!ushort.TryParse(sections[0], out ushort r))
            {
                failedReason = $"移库任务起始库位为{startNodeCode}，参数排{sections[0]}无法转换成整数";
                return false;
            }

            if (!ushort.TryParse(sections[1], out ushort c))
            {
                failedReason = $"移库任务起始库位为{startNodeCode}，参数列{sections[1]}无法转换成整数";
                return false;
            }

            if (!ushort.TryParse(sections[2], out ushort l))
            {
                failedReason = $"移库任务起始库位为{startNodeCode}，参数层{sections[2]}无法转换成整数";
                return false;
            }

            DispatchCell cell = _cellRepository.FindByWmsCellXYZAsync(r, c, l).Result;
            if (cell == null)
            {
                failedReason = $"移库任务{taskId}的起始库位为{startNodeCode}，但此库位未定义";
                return false;
            }

            DahSpecs specs = _dahSpecsRepository.FindBySpecsCodeAsync(cell.CellSpecs).Result;
            if (specs == null)
            {
                failedReason = $"移库任务{taskId}的起始库位的规格{cell.CellSpecs}不存在";
                return false;
            }

            row = (ushort)cell.RowForPlc;
            layer = (ushort)cell.LayerForPlc;
            sectNo = (ushort)cell.SectNoForPlc;
            colNoInSect = (ushort)cell.ColNoInSectForPlc;
            cellSpecsValue = specs.SpecValue;

            return true;
        }
        catch (Exception ex)
        {
            failedReason = $"查询移库任务的起始PLC库位失败：{ex.Message}";
            return false;
        }
    }

    /// <summary>
    /// 获取移库任务终止库位的电气定义的排列层及取档口
    /// </summary>
    /// <param name="taskId"></param>
    /// <param name="row"></param>
    /// <param name="layer"></param>
    /// <param name="sectNo"></param>
    /// <param name="colNoInSect"></param>
    /// <param name="doorNo"></param>
    /// <param name="failedReason"></param>
    /// <returns></returns>
    public bool GetTgtPlcCellXYZOfMoveTask(
        int taskId,
        out ushort row,
        out ushort layer,
        out ushort sectNo,
        out ushort colNoInSect,
        out int cellSpecsValue,
        out string failedReason)
    {
        row = 0;
        layer = 0;
        sectNo = 0;
        colNoInSect = 0;
        cellSpecsValue = 0;
        failedReason = string.Empty;

        try
        {
            DispatchTask task = _taskManager.GetDispatchTaskByTaskIdAsync(taskId).Result;
            if (task == null)
            {
                failedReason = $"根据调度任务Id({taskId})查询调度任务信息失败";
                return false;
            }

            string startNodeCode = task.StartNode;  //移库任务，起始设备是库位
            string endNodeCode = task.EndNode;      //移库任务，终止设备是库位

            string[] sections = endNodeCode.Split("-");
            if (sections.Length != 3)
            {
                failedReason = $"移库任务的终止设备为库位，设备码格式应为zz-xx-yy，但该任务终止设备为{endNodeCode}，格式错误";
                return false;
            }

            if (!ushort.TryParse(sections[0], out ushort r))
            {
                failedReason = $"移库任务终止库位为{endNodeCode}，参数排{sections[0]}无法转换成整数";
                return false;
            }

            if (!ushort.TryParse(sections[1], out ushort c))
            {
                failedReason = $"移库任务终止库位为{endNodeCode}，参数列{sections[1]}无法转换成整数";
                return false;
            }

            if (!ushort.TryParse(sections[2], out ushort l))
            {
                failedReason = $"移库任务终止库位为{endNodeCode}，参数层{sections[2]}无法转换成整数";
                return false;
            }

            DispatchCell cell = _cellRepository.FindByWmsCellXYZAsync(r, c, l).Result;
            if (cell == null)
            {
                failedReason = $"移库任务{taskId}的终止库位为{endNodeCode}，但此库位未定义";
                return false;
            }

            DahSpecs specs = _dahSpecsRepository.FindBySpecsCodeAsync(cell.CellSpecs).Result;
            if (specs == null)
            {
                failedReason = $"移库任务{taskId}的终止库位的规格{cell.CellSpecs}不存在";
                return false;
            }

            row = (ushort)cell.RowForPlc;
            layer = (ushort)cell.LayerForPlc;
            sectNo = (ushort)cell.SectNoForPlc;
            colNoInSect = (ushort)cell.ColNoInSectForPlc;
            cellSpecsValue = specs.SpecValue;

            return true;
        }
        catch (Exception ex)
        {
            failedReason = $"查询移库任务的终止PLC库位失败：{ex.Message}";
            return false;
        }
    }

    /// <summary>
    /// 获取入库任务目标库位对应的密集架位置
    /// </summary>
    /// <param name="taskId"></param>
    /// <param name="mjjColNo"></param>
    /// <param name="mjjZYNo"></param>
    /// <param name="failedReason"></param>
    /// <returns></returns>
    public bool GetMjjTargetPosOfStockInTask(int taskId, out byte mjjColNo, out byte mjjZYNo, out string failedReason)
    {
        mjjColNo = 255;
        mjjZYNo = 255;
        failedReason = string.Empty;

        try
        {
            DispatchTask task = _taskManager.GetDispatchTaskByTaskIdAsync(taskId).Result;
            if (task == null)
            {
                failedReason = $"根据调度任务Id({taskId})查询调度任务信息失败";
                return false;
            }

            string endNodeCode = task.EndNode;      //入库任务，终止设备为库位

            string[] sections = endNodeCode.Split("-");
            if (sections.Length != 3)
            {
                failedReason = $"入库任务的终止设备为库位，设备码格式应为zz-xx-yy，但该任务终止设备为{endNodeCode}，格式错误";
                return false;
            }

            if (!ushort.TryParse(sections[0], out ushort row))
            {
                failedReason = $"入库任务目标库位为{endNodeCode}，参数排{sections[0]}无法转换成整数";
                return false;
            }

            if (!ushort.TryParse(sections[1], out ushort col))
            {
                failedReason = $"入库任务目标库位为{endNodeCode}，参数列{sections[1]}无法转换成整数";
                return false;
            }

            if (!ushort.TryParse(sections[2], out ushort layer))
            {
                failedReason = $"入库任务目标库位为{endNodeCode}，参数层{sections[2]}无法转换成整数";
                return false;
            }


            DispatchCell cell = _cellRepository.FindByWmsCellXYZAsync(row, col, layer).Result;
            if (cell == null)
            {
                failedReason = $"入库任务{taskId}的目标库位为{endNodeCode}，但此库位未定义";
                return false;
            }

            mjjColNo = (byte)_mjjManager.GetMjjColFromWmsCellRow(cell.Row);
            mjjZYNo = (byte)_mjjManager.GetMjjZYNoFromCellRow(cell.Row);

            int mjjColCnt = _options.Value.MjjColCnt;
            if (mjjColNo < 1 || mjjColNo > mjjColCnt)
            {
                failedReason = $"算得的密集架列为{mjjColNo}，不在有效范围1~{mjjColCnt}内";
                return false;
            }

            if (mjjZYNo != 1 && mjjZYNo != 2)
            {
                failedReason = $"算得的密集架左右值为{mjjZYNo}，该值无效，应为1：左，2：右";
                return false;
            }

            string fixCol = _options.Value.MjjFixColPos.ToLower();
            bool fixColAvailable = _options.Value.MjjFixColAvailable;
            if (fixCol == "left" && mjjColNo == 1 && mjjZYNo == 1 && !fixColAvailable)
            {
                failedReason = $"密集架为左固定，且第1列左侧不能使用，算得的密集架列：{mjjColNo}，左右值：{mjjZYNo}，不能到达";
                return false;
            }

            if (fixCol == "right" && mjjColNo == mjjColCnt && mjjZYNo == 2 && !fixColAvailable)
            {
                failedReason = $"密集架为右固定，且第{mjjColNo}列右侧不能使用，算得的密集架列：{mjjColNo}，左右值：{mjjZYNo}，不能到达";
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            failedReason = $"查询入库任务的密集架打开位置失败：{ex.Message}";
            return false;
        }
    }

    /// <summary>
    /// 获取出库任务目标库位的电气定义的排列层以及取档口
    /// </summary>
    /// <param name="taskId"></param>
    /// <param name="row"></param>
    /// <param name="layer"></param>
    /// <param name="sectNo"></param>
    /// <param name="colNoInSect"></param>
    /// <param name="doorNo"></param>
    /// <param name="failedReason"></param>
    /// <returns></returns>
    public bool GetPlcCellXYZOfStockOutTask(
        int taskId,
        out ushort row,
        out ushort layer,
        out ushort sectNo,
        out ushort colNoInSect,
        out int cellSpecsValue,
        out ushort doorNo,
        out string failedReason)
    {
        row = 0;
        layer = 0;
        sectNo = 0;
        colNoInSect = 0;
        cellSpecsValue = 0;
        doorNo = 0;
        failedReason = string.Empty;

        try
        {
            DispatchTask task = _taskManager.GetDispatchTaskByTaskIdAsync(taskId).Result;
            if (task == null)
            {
                failedReason = $"根据调度任务Id({taskId})查询调度任务信息失败";
                return false;
            }

            string startNodeCode = task.StartNode;  //出库任务，起始设备为库位
            string endNodeCode = task.EndNode;      //出库任务，终止设备不是库位

            if (!ushort.TryParse(endNodeCode, out doorNo))
            {
                failedReason = $"出库任务的终止设备非库位，设备码可以转换成整型，但该任务终止设备为{endNodeCode}，无法转换成整型";
                return false;
            }

            string[] sections = startNodeCode.Split("-");
            if (sections.Length != 3)
            {
                failedReason = $"出库任务的起始设备为库位，设备码格式应为zz-xx-yy，但该任务起始设备为{startNodeCode}，格式错误";
                return false;
            }

            if (!ushort.TryParse(sections[0], out ushort r))
            {
                failedReason = $"出库任务起始库位为{startNodeCode}，参数排{sections[0]}无法转换成整数";
                return false;
            }

            if (!ushort.TryParse(sections[1], out ushort c))
            {
                failedReason = $"出库任务起始库位为{startNodeCode}，参数列{sections[1]}无法转换成整数";
                return false;
            }

            if (!ushort.TryParse(sections[2], out ushort l))
            {
                failedReason = $"出库任务起始库位为{startNodeCode}，参数层{sections[2]}无法转换成整数";
                return false;
            }

            DispatchCell cell = _cellRepository.FindByWmsCellXYZAsync(r, c, l).Result;
            if (cell == null)
            {
                failedReason = $"出库任务{taskId}的起始库位为{startNodeCode}，但此库位未定义";
                return false;
            }

            DahSpecs specs = _dahSpecsRepository.FindBySpecsCodeAsync(cell.CellSpecs).Result;
            if (specs == null)
            {
                failedReason = $"出库任务{taskId}的起始库位的规格{cell.CellSpecs}不存在";
                return false;
            }

            row = (ushort)cell.RowForPlc;
            layer = (ushort)cell.LayerForPlc;
            sectNo = (ushort)cell.SectNoForPlc;
            colNoInSect = (ushort)cell.ColNoInSectForPlc;
            cellSpecsValue = specs.SpecValue;

            return true;
        }
        catch (Exception ex)
        {
            failedReason = $"查询出库任务的PLC目标库位信息失败：{ex.Message}";
            return false;
        }
    }

    /// <summary>
    /// 获取出库任务目标库位对应的密集架位置
    /// </summary>
    /// <param name="taskId"></param>
    /// <param name="mjjColNo"></param>
    /// <param name="mjjZYNo"></param>
    /// <param name="failedReason"></param>
    /// <returns></returns>
    public bool GetMjjTargetPosOfStockOutTask(int taskId, out byte mjjColNo, out byte mjjZYNo, out string failedReason)
    {
        mjjColNo = 255;
        mjjZYNo = 255;
        failedReason = string.Empty;

        try
        {
            DispatchTask task = _taskManager.GetDispatchTaskByTaskIdAsync(taskId).Result;
            if (task == null)
            {
                failedReason = $"根据调度任务Id({taskId})查询调度任务信息失败";
                return false;
            }

            string startNodeCode = task.StartNode;      //出库任务，起始设备为库位

            string[] sections = startNodeCode.Split("-");
            if (sections.Length != 3)
            {
                failedReason = $"出库任务的起始设备为库位，设备码格式应为zz-xx-yy，但该任务起始设备为{startNodeCode}，格式错误";
                return false;
            }

            if (!ushort.TryParse(sections[0], out ushort row))
            {
                failedReason = $"出库任务目标库位为{startNodeCode}，参数排{sections[0]}无法转换成整数";
                return false;
            }

            if (!ushort.TryParse(sections[1], out ushort col))
            {
                failedReason = $"出库任务目标库位为{startNodeCode}，参数列{sections[1]}无法转换成整数";
                return false;
            }

            if (!ushort.TryParse(sections[2], out ushort layer))
            {
                failedReason = $"出库任务目标库位为{startNodeCode}，参数层{sections[2]}无法转换成整数";
                return false;
            }

            DispatchCell cell = _cellRepository.FindByWmsCellXYZAsync(row, col, layer).Result;
            if (cell == null)
            {
                failedReason = $"出库任务{taskId}的目标库位为{startNodeCode}，但此库位未定义";
                return false;
            }

            mjjColNo = (byte)_mjjManager.GetMjjColFromWmsCellRow(cell.Row);
            mjjZYNo = (byte)_mjjManager.GetMjjZYNoFromCellRow(cell.Row);

            int mjjColCnt = _options.Value.MjjColCnt;
            if (mjjColNo < 1 || mjjColNo > mjjColCnt)
            {
                failedReason = $"算得的密集架列为{mjjColNo}，不在有效范围1~{mjjColCnt}内";
                return false;
            }

            if (mjjZYNo != 1 && mjjZYNo != 2)
            {
                failedReason = $"算得的密集架左右值为{mjjZYNo}，该值无效，应为1：左，2：右";
                return false;
            }

            string fixCol = _options.Value.MjjFixColPos.ToLower();
            bool fixColAvailable = _options.Value.MjjFixColAvailable;
            if (fixCol == "left" && mjjColNo == 1 && mjjZYNo == 1 && !fixColAvailable)
            {
                failedReason = $"密集架为左固定，且第1列左侧不能使用，算得的密集架列：{mjjColNo}，左右值：{mjjZYNo}，不能到达";
                return false;
            }

            if (fixCol == "right" && mjjColNo == mjjColCnt && mjjZYNo == 2 && !fixColAvailable)
            {
                failedReason = $"密集架为右固定，且第{mjjColNo}列右侧不能使用，算得的密集架列：{mjjColNo}，左右值：{mjjZYNo}，不能到达";
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            failedReason = $"查询出库任务的密集架打开位置失败：{ex.Message}";
            return false;
        }
    }

    /// <summary>
    /// 获取盘点任务的PLC定义的排列层
    /// </summary>
    /// <param name="taskId"></param>
    /// <param name="startRow"></param>
    /// <param name="col"></param>
    /// <param name="startLayer"></param>
    /// <param name="failedReason"></param>
    /// <returns></returns>
    public bool GetPlcCellXYZOfCheckTask(
        int taskId,
        out ushort startRow,
        out ushort startLayer,
        out ushort startSectNo,
        out ushort startColNoInSect,
        out int startCellSpecsVal,
        out ushort endRow,
        out ushort endLayer,
        out ushort endSectNo,
        out ushort endColNoInSect,
        out int endCellSpecsVal,
        out string failedReason)
    {
        startRow = 0;
        startLayer = 0;
        startSectNo = 0;
        startColNoInSect = 0;
        startCellSpecsVal = 0;
        endRow = 0;
        endLayer = 0;
        endSectNo = 0;
        endColNoInSect = 0;
        endCellSpecsVal = 0;
        failedReason = string.Empty;

        try
        {
            DispatchTask task = _taskManager.GetDispatchTaskByTaskIdAsync(taskId).Result;
            if (task == null)
            {
                failedReason = $"根据调度任务Id({taskId})查询调度任务信息失败";
                return false;
            }

            string startNodeCode = task.StartNode;  //盘点任务，起始设备为库位
            string endNodeCode = task.EndNode;      //盘点任务，终止设备为库位

            string[] sections = startNodeCode.Split("-");
            if (sections.Length != 3)
            {
                failedReason = $"盘点任务的起点设备为库位，设备码格式应为zz-xx-yy，但该任务起点设备为{startNodeCode}，格式错误";
                return false;
            }

            if (!ushort.TryParse(sections[0], out ushort sr))
            {
                failedReason = $"盘点任务起点库位为{startNodeCode}，参数排{sections[0]}无法转换成整数";
                return false;
            }

            if (!ushort.TryParse(sections[1], out ushort sc))
            {
                failedReason = $"盘点任务起点库位为{startNodeCode}，参数列{sections[1]}无法转换成整数";
                return false;
            }

            if (!ushort.TryParse(sections[2], out ushort sl))
            {
                failedReason = $"盘点任务起点库位为{startNodeCode}，参数层{sections[2]}无法转换成整数";
                return false;
            }

            DispatchCell cell = _cellRepository.FindByWmsCellXYZAsync(sr, sc, sl).Result;
            if (cell == null)
            {
                failedReason = $"盘点任务{taskId}的起点库位为{startNodeCode}，但此库位未定义";
                return false;
            }

            DahSpecs specs = _dahSpecsRepository.FindBySpecsCodeAsync(cell.CellSpecs).Result;
            if (specs == null)
            {
                failedReason = $"盘点任务{taskId}的起点库位的规格{cell.CellSpecs}不存在";
                return false;
            }

            startRow = (ushort)cell.RowForPlc;
            startLayer = (ushort)cell.LayerForPlc;
            startSectNo = (ushort)cell.SectNoForPlc;
            startColNoInSect = (ushort)cell.ColNoInSectForPlc;
            startCellSpecsVal = specs.SpecValue;

            sections = endNodeCode.Split("-");
            if (sections.Length != 3)
            {
                failedReason = $"盘点任务的终点设备为库位，设备码格式应为zz-xx-yy，但该任务终点设备为{endNodeCode}，格式错误";
                return false;
            }

            if (!ushort.TryParse(sections[0], out ushort er))
            {
                failedReason = $"盘点任务终点库位为{endNodeCode}，参数排{sections[0]}无法转换成整数";
                return false;
            }

            if (!ushort.TryParse(sections[1], out ushort ec))
            {
                failedReason = $"盘点任务终点库位为{endNodeCode}，参数列{sections[1]}无法转换成整数";
                return false;
            }

            if (!ushort.TryParse(sections[2], out ushort el))
            {
                failedReason = $"盘点任务终点库位为{endNodeCode}，参数层{sections[2]}无法转换成整数";
                return false;
            }

            cell = _cellRepository.FindByWmsCellXYZAsync(er, ec, el).Result;
            if (cell == null)
            {
                failedReason = $"盘点任务{taskId}的终点库位为{endNodeCode}，但此库位未定义";
                return false;
            }

            specs = _dahSpecsRepository.FindBySpecsCodeAsync(cell.CellSpecs).Result;
            if (specs == null)
            {
                failedReason = $"盘点任务{taskId}的终点库位的规格{cell.CellSpecs}不存在";
                return false;
            }

            endRow = (ushort)cell.RowForPlc;
            endLayer = (ushort)cell.LayerForPlc;
            endSectNo = (ushort)cell.SectNoForPlc;
            endColNoInSect = (ushort)cell.ColNoInSectForPlc;
            endCellSpecsVal = specs.SpecValue;

            return true;
        }
        catch (Exception ex)
        {
            failedReason = $"查询盘点任务的PLC起止库位失败：{ex.Message}";
            return false;
        }
    }

    /// <summary>
    /// 获取盘点任务的密集架目标位置
    /// </summary>
    /// <param name="taskId"></param>
    /// <param name="mjjColNo"></param>
    /// <param name="mjjZYNo"></param>
    /// <param name="failedReason"></param>
    /// <returns></returns>
    public bool GetMjjTargetPosOfCheckTask(int taskId, out byte mjjColNo, out byte mjjZYNo, out string failedReason)
    {
        mjjColNo = 255;
        mjjZYNo = 255;
        failedReason = string.Empty;

        try
        {
            DispatchTask task = _taskManager.GetDispatchTaskByTaskIdAsync(taskId).Result;
            if (task == null)
            {
                failedReason = $"根据调度任务Id({taskId})查询调度任务信息失败";
                return false;
            }

            string nodeCode = task.EndNode;      //盘点任务，起始设备和终止设备在同一排的同一列

            string[] sections = nodeCode.Split("-");
            if (sections.Length != 3)
            {
                failedReason = $"盘点任务的目标设备为库位，设备码格式应为zz-xx-yy，但该任务目标设备为{nodeCode}，格式错误";
                return false;
            }

            if (!ushort.TryParse(sections[0], out ushort row))
            {
                failedReason = $"盘点任务目标库位为{nodeCode}，参数排{sections[0]}无法转换成整数";
                return false;
            }

            if (!ushort.TryParse(sections[1], out ushort col))
            {
                failedReason = $"盘点任务目标库位为{nodeCode}，参数列{sections[1]}无法转换成整数";
                return false;
            }

            if (!ushort.TryParse(sections[2], out ushort layer))
            {
                failedReason = $"盘点任务目标库位为{nodeCode}，参数层{sections[2]}无法转换成整数";
                return false;
            }

            DispatchCell cell = _cellRepository.FindByWmsCellXYZAsync(row, col, layer).Result;
            if (cell == null)
            {
                failedReason = $"盘点任务{taskId}的目标库位为{nodeCode}，但此库位未定义";
                return false;
            }

            mjjColNo = (byte)_mjjManager.GetMjjColFromWmsCellRow(cell.Row);
            mjjZYNo = (byte)_mjjManager.GetMjjZYNoFromCellRow(cell.Row);

            int mjjColCnt = _options.Value.MjjColCnt;
            if (mjjColNo < 1 || mjjColNo > mjjColCnt)
            {
                failedReason = $"算得的密集架列为{mjjColNo}，不在有效范围1~{mjjColCnt}内";
                return false;
            }

            if (mjjZYNo != 1 && mjjZYNo != 2)
            {
                failedReason = $"算得的密集架左右值为{mjjZYNo}，该值无效，应为1：左，2：右";
                return false;
            }

            string fixCol = _options.Value.MjjFixColPos.ToLower();
            bool fixColAvailable = _options.Value.MjjFixColAvailable;
            if (fixCol == "left" && mjjColNo == 1 && mjjZYNo == 1 && !fixColAvailable)
            {
                failedReason = $"密集架为左固定，且第1列左侧不能使用，算得的密集架列：{mjjColNo}，左右值：{mjjZYNo}，不能到达";
                return false;
            }

            if (fixCol == "right" && mjjColNo == mjjColCnt && mjjZYNo == 2 && !fixColAvailable)
            {
                failedReason = $"密集架为右固定，且第{mjjColNo}列右侧不能使用，算得的密集架列：{mjjColNo}，左右值：{mjjZYNo}，不能到达";
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            failedReason = $"查询盘点任务的密集架打开位置失败：{ex.Message}";
            return false;
        }
    }

    /// <summary>
    /// 获取被指定调度任务占用的缓存位
    /// </summary>
    /// <param name="job">调度任务Job</param>
    /// <param name="cachePos"></param>
    /// <param name="failedReason"></param>
    /// <returns></returns>
    public bool GetCacheOccupiedByJob(DispatchJob job, out byte cachePos, out string failedReason)
    {
        cachePos = 255;
        failedReason = string.Empty;

        DispatchOrder order = _orderManager.GetDispatchOrderByOrderCodeAsync(job.OrderCode).GetAwaiter().GetResult();
        if (order == null)
        {
            failedReason = $"根据订单号{job.OrderCode}，查询不到对应的订单信息";
            return false;
        }

        if (string.IsNullOrEmpty(order.PlateSpecs))
        {
            failedReason = $"根据订单号{job.OrderCode}查询到的订单没有携带档案盒规格信息";
            return false;
        }

        DispatchCache cache;
        bool r = _cacheManager.GetCacheByTaskId(job.TaskId, out cache);
        if (!r)
        {
            failedReason = $"根据调度任务Id:{job.TaskId}，查询缓存位失败";
            return false;
        }

        if (cache == null)
        {
            //failedReason = $"根据调度任务Id:{job.TaskId}，查询不到占用的缓存位";
            //return false;
            cachePos = 0;  //存在不分配缓存位的情况，表示不经过缓存位，直接进行出入库或移库
            return true; 
        }

        if (cache.Specs != order.PlateSpecs)
        {
            failedReason = $"被调度任务Id:{job.TaskId}占用的{cache.CachePos}号缓存位规格为{cache.Specs}，与对应订单要求的规格{order.PlateSpecs}不一致";
            return false;
        }

        cachePos = cache.CachePos;
        return true;
    }

    /// <summary>
    /// 告知PLC龙门不允许进入密集架
    /// </summary>
    /// <param name="failedReason"></param>
    /// <returns></returns>
    public bool TellPlcAboutLmNotAllowedToMjj(out string failedReason)
    {
        failedReason = string.Empty;

        var result = _plcHelper.ReadPlcTag("Plc1", "Mjj_SafePos");
        if (result == null || result.Quality == EnumQuality.Bad)
        {
            failedReason = $"读取不到变量Plc1.Mjj_SafePos";
            return false;
        }

        if (result.Value == "0") //变量Plc1.Mjj_SafePos已经为不允许龙门进入密集架
            return true;

        if (false == _plcHelper.WritePlcTag("Plc1", "Mjj_SafePos", "0"))
        {
            failedReason = "告知PLC龙门不允许进入密集架失败";
            return false;
        }

        //等待5ms
        Thread.Sleep(2);

        //检测 告知PLC，龙门不允许进入密集架 是否成功
        result = _plcHelper.ReadPlcTag("Plc1", "Mjj_SafePos");
        if (result == null || result.Quality == EnumQuality.Bad)
        {
            failedReason = $"检查是否成功告知PLC龙门不允许进入密集架失败：读取不到变量Plc1.Mjj_SafePos";
            return false;
        }

        if (result.Value != "0")
        {
            failedReason = $"检查到未成功告知PLC龙门不允许进入密集架";
            return false;
        }

        return true;
    }

    /// <summary>
    /// 告知PLC龙门允许进入密集架
    /// </summary>
    /// <param name="failedReason"></param>
    /// <returns></returns>
    public bool TellPlcAboutLmAllowedToMjj(out string failedReason)
    {
        failedReason = string.Empty;

        var result = _plcHelper.ReadPlcTag("Plc1", "Mjj_SafePos");
        if (result == null || result.Quality == EnumQuality.Bad)
        {
            failedReason = $"读取不到变量Plc1.Mjj_SafePos";
            return false;
        }

        if (result.Value == "1") //变量Plc1.Mjj_SafePos已经为允许龙门进入密集架
            return true;

        if (false == _plcHelper.WritePlcTag("Plc1", "Mjj_SafePos", "1"))
        {
            failedReason = "告知PLC龙门允许进入密集架失败";
            return false;
        }

        //等待5ms
        Thread.Sleep(2);

        //检测 告知PLC，龙门不允许进入密集架 是否成功
        result = _plcHelper.ReadPlcTag("Plc1", "Mjj_SafePos");
        if (result == null || result.Quality == EnumQuality.Bad)
        {
            failedReason = $"检查是否成功告知PLC龙门允许进入密集架失败：读取不到变量Plc1.Mjj_SafePos";
            return false;
        }

        if (result.Value != "1")
        {
            failedReason = $"检查到未成功告知PLC龙门允许进入密集架";
            return false;
        }

        return true;
    }

    /// <summary>
    /// 获取指定设备节点的指令地址
    /// </summary>
    /// <param name="nodeCode"></param>
    /// <param name="plcName"></param>
    /// <param name="tagName"></param>
    /// <param name="failedReason"></param>
    /// <returns></returns>
    public bool GetPlcCmdTagAddrOfNode(string nodeCode, out string plcName, out string tagName, out string failedReason)
    {
        plcName = string.Empty;
        tagName = string.Empty;
        failedReason = string.Empty;

        DispatchNode node = _nodeManager.GetNodeByNodeCodeAsync(nodeCode).Result;
        if (node == null)
        {
            failedReason = $"执行设备{nodeCode}不存在";
            return false;
        }

        string[] sects = node.CmdTagName.Split(".");
        if (sects.Length != 2 || sects[0] == "" || sects[1] == "")
        {
            failedReason = $"设备{node.NodeCode}的指令地址设置错误，应为\"plcName.tagName\"，但实际为{node.CmdTagName}";
            return false;
        }

        plcName = sects[0];
        tagName = sects[1];
        return true;
    }

    /// <summary>
    /// 获取指定设备节点的指令反馈地址
    /// </summary>
    /// <param name="nodeCode"></param>
    /// <param name="plcName"></param>
    /// <param name="tagName"></param>
    /// <param name="failedReason"></param>
    /// <returns></returns>
    public bool GetPlcResponseTagAddrOfNode(string nodeCode, out string plcName, out string tagName, out string failedReason)
    {
        plcName = string.Empty;
        tagName = string.Empty;
        failedReason = string.Empty;

        DispatchNode node = _nodeManager.GetNodeByNodeCodeAsync(nodeCode).Result;
        if (node == null)
        {
            failedReason = $"执行设备{nodeCode}不存在";
            return false;
        }

        string[] sects = node.ResponseTagName.Split(".");
        if (sects.Length != 2 || sects[0] == "" || sects[1] == "")
        {
            failedReason = $"设备{node.NodeCode}的反馈地址设置错误，应为\"plcName.tagName\"，但实际为{node.ResponseTagName}";
            return false;
        }

        plcName = sects[0];
        tagName = sects[1];
        return true;
    }

    /// <summary>
    /// 判断密集架知否在指定的位置
    /// </summary>
    /// <param name="targetCol"></param>
    /// <param name="targetZYNo"></param>
    /// <param name="failedReason">发生错误时，记录错误信息</param>
    /// <returns>true：在指定位置，false：不在指定位置，null：发生错误，无法判断结果</returns>
    public bool? IsMjjAtPosition(byte targetCol, byte targetZYNo, out string failedReason)
    {
        failedReason = string.Empty;

        string fixCol = _options.Value.MjjFixColPos.ToLower();  //密集架固定列位置
        bool fixColAvailable = _options.Value.MjjFixColAvailable;  //密集架固定列左右面是否都可用
        int mjjColCnt = _options.Value.MjjColCnt; //密集架的列数

        //当密集架为左固定或右固定，且密集架固定列左右面都可用时，目标位在密集架固定列外侧，无须移动密集架，龙门都可以进行取放料
        if (fixCol == "left" && targetCol == 1 && targetZYNo == 1 && fixColAvailable ||
            fixCol == "right" && targetCol == mjjColCnt && targetZYNo == 2 && fixColAvailable)
            return true;

        string colState = _conditionManager.GetConditionValueAsync("ColumnStatus").Result;

        if (string.IsNullOrEmpty(colState))
        {
            failedReason = "获取密集架列状态失败";
            return null;
        }

        if (colState.ToLower() == "error" || colState.ToLower() == "none")
        {
            failedReason = "密集架通讯异常，请检查密集架是否上电，以及密集架网络是否正常";
            return null;
        }

        MjjOpResult isAtPos = _mjjManager.IsMjjAtTargetPosition(targetCol, targetZYNo, colState);
        if (isAtPos.errMsg == null) return isAtPos.success;
        else
        {
            failedReason = $"无法判断密集架是否在目标位置（{targetCol}列，左右为{targetZYNo}）：{isAtPos.errMsg}";
            return null;
        }
    }

    /// <summary>
    /// 判断密集架是否在闭合位置
    /// </summary>
    /// <param name="failedReason">发生错误时，记录错误信息</param>
    /// <returns>true：在闭合位置，false：不在闭合位置，null：发生错误，无法判断结果</returns>
    public bool? IsMjjAtClosePos(out string failedReason)
    {
        failedReason = string.Empty;
        string colState = _conditionManager.GetConditionValueAsync("ColumnStatus").Result;

        if (string.IsNullOrEmpty(colState))
        {
            failedReason = "获取密集架列状态失败";
            return null;
        }

        if (colState.ToLower() == "error" || colState.ToLower() == "none")
        {
            failedReason = "密集架通讯异常，请检查密集架是否上电，以及密集架网络是否正常";
            return null;
        }

        MjjOpResult isAtPos = _mjjManager.IsMjjAtClosedPosition(colState);
        if (isAtPos.errMsg == null) return isAtPos.success;
        else
        {
            failedReason = $"无法判断密集架是否在闭合位置：{isAtPos.errMsg}";
            return false;
        }
    }
}