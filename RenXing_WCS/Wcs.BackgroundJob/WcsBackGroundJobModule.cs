using Wcs.Conditions;
using Wcs.Dispatch;
using Wcs.Jobs.CheckBgJob;
using Wcs.Log;
using Wcs.Mjj;
using Wcs.Orders;
using Wcs.PlcMonitor;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;

namespace Wcs;

[DependsOn(
    typeof(WcsDomainModule),
    typeof(WcsDomainSharedModule)
    )]
public class WcsBackGroundJobModule : AbpModule
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
