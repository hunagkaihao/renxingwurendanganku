using System.Security.Cryptography;
using Ecs.Dispatch;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Application;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;

namespace Ecs;

[DependsOn(
    typeof(EcsDomainModule),
    typeof(EcsApplicationContractsModule),
    typeof(AbpAutoMapperModule),
    typeof(AbpDddApplicationModule)
    )]
public class EcsApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<EcsApplicationModule>();
        });
        context.Services.AddHostedService<TestJob>();
        base.ConfigureServices(context);
    }
}
