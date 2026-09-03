using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Wcs.Dispatch;

public interface IOrderService : IApplicationService
{
    public Task<ResponseDto> CreateStockOrder(AddStockOrderDto para);

    public Task<ResponseDto> CreateStockOrders(AddStockOrdersDto para);

    public Task<AddChkOrderResultDto> CreateCheckDownOrders(AddCheckOrderDto para);

    public Task<AddChkOrderResultDto> ChkOrderDown(AddCheckOrderDto para);

    public Task<CheckOrderResultsDto> GetChkOdRsltByQueryCode(string queryCode);

    public Task<CheckOrderResultsDto> GetChkOdRsltByOrderCode(string orderCode);

    public Task<ResponseDto> AllowOrderToOpenDoor(OpenDoorForOrderDto para);

    public Task<OrderStateDto> GetDispatchOrderState(string orderCode);

    public Task<OrderStatesDto> GetDispatchOrderStates();

    public Task<List<OrderInfoDto>> GetUnFinishedDispatchOrderDtos();

    public Task<List<OrderInfoDto>> GetAllDispatchOrderDtos();

    public Task<OrderInfoDto> GetOneDispatchOrderDto(string orderCode);

    public Task<ResponseDto> ForceDoneDispatchOrderAsync(ForceDoneDto para);

    public Task<ResponseDto> CancelDispatchOrderAsync(CancelOrderDto para);
}