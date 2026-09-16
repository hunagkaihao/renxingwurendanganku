using System.Reflection;
using System.Linq.Expressions;
using System.Collections.Concurrent;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Uow;
using Wcs.Cells;
using Wcs.Cells.Models;
using Wcs.ConfigTool;
using Wcs.Dispatch;
using Wcs.Jobs.JobCmds;
using Wcs.Jobs.JobWorker;
using Wcs.Jobs.Models;
using Wcs.Nodes;
using Wcs.Nodes.Models;
using Wcs.Orders;
using Wcs.Orders.Models;
using Wcs.PlcTool;
using Wcs.Processes.Models;
using Wcs.RedisTool;
using Wcs.Tasks;

int checks = 0;
void Check(bool value, string reason) { if (!value) throw new Exception(reason); checks++; }
void Reject(Action action, string reason)
{
    try { action(); } catch (ArgumentException) { checks++; return; }
    throw new Exception(reason);
}
IRepository<T, int> Repo<T>(out RepoProbe<T> probe) where T : class, Volo.Abp.Domain.Entities.IEntity<int>
{
    var repo = DispatchProxy.Create<IRepository<T, int>, RepoProbe<T>>();
    probe = (RepoProbe<T>)repo; return repo;
}
DoorConfiguration Doors(params string[] codes) => new(Options.Create(new ConfigOptions { DoorCodes = codes.ToList() }));
var options = new ConfigOptions { PlcSimulation = true, PlcSimulationDelayMs = 50, DoorCodes = Enumerable.Range(1, 8).Select(i => $"12{i:D3}").ToList() };
Check(new DoorConfiguration(Options.Create(options)).Codes.Count == 8, "Original eight doors by default");
options.DoorCodes.AddRange(new[] { "12009", "12010", "12024" });
var doors = new DoorConfiguration(Options.Create(options));
Check(doors.Codes.Count == 11 && doors.Contains("12009") && doors.Contains("12024"), "Config controls count; sparse door numbers supported");
Check(!Doors("12009").Contains("12001"), "List is authoritative, not aliases for eight doors");
Check(DoorConfiguration.CommandTag("12010") == "Door10_Cmd", "Two digit door tag");
Reject(() => Doors("12009", "12009"), "Duplicates rejected");
foreach (string invalid in new[] { "", "12000", "120010", "00-001-01", "13001", " 12009" })
    Reject(() => Doors(invalid), "Invalid code rejected");
Reject(() => Doors("12009").RequireDoor("12001"), "Unconfigured door rejected");
Check(Doors().Codes.Count == 0, "Explicit empty list enables no doors");

string yaml = Path.Combine(AppContext.BaseDirectory, "door-codes-test.tmp");
try
{
    foreach (var sample in new[] { ("Wcs:\n  DoorCodes:\n    - \"12009\"\n    - \"12010\"\n", 2), ("Wcs:\n  DoorCodes: []\n", 0), ("Wcs:\n  PlcSimulation: true\n", 0) })
    {
        File.WriteAllText(yaml, sample.Item1);
        var bound = new ConfigOptions();
        var root = new ConfigurationBuilder().AddYamlFile(yaml).Build();
        root.GetSection("Wcs").Bind(bound);
        Check(new DoorConfiguration(Options.Create(bound)).Codes.Count == sample.Item2, "YAML list replaces defaults, including empty list");
    }
}
finally { File.Delete(yaml); }

var nodeRepo = Repo<DispatchNode>(out var nodeData);
nodeData.Items.AddRange(new[] {
    new DispatchNode(1) { NodeCode = "12001", NodeTypeCode = "12", DASpecs = "_5cm", CmdTagName = "Plc1.Door1_Cmd", ResponseTagName = "Plc1.Door1_Response", TaskIdOwnIt = 4321 },
    new DispatchNode(9) { NodeCode = "13001" }, new DispatchNode(10) { NodeCode = "15001" }, new DispatchNode(17) { NodeCode = "18001" }
});
var processRepo = Repo<DispatchProcess>(out var processData);
processData.Items.AddRange(new[] { new DispatchProcess(11, "12001", "15001"), new DispatchProcess(12, "15001", "12001") });
var stepRepo = Repo<DispatchProcessStep>(out var stepData);
// A customized existing inbound process includes door authorization: it must be copied, not replaced by a stock template.
stepData.Items.AddRange(new[] {
    new DispatchProcessStep(221) { ProcessId = 11, Sequence = 1, NodeCode = "12001", JobCmdId = 1, JobWorkerId = 1, NextTrueStep = 2, Describe = "现有入库授权开门" },
    new DispatchProcessStep(222) { ProcessId = 11, Sequence = 2, NodeCode = "13001", JobCmdId = 7, JobWorkerId = 1, NextTrueStep = 0 },
    new DispatchProcessStep(241) { ProcessId = 12, Sequence = 1, NodeCode = "12001", JobCmdId = 1, JobWorkerId = 1, NextTrueStep = 0 }
});
var conditionRepo = Repo<DispatchProcessStepPrecondition>(out var conditionData);
conditionData.Items.Add(new DispatchProcessStepPrecondition { ProcessId = 11, Sequence = 2, ConditionName = "Plc1.Lm_State", ConditionValue = "1" });
var resourceRepo = Repo<DispatchProcessStepResource>(out var resourceData);
resourceData.Items.Add(new DispatchProcessStepResource { ProcessId = 11, Sequence = 1, Resource = "12001,13001" });
var uow = DispatchProxy.Create<IUnitOfWorkManager, UnitManagerProbe>();
var initializer = new DoorConfigurationInitializer(doors, nodeRepo, processRepo, stepRepo, conditionRepo, resourceRepo, uow);
await initializer.EnsureAsync();
Check(nodeData.Items.Count(n => n.NodeTypeCode == "12") == 11, "Missing doors provisioned without deleting existing nodes");
Check(nodeData.Items.Single(n => n.NodeCode == "12001").TaskIdOwnIt == 4321, "Existing resource ownership unchanged");
Check(nodeData.Items.Single(n => n.NodeCode == "12001").DASpecs == "_5cm", "Existing specs unchanged");
Check(nodeData.Items.Single(n => n.NodeCode == "12010").CmdTagName == "Plc1.Door10_Cmd", "New node has correct PLC point");
Check(processData.Items.Count == 22, "Each door has inbound and outbound processes");
var inbound9 = processData.Items.Single(p => p.StartNodeCode == "12009" && p.EndNodeCode == "15001");
var newSteps = stepData.Items.Where(s => s.ProcessId == inbound9.Id).OrderBy(s => s.Sequence).ToList();
Check(newSteps.Count == 2 && newSteps[0].JobCmdId == 1 && newSteps[0].NextTrueStep == 2 && newSteps[1].JobCmdId == 7, "New door retains existing authorized business steps and transitions");
Check(newSteps[0].NodeCode == "12009" && newSteps[1].NodeCode == "13001", "Only door node reference changed");
Check(resourceData.Items.Any(r => r.ProcessId == inbound9.Id && r.Resource == "12009,13001"), "Resource references remapped");
Check(conditionData.Items.Any(c => c.ProcessId == inbound9.Id && c.ConditionName == "Plc1.Lm_State" && c.ConditionValue == "1"), "Preconditions preserved");
Check(stepData.Items.Single(s => s.Id == 221).NodeCode == "12001", "Original process untouched");
int writes = nodeData.Inserts + processData.Inserts + stepData.Inserts + conditionData.Inserts + resourceData.Inserts;
await initializer.EnsureAsync();
Check(writes == nodeData.Inserts + processData.Inserts + stepData.Inserts + conditionData.Inserts + resourceData.Inserts, "Repeated startup inserts nothing");
await new DoorConfigurationInitializer(Doors("12009"), nodeRepo, processRepo, stepRepo, conditionRepo, resourceRepo, uow).EnsureAsync();
Check(nodeData.Items.Any(n => n.NodeCode == "12001") && processData.Items.Any(p => p.Id == 11), "Removing config retains history and in-flight resources");

// With no prior process, generate the repository's original templates.
var emptyProcesses = Repo<DispatchProcess>(out var fallbackProcesses);
var emptySteps = Repo<DispatchProcessStep>(out var fallbackSteps);
var emptyConditions = Repo<DispatchProcessStepPrecondition>(out _);
var emptyResources = Repo<DispatchProcessStepResource>(out _);
await new DoorConfigurationInitializer(Doors("12009"), nodeRepo, emptyProcesses, emptySteps, emptyConditions, emptyResources, uow).EnsureAsync();
Check(fallbackProcesses.Items.Count == 2 && fallbackSteps.Items.Count == 9, "Original four-step inbound and five-step outbound templates reused");

var nodes = new NodeManager(nodeRepo, null, null, null, NullLogger<NodeManager>.Instance);
var cells = DispatchProxy.Create<ICellRepository, CellProbe>();
var orders = new OrderService(NullLogger<OrderService>.Instance, null, cells, nodes, null, null, null, null, doors);
foreach (bool batch in new[] { false, true })
foreach (bool inbound in new[] { true, false })
{
    var request = new AddStockOrderDto { orderCode = "test", plateCode = "123", startNode = inbound ? "12009" : "01-001-01", endNode = inbound ? "01-001-01" : "12010" };
    var result = batch ? await orders.CreateStockOrders(new AddStockOrdersDto { stockOrders = new() { request } }) : await orders.CreateStockOrder(request);
    Check(!result.success && result.message.Contains(inbound ? "入库任务的终点库位" : "出库任务的起点库位"), "Existing order business branch recognizes new doors");
}
var disabled = await orders.CreateStockOrder(new AddStockOrderDto { orderCode = "bad", plateCode = "123", startNode = "12011", endNode = "01-001-01" });
Check(!disabled.success && disabled.message.Contains("未在"), "New task for unconfigured door rejected");

var deviceRedis = DispatchProxy.Create<IRedisClient, RedisProbe>();
var wcsRedis = DispatchProxy.Create<IRedisClient, RedisProbe>();
using var plc = new PlcHelper(Options.Create(options), NullLogger<PlcHelper>.Instance, deviceRedis, wcsRedis);
var device = new DeviceService(null, null, nodes, plc, NullLogger<DeviceService>.Instance, doors);
Check(device.OpenDoorAsync("12009").success, "Direct API accepts configured ninth door");
Check(!device.OpenDoorAsync("12011").success, "Direct API rejects omitted door");
foreach (string code in new[] { "12009", "12010", "12024" })
{
    byte[] frame = new byte[12]; frame[1] = 10; frame[3] = (byte)DoorConfiguration.Number(code);
    Check(plc.WritePlcTag("Plc1", DoorConfiguration.CommandTag(code), Encoding.Latin1.GetString(frame)), "Dynamic door command accepted");
    var deadline = DateTime.UtcNow.AddSeconds(3);
    while (Encoding.Latin1.GetBytes(plc.ReadPlcTag("Plc1", DoorConfiguration.ResponseTag(code)).Value)[5] != 2 && DateTime.UtcNow < deadline) await Task.Delay(10);
    Check(Encoding.Latin1.GetBytes(plc.ReadPlcTag("Plc1", DoorConfiguration.ResponseTag(code)).Value)[5] == 2, "Dynamic door completes");
    Check((await device.GetDoorStateAsync(code)).success, "Dynamic direct door status available");
}
Check(plc.ReadPlcTag("Plc1", "Status_12001").Value == "False", "Two-digit door completion must not update door 1");
Check(!plc.IsPlcTagExist("Plc1", "Door11_Cmd").Value, "Unconfigured simulated point absent");

// Existing production OpenDoorCmd still enforces WMS inbound authorization; outbound does not need it.
var cmds = Repo<DispatchNodeCmd>(out var cmdData);
cmdData.Items.Add(new DispatchNodeCmd("12", Wcs.WcsConsts.NodeType_DoorOpen, 10));
nodes = new NodeManager(nodeRepo, null, cmds, null, NullLogger<NodeManager>.Instance);
var orderRepo = Repo<DispatchOrder>(out var orderData);
var inboundOrder = new DispatchOrder("in", EnumDispatchOrderType.StockIn, "12009", "01-001-01", 1);
orderData.Items.Add(inboundOrder);
orderData.Items.Add(new DispatchOrder("out", EnumDispatchOrderType.StockOut, "01-001-01", "12010", 1));
var manager = new OrderManager(orderRepo, null, null, null, uow, NullLogger<TaskManager>.Instance);
var helper = new JobCmdHelper(Options.Create(options), manager, null, null, null, null, nodes, null, null, null, plc);
var openDoor = new OpenDoorCmd(NullLogger<OpenDoorCmd>.Instance, plc, null, manager, nodes, helper);
var worker = new TestWorker { MyJob = new DispatchJob(950) { NodeCode = "12009", OrderCode = "in" } };
openDoor.Owner = worker;
Check(!openDoor.SendCmdValue().IsOK, "Inbound authorization still required");
inboundOrder.SetCanOpenDoorImmediate(true);
Check(openDoor.SendCmdValue().IsOK, "Authorized inbound opens new door");
worker.MyJob = new DispatchJob(951) { NodeCode = "12010", OrderCode = "out" };
Check(openDoor.SendCmdValue().IsOK, "Outbound opens new door without extra WMS authorization");
Check(((RedisProbe)deviceRedis).Calls.Count == 0, "No PLCServer connection in simulation");
Console.WriteLine($"PASS: {checks} assertions; dynamic list, YAML, additive initialization, preserved workflows/resources, door 9/10/24, original authorization and PLC simulation.");

public class RepoProbe<T> : DispatchProxy
{
    public List<T> Items { get; } = new();
    public int Inserts { get; private set; }
    protected override object Invoke(MethodInfo method, object[] args)
    {
        if (method.Name == "GetListAsync")
        {
            var predicate = args.OfType<Expression<Func<T, bool>>>().FirstOrDefault();
            return Task.FromResult(predicate == null ? Items.ToList() : Items.Where(predicate.Compile()).ToList());
        }
        if (method.Name == "InsertAsync") { Inserts++; Items.Add((T)args[0]); return Task.FromResult((T)args[0]); }
        throw new NotSupportedException(method.Name);
    }
}
public class UnitManagerProbe : DispatchProxy
{
    protected override object Invoke(MethodInfo method, object[] args) => method.Name == "Begin"
        ? DispatchProxy.Create<IUnitOfWork, UnitProbe>() : throw new NotSupportedException(method.Name);
}
public class UnitProbe : DispatchProxy
{
    protected override object Invoke(MethodInfo method, object[] args) => method.Name switch
    { "CompleteAsync" => Task.CompletedTask, "Dispose" => null, _ => throw new NotSupportedException(method.Name) };
}
public class CellProbe : DispatchProxy
{
    protected override object Invoke(MethodInfo method, object[] args) => method.Name == "FindByCellCodeAsync"
        ? Task.FromResult<DispatchCell>(null) : throw new NotSupportedException(method.Name);
}
public class RedisProbe : DispatchProxy
{
    public ConcurrentQueue<string> Calls { get; } = new();
    protected override object Invoke(MethodInfo method, object[] args)
    {
        Calls.Enqueue(method.Name);
        if (method.ReturnType == typeof(void)) return null;
        if (method.ReturnType == typeof(string[])) return Array.Empty<string>();
        throw new NotSupportedException(method.Name);
    }
}
public class TestWorker : IJobWorker
{
    public DispatchJob MyJob { get; set; }
    public IJobCmd MyJobCmd { get; set; }
    public Task Execute() => throw new NotSupportedException();
    public void ForceDone() => throw new NotSupportedException();
    public void RedoCurStep() => throw new NotSupportedException();
    public void ForceDoneCurStep() => throw new NotSupportedException();
    public string GenerateLog(string content) => content;
}
