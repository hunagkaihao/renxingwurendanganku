using Localization.Resources.AbpUi;
using WarehouseManagement.Localization;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Microsoft.Extensions.DependencyInjection;

namespace WarehouseManagement;

[DependsOn(
    typeof(WarehouseManagementApplicationContractsModule),
    typeof(AbpAspNetCoreMvcModule))]
public class WarehouseManagementHttpApiModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        PreConfigure<IMvcBuilder>(mvcBuilder =>
        {
            mvcBuilder.AddApplicationPartIfNotExists(typeof(WarehouseManagementHttpApiModule).Assembly);
        });
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpLocalizationOptions>(options =>
        {
            options.Resources
                .Get<WarehouseManagementResource>()
                .AddBaseTypes(typeof(AbpUiResource));
        });
    }
}
