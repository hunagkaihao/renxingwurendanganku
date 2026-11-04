using System.Linq;
using Microsoft.EntityFrameworkCore;
using WarehouseManagement.Cells;
using WarehouseManagement.Faces.Aggregates;
using WarehouseManagement.Goodss.Aggregates;
using WarehouseManagement.MaterialBoxs.Aggregates;
using WarehouseManagement.StockTasks.Aggregates;
using Xunit;

namespace WarehouseManagement.EntityFrameworkCore;

/// <summary>
/// 检查普通文本的字符限制、长内容例外以及条码查询索引，防止数据库脚本与运行模型脱节。
/// </summary>
public class WarehouseStorageModel_Tests
{
    /// <summary>
    /// 普通业务字符串有明确上限，已有的短字段约束不被默认上限覆盖。
    /// </summary>
    [Fact]
    public void Business_strings_have_limits_and_preserve_existing_constraints()
    {
        using var context = CreateContext();
        foreach (var entity in context.Model.GetEntityTypes())
        {
            foreach (var property in entity.GetProperties().Where(property =>
                         property.ClrType == typeof(string)
                         && property.PropertyInfo?.DeclaringType?.Assembly == typeof(Cell).Assembly
                         && !(entity.ClrType == typeof(Face) && property.Name == nameof(Face.ImageDate))))
            {
                // CheckHis 的状态、类型使用显式 varchar(20)，不依赖 MaxLength 元数据。
                var length = property.GetMaxLength()
                    ?? (property.GetColumnType() == "varchar(20)" ? 20 : 0);
                Assert.True(length > 0, $"{entity.ClrType.Name}.{property.Name} 缺少字符上限");
            }
        }

        Assert.Equal(200, context.Model.FindEntityType(typeof(Cell)).FindProperty(nameof(Cell.CellCode)).GetMaxLength());
        Assert.Equal(128, context.Model.FindEntityType(typeof(Cell)).FindProperty(nameof(Cell.MaterialCode)).GetMaxLength());
        Assert.Equal(1, context.Model.FindEntityType(typeof(Goods)).FindProperty(nameof(Goods.ValidateFlag)).GetMaxLength());
        Assert.Equal(256, context.Model.FindEntityType(typeof(Goods)).FindProperty(nameof(Goods.PicUrl)).GetMaxLength());
        Assert.Equal("varchar(20)", context.Model.FindEntityType(typeof(Cell)).FindProperty(nameof(Cell.CellStatus)).GetColumnType());
    }

    /// <summary>
    /// 人脸内容及框架扩展数据允许超过 200 字符，避免套用普通业务字段规则。
    /// </summary>
    [Fact]
    public void Image_and_framework_payloads_remain_unbounded()
    {
        using var context = CreateContext();
        var face = context.Model.FindEntityType(typeof(Face));
        Assert.Null(face.FindProperty(nameof(Face.ImageDate)).GetMaxLength());
        Assert.Null(face.FindProperty(nameof(Face.ExtraProperties)).GetMaxLength());
    }

    /// <summary>
    /// 联合索引保持业务筛选顺序，条码属性继续对应已改名的数据库列。
    /// </summary>
    [Fact]
    public void Lookup_indexes_use_the_existing_barcode_columns()
    {
        using var context = CreateContext();
        var task = context.Model.FindEntityType(typeof(StockTask));
        var index = Assert.Single(task.GetIndexes().Where(index => index.Properties
            .Select(property => property.Name).SequenceEqual(new[] { nameof(StockTask.MaterialBoxBarcode), nameof(StockTask.TaskStatus) })));
        Assert.Equal("IX_StockTask_MaterialBarcode_TaskStatus", index.GetDatabaseName());
        var box = context.Model.FindEntityType(typeof(MaterialBox));
        Assert.Contains(box.GetIndexes(), candidate => candidate.GetDatabaseName() == "IX_MaterialBox_MaterialBarcode");
    }

    /// <summary>
    /// 创建仅用于读取模型的 SQLite 上下文，不连接或修改业务数据库。
    /// </summary>
    private static WarehouseManagementDbContext CreateContext()
    {
        return new WarehouseManagementDbContext(new DbContextOptionsBuilder<WarehouseManagementDbContext>()
            .UseSqlite("Data Source=:memory:").Options);
    }
}
