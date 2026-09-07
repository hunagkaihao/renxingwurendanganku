using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using WarehouseManagement.MaterialBoxs.Aggregates;
using MaterialAggregate = WarehouseManagement.Material.Aggregates.Material;
using WarehouseManagement.Cells;
using WarehouseManagement.Checks.Aggregates;
using WarehouseManagement.Goodss.Aggregates;
using WarehouseManagement.Plans.Aggregates;
using WarehouseManagement.RfidCodes.Aggregates;
using WarehouseManagement.StockTasks.Aggregates;
using WarehouseManagement.TaskHiss.Aggregates;
using WarehouseManagement.Warehouses.Aggregates;
using WarehouseManagement.Faces.Aggregates;
using WarehouseManagement.Fingers.Aggregates;
using WarehouseManagement.CheckHiss.Aggregates;

namespace WarehouseManagement.EntityFrameworkCore;

[ConnectionStringName(WarehouseManagementDbProperties.ConnectionStringName)]
public class WarehouseManagementDbContext : AbpDbContext<WarehouseManagementDbContext>, IWarehouseManagementDbContext
{
    /* Add DbSet for each Aggregate Root here. Example:
     * public DbSet<Question> Questions { get; set; }
     */
    public DbSet<Goods> Goodss { get; set; }
    public DbSet<GoodsClass> Goodsclasss { get; set; }

    public DbSet<GoodsType> Goodstypes { get; set; }
    public DbSet<MaterialBox> Archivebox { get; set; }
    public DbSet<MaterialBoxDetail> Archiveboxdetail { get; set; }
    public DbSet<MaterialAggregate> Material{ get; set; }
    public DbSet<Rfid> Rfid { get; set; }
    public DbSet<StockTask> Stocktasks { get; set; }
    public DbSet<StockTaskDetail> Stocktaskdetails { get; set; }

    public DbSet<StockTaskType> Stocktasktypes { get; set; }

    public DbSet<TaskHis> Taskhiss { get; set; }

    public DbSet<TaskHisDetail> Taskhisdetails { get; set; }

    public DbSet<Cell> Cells { get; set; }
    public DbSet<Plan> Plans { get; set; }
    public DbSet<PlanList> Planlists { get; set; }
    public DbSet<PlanType> Plantypes { get; set; }
    public DbSet<Check> Checks { get; set; }
    public DbSet<CheckHis> CheckHis { get; set; } 
    public DbSet<CheckDetail> Checkdetails { get; set; }
    public DbSet<CheckDetailHis> CheckdetailsHis { get; set; }
    public DbSet<Warehouse> Warehouses { get; set; }
    public DbSet<WarehouseArea> Warehouseareas { get; set; }
    public DbSet<LogicArea> Logicareas { get; set; }

    public DbSet<Face> Faces { get; set; }

    public DbSet<Vein> Veins { get; set; }


    public WarehouseManagementDbContext(DbContextOptions<WarehouseManagementDbContext> options)
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ConfigureWarehouseManagement();
    }
}
