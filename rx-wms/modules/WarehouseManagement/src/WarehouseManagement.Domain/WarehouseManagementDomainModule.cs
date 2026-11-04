using Volo.Abp.Domain;
using Volo.Abp.Modularity;

namespace WarehouseManagement;

[DependsOn(
    typeof(AbpDddDomainModule),
    typeof(WarehouseManagementDomainSharedModule)
)]
public class WarehouseManagementDomainModule : AbpModule
{

}
