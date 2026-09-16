using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Wcs.Nodes;

namespace Wcs.Dispatch;

// 在调度启动前完成配置门的增量初始化，失败时不启动不完整的调度。
public sealed class DoorConfigurationJob : IHostedService
{
    private readonly IServiceScopeFactory _scopes;
    public DoorConfigurationJob(IServiceScopeFactory scopes) => _scopes = scopes;
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _scopes.CreateScope();
        await scope.ServiceProvider.GetRequiredService<DoorConfigurationInitializer>().EnsureAsync();
    }
    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
