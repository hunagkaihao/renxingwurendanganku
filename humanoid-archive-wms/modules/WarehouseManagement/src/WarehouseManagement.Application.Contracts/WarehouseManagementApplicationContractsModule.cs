using Volo.Abp.Application;
using Volo.Abp.Modularity;
using Volo.Abp.Authorization;

namespace WarehouseManagement;

[DependsOn(
    typeof(WarehouseManagementDomainSharedModule),
    typeof(AbpDddApplicationContractsModule),
    typeof(AbpAuthorizationModule)
    )]
public class WarehouseManagementApplicationContractsModule : AbpModule
{

}
