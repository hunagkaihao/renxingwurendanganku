using System.Net;
using System.Text;
using Lion.AbpPro.ConfigurationOptions;
using Microsoft.Extensions.Options;
using WarehouseManagement.WcsTasks;
using WarehouseManagement.WcsTasks.Dto;

// Exercise the real WMS HTTP serializer/client; the transport records requests and never sends them.
var transport = new RecordingTransport();
var options = new Snapshot();
var manager = new WcsApiManager(transport, options);
var cases = new (string Name, Func<Task> Run, string Request)[]
{
    ("stock order", async () => { await manager.StockOrderCreate("42", "1001", "15001", "01-001-01", 1); }, "POST /wcs/dispatch/order/stockOrderCreate"),
    ("single status with escaped order code", async () => { await manager.State(new StockOrderCreateDto { OrderCode = "A&B 1" }); }, "GET /wcs/dispatch/order/state?orderCode=A%26B%201"),
    ("all statuses", async () => { await manager.States(); }, "GET /wcs/dispatch/order/states"),
    ("check result with escaped query code", async () => { await manager.CheckOrderResult(new CheckOrderResultDto { QueryCode = "Q&1" }); }, "GET /wcs/dispatch/order/checkOrderResultsGetByQueryCode?queryCode=Q%261"),
    ("pause", async () => { await manager.Pause(); }, "POST /wcs/dispatch/core/pause"),
    ("restart", async () => { await manager.Restart(); }, "POST /wcs/dispatch/core/restart"),
    ("communication status", async () => { await manager.CommuState(); }, "GET /wcs/dispatch/device/commuState"),
    ("cancel", async () => { await manager.CancelOrder(42); }, "POST /wcs/dispatch/order/cancelOrder"),
    ("force done", async () => { await manager.ForceDone(42); }, "POST /wcs/dispatch/order/forceDone")
};
var failures = new List<string>();
foreach (var item in cases)
{
    transport.Requests.Clear();
    await item.Run();
    var actual = transport.Requests.SingleOrDefault();
    if (actual != $"http://127.0.0.1:5200 {item.Request}")
        failures.Add($"{item.Name}: expected {item.Request}, actual {actual}");
}
manager.WCSEnable = false;
transport.Requests.Clear();
await manager.States();
if (transport.Requests.Count != 0) failures.Add("Disabled WCS must not send requests");
foreach (var failure in failures) Console.Error.WriteLine($"FAIL: {failure}");
if (failures.Count != 0) return 1;
Console.WriteLine($"PASS: {cases.Length} WMS outbound request contracts and disabled-service guard.");
return 0;

sealed class Snapshot : IOptionsSnapshot<WCSOptions>
{
    public WCSOptions Value { get; } = new() { Server = "http://127.0.0.1:5200", Enable = true };
    public WCSOptions Get(string? name) => Value;
}

sealed class RecordingTransport : HttpMessageHandler, IHttpClientFactory
{
    public List<string> Requests { get; } = new();
    public HttpClient CreateClient(string name) => new(this, disposeHandler: false);
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken token)
    {
        var uri = request.RequestUri!;
        Requests.Add($"{uri.GetLeftPart(UriPartial.Authority)} {request.Method} {uri.PathAndQuery}");
        return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("{\"success\":true,\"message\":\"\",\"orderStates\":[],\"cells\":[],\"commuStates\":[]}", Encoding.UTF8, "application/json")
        });
    }
}
