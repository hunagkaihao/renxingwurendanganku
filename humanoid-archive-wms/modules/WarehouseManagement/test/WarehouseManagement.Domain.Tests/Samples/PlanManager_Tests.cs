using Lion.AbpPro.Extension.Customs.Dtos;
using Shouldly;
using System.Collections.Generic;
using System.Threading.Tasks;
using WarehouseManagement.AgvTasks;
using WarehouseManagement.AgvTasks.Dto;
using WarehouseManagement.Cells;
using WarehouseManagement.Goodss;
using WarehouseManagement.Plans;
using WarehouseManagement.StockTasks;
using WarehouseManagement.StockTasks.Dto;
using WarehouseManagement.StorageBoxs;
using WarehouseManagement.TaskHiss;
using WarehouseManagement.Warehouses;
using Xunit;

namespace WarehouseManagement.Samples;

public class PlanManager_Tests : WarehouseManagementDomainTestBase
{
    private readonly PlanManager _planManager;
    private readonly StorageBoxManager _storageBoxManagement;
    private readonly CellManager _cellManager;
    private readonly WarehouseManager _warehouseManager;
    private readonly GoodsManager _goodsManager;
    private readonly StockTaskManager _stockTaskManager;
    /// <summary>
    /// 计划任务测试
    /// </summary>
    public PlanManager_Tests()
    {
        //_sampleManager = GetRequiredService<SampleManager>();
        _planManager =GetRequiredService<PlanManager>();
        _storageBoxManagement = GetRequiredService<StorageBoxManager>();
        _cellManager = GetRequiredService<CellManager>();
        _warehouseManager = GetRequiredService<WarehouseManager>();
        _goodsManager = GetRequiredService<GoodsManager>();
        _stockTaskManager = GetRequiredService<StockTaskManager>();
        InitContext();//初始化内存数据

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

    }

    [Fact]
    public async Task CreatePlanOutAsync()
    {
        //step1  绑定料箱和库位
        var box = await _storageBoxManagement.UpdateStockCellAsync("B1001", 2);
        box.Id.ShouldBe(1);
        box.CellId.ShouldBe(2);
        var box2 = await _storageBoxManagement.UpdateStockCellAsync("B1002", 3);
        box2.Id.ShouldBe(2);
        box2.CellId.ShouldBe(3);
        List<PlanListDto> planListDtos = new List<PlanListDto>() {
            new PlanListDto(){  GoodsCode="G1001",GoodsBatchNo="",PlanListQty=8},
            new PlanListDto(){  GoodsCode="G1002",GoodsBatchNo="",PlanListQty=5},
        };
        var plan = await  _planManager.CreateAsync("ERPPlan", "ERP001", "2023-05-23", "ZS", 1, 1,"", planListDtos);
        plan.Id.ShouldBe(1);
        plan.Details.Count.ShouldBe(2);
        plan.PlanStatus.ShouldBe(PlanStatus.Waiting);
        plan.Details.Find(f=>f.GoodsCode=="G1001").PlanListQty.ShouldBe(8);
        //plan.Details.Find(f => f.GoodsCode == "G1001").PlanListCreateQty.ShouldBe(8);
        plan.Details.Find(f => f.GoodsCode == "G1002").PlanListQty.ShouldBe(5);
        //plan.Details.Find(f => f.GoodsCode == "G1002").PlanListCreateQty.ShouldBe(5);
        //var tasks= await _stockTaskManager.GetListByPlanIdAsync(1);
        //tasks.Count.ShouldBe(2);
        //var task1 =  tasks.Find(f=>f.Id==1);
        //task1.ManageStatus.ShouldBe(ManageStatus.WaitingExecute);
        //var taskDetails1 = await _stockTaskManager.GetTaskDetailByIdAsync(1);
        //taskDetails1.Find(f => f.GoodsId == 1).ManageListQuantity.ShouldBe(8);
        //var task2 = tasks.Find(f => f.Id == 2);
        //var taskDetails2 = await _stockTaskManager.GetTaskDetailByIdAsync(2);
        //taskDetails2.Find(f => f.GoodsId == 2).ManageListQuantity.ShouldBe(5);
    }

}
