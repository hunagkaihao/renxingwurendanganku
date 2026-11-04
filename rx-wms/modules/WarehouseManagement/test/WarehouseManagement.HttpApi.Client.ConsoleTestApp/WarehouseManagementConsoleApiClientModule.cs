using Volo.Abp.Autofac;
using Volo.Abp.Http.Client.IdentityModel;
using Volo.Abp.Modularity;

namespace WarehouseManagement;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(WarehouseManagementHttpApiClientModule),
    typeof(AbpHttpClientIdentityModelModule)
    )]
public class WarehouseManagementConsoleApiClientModule : AbpModule
{

}
