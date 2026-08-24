using Ecs.Conditions;
using Ecs.Dispatch;
using Ecs.Jobs.CheckBgJob;
using Ecs.Log;
using Ecs.Mjj;
using Ecs.Orders;
using Ecs.PlcMonitor;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;

namespace Ecs;

[DependsOn(
    typeof(EcsDomainModule),
    typeof(EcsDomainSharedModule)
    )]
public class EcsBackGroundJobModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        base.ConfigureServices(context);
        context.Services.AddHostedService<DispatchCoreJob>();
        context.Services.AddHostedService<PlcMonitorJob>();
        context.Services.AddHostedService<MjjMonitorJob>();
        context.Services.AddHostedService<ConditionMonitorJob>();
        context.Services.AddHostedService<OrderCntManageJob>();
        context.Services.AddHostedService<LogCntManageJob>();
        context.Services.AddHostedService<ChkBgJob>();
        context.Services.AddHostedService<ChkTaskJob>();
    }
}
