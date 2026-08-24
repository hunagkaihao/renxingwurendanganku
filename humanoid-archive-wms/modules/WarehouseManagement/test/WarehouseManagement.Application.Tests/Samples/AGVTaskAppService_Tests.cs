using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Lion.AbpPro.ConfigurationOptions;
using Lion.AbpPro.Extension.Customs.Dtos;
using Lion.AbpPro.Extension.Customs.Http;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using Shouldly;
using WarehouseManagement.AgvTasks;
using WarehouseManagement.AgvTasks.Dto;
using WarehouseManagement.Cells;
using WarehouseManagement.StockTasks;
using WarehouseManagement.StockTasks.Dto;
using WarehouseManagement.StorageBoxs;
using WarehouseManagement.StorageBoxs.Dto;
using WarehouseManagement.TaskHiss;
using WarehouseManagement.Warehouses;
using Xunit;

namespace WarehouseManagement.Samples;

public class AGVTaskAppService_Tests : WarehouseManagementApplicationTestBase
{

    private readonly RcsApiManager _rcsApiManager;

    public AGVTaskAppService_Tests()
    {
        //Arrange
        var mockFactory = new Mock<IHttpClientFactory>();
        var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("{'code':'0','message':'success'}"),
            });



        var client = new HttpClient(mockHttpMessageHandler.Object);
        mockFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);
        AGVOptions agvoptions = new AGVOptions()
        {
            Server = "https://lims.nbjlw.com:8091",
            Enable = "false"
        };
        var mock = new Mock<IOptionsSnapshot<AGVOptions>>();
        mock.Setup(m => m.Value).Returns(agvoptions);

        _rcsApiManager = new RcsApiManager(mockFactory.Object, mock.Object);

    }

    [Fact]
    public async Task CreateTaskAsync()
    {
      var aa=  await _rcsApiManager.CancelTaskAsync("aa");
        aa.Code.ShouldBe("0");
    }

        //[Fact]
        //public async Task CreateTaskAsync()
        //{
        //string reqCode = Guid.NewGuid().ToString("N");
        //string taskTyp = "";
        ////Arrange
        //var mockFactory = new Mock<IHttpClientFactory>();
        //var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        //mockHttpMessageHandler.Protected()
        //    .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
        //    .ReturnsAsync(new HttpResponseMessage
        //    {
        //        StatusCode = HttpStatusCode.OK,
        //        Content = new StringContent("{'name':thecodebuzz,'city':'USA'}"),
        //    });



        //var client = new HttpClient(mockHttpMessageHandler.Object);
        //mockFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);

        //RcsApiManager rcsApiManager = new RcsApiManager(mockFactory.Object, null);
        //var response = await _rcsApiManager.CreateTaskAsync(reqCode, taskTyp, null, null, null);
        //response.ShouldNotBeNull();
        //}
        //[Fact]
        //public async Task GetAsync()
        //{
        //    //PagingStorageBoxListInput input =new PagingStorageBoxListInput() { 
        //    //    PageSize = 10,
        //    //    PageIndex = 1
        //    //};
        //    //var result = await _storageBoxAppService.GetPagingListAsync(input);
        //    ////Assert
        //    //result.TotalCount.ShouldBeGreaterThan(0);
        //    //result.Items.ShouldContain(u => u.Id == 1);
        //}
    }

