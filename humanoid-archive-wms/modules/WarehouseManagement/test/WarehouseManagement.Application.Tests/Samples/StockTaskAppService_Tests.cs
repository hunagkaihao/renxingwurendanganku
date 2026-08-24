using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Lion.AbpPro.ConfigurationOptions;
using Lion.AbpPro.Extension.Customs.Dtos;
using Lion.AbpPro.Extension.Customs.Http;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using Shouldly;
using WarehouseManagement.AgvTasks;
using WarehouseManagement.AgvTasks.Dto;
using WarehouseManagement.Cells;
using WarehouseManagement.StockTasks;
using WarehouseManagement.StockTasks.Dto;
using WarehouseManagement.StorageBoxs;
using WarehouseManagement.StorageBoxs.Dto;
using WarehouseManagement.TaskHiss;
using WarehouseManagement.Warehouses;
using Xunit;

namespace WarehouseManagement.Samples;

public class StockTaskAppService_Tests : WarehouseManagementApplicationTestBase
{
    private readonly IStockTaskAppService _sampleAppService;
    private readonly IAgvTaskAppService _agvTaskAppService;
    private readonly StorageBoxManager _storageBoxManagement;
    private readonly CellManager _cellManager;
    private readonly WarehouseManager _warehouseManager;
    private readonly AgvTaskManager _agvTaskManager;
    private readonly StockTaskManager _stockTaskManager;
    private readonly TaskHisManager _taskHisManager;
    



    //private readonly IHttpClientFactory _httpClientFactory;
    //private readonly AGVOptions _aGVOptions;

    public StockTaskAppService_Tests()
    {
        _storageBoxManagement = GetRequiredService<StorageBoxManager>();
        _cellManager = GetRequiredService<CellManager>();
        _warehouseManager = GetRequiredService<WarehouseManager>();
        _agvTaskManager = GetRequiredService<AgvTaskManager>();
        _agvTaskAppService = GetRequiredService<IAgvTaskAppService>();
        _stockTaskManager = GetRequiredService<StockTaskManager>();
        //_httpClientFactory = GetRequiredService<HttpClientHelper>();
        //_aGVOptions = GetRequiredService<AGVOptions>();
        InitContext();//初始化内存数据
        _sampleAppService = GetRequiredService<IStockTaskAppService>();
         _taskHisManager = GetRequiredService<TaskHisManager>();

    }

    private async void InitContext()
    {
        //初始化仓库
        await _warehouseManager.CreateAsync("CTU01","1#仓库","CTU");
        //初始化料箱
        {
            await _storageBoxManagement.CreateAsync("B1001", "1");
            await _storageBoxManagement.CreateAsync("B1002", "1");
            await _storageBoxManagement.CreateAsync("B1003", "1");
        }

        //初始化高位库位
        {
            await _cellManager.CreateAsync("01-01-01", "CTUCell", "01-01-01", 1);
            await _cellManager.CreateAsync("01-02-01", "CTUCell", "01-02-01", 1);
            await _cellManager.CreateAsync("01-01-02", "CTUCell", "01-01-02", 1);
            await _cellManager.CreateAsync("01-02-02", "CTUCell", "01-02-02", 1);
        }
        //初始化分拨墙库位
        {
            await _cellManager.CreateAsync("21-01-01", "WallCell", "21-01-01", 1);
            await _cellManager.CreateAsync("21-02-01", "WallCell", "21-02-01", 1);
            await _cellManager.CreateAsync("21-01-02", "WallCell", "21-01-02", 1);
            await _cellManager.CreateAsync("21-02-02", "WallCell", "21-02-02", 1);
        }
    }
    /// <summary>
    /// 入库任务创建测试[正常完成任务]
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task CreateStockInAsync()
    {

        CreateStockTaskDto input = new CreateStockTaskDto() { StorageBoxId=1,
            ManageTypeCode= "CTUNPFullStockIn",
            StartCellCode= "21-01-02",
            EndCellId=0
        };
        //step1  创建入库任务
        var result = await _sampleAppService.CreateCTUStockInAsync(input);
        result.Id.ShouldBe(1);
        result.ManageStatus.ShouldBe(ManageStatus.WaitingExecute);
        result.StartCellCode.ShouldBe("21-01-02");
        result.StockBarcode.ShouldBe("B1001");
        result.EndCellCode.ShouldNotBeNullOrEmpty();
        result.ManageTypeCode.ShouldBe(ManageType.CTUNPFullStockIn);
        IdIntInput idIntInput = new IdIntInput() { Id =1};
        //step2 将入库任务设置为执行
        var executResult = await _sampleAppService.SetAsExecutingAsync(idIntInput);
        executResult.ManageStatus.ShouldBe(ManageStatus.Executing);
        //有可能一个入库任务对应多个CTU任务
        var agvTaskResult =await _agvTaskManager.FindByStockTaskIdAsync(executResult.Id);
        agvTaskResult.BoxCode.ShouldBe(executResult.StockBarcode);
        agvTaskResult.Id.ShouldBe(1);
        agvTaskResult.TaskTyp.ShouldBe("CTUIn");
        agvTaskResult.AgvTaskStatus.ShouldBe(AgvTaskStatus.Executing);
        var startCell = await _cellManager.GetByIdAsync((int)(result.StartCellId));
        //检查起点库位状态是否正确
        startCell.RunStatus.ShouldBe(CellRunStatus.Selected);
        startCell.CellStatus.ShouldBe(CellStatus.Have);
        var endCell = await _cellManager.GetByIdAsync((int)(result.EndCellId));
        //检查终点库位状态是否正确
        endCell.RunStatus.ShouldBe(CellRunStatus.Selected);
        endCell.CellStatus.ShouldBe(CellStatus.Nohave);
        /// <summary>
        /// step3  测试AGV执行回调
        /// AGV任务设置已开始
        /// StockTask设置为已开始
        /// </summary>
        AgvCallBackRequest agvCallBackRequest = new AgvCallBackRequest() {
            Method= "taskStart",
            TaskCode= agvTaskResult.Id.ToString()
        };
        var ctuCallBackResult= await _agvTaskAppService.CtuCallbackAsync(agvCallBackRequest);
        ctuCallBackResult.Code.ShouldBe("0");
        var taskStartStockTask =await _stockTaskManager.FindByIdAsync(agvTaskResult.StockTaskId);
        taskStartStockTask.Id.ShouldBe(1);
        taskStartStockTask.ManageTypeCode.ShouldBe(ManageType.CTUNPFullStockIn);
        taskStartStockTask.ManageStatus.ShouldBe(ManageStatus.CTUTaskStart);
        var taskStartAgvTask = await _agvTaskManager.FindByIdAsync(agvTaskResult.Id);
        taskStartAgvTask.Id.ShouldBe(1);
        taskStartAgvTask.AgvTaskStatus.ShouldBe(AgvTaskStatus.TaskStart);
        /// <summary>
        /// step4  测试AGV执行回调
        /// AGV任务设置出储位
        /// StockTask设置为出储位
        /// </summary>
        AgvCallBackRequest cellOutAgvCallBackRequest = new AgvCallBackRequest()
        {
            Method = "cellOut",
            TaskCode = agvTaskResult.Id.ToString()
        };
        var cellOutCtuCallBackResult = await _agvTaskAppService.CtuCallbackAsync(cellOutAgvCallBackRequest);
        cellOutCtuCallBackResult.Code.ShouldBe("0");
        var cellOutStockTask = await _stockTaskManager.FindByIdAsync(agvTaskResult.StockTaskId);
        cellOutStockTask.Id.ShouldBe(1);
        cellOutStockTask.ManageTypeCode.ShouldBe(ManageType.CTUNPFullStockIn);
        cellOutStockTask.ManageStatus.ShouldBe(ManageStatus.CTUCellOut);
        var cellOutAgvTask = await _agvTaskManager.FindByIdAsync(agvTaskResult.Id);
        cellOutAgvTask.Id.ShouldBe(1);
        cellOutAgvTask.AgvTaskStatus.ShouldBe(AgvTaskStatus.CellOut);
        /// <summary>
        /// step4  测试AGV执行回调
        /// AGV任务设置已完成
        /// StockTask设置为已完成
        /// 更新出入库库位状态
        /// 更新料箱的库位
        /// 创建任务历史数据
        /// </summary>
        AgvCallBackRequest finishAgvCallBackRequest = new AgvCallBackRequest()
        {
            Method = "taskFinish",
            TaskCode = agvTaskResult.Id.ToString()
        };
        var finishCtuCallBackResult = await _agvTaskAppService.CtuCallbackAsync(finishAgvCallBackRequest);
        finishCtuCallBackResult.Code.ShouldBe("0");
        var finishStockTask = await _stockTaskManager.FindByIdAsync(agvTaskResult.StockTaskId);
        finishStockTask.ShouldBeNull();
        //finishStockTask.ManageTypeCode.ShouldBe(ManageType.CTUNPFullStockIn);
        //finishStockTask.ManageStatus.ShouldBe(ManageStatus.Complete);
        var finishAgvTask = await _agvTaskManager.FindByIdAsync(agvTaskResult.Id);
        finishAgvTask.Id.ShouldBe(1);
        finishAgvTask.AgvTaskStatus.ShouldBe(AgvTaskStatus.Complete);
        var finishStartCell = await _cellManager.GetByCodeAsync(finishAgvTask.StartPositionCode);
        //检查起点库位状态是否正确
        finishStartCell.RunStatus.ShouldBe(CellRunStatus.Enable);
        finishStartCell.CellStatus.ShouldBe(CellStatus.Nohave);
        var finishEndCell = await _cellManager.GetByCodeAsync(finishAgvTask.EndPositionCode);
        //检查终点库位状态是否正确
        finishEndCell.RunStatus.ShouldBe(CellRunStatus.Enable);
        finishEndCell.CellStatus.ShouldBe(CellStatus.Have);
        var finishBox = await _storageBoxManagement.GetByBoxCodeAsync(finishAgvTask.BoxCode);
        finishBox.CellId.ShouldBe(finishEndCell.Id);
        //检车创建的历史任务
        var finishTaskHis =await  _taskHisManager.FindByTaskIdAsync(finishAgvTask.StockTaskId);
        finishTaskHis.Id.ShouldBe(1);
        finishTaskHis.ManageStatus.ShouldBe(ManageStatus.Complete);
        finishTaskHis.StockBarcode.ShouldBe(finishAgvTask.BoxCode);
        //finishTaskHis.ManageTypeCode.ShouldBe(finishStockTask.ManageTypeCode);
        finishTaskHis.StartCellPosition.ShouldBe(finishAgvTask.StartPositionCode);
        finishTaskHis.EndCellPosition.ShouldBe(finishAgvTask.EndPositionCode);
    }
    /// <summary>
    /// 入库任务创建测试[等待执行-虚拟完成任务]
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task CreateStockInVirtualFinishAsync()
    {

        CreateStockTaskDto input = new CreateStockTaskDto()
        {
            StorageBoxId = 1,
            ManageTypeCode = "CTUNPFullStockIn",
            StartCellCode = "21-01-02",
            EndCellId = 0
        };
        //step1  创建入库任务
        var result = await _sampleAppService.CreateCTUStockInAsync(input);
        result.Id.ShouldBe(1);
        result.ManageStatus.ShouldBe(ManageStatus.WaitingExecute);
        result.StartCellCode.ShouldBe("21-01-02");
        result.StockBarcode.ShouldBe("B1001");
        result.EndCellCode.ShouldNotBeNullOrEmpty();
        result.ManageTypeCode.ShouldBe(ManageType.CTUNPFullStockIn);
        IdIntInput idIntInput = new IdIntInput() { Id = 1 };
      
        /// <summary>
        /// step2  测试设置虚拟完成任务
        /// StockTask设置为已完成
        /// 更新出入库库位状态
        /// 更新料箱的库位
        /// 创建任务历史数据
        /// </summary>
        var finishStockTask = await _stockTaskManager.SetAsCompletedAsync(result.Id);
        finishStockTask.Id.ShouldBe(result.Id);
        var finishStartCell = await _cellManager.GetByCodeAsync(result.StartCellCode);
        //检查起点库位状态是否正确
        finishStartCell.RunStatus.ShouldBe(CellRunStatus.Enable);
        finishStartCell.CellStatus.ShouldBe(CellStatus.Nohave);
        var finishEndCell = await _cellManager.GetByCodeAsync(result.EndCellCode);
        //检查终点库位状态是否正确
        finishEndCell.RunStatus.ShouldBe(CellRunStatus.Enable);
        finishEndCell.CellStatus.ShouldBe(CellStatus.Have);
        var finishBox = await _storageBoxManagement.GetByBoxCodeAsync(result.StockBarcode);
        finishBox.CellId.ShouldBe(finishEndCell.Id);
        //检车创建的历史任务
        var finishTaskHis = await _taskHisManager.FindByTaskIdAsync(result.Id);
        finishTaskHis.Id.ShouldBe(1);
        finishTaskHis.ManageStatus.ShouldBe(ManageStatus.Complete);
        finishTaskHis.StockBarcode.ShouldBe(result.StockBarcode);
        //finishTaskHis.ManageTypeCode.ShouldBe(finishStockTask.ManageTypeCode);
        finishTaskHis.StartCellPosition.ShouldBe(result.StartCellCode);
        finishTaskHis.EndCellPosition.ShouldBe(result.EndCellCode);
    }
    /// <summary>
    /// 入库任务创建测试[中途取消任务]
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task CreateStockInWithCancelAsync()
    {

        CreateStockTaskDto input = new CreateStockTaskDto()
        {
            StorageBoxId = 1,
            ManageTypeCode = "CTUNPFullStockIn",
            StartCellCode = "21-01-02",
            EndCellId = 0
        };
        //step1  创建入库任务
        var result = await _sampleAppService.CreateCTUStockInAsync(input);
        result.Id.ShouldBe(1);
        result.ManageStatus.ShouldBe(ManageStatus.WaitingExecute);
        result.StartCellCode.ShouldBe("21-01-02");
        result.StockBarcode.ShouldBe("B1001");
        result.EndCellCode.ShouldNotBeNullOrEmpty();
        result.ManageTypeCode.ShouldBe(ManageType.CTUNPFullStockIn);
        IdIntInput idIntInput = new IdIntInput() { Id = 1 };
        //step2 将入库任务设置为执行
        var executResult = await _sampleAppService.SetAsExecutingAsync(idIntInput);
        executResult.ManageStatus.ShouldBe(ManageStatus.Executing);
        //有可能一个入库任务对应多个CTU任务
        var agvTaskResult = await _agvTaskManager.FindByStockTaskIdAsync(executResult.Id);
        agvTaskResult.BoxCode.ShouldBe(executResult.StockBarcode);
        agvTaskResult.Id.ShouldBe(1);
        agvTaskResult.TaskTyp.ShouldBe("CTUIn");
        agvTaskResult.AgvTaskStatus.ShouldBe(AgvTaskStatus.Executing);
        var startCell = await _cellManager.GetByIdAsync((int)(result.StartCellId));
        //检查起点库位状态是否正确
        startCell.RunStatus.ShouldBe(CellRunStatus.Selected);
        startCell.CellStatus.ShouldBe(CellStatus.Have);
        var endCell = await _cellManager.GetByIdAsync((int)(result.EndCellId));
        //检查终点库位状态是否正确
        endCell.RunStatus.ShouldBe(CellRunStatus.Selected);
        endCell.CellStatus.ShouldBe(CellStatus.Nohave);
        /// <summary>
        /// step3  测试AGV执行回调
        /// AGV任务设置已开始
        /// StockTask设置为已开始
        /// </summary>
        AgvCallBackRequest agvCallBackRequest = new AgvCallBackRequest()
        {
            Method = "taskStart",
            TaskCode = agvTaskResult.Id.ToString()
        };
        var ctuCallBackResult = await _agvTaskAppService.CtuCallbackAsync(agvCallBackRequest);
        ctuCallBackResult.Code.ShouldBe("0");
        var taskStartStockTask = await _stockTaskManager.FindByIdAsync(agvTaskResult.StockTaskId);
        taskStartStockTask.Id.ShouldBe(1);
        taskStartStockTask.ManageTypeCode.ShouldBe(ManageType.CTUNPFullStockIn);
        taskStartStockTask.ManageStatus.ShouldBe(ManageStatus.CTUTaskStart);
        var taskStartAgvTask = await _agvTaskManager.FindByIdAsync(agvTaskResult.Id);
        taskStartAgvTask.Id.ShouldBe(1);
        taskStartAgvTask.AgvTaskStatus.ShouldBe(AgvTaskStatus.TaskStart);
        /// <summary>
        /// step4  测试AGV执行回调
        /// AGV任务设置出储位
        /// StockTask设置为出储位
        /// </summary>
        {
            AgvCallBackRequest cellOutAgvCallBackRequest = new AgvCallBackRequest()
            {
                Method = "cellOut",
                TaskCode = agvTaskResult.Id.ToString()
            };
            var cellOutCtuCallBackResult = await _agvTaskAppService.CtuCallbackAsync(cellOutAgvCallBackRequest);
            cellOutCtuCallBackResult.Code.ShouldBe("0");
            var cellOutStockTask = await _stockTaskManager.FindByIdAsync(agvTaskResult.StockTaskId);
            cellOutStockTask.Id.ShouldBe(1);
            cellOutStockTask.ManageTypeCode.ShouldBe(ManageType.CTUNPFullStockIn);
            cellOutStockTask.ManageStatus.ShouldBe(ManageStatus.CTUCellOut);
            var cellOutAgvTask = await _agvTaskManager.FindByIdAsync(agvTaskResult.Id);
            cellOutAgvTask.Id.ShouldBe(1);
            cellOutAgvTask.AgvTaskStatus.ShouldBe(AgvTaskStatus.CellOut);
        }
        /// <summary>
        /// step4  测试AGV执行回调
        /// AGV任务设置已取消
        /// StockTask设置为已完成
        /// 更新出入库库位状态
        /// 更新料箱的库位
        /// 创建任务历史数据
        /// </summary>
        AgvCallBackRequest finishAgvCallBackRequest = new AgvCallBackRequest()
        {
            Method = "taskCancel",
            TaskCode = agvTaskResult.Id.ToString()
        };
        var finishCtuCallBackResult = await _agvTaskAppService.CtuCallbackAsync(finishAgvCallBackRequest);
        finishCtuCallBackResult.Code.ShouldBe("0");
        var finishStockTask = await _stockTaskManager.FindByIdAsync(agvTaskResult.StockTaskId);
        finishStockTask.ShouldBeNull();
        //finishStockTask.ManageTypeCode.ShouldBe(ManageType.CTUNPFullStockIn);
        //finishStockTask.ManageStatus.ShouldBe(ManageStatus.Complete);
        var finishAgvTask = await _agvTaskManager.FindByIdAsync(agvTaskResult.Id);
        finishAgvTask.Id.ShouldBe(1);
        finishAgvTask.AgvTaskStatus.ShouldBe(AgvTaskStatus.Cancel);
        var finishStartCell = await _cellManager.GetByCodeAsync(finishAgvTask.StartPositionCode);
        //检查起点库位状态是否正确
        finishStartCell.RunStatus.ShouldBe(CellRunStatus.Enable);
        finishStartCell.CellStatus.ShouldBe(CellStatus.Have);
        var finishEndCell = await _cellManager.GetByCodeAsync(finishAgvTask.EndPositionCode);
        //检查终点库位状态是否正确
        finishEndCell.RunStatus.ShouldBe(CellRunStatus.Enable);
        finishEndCell.CellStatus.ShouldBe(CellStatus.Nohave);
        var finishBox = await _storageBoxManagement.GetByBoxCodeAsync(finishAgvTask.BoxCode);
        finishBox.CellId.ShouldBe(0);
        //检车创建的历史任务
        var finishTaskHis = await _taskHisManager.FindByTaskIdAsync(finishAgvTask.StockTaskId);
        finishTaskHis.Id.ShouldBe(1);
        finishTaskHis.ManageStatus.ShouldBe(ManageStatus.Cancel);
        finishTaskHis.StockBarcode.ShouldBe(finishAgvTask.BoxCode);
        //finishTaskHis.ManageTypeCode.ShouldBe(finishStockTask.ManageTypeCode);
        finishTaskHis.StartCellPosition.ShouldBe(finishAgvTask.StartPositionCode);
        finishTaskHis.EndCellPosition.ShouldBe(finishAgvTask.EndPositionCode);
    }
    /// <summary>
    /// 入库任务创建测试[等待执行---取消任务]
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task CreateStockInWithWaitCancelAsync()
    {

        CreateStockTaskDto input = new CreateStockTaskDto()
        {
            StorageBoxId = 1,
            ManageTypeCode = "CTUNPFullStockIn",
            StartCellCode = "21-01-02",
            EndCellId = 0
        };
        //step1  创建入库任务
        var result = await _sampleAppService.CreateCTUStockInAsync(input);
        result.Id.ShouldBe(1);
        result.ManageStatus.ShouldBe(ManageStatus.WaitingExecute);
        result.StartCellCode.ShouldBe("21-01-02");
        result.StockBarcode.ShouldBe("B1001");
        result.EndCellCode.ShouldNotBeNullOrEmpty();
        result.ManageTypeCode.ShouldBe(ManageType.CTUNPFullStockIn);
        IdIntInput idIntInput = new IdIntInput() { Id = 1 };
        /// <summary>
        /// step2  测试取消出入库任务
        /// 更新出入库库位状态
        /// 创建任务历史数据
        /// </summary>
        var cancekStockTask = await _stockTaskManager.SetAsCancelAsync(1);
        var finishStartCell = await _cellManager.GetByCodeAsync(result.StartCellCode);
        //检查起点库位状态是否正确
        finishStartCell.RunStatus.ShouldBe(CellRunStatus.Enable);
        //finishStartCell.CellStatus.ShouldBe(CellStatus.Have);
        var finishEndCell = await _cellManager.GetByCodeAsync(result.EndCellCode);
        //检查终点库位状态是否正确
        finishEndCell.RunStatus.ShouldBe(CellRunStatus.Enable);
        //finishEndCell.CellStatus.ShouldBe(CellStatus.Nohave);
        var finishBox = await _storageBoxManagement.GetByBoxCodeAsync(result.StockBarcode);
        finishBox.CellId.ShouldBe(0);
        //检车创建的历史任务
        var finishTaskHis = await _taskHisManager.FindByTaskIdAsync(result.Id);
        finishTaskHis.Id.ShouldBe(1);
        finishTaskHis.ManageStatus.ShouldBe(ManageStatus.Cancel);
        finishTaskHis.StockBarcode.ShouldBe(result.StockBarcode);
        //finishTaskHis.ManageTypeCode.ShouldBe(finishStockTask.ManageTypeCode);
        finishTaskHis.StartCellPosition.ShouldBe(result.StartCellCode);
        finishTaskHis.EndCellPosition.ShouldBe(result.EndCellCode);
    }
    /// <summary>
    /// 基本出库流程测试[正常完成任务]
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task CreateStockOutAsync()
    {
        //step1  绑定料箱和库位
        var box= await _storageBoxManagement.UpdateStockCellAsync("B1001",2);
        box.Id.ShouldBe(1);
        box.CellId.ShouldBe(2);
        //step2  创建出库任务
        CreateStockTaskDto input = new CreateStockTaskDto()
        {
            StorageBoxId = 1,
            ManageTypeCode = "CTUNpFullStockOut",
            EndCellId = 0
        };        
        var result = await _sampleAppService.CreateCTUStockOutAsync(input);
        result.Id.ShouldBe(1);
        result.ManageStatus.ShouldBe(ManageStatus.WaitingExecute);
        result.StartCellCode.ShouldBe("01-02-01");
        result.StockBarcode.ShouldBe("B1001");
        result.EndCellCode.ShouldNotBeNullOrEmpty();
        result.ManageTypeCode.ShouldBe(ManageType.CTUNpFullStockOut);
        IdIntInput idIntInput = new IdIntInput() { Id = 1 };
        //step3 将入库任务设置为执行
        var executResult = await _sampleAppService.SetAsExecutingAsync(idIntInput);
        executResult.ManageStatus.ShouldBe(ManageStatus.Executing);
        //有可能一个入库任务对应多个CTU任务
        var agvTaskResult = await _agvTaskManager.FindByStockTaskIdAsync(executResult.Id);
        agvTaskResult.BoxCode.ShouldBe(executResult.StockBarcode);
        agvTaskResult.Id.ShouldBe(1);
        agvTaskResult.TaskTyp.ShouldBe("CTUOut");
        agvTaskResult.AgvTaskStatus.ShouldBe(AgvTaskStatus.Executing);
        var startCell = await _cellManager.GetByIdAsync((int)(result.StartCellId));
        //检查起点库位状态是否正确
        startCell.RunStatus.ShouldBe(CellRunStatus.Selected);
        //startCell.CellStatus.ShouldBe(CellStatus.Have);
        var endCell = await _cellManager.GetByIdAsync((int)(result.EndCellId));
        //检查终点库位状态是否正确
        endCell.RunStatus.ShouldBe(CellRunStatus.Selected);
        endCell.CellStatus.ShouldBe(CellStatus.Nohave);
        /// <summary>
        /// step4  测试AGV执行回调
        /// AGV任务设置已开始
        /// StockTask设置为已开始
        /// </summary>
        AgvCallBackRequest agvCallBackRequest = new AgvCallBackRequest()
        {
            Method = "taskStart",
            TaskCode = agvTaskResult.Id.ToString()
        };
        var ctuCallBackResult = await _agvTaskAppService.CtuCallbackAsync(agvCallBackRequest);
        ctuCallBackResult.Code.ShouldBe("0");
        var taskStartStockTask = await _stockTaskManager.FindByIdAsync(agvTaskResult.StockTaskId);
        taskStartStockTask.Id.ShouldBe(1);
        taskStartStockTask.ManageTypeCode.ShouldBe(ManageType.CTUNpFullStockOut);
        taskStartStockTask.ManageStatus.ShouldBe(ManageStatus.CTUTaskStart);
        var taskStartAgvTask = await _agvTaskManager.FindByIdAsync(agvTaskResult.Id);
        taskStartAgvTask.Id.ShouldBe(1);
        taskStartAgvTask.AgvTaskStatus.ShouldBe(AgvTaskStatus.TaskStart);
        /// <summary>
        /// step5  测试AGV执行回调
        /// AGV任务设置出储位
        /// StockTask设置为出储位
        /// </summary>
        AgvCallBackRequest cellOutAgvCallBackRequest = new AgvCallBackRequest()
        {
            Method = "cellOut",
            TaskCode = agvTaskResult.Id.ToString()
        };
        var cellOutCtuCallBackResult = await _agvTaskAppService.CtuCallbackAsync(cellOutAgvCallBackRequest);
        cellOutCtuCallBackResult.Code.ShouldBe("0");
        var cellOutStockTask = await _stockTaskManager.FindByIdAsync(agvTaskResult.StockTaskId);
        cellOutStockTask.Id.ShouldBe(1);
        cellOutStockTask.ManageTypeCode.ShouldBe(ManageType.CTUNpFullStockOut);
        cellOutStockTask.ManageStatus.ShouldBe(ManageStatus.CTUCellOut);
        var cellOutAgvTask = await _agvTaskManager.FindByIdAsync(agvTaskResult.Id);
        cellOutAgvTask.Id.ShouldBe(1);
        cellOutAgvTask.AgvTaskStatus.ShouldBe(AgvTaskStatus.CellOut);
        /// <summary>
        /// step6  测试AGV执行回调
        /// AGV任务设置已完成
        /// StockTask设置为已完成
        /// 更新出入库库位状态
        /// 更新料箱的库位
        /// 创建任务历史数据
        /// </summary>
        AgvCallBackRequest finishAgvCallBackRequest = new AgvCallBackRequest()
        {
            Method = "taskFinish",
            TaskCode = agvTaskResult.Id.ToString()
        };
        var finishCtuCallBackResult = await _agvTaskAppService.CtuCallbackAsync(finishAgvCallBackRequest);
        finishCtuCallBackResult.Code.ShouldBe("0");
        var finishStockTask = await _stockTaskManager.FindByIdAsync(agvTaskResult.StockTaskId);
        finishStockTask.ShouldBeNull();
        //finishStockTask.ManageTypeCode.ShouldBe(ManageType.CTUNPFullStockIn);
        //finishStockTask.ManageStatus.ShouldBe(ManageStatus.Complete);
        var finishAgvTask = await _agvTaskManager.FindByIdAsync(agvTaskResult.Id);
        finishAgvTask.Id.ShouldBe(1);
        finishAgvTask.AgvTaskStatus.ShouldBe(AgvTaskStatus.Complete);
        var finishStartCell = await _cellManager.GetByCodeAsync(finishAgvTask.StartPositionCode);
        //检查起点库位状态是否正确
        finishStartCell.RunStatus.ShouldBe(CellRunStatus.Enable);
        finishStartCell.CellStatus.ShouldBe(CellStatus.Nohave);
        var finishEndCell = await _cellManager.GetByCodeAsync(finishAgvTask.EndPositionCode);
        //检查终点库位状态是否正确
        finishEndCell.RunStatus.ShouldBe(CellRunStatus.Enable);
        finishEndCell.CellStatus.ShouldBe(CellStatus.Have);
        var finishBox = await _storageBoxManagement.GetByBoxCodeAsync(finishAgvTask.BoxCode);
        finishBox.CellId.ShouldBe(finishEndCell.Id);
        //检车创建的历史任务
        var finishTaskHis = await _taskHisManager.FindByTaskIdAsync(finishAgvTask.StockTaskId);
        finishTaskHis.Id.ShouldBe(1);
        finishTaskHis.ManageStatus.ShouldBe(ManageStatus.Complete);
        finishTaskHis.StockBarcode.ShouldBe(finishAgvTask.BoxCode);
        //finishTaskHis.ManageTypeCode.ShouldBe(finishStockTask.ManageTypeCode);
        finishTaskHis.StartCellPosition.ShouldBe(finishAgvTask.StartPositionCode);
        finishTaskHis.EndCellPosition.ShouldBe(finishAgvTask.EndPositionCode);
    }

    /// <summary>
    /// 基本出库流程测试[中途取消任务]
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task CreateStockOutCancelAsync()
    {
        //step1  绑定料箱和库位
        var box = await _storageBoxManagement.UpdateStockCellAsync("B1001", 2);
        box.Id.ShouldBe(1);
        box.CellId.ShouldBe(2);
        //step2  创建出库任务
        CreateStockTaskDto input = new CreateStockTaskDto()
        {
            StorageBoxId = 1,
            ManageTypeCode = "CTUNpFullStockOut",
            EndCellId = 0
        };
        var result = await _sampleAppService.CreateCTUStockOutAsync(input);
        result.Id.ShouldBe(1);
        result.ManageStatus.ShouldBe(ManageStatus.WaitingExecute);
        result.StartCellCode.ShouldBe("01-02-01");
        result.StockBarcode.ShouldBe("B1001");
        result.EndCellCode.ShouldNotBeNullOrEmpty();
        result.ManageTypeCode.ShouldBe(ManageType.CTUNpFullStockOut);
        IdIntInput idIntInput = new IdIntInput() { Id = 1 };
        //step3 将入库任务设置为执行
        var executResult = await _sampleAppService.SetAsExecutingAsync(idIntInput);
        executResult.ManageStatus.ShouldBe(ManageStatus.Executing);
        //有可能一个入库任务对应多个CTU任务
        var agvTaskResult = await _agvTaskManager.FindByStockTaskIdAsync(executResult.Id);
        agvTaskResult.BoxCode.ShouldBe(executResult.StockBarcode);
        agvTaskResult.Id.ShouldBe(1);
        agvTaskResult.TaskTyp.ShouldBe("CTUOut");
        agvTaskResult.AgvTaskStatus.ShouldBe(AgvTaskStatus.Executing);
        var startCell = await _cellManager.GetByIdAsync((int)(result.StartCellId));
        //检查起点库位状态是否正确
        startCell.RunStatus.ShouldBe(CellRunStatus.Selected);
        //startCell.CellStatus.ShouldBe(CellStatus.Have);
        var endCell = await _cellManager.GetByIdAsync((int)(result.EndCellId));
        //检查终点库位状态是否正确
        endCell.RunStatus.ShouldBe(CellRunStatus.Selected);
        endCell.CellStatus.ShouldBe(CellStatus.Nohave);
        /// <summary>
        /// step4  测试AGV执行回调
        /// AGV任务设置已开始
        /// StockTask设置为已开始
        /// </summary>
        AgvCallBackRequest agvCallBackRequest = new AgvCallBackRequest()
        {
            Method = "taskStart",
            TaskCode = agvTaskResult.Id.ToString()
        };
        var ctuCallBackResult = await _agvTaskAppService.CtuCallbackAsync(agvCallBackRequest);
        ctuCallBackResult.Code.ShouldBe("0");
        var taskStartStockTask = await _stockTaskManager.FindByIdAsync(agvTaskResult.StockTaskId);
        taskStartStockTask.Id.ShouldBe(1);
        taskStartStockTask.ManageTypeCode.ShouldBe(ManageType.CTUNpFullStockOut);
        taskStartStockTask.ManageStatus.ShouldBe(ManageStatus.CTUTaskStart);
        var taskStartAgvTask = await _agvTaskManager.FindByIdAsync(agvTaskResult.Id);
        taskStartAgvTask.Id.ShouldBe(1);
        taskStartAgvTask.AgvTaskStatus.ShouldBe(AgvTaskStatus.TaskStart);
        /// <summary>
        /// step5  测试AGV执行回调
        /// AGV任务设置出储位
        /// StockTask设置为出储位
        /// </summary>
        AgvCallBackRequest cellOutAgvCallBackRequest = new AgvCallBackRequest()
        {
            Method = "cellOut",
            TaskCode = agvTaskResult.Id.ToString()
        };
        var cellOutCtuCallBackResult = await _agvTaskAppService.CtuCallbackAsync(cellOutAgvCallBackRequest);
        cellOutCtuCallBackResult.Code.ShouldBe("0");
        var cellOutStockTask = await _stockTaskManager.FindByIdAsync(agvTaskResult.StockTaskId);
        cellOutStockTask.Id.ShouldBe(1);
        cellOutStockTask.ManageTypeCode.ShouldBe(ManageType.CTUNpFullStockOut);
        cellOutStockTask.ManageStatus.ShouldBe(ManageStatus.CTUCellOut);
        var cellOutAgvTask = await _agvTaskManager.FindByIdAsync(agvTaskResult.Id);
        cellOutAgvTask.Id.ShouldBe(1);
        cellOutAgvTask.AgvTaskStatus.ShouldBe(AgvTaskStatus.CellOut);
        /// <summary>
        /// step6  测试AGV执行回调
        /// AGV任务设置已完成
        /// StockTask设置为已完成
        /// 更新出入库库位状态
        /// 更新料箱的库位
        /// 创建任务历史数据
        /// </summary>
        AgvCallBackRequest finishAgvCallBackRequest = new AgvCallBackRequest()
        {
            Method = "taskCancel",
            TaskCode = agvTaskResult.Id.ToString()
        };
        var finishCtuCallBackResult = await _agvTaskAppService.CtuCallbackAsync(finishAgvCallBackRequest);
        finishCtuCallBackResult.Code.ShouldBe("0");
        var finishStockTask = await _stockTaskManager.FindByIdAsync(agvTaskResult.StockTaskId);
        finishStockTask.ShouldBeNull();
        //finishStockTask.ManageTypeCode.ShouldBe(ManageType.CTUNPFullStockIn);
        //finishStockTask.ManageStatus.ShouldBe(ManageStatus.Complete);
        var finishAgvTask = await _agvTaskManager.FindByIdAsync(agvTaskResult.Id);
        finishAgvTask.Id.ShouldBe(1);
        finishAgvTask.AgvTaskStatus.ShouldBe(AgvTaskStatus.Cancel);
        var finishStartCell = await _cellManager.GetByCodeAsync(finishAgvTask.StartPositionCode);
        //检查起点库位状态是否正确
        finishStartCell.RunStatus.ShouldBe(CellRunStatus.Enable);
        //finishStartCell.CellStatus.ShouldBe(CellStatus.Nohave);
        var finishEndCell = await _cellManager.GetByCodeAsync(finishAgvTask.EndPositionCode);
        //检查终点库位状态是否正确
        finishEndCell.RunStatus.ShouldBe(CellRunStatus.Enable);
        //finishEndCell.CellStatus.ShouldBe(CellStatus.Have);
        var finishBox = await _storageBoxManagement.GetByBoxCodeAsync(finishAgvTask.BoxCode);
        finishBox.CellId.ShouldBe(finishStartCell.Id);
        //检车创建的历史任务
        var finishTaskHis = await _taskHisManager.FindByTaskIdAsync(finishAgvTask.StockTaskId);
        finishTaskHis.Id.ShouldBe(1);
        finishTaskHis.ManageStatus.ShouldBe(ManageStatus.Cancel);
        finishTaskHis.StockBarcode.ShouldBe(finishAgvTask.BoxCode);
        //finishTaskHis.ManageTypeCode.ShouldBe(finishStockTask.ManageTypeCode);
        finishTaskHis.StartCellPosition.ShouldBe(finishAgvTask.StartPositionCode);
        finishTaskHis.EndCellPosition.ShouldBe(finishAgvTask.EndPositionCode);
    }
    //[Fact]
    //public async Task CreateTaskAsync()
    //{
    //string reqCode = Guid.NewGuid().ToString("N");
    //string taskTyp = "";
    ////Arrange
    //var mockFactory = new Mock<IHttpClientFactory>();
    //var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
    //mockHttpMessageHandler.Protected()
    //    .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
    //    .ReturnsAsync(new HttpResponseMessage
    //    {
    //        StatusCode = HttpStatusCode.OK,
    //        Content = new StringContent("{'name':thecodebuzz,'city':'USA'}"),
    //    });



    //var client = new HttpClient(mockHttpMessageHandler.Object);
    //mockFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);

    //RcsApiManager rcsApiManager = new RcsApiManager(mockFactory.Object, null);
    //var response = await _rcsApiManager.CreateTaskAsync(reqCode, taskTyp, null, null, null);
    //response.ShouldNotBeNull();
    //}
    //[Fact]
    //public async Task GetAsync()
    //{
    //    //PagingStorageBoxListInput input =new PagingStorageBoxListInput() { 
    //    //    PageSize = 10,
    //    //    PageIndex = 1
    //    //};
    //    //var result = await _storageBoxAppService.GetPagingListAsync(input);
    //    ////Assert
    //    //result.TotalCount.ShouldBeGreaterThan(0);
    //    //result.Items.ShouldContain(u => u.Id == 1);
    //}

    [Fact]
    public async Task CreateBoxAsync()
    {
        //CreateStorageBoxDto input = new CreateStorageBoxDto() { StorageBoxBarcode = "C001" };
        //var result = await _storageBoxAppService.CreateAsync(input);
        //CreateStorageBoxDto input2 = new CreateStorageBoxDto() { StorageBoxBarcode = "C002" };
        //var result2 = await _storageBoxAppService.CreateAsync(input2);
        //Assert
        //PagingStorageBoxListInput findInput = new PagingStorageBoxListInput()
        //{
        //    PageSize = 10,
        //    PageIndex = 1
        //};
        //var findResult = await _storageBoxAppService.GetPagingListAsync(findInput);
        ////Assert
        //findResult.TotalCount.ShouldBeGreaterThan(0);
        //findResult.Items.ShouldContain(u => u.StorageBoxBarcode == "C002");
    }
}
