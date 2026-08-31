using Wcs.Localization;
using Localization.Resources.AbpUi;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;

namespace Wcs;

[DependsOn(
    typeof(WcsApplicationContractsModule),
    typeof(AbpAspNetCoreMvcModule)
    )]
public class WcsHttpApiModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        base.ConfigureServices(context);
        ConfigureLocalization();
    }

    private void ConfigureLocalization()
    {
        Configure<AbpLocalizationOptions>(options =>
        {
            options.Resources
                .Get<WcsResource>()
                .AddBaseTypes(
                    typeof(AbpUiResource)
                );
        });
    }
}
