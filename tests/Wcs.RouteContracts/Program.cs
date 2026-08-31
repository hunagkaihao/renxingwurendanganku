using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Wcs.Dispatch;
using Wcs.ConfigTool;

// Inspect compiled route attributes without starting hosted jobs or touching PLC/DB/Redis.
var controllers = typeof(OrderController).Assembly.GetTypes()
    .Where(t => !t.IsAbstract && typeof(ControllerBase).IsAssignableFrom(t));
var endpoints = new HashSet<string>(StringComparer.Ordinal);
var failures = new List<string>();
var origins = new (string Input, string Expected)[]
{
    ("http://127.0.0.1:5200", "http://127.0.0.1:5200"),
    (" http://127.0.0.1:6200/ ,https://127.0.0.1:6201", "http://127.0.0.1:6200"),
    ("http://*:5200", "http://127.0.0.1:5200"),
    ("http://+:5200", "http://127.0.0.1:5200"),
    ("http://0.0.0.0:5200", "http://127.0.0.1:5200"),
    ("http://[::]:5200", "http://[::1]:5200")
};
foreach (var origin in origins)
    if (SelfCallEndpoint.Resolve(origin.Input) != origin.Expected)
        failures.Add($"Self-call endpoint resolution failed for {origin.Input}");
foreach (var invalid in new[] { "", ",,", "not-a-url", "ftp://127.0.0.1:5200" })
{
    try { SelfCallEndpoint.Resolve(invalid); failures.Add($"Invalid endpoint accepted: {invalid}"); }
    catch (ArgumentException) { }
}
var routeCount = 0;
foreach (var controller in controllers)
{
    var prefixes = controller.GetCustomAttributes<RouteAttribute>().ToArray();
    foreach (var prefix in prefixes)
    {
        routeCount++;
        if (!prefix.Template.StartsWith("wcs/", StringComparison.Ordinal))
            failures.Add($"{controller.Name}: unexpected route prefix '{prefix.Template}'");
        foreach (var method in controller.GetMethods(BindingFlags.Public | BindingFlags.Instance))
        foreach (var action in method.GetCustomAttributes<HttpMethodAttribute>())
        foreach (var verb in action.HttpMethods)
        {
            var template = action.Template ?? "";
            var path = template.StartsWith('/') || template.StartsWith("~/")
                ? template.TrimStart('~', '/')
                : $"{prefix.Template.Trim('/')}/{template.Trim('/')}";
            endpoints.Add($"{verb} /{path}");
            if (!path.StartsWith("wcs/", StringComparison.Ordinal))
                failures.Add($"{controller.Name}.{method.Name}: unexpected endpoint '{path}'");
        }
    }
}

// Literal consumer contracts: catches missing registrations, legacy aliases, and verb drift.
string[] expected =
{
    "POST /wcs/dispatch/order/stockOrderCreate",
    "POST /wcs/dispatch/order/checkOrderCreate",
    "GET /wcs/dispatch/order/checkOrderResultsGetByQueryCode",
    "GET /wcs/dispatch/order/state",
    "GET /wcs/dispatch/order/states",
    "GET /wcs/dispatch/order/unDoneOrders",
    "GET /wcs/dispatch/order/oneOrder",
    "POST /wcs/dispatch/order/cancelOrder",
    "POST /wcs/dispatch/order/forceDone",
    "POST /wcs/dispatch/core/pause",
    "POST /wcs/dispatch/core/restart",
    "GET /wcs/dispatch/core/wcsStatus",
    "GET /wcs/dispatch/device/commuState",
    "GET /wcs/plc/plcMonitor",
    "GET /wcs/mjj/mjjStatusOfNmValMapList",
    "GET /wcs/log/query",
    "POST /wcs/test/start",
    "POST /wcs/test/restart",
    "POST /wcs/test/stop"
};
if (routeCount == 0) failures.Add("No controller routes discovered");
foreach (var endpoint in expected)
    if (!endpoints.Contains(endpoint)) failures.Add($"Missing {endpoint}");
foreach (var failure in failures) Console.Error.WriteLine($"FAIL: {failure}");
if (failures.Count > 0) return 1;
Console.WriteLine($"PASS: {routeCount} controller prefixes, {endpoints.Count} endpoints, {expected.Length} consumer contracts; wcs only; 10 self-call endpoint cases.");
return 0;
