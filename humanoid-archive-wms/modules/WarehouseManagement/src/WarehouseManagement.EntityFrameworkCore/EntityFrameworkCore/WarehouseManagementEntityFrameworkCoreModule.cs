using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace WarehouseManagement.EntityFrameworkCore;

[DependsOn(
    typeof(WarehouseManagementDomainModule),
    typeof(AbpEntityFrameworkCoreModule)
)]
public class WarehouseManagementEntityFrameworkCoreModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAbpDbContext<WarehouseManagementDbContext>(options =>
        {
            /* Add custom repositories here. Example:
             * options.AddRepository<Question, EfCoreQuestionRepository>();
             */
            options.AddDefaultRepositories(true);
        });
    }
}
