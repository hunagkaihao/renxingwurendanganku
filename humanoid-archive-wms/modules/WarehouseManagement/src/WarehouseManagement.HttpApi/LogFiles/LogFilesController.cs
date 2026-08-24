using Lion.AbpPro.Extension.Customs.Dtos;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using WarehouseManagement.LogFiles.Dto;
namespace WarehouseManagement.LogFiles
{
    [Route("LogFiles")]
    public class LogFilesController : WarehouseManagementController, ILogFileAppService
    {
        private readonly ILogFileAppService _logFileAppService;

        public LogFilesController(ILogFileAppService logFileAppService)
        {
            _logFileAppService = logFileAppService;
        }

        [HttpPost("page")]
        [SwaggerOperation(summary: "获取清单", Tags = new[] { "LogFiles" })]
        public async Task<PagedResultDto<LogFileDto>> GetPagingListAsync()
        {
            return await _logFileAppService.GetPagingListAsync(); ;
        }
    }
}
