using System.Collections.Generic;
using System.Threading.Tasks;
using Wcs.Controllers;
using Wcs.WMS;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace Wcs.Dispatch;

[Route("ecs/dispatch")]
[ApiController]
public class OrderController : WcsController
{
    private readonly IOrderService _orderService;
    private readonly IWMSService _wmsService;

    public OrderController(IOrderService orderService,IWMSService wmsService)
    {
        _orderService = orderService;
        _wmsService = wmsService;
    }

    [HttpPost("order/stockOrderCreate")]
    [MiddlewareFilter(typeof(ApiLogPipeline))]
    public async Task<ResponseDto> CreateStockOrder(AddStockOrderDto para)
    {
        ResponseDto responseDto = await _orderService.CreateStockOrder(para).ConfigureAwait(false);
        //通知wms状态发生改变（WCS接收到任务)
        if (responseDto.success == true)
        {
            TaskStatusDto statusDto = new()
            {
                OrderCode =para.orderCode,
                ExecState = "WcsCatched",
            };
            await _wmsService.SendTaskStatus(statusDto);
        }
        return responseDto;
    }

    [HttpPost("order/stockOrdersCreate")]
    [MiddlewareFilter(typeof(ApiLogPipeline))]
    public async Task<ResponseDto> CreateStockOrders(AddStockOrdersDto para)
    {
        return await _orderService.CreateStockOrders(para);
    }

    [HttpPost("order/checkOrderCreate")]
    [MiddlewareFilter(typeof(ApiLogPipeline))]
    public async Task<AddChkOrderResultDto> CreateCheckDownOrders(AddCheckOrderDto para)
    {
        return await _orderService.CreateCheckDownOrders(para).ConfigureAwait(false);
    }

    [HttpGet("order/checkOrderResultsGetByQueryCode")]
    public async Task<CheckOrderResultsDto> GetChkOdRsltByQueryCode(string queryCode)
    {
        return await _orderService.GetChkOdRsltByQueryCode(queryCode).ConfigureAwait(false);
    }

    [HttpGet("order/checkOrderResultsGetByOrderCode")]
    public async Task<CheckOrderResultsDto> GetChkOdRsltByOrderCode(string orderCode)
    {
        return await _orderService.GetChkOdRsltByOrderCode(orderCode).ConfigureAwait(false);
    }

    [HttpPost("order/doorCanOpenByOrder")]
    [MiddlewareFilter(typeof(ApiLogPipeline))]
    public async Task<ResponseDto> AllowOrderToOpenDoor(OpenDoorForOrderDto para)
    {
        return await _orderService.AllowOrderToOpenDoor(para).ConfigureAwait(false);
    }

    [HttpGet("order/state")]    
    [MiddlewareFilter(typeof(ApiLogPipeline))]
    public async Task<OrderStateDto> GetDispatchOrderState(string orderCode)
    {
        return await _orderService.GetDispatchOrderState(orderCode);
    }

    [HttpGet("order/states")]
    public async Task<OrderStatesDto> GetDispatchOrderStates()
    {
        return await _orderService.GetDispatchOrderStates();
    }

    [HttpGet("order/unDoneOrders")]
    public async Task<List<OrderInfoDto>> GetUnFinishedDispatchOrderDtos()
    {
        return await _orderService.GetUnFinishedDispatchOrderDtos();
    }

    [HttpGet("order/allOrders")]
    public async Task<List<OrderInfoDto>> GetAllDispatchOrderDtos()
    {
        return await _orderService.GetAllDispatchOrderDtos();
    }

    [HttpGet("order/oneOrder")]
    public async Task<OrderInfoDto> GetOneDispatchOrderDto(string orderCode)
    {
        return await _orderService.GetOneDispatchOrderDto(orderCode);
    }

    [HttpPost("order/forceDone")]
    [MiddlewareFilter(typeof(ApiLogPipeline))]
    public async Task<ResponseDto> ForceDoneDispatchOrderAsync(ForceDoneDto para)
    {
        return await _orderService.ForceDoneDispatchOrderAsync(para).ConfigureAwait(false);
    }

    [HttpPost("order/cancelOrder")]
    [MiddlewareFilter(typeof(ApiLogPipeline))]
    public async Task<ResponseDto> CancelDispatchOrderAsync(CancelOrderDto para)
    {
        return await _orderService.CancelDispatchOrderAsync(para);
    }

}