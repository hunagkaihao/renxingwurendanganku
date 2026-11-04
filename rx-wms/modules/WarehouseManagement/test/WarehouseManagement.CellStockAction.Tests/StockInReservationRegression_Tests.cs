using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Lion.AbpPro.ConfigurationOptions;
using Lion.AbpPro.Settings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;
using Shouldly;
using Volo.Abp;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Entities;
using Volo.Abp.ObjectMapping;
using Volo.Abp.Uow;
using WarehouseManagement.Cells;
using WarehouseManagement.Material;
using WarehouseManagement.MaterialBoxs;
using WarehouseManagement.MaterialBoxs.Aggregates;
using WarehouseManagement.Plans;
using WarehouseManagement.Plans.Aggregates;
using WarehouseManagement.StockTasks;
using WarehouseManagement.StockTasks.Aggregates;
using WarehouseManagement.StockTasks.Dto;
using WarehouseManagement.WcsTasks;
using WarehouseManagement.WcsTasks.Dto;
using Xunit;
using MaterialEntity = WarehouseManagement.Material.Aggregates.Material;
using StockTaskStatus = WarehouseManagement.StockTasks.TaskStatus;

namespace WarehouseManagement.Samples;

/// <summary>
/// 验证指定库位预约和同条码临时出库后的完整回库状态环路。
/// </summary>
public class StockInReservationRegression_Tests
{
    /// <summary>只传目标编码时，预约和实际下发必须使用同一库位。</summary>
    [Fact]
    public async Task Reservation_by_cell_code_keeps_target_when_dispatched()
    {
        var fixture = new ReservationFixture();
        var result = await fixture.Service.CreateWCSIn(fixture.Request(endCellCode: " 1-2-1 "));

        result.EndCellId.ShouldBe(fixture.Target.Id);
        result.EndCellCode.ShouldBe(fixture.Target.CellCode);
        var order = await fixture.Manager.PrepareWcsStockOrderAsync(result.Id);
        order.EndNode.ShouldBe(fixture.Target.CellCode);
        fixture.Target.RunStatus.ShouldBe(CellRunStatus.Selected);
    }

    /// <summary>未传目标仍沿用原有自动分配能力。</summary>
    [Fact]
    public async Task Reservation_without_target_allocates_an_available_cell()
    {
        var fixture = new ReservationFixture();
        var result = await fixture.Service.CreateWCSIn(fixture.Request());

        var order = await fixture.Manager.PrepareWcsStockOrderAsync(result.Id);

        order.EndNode.ShouldBe(fixture.Target.CellCode);
    }

    /// <summary>编码不存在、ID 与编码不一致时不得创建物料或任务。</summary>
    [Theory]
    [InlineData("9-9-9", 0)]
    [InlineData("1-2-1", 1)]
    public async Task Invalid_target_is_rejected_before_creating_records(string code, int id)
    {
        var fixture = new ReservationFixture();

        await Should.ThrowAsync<UserFriendlyException>(() => fixture.Service.CreateWCSIn(fixture.Request(code, id)));

        fixture.Boxes.ShouldBeEmpty();
        fixture.Tasks.ShouldBeEmpty();
    }

    /// <summary>指定库位必须为空闲货架且规格匹配。</summary>
    [Theory]
    [InlineData("occupied")]
    [InlineData("locked")]
    [InlineData("disabled")]
    [InlineData("status")]
    [InlineData("model")]
    [InlineData("station")]
    public async Task Unavailable_target_is_rejected(string scenario)
    {
        var fixture = new ReservationFixture();
        switch (scenario)
        {
            case "occupied": fixture.Target.MaterialCode = "OTHER-BARCODE"; break;
            case "locked": fixture.Target.SetRunStatus(CellRunStatus.Selected.ToString()); break;
            case "disabled": fixture.Target.SetRunStatus(CellRunStatus.Disable.ToString()); break;
            case "status": fixture.Target.SetCellStatus(CellStatus.Have.ToString()); break;
            case "model": fixture.Target.CellModel = "OtherType"; break;
            case "station": fixture.Target.CellType = CellType.Station; break;
        }

        await Should.ThrowAsync<UserFriendlyException>(() => fixture.Service.CreateWCSIn(fixture.Request("1-2-1")));

        fixture.Tasks.ShouldBeEmpty();
    }

    /// <summary>指定库位预约后被其他任务锁定，下发时仍须重新校验。</summary>
    [Fact]
    public async Task Target_reserved_after_appointment_is_rechecked_before_dispatch()
    {
        var fixture = new ReservationFixture();
        var result = await fixture.Service.CreateWCSIn(fixture.Request("1-2-1"));
        fixture.Target.SetRunStatus(CellRunStatus.Selected.ToString());

        await Should.ThrowAsync<UserFriendlyException>(() => fixture.Manager.PrepareWcsStockOrderAsync(result.Id));

        fixture.Tasks.Single().TaskStatus.ShouldBe(StockTaskStatus.WaitingExecute);
    }

    /// <summary>借出完成后可复用原物料记录回库，历史任务及有效期起算时间保留。</summary>
    [Fact]
    public async Task Borrowed_barcode_can_return_without_recreating_material_or_resetting_age()
    {
        var fixture = new ReservationFixture();
        var originalInDate = "2026-01-01 10:00:00";
        var box = fixture.AddBox();
        box.CellId = fixture.Target.Id;
        box.MaterialInDate = originalInDate;
        fixture.Target.MaterialCode = box.MaterialBoxBarcode;
        fixture.Target.SetCellStatus(CellStatus.Have.ToString());
        var outbound = fixture.AddTask(TaskType.HPSortStockOut, StockTaskStatus.Executing);
        outbound.StartCellId = fixture.Target.Id;
        outbound.EndCellId = fixture.Station.Id;

        await fixture.Complete(outbound);
        box.CellId.ShouldBe(0);
        box.BorrowFlag.ShouldBe(1);
        box.MaterialOutTime.ShouldNotBeNull();

        var inbound = await fixture.Service.CreateWCSIn(fixture.Request("1-2-1"));
        fixture.Boxes.Count.ShouldBe(1);
        fixture.Boxes.Single().ShouldBeSameAs(box);
        fixture.Tasks.Count.ShouldBe(2);
        outbound.TaskStatus.ShouldBe(StockTaskStatus.Complete);
        box.MaterialInDate.ShouldBe(originalInDate);
        box.MaterialOutTime.ShouldNotBeNull();
        await fixture.Manager.PrepareWcsStockOrderAsync(inbound.Id);
        await fixture.Complete(fixture.Tasks.Single(task => task.Id == inbound.Id));

        box.CellId.ShouldBe(fixture.Target.Id);
        box.BorrowFlag.ShouldBe(0);
        box.MaterialOutTime.ShouldBeNull();
        box.MaterialInDate.ShouldBe(originalInDate);
        fixture.Target.MaterialCode.ShouldBe(box.MaterialBoxBarcode);
        fixture.Target.CellStatus.ShouldBe(CellStatus.Have);
        fixture.Target.RunStatus.ShouldBe(CellRunStatus.Enable);
        fixture.Tasks.All(task => task.TaskStatus == StockTaskStatus.Complete).ShouldBeTrue();
    }

    /// <summary>在库、未完成出库、已有活动任务及异常完成条码不能重复预约。</summary>
    [Theory]
    [InlineData("inStock")]
    [InlineData("neverOut")]
    [InlineData("active")]
    [InlineData("exceptionComplete")]
    [InlineData("differentMaterial")]
    public async Task Existing_barcode_can_only_return_after_confirmed_stock_out(string scenario)
    {
        var fixture = new ReservationFixture();
        var box = fixture.AddBox();
        box.SetStockOut(true);
        switch (scenario)
        {
            case "inStock": box.CellId = fixture.Target.Id; break;
            case "neverOut": box.MaterialOutTime = null; break;
            case "active": fixture.AddTask(TaskType.NPFullStockIn, StockTaskStatus.WaitingExecute); break;
            case "exceptionComplete": fixture.AddTask(TaskType.NPFullStockIn, StockTaskStatus.ExceptionComplete); break;
            case "differentMaterial": box.Details.Clear(); box.AddDetail(box.Id, 99); break;
        }
        var taskCount = fixture.Tasks.Count;

        await Should.ThrowAsync<UserFriendlyException>(() => fixture.Service.CreateWCSIn(fixture.Request("1-2-1")));

        fixture.Boxes.Count.ShouldBe(1);
        fixture.Tasks.Count.ShouldBe(taskCount);
    }

    /// <summary>一体机直接创建任务也不能绕过在库或活动条码限制。</summary>
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Direct_inbound_creation_rejects_existing_inventory_or_active_task(bool inStock)
    {
        var fixture = new ReservationFixture();
        var box = fixture.AddBox();
        if (inStock) box.CellId = fixture.Target.Id;
        else fixture.AddTask(TaskType.NPFullStockIn, StockTaskStatus.WaitingExecute);

        await Should.ThrowAsync<UserFriendlyException>(() => fixture.Manager.CreateWCSIn(TaskType.NPFullStockIn.ToString(), box));

        fixture.Tasks.Count.ShouldBe(inStock ? 0 : 1);
    }

    /// <summary>一体机已把物料绑定到柜门时，保留该柜门作为合法入库起点。</summary>
    [Fact]
    public async Task Direct_inbound_preserves_bound_station_as_start()
    {
        var fixture = new ReservationFixture();
        var box = fixture.AddBox();
        box.CellId = fixture.Station.Id;
        fixture.Station.MaterialCode = box.MaterialBoxBarcode;

        var task = await fixture.Manager.CreateWCSIn(TaskType.NPFullStockIn.ToString(), box);
        var order = await fixture.Manager.PrepareWcsStockOrderAsync(task.Id);

        task.StartCellId.ShouldBe(fixture.Station.Id);
        order.StartNode.ShouldBe(fixture.Station.CellCode);
        order.EndNode.ShouldBe(fixture.Target.CellCode);
    }

    /// <summary>同条码历史任务仍保留时，扫码派发本次等待执行的回库预约。</summary>
    [Fact]
    public async Task Scan_dispatches_current_reservation_instead_of_completed_barcode_history()
    {
        var fixture = new ReservationFixture();
        fixture.AddBox().SetStockOut(true);
        var oldTask = fixture.AddTask(TaskType.NPFullStockIn, StockTaskStatus.Complete);
        var reservation = await fixture.Service.CreateWCSIn(fixture.Request("1-2-1"));

        (await fixture.Service.ScanAndDispatchToWCS("BARCODE-1")).ShouldBeTrue();

        oldTask.TaskStatus.ShouldBe(StockTaskStatus.Complete);
        fixture.Tasks.Single(task => task.Id == reservation.Id).TaskStatus.ShouldBe(StockTaskStatus.OrderCatched);
        fixture.Tasks.Single(task => task.Id == reservation.Id).EndCellId.ShouldBe(fixture.Target.Id);
    }

    /// <summary>历史原库位已占用时，自动回库必须选择其他空闲库位。</summary>
    [Fact]
    public async Task Return_does_not_reuse_an_occupied_previous_cell()
    {
        var fixture = new ReservationFixture();
        var box = fixture.AddBox();
        box.SetStockOut(false);
        var previousCell = fixture.AddCell(3, "1-3-1");
        previousCell.MaterialCode = "OTHER-BARCODE";
        previousCell.SetCellStatus(CellStatus.Have.ToString());
        var outbound = fixture.AddTask(TaskType.NpFullStockOut, StockTaskStatus.Complete);
        outbound.StartCellId = previousCell.Id;
        var result = await fixture.Service.CreateWCSIn(fixture.Request());

        var order = await fixture.Manager.PrepareWcsStockOrderAsync(result.Id);

        order.EndNode.ShouldBe(fixture.Target.CellCode);
        previousCell.MaterialCode.ShouldBe("OTHER-BARCODE");
    }

    /// <summary>使用真实应用与领域服务，仓储以集合替代以隔离数据库和 WCS 设备。</summary>
    private sealed class ReservationFixture
    {
        /// <summary>本次测试的物料记录，复用检查基于对象标识。</summary>
        public readonly List<MaterialBox> Boxes = new();
        /// <summary>本次测试的历史及当前出入库任务。</summary>
        public readonly List<StockTask> Tasks = new();
        /// <summary>本次测试的货架和柜门点位。</summary>
        private readonly List<Cell> _cells = new();
        /// <summary>实际执行预约业务的应用服务。</summary>
        public StockTaskAppService Service { get; }
        /// <summary>实际执行下发及回调业务的领域服务。</summary>
        public StockTaskManager Manager { get; }
        /// <summary>指定入库货架。</summary>
        public Cell Target { get; }
        /// <summary>供自动分配使用的柜门。</summary>
        public Cell Station { get; }

        /// <summary>配置内存仓储、对象映射和事务边界，业务方法不做替换。</summary>
        public ReservationFixture()
        {
            Target = AddCell(2, "1-2-1");
            Station = AddCell(1, "1-1-1", CellType.Station);
            var cellRepository = new Mock<ICellRepository>();
            cellRepository.Setup(repository => repository.FindByIdAsync(It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
                .Returns((int id, bool details, CancellationToken token) => Task.FromResult(_cells.SingleOrDefault(cell => cell.Id == id)));
            cellRepository.Setup(repository => repository.FindByCodeAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
                .Returns((string code, bool details, CancellationToken token) => Task.FromResult(_cells.SingleOrDefault(cell => cell.CellCode == code)));
            cellRepository.Setup(repository => repository.GetListAsync(It.IsAny<Expression<Func<Cell, bool>>>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
                .Returns((Expression<Func<Cell, bool>> predicate, bool details, CancellationToken token) => Task.FromResult(_cells.Where(predicate.Compile()).ToList()));
            cellRepository.Setup(repository => repository.UpdateAsync(It.IsAny<Cell>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
                .Returns((Cell cell, bool save, CancellationToken token) => Task.FromResult(cell));
            var settings = new Mock<ISettingAppService>();
            settings.Setup(service => service.GetSetting(It.IsAny<string>())).ReturnsAsync("false");
            var cellOptions = new Mock<IOptionsSnapshot<AllocationRulesOptions>>();
            cellOptions.SetupGet(options => options.Value).Returns(new AllocationRulesOptions());
            var cellManager = new CellManager(cellRepository.Object, cellOptions.Object, settings.Object);

            var boxRepository = new Mock<IMaterialBoxRepository>();
            boxRepository.Setup(repository => repository.FindByMaterialBoxcodeAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
                .Returns((string barcode, bool details, CancellationToken token) => Task.FromResult(Boxes.SingleOrDefault(box => box.MaterialBoxBarcode == barcode)));
            boxRepository.Setup(repository => repository.FindByRfidCodeAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
                .Returns((string barcode, bool details, CancellationToken token) => Task.FromResult(Boxes.SingleOrDefault(box => box.MaterialBoxBarcode == barcode)));
            boxRepository.Setup(repository => repository.InsertAsync(It.IsAny<MaterialBox>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
                .Returns((MaterialBox box, bool save, CancellationToken token) => { SetId(box, Boxes.Count + 1); Boxes.Add(box); return Task.FromResult(box); });
            boxRepository.Setup(repository => repository.UpdateAsync(It.IsAny<MaterialBox>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
                .Returns((MaterialBox box, bool save, CancellationToken token) => Task.FromResult(box));
            var boxManager = new MaterialBoxManager(boxRepository.Object);
            var taskRepository = new Mock<IStockTaskRepository>();
            taskRepository.Setup(repository => repository.GetListAsync(It.IsAny<Expression<Func<StockTask, bool>>>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
                .Returns((Expression<Func<StockTask, bool>> predicate, bool details, CancellationToken token) => Task.FromResult(Tasks.Where(predicate.Compile()).ToList()));
            taskRepository.Setup(repository => repository.FindByIdAsync(It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
                .Returns((int id, bool details, CancellationToken token) => Task.FromResult(Tasks.SingleOrDefault(task => task.Id == id)));
            taskRepository.Setup(repository => repository.FindByBarcodeAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
                .Returns((string barcode, bool details, CancellationToken token) => Task.FromResult(Tasks.FirstOrDefault(task => task.MaterialBoxBarcode == barcode)));
            taskRepository.Setup(repository => repository.InsertAsync(It.IsAny<StockTask>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
                .Returns((StockTask task, bool save, CancellationToken token) => { SetId(task, Tasks.Count + 1); Tasks.Add(task); return Task.FromResult(task); });
            taskRepository.Setup(repository => repository.UpdateAsync(It.IsAny<StockTask>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
                .Returns((StockTask task, bool save, CancellationToken token) => Task.FromResult(task));
            taskRepository.Setup(repository => repository.LockStatusUpdateAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            var planRepository = new Mock<IPlanRepository>();
            planRepository.Setup(repository => repository.GetListAsync(It.IsAny<Expression<Func<Plan, bool>>>(), It.IsAny<bool>(), It.IsAny<CancellationToken>())).ReturnsAsync(new List<Plan>());
            var planManager = new PlanManager(planRepository.Object, null, cellRepository.Object, null, cellManager, null);
            var fbqOptions = new Mock<IOptionsSnapshot<FbqOptions>>();
            fbqOptions.SetupGet(options => options.Value).Returns(new FbqOptions());
            var dataFilter = new Mock<Volo.Abp.Data.IDataFilter>();
            dataFilter.Setup(filter => filter.Disable<ISoftDelete>()).Returns(Mock.Of<IDisposable>());
            var wcsOptions = new Mock<IOptionsSnapshot<WCSOptions>>();
            wcsOptions.SetupGet(options => options.Value).Returns(new WCSOptions { WCSSimulation = true });
            var wcsManager = new WcsApiManager(null, wcsOptions.Object);
            Manager = new StockTaskManager(taskRepository.Object, cellRepository.Object, null, cellManager, null, null,
                fbqOptions.Object, wcsManager, boxManager, null, planManager, null, dataFilter.Object);
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(work => work.CompleteAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            var unitOfWorkManager = new Mock<IUnitOfWorkManager>();
            unitOfWorkManager.Setup(manager => manager.Begin(It.IsAny<AbpUnitOfWorkOptions>(), It.IsAny<bool>())).Returns(unitOfWork.Object);
            var material = new MaterialEntity { MaterialCode = "MAT-1", MaterialName = "物料一", MaterialType = "Inch3", MaterialUnit = "g", ValidityDays = 365 };
            SetId(material, 10);
            var materialRepository = new Mock<IMaterialRepository>();
            materialRepository.Setup(repository => repository.FindByMaterialCodeAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
                .Returns((string code, bool details, CancellationToken token) => Task.FromResult(code == material.MaterialCode ? material : null));
            var mapper = new Mock<IObjectMapper<WarehouseManagementApplicationModule>>();
            mapper.Setup(value => value.Map<StockTask, StockTaskDto>(It.IsAny<StockTask>())).Returns((StockTask task) => new StockTaskDto
            {
                Id = task.Id, EndCellId = task.EndCellId ?? 0, EndCellCode = task.EndCellCode,
                MaterialBoxBarcode = task.MaterialBoxBarcode, TaskStatus = task.TaskStatus
            });
            var provider = new ServiceCollection().AddSingleton(mapper.Object).AddSingleton(unitOfWorkManager.Object).BuildServiceProvider();
            Manager.LazyServiceProvider = new AbpLazyServiceProvider(provider);
            Service = new StockTaskAppService(taskRepository.Object, Manager, planManager, null, null, cellRepository.Object,
                boxRepository.Object, cellManager, planRepository.Object, wcsManager, boxManager, materialRepository.Object, unitOfWorkManager.Object, null)
            {
                LazyServiceProvider = new AbpLazyServiceProvider(provider)
            };
        }

        /// <summary>生成最小预约请求，可按需指定目标编码或主键。</summary>
        public CreateStockInTaskDto Request(string endCellCode = null, int endCellId = 0) => new()
        {
            MaterialCode = "MAT-1", MaterialBarcode = "BARCODE-1", MaterialCreateTime = "2026-01-01 10:00:00",
            EndCellCode = endCellCode, EndCellId = endCellId
        };

        /// <summary>登记一个与基础物料关联的实物条码。</summary>
        public MaterialBox AddBox()
        {
            var box = new MaterialBox("物料一", "BARCODE-1") { CellModel = "Inch3", MaterialUnit = "g", RetentionPeriod = "365" };
            SetId(box, Boxes.Count + 1);
            box.AddDetail(box.Id, 10);
            Boxes.Add(box);
            return box;
        }

        /// <summary>增加一个当前或历史任务，保留原对象供断言。</summary>
        public StockTask AddTask(TaskType type, StockTaskStatus status)
        {
            var task = new StockTask(type.ToString(), "BARCODE-1") { TaskStatus = status };
            SetId(task, Tasks.Count + 1);
            Tasks.Add(task);
            return task;
        }

        /// <summary>增加满足当前仓库配置的空闲点位。</summary>
        public Cell AddCell(int id, string code, CellType type = CellType.Cell)
        {
            var cell = new Cell(code, type.ToString(), code, 1) { CellModel = "Inch3" };
            SetId(cell, id);
            _cells.Add(cell);
            return cell;
        }

        /// <summary>通过公开 WCS 回调处理完整库存状态变更。</summary>
        public Task<ResultWcsTaskDto> Complete(StockTask task) => Manager.WcsCallBack(new WcsCallBackRequest
        {
            OrderCode = task.Id.ToString(), Status = WcsTaskStatus.Completed
        });

        /// <summary>为内存仓储模拟数据库生成的整数主键。</summary>
        private static void SetId(Entity<int> entity, int id) => typeof(Entity<int>).GetProperty(nameof(Entity<int>.Id), BindingFlags.Public | BindingFlags.Instance).SetValue(entity, id);
    }
}
