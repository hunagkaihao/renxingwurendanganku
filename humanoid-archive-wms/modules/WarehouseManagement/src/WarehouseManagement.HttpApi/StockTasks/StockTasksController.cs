using Lion.AbpPro.Extension.Customs.Dtos;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using WarehouseManagement.StockTasks.Dto;

namespace WarehouseManagement.StockTasks
{
    [Route("StockTasks")]
    public class StockTasksController : WarehouseManagementController, IStockTaskAppService
    {
        private readonly IStockTaskAppService _stockTaskAppService;

        public StockTasksController(IStockTaskAppService stockTaskAppService)
        {
            _stockTaskAppService = stockTaskAppService;
        }

        [HttpPost("delete")]
        [SwaggerOperation(summary: "删除任务", Tags = new[] { "StockTasks" })]
        public async Task DeleteAsync(IdIntInput input)
        {
            await _stockTaskAppService.DeleteAsync(input);
        }
        [HttpPost("page")]
        [SwaggerOperation(summary: "获取任务清单", Tags = new[] { "StockTasks" })]
        public async Task<PagedResultDto<StockTaskDto>> GetPagingListAsync(PagingStockTaskListInput input)
        {
            return await _stockTaskAppService.GetPagingListAsync(input); ;
        }
        [HttpPost("pageDetail")]
        [SwaggerOperation(summary: "获取任务清单明细", Tags = new[] { "StockTasks" })]
        public async Task<PagedResultDto<StockTaskDetailDto>> GetPagingDetailListAsync(PagingStockTaskDetailInput input)
        {
            return await _stockTaskAppService.GetPagingDetailListAsync(input);
        }
        [HttpPost("pageDetailByArchiveId")]
        [SwaggerOperation(summary: "获取档案出入库任务清单明细", Tags = new[] { "StockTasks" })]
        public async Task<PagedResultDto<StockTaskDetailDto>> GetPagingDetailListByArchiveIdAsync(PagingStockTaskDetailInput input)
        {
            return await _stockTaskAppService.GetPagingDetailListByArchiveIdAsync(input);
        }
        [HttpPost("update")]
        [SwaggerOperation(summary: "修改档案盒", Tags = new[] { "StockTasks" })]
        public async Task<StockTaskDto> UpdateAsync(UpdateStockTaskDto input)
        {
            return await _stockTaskAppService.UpdateAsync(input);
        }
        [HttpPost("taskCancel")]
        [SwaggerOperation(summary: "取消任务", Tags = new[] { "StockTasks" })]
        public async Task<StockTaskDto> SetAsCancelAsync(IdIntInput input)
        {
            return await _stockTaskAppService.SetAsCancelAsync(input);
        }
        [HttpPost("pickOutTask")]
        [SwaggerOperation(summary: "创建借阅出库任务", Tags = new[] { "StockTasks" })]
        public async Task<bool> PickOutTask(List<PickOutDto> input)
        {
            return await _stockTaskAppService.PickOutTask(input);
        }

        [HttpPost("createWCSIn")]
        [SwaggerOperation(summary: "创建档案入库任务", Tags = new[] { "StockTasks" })]
        public async Task<StockTaskDto> CreateWCSIn(CreateStockTaskDto input)
        {
            return await _stockTaskAppService.CreateWCSIn(input);
        }
        [HttpPost("wcsInSetCell")]
        [SwaggerOperation(summary: "档案任务分配库位", Tags = new[] { "StockTasks" })]
        public async Task<Boolean> WCSSetCell(int input)
        {
            return await _stockTaskAppService.WCSSetCell(input);
        }
        [HttpPost("openDoorAndWCSInExcute")]
        [SwaggerOperation(summary: "扫码打开柜门下达给WCS", Tags = new[] { "StockTasks" })]
        public async Task<StockTaskDto> OpenDoorAndWCSInExcute(int input)
        {
            return await _stockTaskAppService.OpenDoorAndWCSInExcute(input);
        }

        [HttpPost("createWCSOut")]
        [SwaggerOperation(summary: "创建档案出库任务", Tags = new[] { "StockTasks" })]
        public async Task<StockTaskDto> CreateWCSOut(CreateStockTaskDto input)
        {
            return await _stockTaskAppService.CreateWCSOut(input);
        }
        [HttpPost("batBoxInByArea")]
        [SwaggerOperation(summary: "创建档案批量入库任务", Tags = new[] { "StockTasks" })]
        public async Task<bool> BatBoxInByArea(string input)
        {
            return await _stockTaskAppService.BatBoxInByArea(input);
        }
        [HttpPost("clientInCell")]
        [SwaggerOperation(summary: "一体机创建档案入库任务", Tags = new[] { "StockTasks" })]
        public async Task<bool> TaskAssignUseRfid(string rfid)
        {
            return await _stockTaskAppService.TaskAssignUseRfid(rfid);
        }
        [HttpPost("clientOutCell")]
        [SwaggerOperation(summary: "一体机创建档案出库任务", Tags = new[] { "StockTasks" })]
        public async Task<bool> ClientOutCell(string rfid)
        {
            return await _stockTaskAppService.ClientOutCell(rfid);
        }
        [HttpPost("allInOutTask")]
        [SwaggerOperation(summary: "一体机档案任务", Tags = new[] { "StockTasks" })]
        public async Task<List<StockTaskDto>> GetInOutTask()
        {
            return await _stockTaskAppService.GetInOutTask();
        }
        [HttpPost("openDoor")]
        [SwaggerOperation(summary: "一体机档案任务", Tags = new[] { "StockTasks" })]
        public async Task ControlDoorOpen(int stockId)
        {
            await _stockTaskAppService.ControlDoorOpen(stockId);
        }
        [HttpPost("taskAssign")]
        [SwaggerOperation(summary: "一体机任务自动分配", Tags = new[] { "StockTasks" })]
        public async Task TaskAssign(int stockId)
        {
            await _stockTaskAppService.TaskAssign(stockId);
        }
        [HttpPost("forceComplete")]
        [SwaggerOperation(summary: "一体机任务强制完成任务", Tags = new[] { "StockTasks" })]
        public async Task ForceComplete(int stockId)
        {
            await _stockTaskAppService.ForceComplete(stockId);
        }
        [HttpPost("createBatTest")]
        [SwaggerOperation(summary: "疲劳测试", Tags = new[] { "StockTasks" })]
        public async Task CreateBatTest()
        {
            await _stockTaskAppService.CreateBatTest();
        }
        
    }
}
