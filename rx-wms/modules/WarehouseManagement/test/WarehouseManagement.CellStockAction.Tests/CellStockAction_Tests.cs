using Shouldly;
using Volo.Abp;
using WarehouseManagement.Cells;
using Xunit;

namespace WarehouseManagement.Samples;

public class CellStockAction_Tests
{
    [Fact]
    public void Empty_cell_can_receive_material()
    {
        var cell = CreateCell();

        Should.NotThrow(cell.EnsureCanStockIn);
    }

    [Fact]
    public void Occupied_cell_cannot_receive_material()
    {
        var cell = CreateCell("MAT-1001");

        var exception = Should.Throw<UserFriendlyException>(cell.EnsureCanStockIn);
        exception.Message.ShouldBe("该库位已有物料，无法入库");
    }

    [Fact]
    public void Occupied_cell_can_dispatch_material()
    {
        var cell = CreateCell("MAT-1001");

        Should.NotThrow(cell.EnsureCanStockOut);
    }

    [Fact]
    public void Empty_cell_cannot_dispatch_material()
    {
        var cell = CreateCell("   ");

        var exception = Should.Throw<UserFriendlyException>(cell.EnsureCanStockOut);
        exception.Message.ShouldBe("该库位无物料，无法出库");
    }

    private static Cell CreateCell(string materialCode = null)
    {
        return new Cell("01-01-01", "Cell", "01-01-01")
        {
            MaterialCode = materialCode,
        };
    }
}
