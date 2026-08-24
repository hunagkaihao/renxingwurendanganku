using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseManagement.Cells;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using WarehouseManagement.Cells.Dto;
using WarehouseManagement.Boards.Dto;

namespace WarehouseManagement.Boards
{
    [Route("Boards")]
    public class BoardsController : WarehouseManagementController, IBoardAppService
    {
        public readonly IBoardAppService _boardAppService;

        public BoardsController(IBoardAppService boardAppService)
        {
            _boardAppService = boardAppService;
        }

        [HttpPost("getSevenDayTasks")]
        [SwaggerOperation(summary: "获取七日出入库任务", Tags = new[] { "Boards" })]
        public async Task<SevenDayTasksDto> GetSevenDayTasks()
        {
            return await _boardAppService.GetSevenDayTasks();
        }
        [HttpPost("getStockInfo")]
        [SwaggerOperation(summary: "获取库存信息", Tags = new[] { "Boards" })]
        public async Task<StockInfoDto> GetStockInfo()
        {
            return await _boardAppService.GetStockInfo();
        }

    }
}
