using Volo.Abp.Domain;
using Volo.Abp.Modularity;

namespace Wcs;

[DependsOn(
    typeof(WcsDomainSharedModule),
    typeof(WcsToolsModule),
    typeof(AbpDddDomainModule)
)]
public class WcsDomainModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        base.ConfigureServices(context);
    }
}
