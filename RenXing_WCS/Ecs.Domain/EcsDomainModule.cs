using Volo.Abp.Domain;
using Volo.Abp.Modularity;

namespace Ecs;

[DependsOn(
    typeof(EcsDomainSharedModule),
    typeof(EcsToolsModule),
    typeof(AbpDddDomainModule)
)]
public class EcsDomainModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        base.ConfigureServices(context);
    }
}
