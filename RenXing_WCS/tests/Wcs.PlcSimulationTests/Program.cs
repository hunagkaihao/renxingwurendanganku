using System.Collections.Concurrent;
using System.Reflection;
using System.Text;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Wcs.ConfigTool;
using Wcs.PlcTool;
using Wcs.RedisTool;

int assertions = 0;
void Check(bool condition, string message)
{
    if (!condition) throw new Exception(message);
    assertions++;
}
async Task Until(Func<bool> condition, string message)
{
    var end = DateTime.UtcNow.AddSeconds(4);
    while (!condition() && DateTime.UtcNow < end) await Task.Delay(10);
    Check(condition(), message);
}
string Command(int number, int job, int length = 26)
{
    var bytes = new byte[length];
    bytes[0] = (byte)(number >> 8); bytes[1] = (byte)number;
    bytes[2] = (byte)(job >> 8); bytes[3] = (byte)job;
    return Encoding.Latin1.GetString(bytes);
}
byte[] Response(PlcHelper helper, string tag = "Lm_Response") => Encoding.Latin1.GetBytes(helper.ReadPlcTag("Plc1", tag).Value);
var device = DispatchProxy.Create<IRedisClient, RedisProbe>();
var business = DispatchProxy.Create<IRedisClient, RedisProbe>();
var deviceProbe = (RedisProbe)device;
var options = new ConfigOptions { PlcSimulation = true, PlcSimulationDelayMs = 100, RemovePlcTagTempValueOnStart = true, DoorCodes = Enumerable.Range(1, 8).Select(i => $"12{i:D3}").ToList() };
using var plc = new PlcHelper(Options.Create(options), NullLogger<PlcHelper>.Instance, device, business);
Check(plc.IsSimulation, "Simulation enabled");
Check(deviceProbe.Calls.Count == 0, "Simulation must not connect/register to PLC Redis");
Check(plc.ReadPlcTag("Plc1", "Lm_State").Value == "1", "Ready condition");
Check(plc.ReadPlcTag("Plc1", "Lm_Zero").Value == "1", "Home condition");
Check(plc.IsPlcTagExist("OtherPlc", "Lm_State") == false, "Unknown PLC rejected");
Check(plc.IsPlcTagExist("Plc1", "Typo") == false, "Unknown point rejected");
Check(!plc.WritePlcTag("Plc1", "Lm_Cmd", "short"), "Malformed frame rejected");
Check(!plc.WritePlcTag("Plc1", "Lm_Cmd", Command(99, 1)), "Unknown command rejected");
var events = new ConcurrentQueue<string>();
Check(await plc.SubscribeAsync("Plc1", "Lm_State", (_, _, value) => events.Enqueue(value.Value)), "Async subscription");
Check(plc.Subscribe("Plc1", "Lm_Response", (_, _, _) => { }), "Sync subscription");

// Exercise all ordinary commands in order: stock in, stock out, relocation, avoid, home.
int job = 300;
foreach (int command in new[] { 7, 8, 2, 3, 11, 12, 5, 1 })
{
    job++;
    string frame = Command(command, job);
    Check(await plc.WritePlcTagAsync("Plc1", "Lm_Cmd", frame), $"Command {command} accepted");
    Check(Response(plc)[5] == 1, "Executing is not completed immediately");
    Check(plc.ReadPlcTag("Plc1", "Lm_State").Value == "0", "Device busy");
    Check(plc.WritePlcTag("Plc1", "Lm_Cmd", frame), "In-flight retry is idempotent");
    Check(!plc.WritePlcTag("Plc1", "Lm_Cmd", Command(command, job + 1)), "Cannot overwrite running job");
    await Until(() => Response(plc)[5] == 2, $"Command {command} completed");
    byte[] response = Response(plc);
    Check(response.Length == 12 && response[1] == command && ((response[2] << 8) | response[3]) == job, "Response belongs to exact job and command");
    Check((await plc.ReadPlcTagAsync("Plc1", "Lm_State")).Value == "1", "Next step ready");
}
Check(plc.ReadPlcTag("Plc1", "Lm_Zero").Value == "1" && plc.ReadPlcTag("Plc1", "Lm_SafePos").Value == "1", "Home restores ready positions");
Check(events.Contains("0") && events.Contains("1"), "Monitors receive state changes");
var copy = plc.ReadPlcTag("Plc1", "Lm_State"); copy.Value = "corrupt";
Check(plc.ReadPlcTag("Plc1", "Lm_State").Value == "1", "Read snapshot isolated");

for (int door = 1; door <= 8; door++)
{
    Check(plc.WritePlcTag("Plc1", $"Door{door}_Cmd", Command(10, 500 + door, 12)), "Door command accepted");
    await Until(() => Response(plc, $"Door{door}_Response")[5] == 2, "Door command completes");
    Check(plc.ReadPlcTag("Plc1", $"Status_1200{door}").Value == "True", "Door state reflects completion");
}
Check(plc.WritePlcTag("Plc1", "Status_12001", "False"), "Reset simulated door sensor");
Check(plc.WritePlcTag("Plc1", "Cmd_12001", "True"), "Direct door API accepted");
await Until(() => plc.ReadPlcTag("Plc1", "Status_12001").Value == "True", "Direct door API completes");
Check(plc.WritePlcTag("Plc1", "Mover_Cmd", Command(11, 601, 8)), "Mover extend");
await Until(() => plc.ReadPlcTag("Plc1", "Mover_Pos").Value == "1", "Mover extended");
Check(plc.WritePlcTag("Plc1", "Mover_Cmd", Command(12, 602, 8)), "Mover retract");
await Until(() => plc.ReadPlcTag("Plc1", "Mover_Pos").Value == "2", "Mover retracted");

Check(!plc.WritePlcTag("Plc1", "Lm_Cmd", Command(4, 700)), "Scan requires real configured cells");
for (int scan = 0; scan < 2; scan++)
{
    // Same priming and consumption used by LMReadCellCmd/ChkBgJob.
    plc.IsPlcTagValueChange("Plc1", "CellChkFinished");
    plc.IsPlcTagValueChange("Plc1", "AllCheckFinished");
    var cells = new[] { (1, 2), (1, 3), (2, 1) };
    Check(plc.WriteSimulatedCheck("Plc1", "Lm_Cmd", Command(4, 700 + scan), cells), "Multi-cell check accepted");
    foreach (var cell in cells)
    {
        bool changed = false;
        await Until(() => changed || (changed = plc.IsPlcTagValueChange("Plc1", "CellChkFinished")), "Scan signal observed");
        Check(plc.ReadPlcTag("Plc1", "SectionNoChked").Value == cell.Item1.ToString() && plc.ReadPlcTag("Plc1", "ColNoChked").Value == cell.Item2.ToString(), "Exact scan coordinates");
        Check(plc.ReadPlcTag("Plc1", "BarcodeChked").Value == "0", "Scan explicitly empty");
        await Task.Delay(180);
        Check(!plc.IsPlcTagValueChange("Plc1", "CellChkFinished") && Response(plc)[5] == 1, "No dropped cells: wait for result acknowledgement");
        plc.AcknowledgeSimulatedCheckCell();
    }
    bool allChanged = false;
    await Until(() => allChanged || (allChanged = plc.IsPlcTagValueChange("Plc1", "AllCheckFinished")), "All-check signal after last acknowledgement");
    Check(Response(plc)[5] == 2 && plc.ReadPlcTag("Plc1", "Lm_State").Value == "1", "Scan releases device");
}
Check(plc.WriteSimulatedCheck("Plc1", "Lm_Cmd", Command(4, 800), new[] { (1, 1) }), "Start cancellable scan");
plc.StopSimulatedCheck();
Check(plc.WritePlcTag("Plc1", "Lm_Cmd", Command(1, 801)), "Stopped check does not block next job");
await Until(() => Response(plc)[5] == 2, "Home after check stop");
// Use the unchanged production command builder and completion parser, not only simulator assertions.
var nodeRepo = DispatchProxy.Create<Volo.Abp.Domain.Repositories.IRepository<Wcs.Nodes.Models.DispatchNode, int>, RepositoryProbe<Wcs.Nodes.Models.DispatchNode>>();
((RepositoryProbe<Wcs.Nodes.Models.DispatchNode>)nodeRepo).Items.Add(new Wcs.Nodes.Models.DispatchNode(1)
{
    NodeCode = "13001", CmdTagName = "Plc1.Lm_Cmd", ResponseTagName = "Plc1.Lm_Response"
});
var cmdRepo = DispatchProxy.Create<Volo.Abp.Domain.Repositories.IRepository<Wcs.Nodes.Models.DispatchNodeCmd, int>, RepositoryProbe<Wcs.Nodes.Models.DispatchNodeCmd>>();
((RepositoryProbe<Wcs.Nodes.Models.DispatchNodeCmd>)cmdRepo).Items.Add(new Wcs.Nodes.Models.DispatchNodeCmd("13", Wcs.WcsConsts.NodeType_LMToZeroPos, 1));
var nodes = new Wcs.Nodes.NodeManager(nodeRepo, null, cmdRepo, null, NullLogger<Wcs.Nodes.NodeManager>.Instance);
var commandHelper = new Wcs.Jobs.JobCmds.JobCmdHelper(Options.Create(options), null, null, null, null, null, nodes, null, null, null, plc);
var home = new Wcs.Jobs.JobCmds.LMToZeroPosCmd(NullLogger<Wcs.Jobs.JobCmds.LMToZeroPosCmd>.Instance, plc, nodes, commandHelper);
var worker = new TestWorker { MyJob = new Wcs.Jobs.Models.DispatchJob(850) { NodeCode = "13001", OrderCode = "local-test" } };
home.Owner = worker;
Check(home.SendCmdValue().IsOK, "Production home command sends through simulated PLC boundary");
Check(!home.IsCmdFinished().IsOK, "Production parser waits while executing");
await Until(() => home.IsCmdFinished().IsOK, "Production parser accepts simulated completion");
worker.MyJob = new Wcs.Jobs.Models.DispatchJob(851) { NodeCode = "13001" };
Check(!home.IsCmdFinished().IsOK, "Production parser rejects completion for previous job");
Check(deviceProbe.Calls.Count == 0, "No real PLCServer calls throughout simulated workflow");

var realDevice = DispatchProxy.Create<IRedisClient, RedisProbe>();
var realBusiness = DispatchProxy.Create<IRedisClient, RedisProbe>();
using var real = new PlcHelper(Options.Create(new ConfigOptions()), NullLogger<PlcHelper>.Instance, realDevice, realBusiness);
Check(!real.IsSimulation, "Default is real mode");
Check(((RedisProbe)realDevice).Calls.Contains("Build") && ((RedisProbe)realDevice).Calls.Contains("Publish"), "Real mode still builds and registers");
real.IsPlcTagExist("Plc1", "Lm_State");
await real.ReadPlcTagAsync("Plc1", "Lm_State");
Check(((RedisProbe)realDevice).Calls.Contains("IsKeyExist") && ((RedisProbe)realDevice).Calls.Contains("GetStringValueAsync"), "Real reads remain routed to Redis");
plc.Dispose();
Check(!plc.WritePlcTag("Plc1", "Lm_Cmd", Command(1, 900)), "Disposed simulation rejects commands");
Console.WriteLine($"PASS: {assertions} assertions; PLCServer-free command flows, scan acknowledgement, default real routing.");

public class RedisProbe : DispatchProxy
{
    public ConcurrentQueue<string> Calls { get; } = new();
    private readonly ConcurrentDictionary<string, string> _hash = new();
    protected override object Invoke(MethodInfo method, object[] args)
    {
        Calls.Enqueue(method.Name);
        if (method.Name == "GetHashValue") return _hash.TryGetValue(args[0] + ":" + args[1], out var value) ? value : null;
        if (method.Name == "SetHashValue") { _hash[args[0] + ":" + args[1]] = (string)args[2]; return null; }
        if (method.ReturnType == typeof(void)) return null;
        if (method.ReturnType == typeof(bool)) return true;
        if (method.ReturnType == typeof(string[])) return Array.Empty<string>();
        if (method.ReturnType == typeof(Task<string>)) return Task.FromResult<string>(null);
        if (method.ReturnType == typeof(Task<bool>)) return Task.FromResult(true);
        if (method.ReturnType == typeof(Task)) return Task.CompletedTask;
        return null;
    }
}

public class RepositoryProbe<T> : DispatchProxy
{
    public List<T> Items { get; } = new();
    protected override object Invoke(MethodInfo method, object[] args)
    {
        if (method.Name == "GetListAsync")
        {
            var predicate = args.OfType<System.Linq.Expressions.Expression<Func<T, bool>>>().FirstOrDefault();
            return Task.FromResult(predicate == null ? Items.ToList() : Items.Where(predicate.Compile()).ToList());
        }
        throw new NotSupportedException(method.Name);
    }
}

public class TestWorker : Wcs.Jobs.JobWorker.IJobWorker
{
    public Wcs.Jobs.Models.DispatchJob MyJob { get; set; }
    public Wcs.Jobs.JobCmds.IJobCmd MyJobCmd { get; set; }
    public Task Execute() => throw new NotSupportedException();
    public void ForceDone() => throw new NotSupportedException();
    public void RedoCurStep() => throw new NotSupportedException();
    public void ForceDoneCurStep() => throw new NotSupportedException();
    public string GenerateLog(string content) => content;
}