using WarehouseManagement.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace WarehouseManagement;

/* Domain tests are configured to use the EF Core provider.
 * You can switch to MongoDB, however your domain tests should be
 * database independent anyway.
 */
[DependsOn(
    typeof(WarehouseManagementEntityFrameworkCoreTestModule)
    )]
public class WarehouseManagementDomainTestModule : AbpModule
{

}
