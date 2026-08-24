using Microsoft.EntityFrameworkCore;
using Volo.Abp;
using Volo.Abp.EntityFrameworkCore.Modeling;
using WarehouseManagement.ArchiveBoxs.Aggregates;
using WarehouseManagement.Cells;
using WarehouseManagement.Checks.Aggregates;
using WarehouseManagement.Goodss.Aggregates;
using WarehouseManagement.Plans.Aggregates;
using WarehouseManagement.RfidCodes.Aggregates;
using WarehouseManagement.StockTasks.Aggregates;
using WarehouseManagement.Archives.Aggregates;
using WarehouseManagement.TaskHiss.Aggregates;
using WarehouseManagement.Warehouses.Aggregates;
using Check = WarehouseManagement.Checks.Aggregates.Check;
using WarehouseManagement.Faces.Aggregates;
using WarehouseManagement.Fingers.Aggregates;
using WarehouseManagement.CheckHiss.Aggregates;

namespace WarehouseManagement.EntityFrameworkCore;

public static class WarehouseManagementDbContextModelCreatingExtensions
{
    public static void ConfigureWarehouseManagement(
        this ModelBuilder builder)
    {
        Volo.Abp.Check.NotNull(builder, nameof(builder));

        /* Configure all entities here. Example:

        builder.Entity<Question>(b =>
        {
            //Configure table & schema name
            b.ToTable(WarehouseManagementDbProperties.DbTablePrefix + "Questions", WarehouseManagementDbProperties.DbSchema);

            b.ConfigureByConvention();

            //Properties
            b.Property(q => q.Title).IsRequired().HasMaxLength(QuestionConsts.MaxTitleLength);

            //Relations
            b.HasMany(question => question.Tags).WithOne().HasForeignKey(qt => qt.QuestionId);

            //Indexes
            b.HasIndex(q => q.CreationTime);
        });
        */
        builder.Entity<Goods>(b =>
        {
            b.ToTable(WarehouseManagementDbProperties.DbTablePrefix + nameof(Goods), WarehouseManagementDbProperties.DbSchema);
            b.HasIndex(q => q.CreationTime);
            b.ConfigureByConvention();
        });
        builder.Entity<GoodsClass>(b =>
        {
            b.ToTable(WarehouseManagementDbProperties.DbTablePrefix + nameof(GoodsClass), WarehouseManagementDbProperties.DbSchema);
            b.ConfigureByConvention();
        });
        builder.Entity<GoodsType>(b =>
        {
            b.ToTable(WarehouseManagementDbProperties.DbTablePrefix + nameof(GoodsType), WarehouseManagementDbProperties.DbSchema);
            b.ConfigureByConvention();
        });

        builder.Entity<ArchiveBox>(b =>
        {
            b.ToTable(WarehouseManagementDbProperties.DbTablePrefix + nameof(ArchiveBox), WarehouseManagementDbProperties.DbSchema);
            b.HasIndex(q => q.CreationTime);
            b.ConfigureByConvention();
        });
        builder.Entity<ArchiveBoxDetail>(b =>
        {
            b.ToTable(WarehouseManagementDbProperties.DbTablePrefix + nameof(ArchiveBoxDetail), WarehouseManagementDbProperties.DbSchema);
            b.HasIndex(q => q.CreationTime);
            b.ConfigureByConvention();
        });
        builder.Entity<Archive>(b =>
        {
            b.ToTable(WarehouseManagementDbProperties.DbTablePrefix + nameof(Archive), WarehouseManagementDbProperties.DbSchema);
            b.HasIndex(q => q.CreationTime);
            b.ConfigureByConvention();
        });
        builder.Entity<Rfid>(b =>
        {
            b.ToTable(WarehouseManagementDbProperties.DbTablePrefix + nameof(Rfid), WarehouseManagementDbProperties.DbSchema);
            b.HasIndex(q => q.CreationTime);
            b.ConfigureByConvention();
        });

        builder.Entity<Cell>(b =>
        {
            b.ToTable(WarehouseManagementDbProperties.DbTablePrefix + nameof(Cell), WarehouseManagementDbProperties.DbSchema);
            b.HasIndex(q => q.CreationTime);
            b.Property(b => b.CellType).HasColumnType("varchar(20)");
            b.Property(b => b.CellStatus).HasColumnType("varchar(20)");
            b.Property(b => b.RunStatus).HasColumnType("varchar(20)");
            b.ConfigureByConvention();
        });
        builder.Entity<StockTask>(b =>
        {
            b.ToTable(WarehouseManagementDbProperties.DbTablePrefix + nameof(StockTask), WarehouseManagementDbProperties.DbSchema);
            b.HasIndex(q => q.CreationTime);
            b.Property(b => b.ManageTypeCode).HasColumnType("varchar(20)");
            b.Property(b => b.ManageStatus).HasColumnType("varchar(20)");
            //b.HasMany(u => u.Goodss).WithOne().HasForeignKey(ur => ur.StorageBoxId).IsRequired();
            b.ConfigureByConvention();
        });
        builder.Entity<StockTaskDetail>(b =>
        {
            b.ToTable(WarehouseManagementDbProperties.DbTablePrefix + nameof(StockTaskDetail), WarehouseManagementDbProperties.DbSchema);
            b.HasIndex(q => q.CreationTime);
            b.ConfigureByConvention();
        });
        builder.Entity<StockTaskType>(b =>
        {
            b.ToTable(WarehouseManagementDbProperties.DbTablePrefix + nameof(StockTaskType), WarehouseManagementDbProperties.DbSchema);
            b.HasIndex(q => q.ManageTypeCode);
            b.ConfigureByConvention();
        });
        builder.Entity<TaskHis>(b =>
        {
            b.ToTable(WarehouseManagementDbProperties.DbTablePrefix + nameof(TaskHis), WarehouseManagementDbProperties.DbSchema);
            b.HasIndex(q => q.CreationTime);
            b.Property(b => b.ManageTypeCode).HasColumnType("varchar(20)");
            b.Property(b => b.ManageStatus).HasColumnType("varchar(20)");
            //b.HasMany(u => u.Goodss).WithOne().HasForeignKey(ur => ur.StorageBoxId).IsRequired();
            b.ConfigureByConvention();
        });
        builder.Entity<TaskHisDetail>(b =>
        {
            b.ToTable(WarehouseManagementDbProperties.DbTablePrefix + nameof(TaskHisDetail), WarehouseManagementDbProperties.DbSchema);
            b.HasIndex(q => q.CreationTime);
            b.ConfigureByConvention();
        });
       


        builder.Entity<Plan>(b =>
        {
            b.ToTable(WarehouseManagementDbProperties.DbTablePrefix + nameof(Plan), WarehouseManagementDbProperties.DbSchema);
            b.HasIndex(q => q.CreationTime);
            b.Property(b => b.PlanStatus).HasColumnType("varchar(20)");
            //b.HasMany(u => u.Goodss).WithOne().HasForeignKey(ur => ur.StorageBoxId).IsRequired();
            b.ConfigureByConvention();
        });
        builder.Entity<PlanList>(b =>
        {
            b.ToTable(WarehouseManagementDbProperties.DbTablePrefix + nameof(PlanList), WarehouseManagementDbProperties.DbSchema);
            b.HasIndex(q => q.CreationTime);
            b.Property(b => b.PlanListStatus).HasColumnType("varchar(20)");
            b.ConfigureByConvention();
        });
        builder.Entity<PlanType>(b =>
        {
            b.ToTable(WarehouseManagementDbProperties.DbTablePrefix + nameof(PlanType), WarehouseManagementDbProperties.DbSchema);
            b.HasIndex(q => q.PlanTypeCode);
            b.ConfigureByConvention();
        });
        builder.Entity<Check>(b =>
        {
            b.ToTable(WarehouseManagementDbProperties.DbTablePrefix + nameof(Check), WarehouseManagementDbProperties.DbSchema);
            b.HasIndex(q => q.CreationTime);
            b.Property(b => b.CheckType).HasColumnType("varchar(20)");
            b.Property(b => b.CheckStatus).HasColumnType("varchar(20)");
            //b.HasMany(u => u.Goodss).WithOne().HasForeignKey(ur => ur.StorageBoxId).IsRequired();
            b.ConfigureByConvention();
        });
        builder.Entity<CheckHis>(b =>
        {
            b.ToTable(WarehouseManagementDbProperties.DbTablePrefix + nameof(CheckHis), WarehouseManagementDbProperties.DbSchema);
            b.HasIndex(q => q.CreationTime);
            b.Property(b => b.CheckType).HasColumnType("varchar(20)");
            b.Property(b => b.CheckStatus).HasColumnType("varchar(20)");
            //b.HasMany(u => u.Goodss).WithOne().HasForeignKey(ur => ur.StorageBoxId).IsRequired();
            b.ConfigureByConvention();
        });
        builder.Entity<CheckDetail>(b =>
        {
            b.ToTable(WarehouseManagementDbProperties.DbTablePrefix + nameof(CheckDetail), WarehouseManagementDbProperties.DbSchema);
            b.HasIndex(q => q.CreationTime);
            b.ConfigureByConvention();
        });
        builder.Entity<CheckDetailHis>(b =>
        {
            b.ToTable(WarehouseManagementDbProperties.DbTablePrefix + nameof(CheckDetailHis), WarehouseManagementDbProperties.DbSchema);
            b.HasIndex(q => q.CreationTime);
            b.ConfigureByConvention();
        });
        builder.Entity<Warehouse>(b =>
        {
            b.ToTable(WarehouseManagementDbProperties.DbTablePrefix + nameof(Warehouse), WarehouseManagementDbProperties.DbSchema);
            b.HasIndex(q => q.CreationTime);
            b.Property(b => b.WarehouseType).HasColumnType("varchar(20)");
            b.ConfigureByConvention();
        });
        builder.Entity<WarehouseArea>(b =>
        {
            b.ToTable(WarehouseManagementDbProperties.DbTablePrefix + nameof(WarehouseArea), WarehouseManagementDbProperties.DbSchema);
            b.HasIndex(q => q.CreationTime);
            b.Property(b => b.WarehouseAreaType).HasColumnType("varchar(20)");
            b.ConfigureByConvention();
        });
        builder.Entity<LogicArea>(b =>
        {
            b.ToTable(WarehouseManagementDbProperties.DbTablePrefix + nameof(LogicArea), WarehouseManagementDbProperties.DbSchema);
            b.HasIndex(q => q.CreationTime);
            b.ConfigureByConvention();
        });


        builder.Entity<Face>(b =>
        {
            b.ToTable(WarehouseManagementDbProperties.DbTablePrefix + nameof(Face), WarehouseManagementDbProperties.DbSchema);
            b.HasIndex(q => q.CreationTime);
            b.ConfigureByConvention();
        });

        builder.Entity<Vein>(b =>
        {
            b.ToTable(WarehouseManagementDbProperties.DbTablePrefix + nameof(Vein), WarehouseManagementDbProperties.DbSchema);
            b.HasIndex(q => q.CreationTime);
            b.ConfigureByConvention();
        });
    }
}
