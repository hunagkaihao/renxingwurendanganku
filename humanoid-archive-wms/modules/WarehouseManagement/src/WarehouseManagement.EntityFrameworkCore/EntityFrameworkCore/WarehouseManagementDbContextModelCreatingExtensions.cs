using Microsoft.EntityFrameworkCore;
using Volo.Abp;
using System.Linq;
using Volo.Abp.EntityFrameworkCore.Modeling;
using WarehouseManagement.MaterialBoxs.Aggregates;
using WarehouseManagement.Cells;
using WarehouseManagement.Checks.Aggregates;
using WarehouseManagement.Goodss.Aggregates;
using WarehouseManagement.Plans.Aggregates;
using WarehouseManagement.RfidCodes.Aggregates;
using WarehouseManagement.StockTasks.Aggregates;
using MaterialAggregate = WarehouseManagement.Material.Aggregates.Material;
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

        builder.Entity<MaterialBox>(b =>
        {
            b.ToTable(WarehouseManagementDbProperties.DbTablePrefix + nameof(MaterialBox), WarehouseManagementDbProperties.DbSchema);
            // 数据库统一使用 MaterialBarcode，保留现有实体属性以兼容内部业务调用。
            b.Property(q => q.MaterialBoxBarcode).HasColumnName("MaterialBarcode");
            // 数据库统一使用 MaterialBarcode，保留现有实体属性以兼容内部业务调用。
            b.Property(q => q.MaterialBoxBarcode).HasColumnName("MaterialBarcode");
            b.HasIndex(q => q.CreationTime);
            b.Property(q => q.MaterialOutTime).HasColumnType("datetime");
            // 支持实物条码、名称及所在库位的精确查找。
            b.HasIndex(q => q.MaterialBoxBarcode);
            b.HasIndex(q => q.MaterialBoxName);
            b.HasIndex(q => q.CellId);
            b.ConfigureByConvention();
        });
        builder.Entity<Cell>(b =>
        {
            b.Property(q => q.MaterialCode).HasMaxLength(128);
        });
        builder.Entity<MaterialBoxDetail>(b =>
        {
            b.ToTable(WarehouseManagementDbProperties.DbTablePrefix + nameof(MaterialBoxDetail), WarehouseManagementDbProperties.DbSchema);
            b.HasIndex(q => q.CreationTime);
            // 支持按物料关联未删除的库存明细。
            b.HasIndex(q => new { q.MaterialId, q.IsDeleted });
            b.ConfigureByConvention();
        });
        builder.Entity<MaterialAggregate>(b =>
        {
            b.ToTable(WarehouseManagementDbProperties.DbTablePrefix + nameof(Material), WarehouseManagementDbProperties.DbSchema);
            b.HasIndex(q => q.CreationTime);
            // 支持物料编码和 RFID 的精确定位。
            b.HasIndex(q => q.MaterialCode);
            b.HasIndex(q => q.RfidId);
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
            // 编码和名称分别用于 OR 查询；物料编码用于占位校验。
            b.HasIndex(q => q.CellCode);
            b.HasIndex(q => q.CellName);
            b.HasIndex(q => q.MaterialCode);
            b.Property(b => b.CellType).HasColumnType("varchar(20)");
            b.Property(b => b.CellStatus).HasColumnType("varchar(20)");
            b.Property(b => b.RunStatus).HasColumnType("varchar(20)");
            b.ConfigureByConvention();
        });
        builder.Entity<StockTask>(b =>
        {
            b.ToTable(WarehouseManagementDbProperties.DbTablePrefix + nameof(StockTask), WarehouseManagementDbProperties.DbSchema);
            // 实物条码列已统一命名，任务业务仍通过原实体属性读写。
            b.Property(q => q.MaterialBoxBarcode).HasColumnName("MaterialBarcode");
            // 实物条码列已统一命名，任务业务仍通过原实体属性读写。
            b.Property(q => q.MaterialBoxBarcode).HasColumnName("MaterialBarcode");
            b.HasIndex(q => q.CreationTime);
            b.Property(b => b.TaskTypeCode).HasColumnType("varchar(20)");
            // 支持按实物条码查找任务并筛选执行状态。
            b.HasIndex(q => new { q.MaterialBoxBarcode, q.TaskStatus });
            b.Property(b => b.TaskStatus).HasColumnType("varchar(20)");
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
            b.Property(b => b.TaskTypeCode).HasColumnType("varchar(20)");
            b.Property(b => b.TaskStatus).HasColumnType("varchar(20)");
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
            // 支持历史盘点明细按盘点单、关联任务查询。
            b.HasIndex(q => q.CheckId);
            b.HasIndex(q => q.TaskId);
            // 盘点历史明细数据库列已统一为 TaskId。
            b.Property(b => b.TaskId).HasColumnName("TaskId");
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

        ConfigureStringLengths(builder);
    }

    /// <summary>
    /// 为本模块声明的普通字符串设置默认字符上限，保留已有长度、显式列类型和长内容字段。
    /// 不处理 ABP 基类中的扩展属性或并发标记，也不影响其他模块。
    /// </summary>
    /// <param name="builder">已注册仓储实体的模型构建器。</param>
    private static void ConfigureStringLengths(ModelBuilder builder)
    {
        foreach (var entity in builder.Model.GetEntityTypes()
                     .Where(entity => entity.ClrType.Assembly == typeof(Cell).Assembly))
        {
            foreach (var property in entity.GetProperties())
            {
                if (property.ClrType != typeof(string)
                    || property.PropertyInfo?.DeclaringType?.Assembly != typeof(Cell).Assembly
                    || property.GetMaxLength().HasValue
                    || property.GetColumnType() != null
                    || (entity.ClrType == typeof(Face) && property.Name == nameof(Face.ImageDate)))
                {
                    continue;
                }

                property.SetMaxLength(WarehouseManagementDbProperties.DefaultStringMaxLength);
            }
        }
    }
}
