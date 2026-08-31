using Wcs.Caches.Models;
using Wcs.Cells.Models;
using Wcs.Conditions.Models;
using Wcs.DahSpecss.Models;
using Wcs.Jobs.Models;
using Wcs.Nodes.Models;
using Wcs.Orders.Models;
using Wcs.Processes.Models;
using Wcs.Tasks.Models;
using Microsoft.EntityFrameworkCore;

using System.Linq;

using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace Wcs.EntityFrameworkCore;

[ConnectionStringName("Default")]
public class WcsDbContext :
    AbpDbContext<WcsDbContext>
{
    /* Add DbSet properties for your Aggregate Roots / Entities here. */
    public DbSet<DispatchWarehouse> dispatchwarehouses { get; set; }
    public DbSet<DispatchCell> dispatchcells { get; set; }
    public DbSet<DispatchCache> dispatchcaches { get; set; }
    public DbSet<DispatchNode> dispatchnodes { get; set; }
    public DbSet<DispatchNodeType> dispatchnodetypes { get; set; }
    public DbSet<DispatchNodeCmd> dispatchnodecmds { get; set; }
    public DbSet<DispatchProcess> dispatchprocesses { get; set; }
    public DbSet<DispatchProcessStep> dispatchprocesssteps { get; set; }
    public DbSet<DispatchProcessStepPrecondition> dispatchprocesssteppreconditions { get; set; }
    public DbSet<DispatchProcessStepResource> dispatchprocessstepresources { get; set; }
    public DbSet<DispatchOrder> dispatchorders { get; set; }
    public DbSet<DispatchChkOrderRslt> dispatchchkorderrslts { get; set; }
    public DbSet<DispatchTask> dispatchtasks { get; set; }
    public DbSet<DispatchTaskId> dispatchtaskids { get; set; }
    public DbSet<DispatchJob> dispatchjobs { get; set; }
    public DbSet<DispatchJobId> dispatchjobids { get; set; }
    public DbSet<DispatchJobCmd> dispatchjobcmds { get; set; }
    public DbSet<DispatchJobWorker> dispatchjobworkers { get; set; }
    public DbSet<DispatchCondition> dispatchconditions { get; set; }
    public DbSet<DahSpecs> dahspecs { get; set; }


    public WcsDbContext(DbContextOptions<WcsDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);


        // 配置所有实体适应SQLite
        foreach (var entity in builder.Model.GetEntityTypes())
        {
            var properties = entity.ClrType.GetProperties()
                .Where(p => p.PropertyType == typeof(int) && p.Name == "Id");

            foreach (var property in properties)
            {
                builder.Entity(entity.ClrType)
                    .Property(property.Name)
                    .HasColumnType("INTEGER")
                    .ValueGeneratedOnAdd();
            }
        }
        /* Configure your own tables/entities inside here */

        //builder.Entity<YourEntity>(b =>
        //{
        //    b.ToTable(WcsConsts.DbTablePrefix + "YourEntities", WcsConsts.DbSchema);
        //    b.ConfigureByConvention(); //auto configure for the base class props
        //    //...
        //});

    }
}
