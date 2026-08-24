using Lion.AbpPro.Extension.Customs.Dtos;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using WarehouseManagement.Plans.Dto;

namespace WarehouseManagement.Plans
{
    [Route("Plans")]
    public class PlansController : WarehouseManagementController, IPlanAppService
    {
        private readonly IPlanAppService _PlanAppService;

        public PlansController(IPlanAppService PlanAppService)
        {
            _PlanAppService = PlanAppService;
        }
        [HttpPost("createPlan")]
        [SwaggerOperation(summary: "创建计划任务", Tags = new[] { "Plans" })]
        public async Task CreatePlanAsync(CreatePlanDto input)
        {
            await _PlanAppService.CreatePlanAsync(input);
        }
        [HttpPost("delete")]
        [SwaggerOperation(summary: "删除计划任务", Tags = new[] { "Plans" })]
        public async Task DeleteAsync(IdIntInput input)
        {
            await _PlanAppService.DeleteAsync(input);
        }

  
        [HttpPost("page")]
        [SwaggerOperation(summary: "获取计划任务清单", Tags = new[] { "Plans" })]
        public async Task<PagedResultDto<PlanDto>> GetPagingListAsync(PagingPlanListInput input)
        {
            return await _PlanAppService.GetPagingListAsync(input); ;
        }
        [HttpPost("update")]
        [SwaggerOperation(summary: "修改计划任务", Tags = new[] { "Plans" })]
        public async Task<PlanDto> UpdateAsync(UpdatePlanDto input)
        {
            return await _PlanAppService.UpdateAsync(input);
        }
        [HttpPost("setExecuting")]
        [SwaggerOperation(summary: "修改计划任务", Tags = new[] { "Plans" })]
        public async Task<bool> SetExecuting(int planId)
        {
             return await _PlanAppService.SetExecuting(planId);
        }

    }
}
