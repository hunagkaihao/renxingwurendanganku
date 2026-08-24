using System.Threading.Tasks;
using Shouldly;
using WarehouseManagement.Cells;
using Xunit;

namespace WarehouseManagement.Samples;

public class SampleAppService_Tests : WarehouseManagementApplicationTestBase
{
    private readonly ICellAppService _sampleAppService;

    public SampleAppService_Tests()
    {
        _sampleAppService = GetRequiredService<ICellAppService>();
    }

    [Fact]
    public async Task GetAsync()
    {
        //var result = await _sampleAppService.CreateAsync();
        //result.Value.ShouldBe(42);
    }

    [Fact]
    public async Task GetAuthorizedAsync()
    {
        //var result = await _sampleAppService.GetAuthorizedAsync();
        //result.Value.ShouldBe(42);
    }
}
