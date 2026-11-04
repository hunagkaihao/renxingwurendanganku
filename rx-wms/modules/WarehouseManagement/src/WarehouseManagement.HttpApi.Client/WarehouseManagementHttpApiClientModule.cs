using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Http.Client;
using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;

namespace WarehouseManagement;

[DependsOn(
    typeof(WarehouseManagementApplicationContractsModule),
    typeof(AbpHttpClientModule))]
public class WarehouseManagementHttpApiClientModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddHttpClientProxies(
            typeof(WarehouseManagementApplicationContractsModule).Assembly,
            WarehouseManagementRemoteServiceConsts.RemoteServiceName
        );

        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<WarehouseManagementHttpApiClientModule>();
        });

    }
}
