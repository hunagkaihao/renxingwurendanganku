using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace WarehouseManagement.EntityFrameworkCore;

[ConnectionStringName(WarehouseManagementDbProperties.ConnectionStringName)]
public interface IWarehouseManagementDbContext : IEfCoreDbContext
{
    /* Add DbSet for each Aggregate Root here. Example:
     * DbSet<Question> Questions { get; }
     */
}
