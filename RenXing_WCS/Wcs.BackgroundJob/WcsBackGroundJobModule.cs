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
        // 当前 WMS 主动下发盘点任务，由 ChkBgJob 执行。
        // 不注册遗留 ChkTaskJob：其拉取接口 /wms/checkTask/checkTaskPagedGet 在当前 WMS 中不存在。
    }
}
