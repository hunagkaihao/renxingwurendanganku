using System;
using System.Collections.Generic;
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
using WarehouseManagement.Goodss;
using WarehouseManagement.Plans;
using WarehouseManagement.SendRestApis;
using WarehouseManagement.StockTasks;
using WarehouseManagement.StockTasks.Dto;
using WarehouseManagement.StorageBoxs;
using WarehouseManagement.StorageBoxs.Dto;
using WarehouseManagement.TaskHiss;
using WarehouseManagement.Warehouses;
using Xunit;

namespace WarehouseManagement.Samples;

public class PlanAppService_Tests : WarehouseManagementApplicationTestBase
{
    private readonly IStockTaskAppService _stockTaskAppService;
    private readonly IAgvTaskAppService _agvTaskAppService;
    private readonly StorageBoxManager _storageBoxManagement;
    private readonly CellManager _cellManager;
    private readonly WarehouseManager _warehouseManager;
    private readonly AgvTaskManager _agvTaskManager;
    private readonly StockTaskManager _stockTaskManager;
    private readonly TaskHisManager _taskHisManager;
    private readonly PlanManager _planManager;
    private readonly GoodsManager _goodsManager;
    private readonly SendRestApiManager _sendRestApiManager;
    private readonly ISendRestApiRepository _sendRestApiRepository;

    //private readonly IHttpClientFactory _httpClientFactory;
    //private readonly AGVOptions _aGVOptions;

    public PlanAppService_Tests()
    {
        _sendRestApiRepository = GetRequiredService<ISendRestApiRepository>();
        //Arrange
        var mockFactory = new Mock<IHttpClientFactory>();
        var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("{'code':'0','message':'success'}"),
            });



        var client = new HttpClient(mockHttpMessageHandler.Object);
        mockFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);
        ERPOptions agvoptions = new ERPOptions()
        {
            Server = "https://lims.nbjlw.com:8091",
            Enable = "false"
        };
        var mock = new Mock<IOptionsSnapshot<ERPOptions>>();
        mock.Setup(m => m.Value).Returns(agvoptions);
        _sendRestApiManager = new SendRestApiManager(_sendRestApiRepository,mockFactory.Object, mock.Object);
        _planManager = GetRequiredService<PlanManager>();
        _goodsManager = GetRequiredService<GoodsManager>();
        _storageBoxManagement = GetRequiredService<StorageBoxManager>();
        _cellManager = GetRequiredService<CellManager>();
        _warehouseManager = GetRequiredService<WarehouseManager>();
        _agvTaskManager = GetRequiredService<AgvTaskManager>();
        _agvTaskAppService = GetRequiredService<IAgvTaskAppService>();
        _stockTaskManager = GetRequiredService<StockTaskManager>();
        //_httpClientFactory = GetRequiredService<HttpClientHelper>();
        //_aGVOptions = GetRequiredService<AGVOptions>();
        InitContext();//初始化内存数据
        _stockTaskAppService = GetRequiredService<IStockTaskAppService>();
         _taskHisManager = GetRequiredService<TaskHisManager>();

    }

    private async void InitContext()
    {
        //初始化仓库
        await _warehouseManager.CreateAsync("CTU01", "1#仓库", "CTU");
        //初始化料箱
        {
            await _storageBoxManagement.CreateAsync("B1001", "1");
            await _storageBoxManagement.CreateAsync("B1002", "1");
            await _storageBoxManagement.CreateAsync("B1003", "1");
        }
        //初始化物料
        {
            await _goodsManager.CreateAsync("G1001", "G1001", "G1001", "G1001");
            await _goodsManager.CreateAsync("G1002", "G1002", "G1002", "G1002");
            await _goodsManager.CreateAsync("G1003", "G1003", "G1003", "G1003");
        }
        ////初始化组箱
        //{
        //    await _storageBoxManagement.CreateDetailAsync(1, 1, 10, "",0, null);
        //    await _storageBoxManagement.CreateDetailAsync(2, 2, 5, "", 0, null);
        //    await _storageBoxManagement.CreateDetailAsync(2, 3, 8, "", 0, null);
        //}
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
    /// 计划入库流程测试
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task CreatePlanInAsync()
    {
        //step1  创建计划入库任务
        List<PlanListDto> planListDtos = new List<PlanListDto>() {
            new PlanListDto(){  GoodsCode="G1001",GoodsBatchNo="",OwnerCode="1",OrderItem="1",WarehouseCode="CTU01",PlanListQty=8},
            new PlanListDto(){  GoodsCode="G1002",GoodsBatchNo="",OwnerCode="1",OrderItem="2",WarehouseCode="CTU01",PlanListQty=5},
        };
        var plan = await _planManager.CreateInOrderAsync("ERPPlanIn", "ERPIN001", "2023-05-23", "ZS", 1, 1, "", planListDtos);
        plan.Id.ShouldBe(1);
        plan.PlanBillNo.ShouldBe("ERPIN001");
        plan.Details.Count.ShouldBe(2);
        plan.PlanStatus.ShouldBe(PlanStatus.Waiting);
        plan.Details.Find(f => f.GoodsCode == "G1001").PlanListQty.ShouldBe(8);
        plan.Details.Find(f => f.GoodsCode == "G1002").PlanListQty.ShouldBe(5);
        //step2  组箱入库
        List<GoodsInBox> goodsInBoxs = new List<GoodsInBox>() {
            new GoodsInBox(){ GoodsId =1,Quantity=8, PlanListId=1 },
            new GoodsInBox(){ GoodsId =2,Quantity=5, PlanListId=2 },
        };
        var box = await _planManager.BindGoodssAsync("B1001", goodsInBoxs);
        box.Details.Count.ShouldBe(2);
        box.Details.Find(f => f.GoodsId == 1).Quantity.ShouldBe(8);
        box.Details.Find(f => f.GoodsId == 2).Quantity.ShouldBe(5);
        box.Details.Find(f => f.GoodsId == 1).PlanListId.ShouldBe(1);
        box.Details.Find(f => f.GoodsId == 2).PlanListId.ShouldBe(2);
        //step 3 检查plan中创建数量
        plan = await _planManager.FindByIdAsync(1);
        plan.Details.Find(f => f.GoodsCode == "G1001").PlanListCreateQty.ShouldBe(8);
        plan.Details.Find(f => f.GoodsCode == "G1002").PlanListCreateQty.ShouldBe(5);
        //step 组盘入库
        await CreateStockInAsync(true);
        //step 3 检查plan中创建数量
        plan = await _planManager.FindByIdAsync(1);
        plan.Details.Find(f => f.GoodsCode == "G1001").PlanListFinishedQty.ShouldBe(8);
        plan.Details.Find(f => f.GoodsCode == "G1002").PlanListFinishedQty.ShouldBe(5);
        //step 3 检查BOX中planList
        var boxFinish = await _storageBoxManagement.GetByBoxCodeAsync("B1001");
        boxFinish.Details.Find(f => f.GoodsId == 1).PlanListId.ShouldBe(0);
        boxFinish.Details.Find(f => f.GoodsId == 1).PlanListId.ShouldBe(0);
    }
    /// <summary>
    /// 入库任务创建测试[正常完成任务]
    /// </summary>
    /// <returns></returns>
    public async Task CreateStockInAsync(bool planFlag=false)
    {

        CreateStockTaskDto input = new CreateStockTaskDto() { StorageBoxId=1,
            ManageTypeCode= "CTUNPFullStockIn",
            StartCellCode= "21-01-02",
            EndCellId=0
        };
        //step1  创建入库任务
        var result = await _stockTaskAppService.CreateCTUStockInAsync(input);
        result.Id.ShouldBe(1);
        result.ManageStatus.ShouldBe(ManageStatus.WaitingExecute);
        result.StartCellCode.ShouldBe("21-01-02");
        result.StockBarcode.ShouldBe("B1001");
        result.EndCellCode.ShouldNotBeNullOrEmpty();
        result.ManageTypeCode.ShouldBe(ManageType.CTUNPFullStockIn);
        //step 3 检查plan中创建数量
        if (planFlag)
        {
            var plan = await _planManager.FindByIdAsync(1);
            plan.Details.Find(f => f.GoodsCode == "G1001").PlanListExecuteQty.ShouldBe(8);
            plan.Details.Find(f => f.GoodsCode == "G1002").PlanListExecuteQty.ShouldBe(5);
        }
        IdIntInput idIntInput = new IdIntInput() { Id =1};
        //step2 将入库任务设置为执行
        var executResult = await _stockTaskAppService.SetAsExecutingAsync(idIntInput);
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
    /// 无计划入库流程测试
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task CreateNPInAsync()
    {
        //step1  组箱
        List<GoodsInBox> goodsInBoxs = new List<GoodsInBox>() {
            new GoodsInBox(){ GoodsId =1,Quantity=8 },
            new GoodsInBox(){ GoodsId =2,Quantity=5 },
        };
        var box = await _planManager.BindGoodssAsync("B1001", goodsInBoxs);
        box.Details.Count.ShouldBe(2);
        box.Details.Find(f => f.GoodsId == 1).Quantity.ShouldBe(8);
        box.Details.Find(f => f.GoodsId == 2).Quantity.ShouldBe(5);
        box.Details.Find(f => f.GoodsId == 1).PlanListId.ShouldBe(0);
        box.Details.Find(f => f.GoodsId == 2).PlanListId.ShouldBe(0);
        //step 组盘入库
        await CreateStockInAsync();

    }
}
