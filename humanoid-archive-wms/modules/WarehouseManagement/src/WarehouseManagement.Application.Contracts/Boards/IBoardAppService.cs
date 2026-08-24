
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using WarehouseManagement.Archives.Dto;
using WarehouseManagement.Boards.Dto;

namespace WarehouseManagement.Boards
{
    public interface IBoardAppService : IApplicationService
    {
        //获取七日出入库数量
        Task<SevenDayTasksDto> GetSevenDayTasks();
        //获取库存信息
        Task<StockInfoDto> GetStockInfo();
        
    }
}
