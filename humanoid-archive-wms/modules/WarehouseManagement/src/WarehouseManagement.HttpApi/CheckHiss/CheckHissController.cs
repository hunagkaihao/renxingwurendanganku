using System.Threading.Tasks;
using Lion.AbpPro.Extension.Customs.Dtos;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Volo.Abp.Application.Dtos;
using WarehouseManagement.CheckHiss.Dto;

namespace WarehouseManagement.CheckHiss
{
    [Route("CheckHiss")]
    public class CheckHissController : WarehouseManagementController, ICheckHisAppService
    {
        private readonly ICheckHisAppService _checkHisAppService;

        public CheckHissController(ICheckHisAppService checkHisAppService)
        {
            _checkHisAppService = checkHisAppService;
        }

        [HttpPost("page")]
        [SwaggerOperation(summary: "获取出盘点计划历史", Tags = new[] { "CheckHiss" })]
        public async Task<PagedResultDto<CheckHisDto>> GetPagingListAsync(PagingCheckHisDto input)
        {
            return await _checkHisAppService.GetPagingListAsync(input); ;
        }
        [HttpPost("pageDetail")]
        [SwaggerOperation(summary: "获取盘点计划历史明细", Tags = new[] { "CheckHiss" })]
        public async Task<PagedResultDto<CheckDetailHisDto>> GetPagingDetailListAsync(PagingCheckDetailHisDto input)
        {
            return await _checkHisAppService.GetPagingDetailListAsync(input);
        }
    }
}
