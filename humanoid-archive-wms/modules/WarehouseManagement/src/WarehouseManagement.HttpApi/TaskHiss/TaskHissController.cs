using Lion.AbpPro.Extension.Customs.Dtos;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using WarehouseManagement.TaskHiss.Dto;

namespace WarehouseManagement.TaskHiss
{
    [Route("TaskHiss")]
    public class TaskHissController : WarehouseManagementController, ITaskHisAppService
    {
        private readonly ITaskHisAppService _taskHisAppService;

        public TaskHissController(ITaskHisAppService taskHisAppService)
        {
            _taskHisAppService = taskHisAppService;
        }

        [HttpPost("page")]
        [SwaggerOperation(summary: "获取出入库记录", Tags = new[] { "TaskHiss" })]
        public async Task<PagedResultDto<TaskHisDto>> GetPagingListAsync(PagingTaskHisListInput input)
        {
            return await _taskHisAppService.GetPagingListAsync(input); ;
        }
        [HttpPost("pageDetail")]
        [SwaggerOperation(summary: "获取出入库明细", Tags = new[] { "TaskHiss" })]
        public async Task<PagedResultDto<TaskHisDetailDto>> GetPagingDetailListAsync(
    PagingTaskHisDetailInput input)
        {
            return await _taskHisAppService.GetPagingDetailListAsync(input);
        }
    }
}
