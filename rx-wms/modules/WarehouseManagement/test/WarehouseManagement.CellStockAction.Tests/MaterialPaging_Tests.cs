using System;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using Shouldly;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Linq;
using Volo.Abp.ObjectMapping;
using WarehouseManagement.Material;
using WarehouseManagement.Material.Dto;
using MaterialAggregate = WarehouseManagement.Material.Aggregates.Material;
using Xunit;

namespace WarehouseManagement.Samples;

/// <summary>
/// 验证基础物料分页的实际服务行为，区分页大小、总条数和尾页条数。
/// </summary>
public class MaterialPaging_Tests
{
    /// <summary>
    /// 请求 200 条时，第一页应返回 200 条；尾页和数据不足时按实际数量返回。
    /// </summary>
    /// <param name="availableCount">仓储中可见的基础物料总数。</param>
    /// <param name="pageIndex">从 1 开始的页码。</param>
    /// <param name="expectedCount">本页应返回的记录数。</param>
    /// <param name="firstId">按 ID 倒序后，本页第一条物料的 ID。</param>
    /// <param name="lastId">按 ID 倒序后，本页最后一条物料的 ID。</param>
    [Theory]
    [InlineData(208, 1, 200, 208, 9)]
    [InlineData(208, 2, 8, 8, 1)]
    [InlineData(8, 1, 8, 8, 1)]
    public async Task Page_returns_up_to_requested_size_and_reports_total(
        int availableCount, int pageIndex, int expectedCount, int firstId, int lastId)
    {
        var materials = Enumerable.Range(1, availableCount)
            .Select(id => (MaterialAggregate)new TestMaterial(id, "基础物料"))
            .AsQueryable();
        var service = CreateService(materials);

        var result = await service.PageAsync(new PagingMaterialListInput
        {
            PageIndex = pageIndex,
            PageSize = 200,
        });

        result.TotalCount.ShouldBe(availableCount);
        result.Items.Count.ShouldBe(expectedCount);
        result.Items.First().Id.ShouldBe(firstId);
        result.Items.Last().Id.ShouldBe(lastId);
        result.Items.Select(item => item.Id).Distinct().Count().ShouldBe(expectedCount);
    }

    /// <summary>
    /// 关键字限制匹配数量时，总条数和本页数量都只计算匹配的基础物料。
    /// </summary>
    [Fact]
    public async Task Page_reports_only_materials_matching_name_filter()
    {
        var materials = Enumerable.Range(1, 208)
            .Select(id => (MaterialAggregate)new TestMaterial(id, id <= 8 ? "同步物料" : "其他物料"))
            .AsQueryable();
        var service = CreateService(materials);

        var result = await service.PageAsync(new PagingMaterialListInput
        {
            PageIndex = 1,
            PageSize = 200,
            Filter = "同步",
        });

        result.TotalCount.ShouldBe(8);
        result.Items.Count.ShouldBe(8);
        result.Items.All(item => item.MaterialName == "同步物料").ShouldBeTrue();
    }

    /// <summary>
    /// 使用内存仓储和真实查询执行器创建服务；映射替身只转换结果，不参与筛选或分页。
    /// </summary>
    /// <param name="materials">当前请求可见的基础物料查询集。</param>
    /// <returns>执行生产分页方法的应用服务。</returns>
    private static MaterialAppService CreateService(IQueryable<MaterialAggregate> materials)
    {
        var repository = new Mock<IMaterialRepository>();
        repository.Setup(item => item.GetQueryableAsync()).ReturnsAsync(materials);

        var mapper = new Mock<IObjectMapper>();
        mapper.Setup(item => item.Map<MaterialAggregate, MaterialPageDto>(It.IsAny<MaterialAggregate>()))
            .Returns((MaterialAggregate material) => new MaterialPageDto
            {
                Id = material.Id,
                MaterialCode = material.MaterialCode,
                MaterialName = material.MaterialName,
            });

        var lazyProvider = new Mock<IAbpLazyServiceProvider>();
        lazyProvider.Setup(item => item.LazyGetRequiredService<IAsyncQueryableExecuter>())
            .Returns(new AsyncQueryableExecuter(Array.Empty<IAsyncQueryableProvider>()));
        lazyProvider.Setup(item => item.LazyGetService<IObjectMapper>(It.IsAny<Func<IServiceProvider, object>>()))
            .Returns(mapper.Object);

        return new MaterialAppService(repository.Object, null, null, null, null)
        {
            LazyServiceProvider = lazyProvider.Object,
        };
    }

    /// <summary>
    /// 为分页测试赋予固定主键的基础物料，避免依赖数据库自增行为。
    /// </summary>
    private sealed class TestMaterial : MaterialAggregate
    {
        /// <summary>
        /// 创建具有固定主键和名称的基础物料。
        /// </summary>
        /// <param name="id">大于 0 的物料主键。</param>
        /// <param name="name">用于关键字查询的物料名称。</param>
        public TestMaterial(int id, string name)
        {
            Id = id;
            MaterialCode = $"MAT-{id:D4}";
            MaterialName = name;
        }
    }
}
