using System.Threading.Tasks;
using Lion.AbpPro.Extension.Customs.Dtos;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Volo.Abp.Application.Dtos;
using WarehouseManagement.Checks.Dto;

namespace WarehouseManagement.Checks
{
    [Route("Checks")]
    public class ChecksController : WarehouseManagementController, ICheckAppService
    {
        private readonly ICheckAppService _checkAppService;

        public ChecksController(ICheckAppService checkAppService)
        {
            _checkAppService = checkAppService;
        }

        [HttpPost("createWithArea")]
        [SwaggerOperation(summary: "创建区域盘点计划", Tags = new[] { "Checks" })]
        public async Task<CheckDto> CreateCheckByAreaAsync(CreateCheckDto input)
        {
            return await _checkAppService.CreateCheckByAreaAsync(input);
        }
        [HttpPost("checkExecute")]
        [SwaggerOperation(summary: "执行计划", Tags = new[] { "Checks" })]
        public async Task<bool> SetAsExecutingAsync(IdIntInput input)
        {
            return await _checkAppService.SetAsExecutingAsync(input);
        }
        [HttpPost("page")]
        [SwaggerOperation(summary: "获取出盘点计划", Tags = new[] { "Checks" })]
        public async Task<PagedResultDto<CheckDto>> GetPagingListAsync(PagingCheckListInput input)
        {
            return await _checkAppService.GetPagingListAsync(input); ;
        }
        [HttpPost("pageDetail")]
        [SwaggerOperation(summary: "获取盘点计划明细", Tags = new[] { "Checks" })]
        public async Task<PagedResultDto<CheckDetailDto>> GetPagingDetailListAsync(PagingCheckDetailInput input)
        {
            return await _checkAppService.GetPagingDetailListAsync(input);
        }
        [HttpPost("delete")]
        [SwaggerOperation(summary: "删除盘点计划", Tags = new[] { "Checks" })]
        public async Task DeleteAsync(IdIntInput input)
        {
            await _checkAppService.DeleteAsync(input);
        }
        [HttpPost("updateRealAmount")]
        [SwaggerOperation(summary: "更新盘点数量", Tags = new[] { "Checks" })]
        public async Task UpdateRealAmountAsync(UpdateCheckDetailDto input)
        {
            await _checkAppService.UpdateRealAmountAsync(input);
        }
        [HttpPost("inventoryConfirm")]
        [SwaggerOperation(summary: "账实一致", Tags = new[] { "Checks" })]
        public async Task<bool> InventoryConfirm(IdIntInput input)
        {
            return await _checkAppService.InventoryConfirm(input);
        }
        [HttpPost("inventoryLossConfirm")]
        [SwaggerOperation(summary: "盘亏确认", Tags = new[] { "Checks" })]
        public async Task<bool> InventoryLossConfirm(IdIntInput input)
        {
            return await _checkAppService.InventoryLossConfirm(input);
        }
        [HttpPost("createSurplusIn")]
        [SwaggerOperation(summary: "盘盈入库", Tags = new[] { "Checks" })]
        public async Task CreateSurplusIn(string boxRfid,string cellName )
        {
            await _checkAppService.CreateSurplusIn(boxRfid, cellName);
        }
        [HttpPost("createLossOut")]
        [SwaggerOperation(summary: "盘亏出库", Tags = new[] { "Checks" })]
        public async Task CreateLossOut(string boxRfid, string cellName)
        {
            await _checkAppService.CreateLossOut(boxRfid, cellName);
        }
        [HttpPost("checkComplete")]
        [SwaggerOperation(summary: "盘点完成", Tags = new[] { "Checks" })]
        public async Task<bool> CheckComplete(string input)
        {
            return await _checkAppService.CheckComplete(input);
        }


    }
}
