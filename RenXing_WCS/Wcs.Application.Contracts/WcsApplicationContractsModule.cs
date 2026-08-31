using Volo.Abp.Application;
using Volo.Abp.Modularity;

namespace Wcs;

[DependsOn(
    typeof(WcsDomainSharedModule),
    typeof(AbpDddApplicationContractsModule)
)]
public class WcsApplicationContractsModule : AbpModule
{

}
