using System;
using System.Collections.Generic;
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
using WarehouseManagement.ERPApis;
using WarehouseManagement.ERPApis.Dto;
using WarehouseManagement.Goodss;
using WarehouseManagement.StockTasks;
using WarehouseManagement.StockTasks.Dto;
using WarehouseManagement.StorageBoxs;
using WarehouseManagement.StorageBoxs.Dto;
using WarehouseManagement.TaskHiss;
using WarehouseManagement.Warehouses;
using Xunit;

namespace WarehouseManagement.Samples;

public class ERPApiAppService_Tests : WarehouseManagementApplicationTestBase
{
    private readonly IERPApiAppService _eRPApiAppService;
    private readonly StorageBoxManager _storageBoxManagement;
    private readonly CellManager _cellManager;
    private readonly WarehouseManager _warehouseManager;
    private readonly GoodsManager _goodsManager;



    //private readonly IHttpClientFactory _httpClientFactory;
    //private readonly AGVOptions _aGVOptions;

    public ERPApiAppService_Tests()
    {
        _storageBoxManagement = GetRequiredService<StorageBoxManager>();
        _cellManager = GetRequiredService<CellManager>();
        _warehouseManager = GetRequiredService<WarehouseManager>();
        _goodsManager = GetRequiredService<GoodsManager>();
        //_httpClientFactory = GetRequiredService<HttpClientHelper>();
        //_aGVOptions = GetRequiredService<AGVOptions>();
        InitContext();//初始化内存数据
        _eRPApiAppService = GetRequiredService<IERPApiAppService>();

    }

    private async void InitContext()
    {
        //初始化仓库
        await _warehouseManager.CreateAsync("CTU01", "1#仓库", "CTU");
        //初始化料箱
        {
            await _storageBoxManagement.CreateAsync("B1001", "1");
            await _storageBoxManagement.CreateAsync("B1002", "1");
            await _storageBoxManagement.CreateAsync("B1003", "1");
        }
        //初始化物料
        {
            await _goodsManager.CreateAsync("G1001", "G1001", "G1001", "G1001");
            await _goodsManager.CreateAsync("G1002", "G1002", "G1002", "G1002");
            await _goodsManager.CreateAsync("G1003", "G1003", "G1003", "G1003");
        }
        //初始化组箱
        {
            await _storageBoxManagement.CreateDetailAsync(1, 1, 10, "", 0, null, null);
            await _storageBoxManagement.CreateDetailAsync(2, 2, 5, "", 0, null, null);
            await _storageBoxManagement.CreateDetailAsync(2, 3, 8, "", 0, null, null);
        }
        //初始化高位库位
        {
            await _cellManager.CreateAsync("01-01-01", "CTUCell", "01-01-01", 1);
            await _cellManager.CreateAsync("01-02-01", "CTUCell", "01-02-01", 1);
            await _cellManager.CreateAsync("01-01-02", "CTUCell", "01-01-02", 1);
            await _cellManager.CreateAsync("01-02-02", "CTUCell", "01-02-02", 1);
        }
    }
    [Fact]
    public async Task GetWMSStocksAsync()
    {
        //step1  绑定料箱和库位
        var box = await _storageBoxManagement.UpdateStockCellAsync("B1001", 2);
        box.Id.ShouldBe(1);
        box.CellId.ShouldBe(2);
        //var box2 = await _storageBoxManagement.UpdateStockCellAsync("B1002", 3);
        //box2.Id.ShouldBe(2);
        //box2.CellId.ShouldBe(3);
        var goodCodes = new List<WMSGoodsDto>() { new WMSGoodsDto() { GoodsCode= "G1001" }, new WMSGoodsDto() { GoodsCode = "G1002" } };
        var stocks = await _eRPApiAppService.GetWMSStocksAsync(goodCodes);
        stocks.Count.ShouldBe(2);
        var stockA =stocks.Find(f => f.GoodsCode == "G1001");
        stockA.Quantity.ShouldBe(10);
        stockA.BoxCode.ShouldBe("B1001");
        stockA.CellCode.ShouldBe("01-02-01");
        var stockB = stocks.Find(f => f.GoodsCode == "G1002");
        stockB.Quantity.ShouldBe(5);
        stockB.BoxCode.ShouldBe("B1002");
        stockB.CellCode.ShouldBeNull();
    }

}
