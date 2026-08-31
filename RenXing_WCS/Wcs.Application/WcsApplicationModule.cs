using System.Security.Cryptography;
using Wcs.Dispatch;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Application;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;

namespace Wcs;

[DependsOn(
    typeof(WcsDomainModule),
    typeof(WcsApplicationContractsModule),
    typeof(AbpAutoMapperModule),
    typeof(AbpDddApplicationModule)
    )]
public class WcsApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<WcsApplicationModule>();
        });
        context.Services.AddHostedService<TestJob>();
        base.ConfigureServices(context);
    }
}
