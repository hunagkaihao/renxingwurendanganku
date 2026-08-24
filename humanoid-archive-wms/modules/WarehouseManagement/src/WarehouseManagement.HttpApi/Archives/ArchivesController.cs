using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WarehouseManagement.Archives.Dto;
using Swashbuckle.AspNetCore.Annotations;
using Volo.Abp.Application.Dtos;

namespace WarehouseManagement.Archives
{
    [Route("Archives")]
    public class ArchivesController : WarehouseManagementController, IArchiveAppService
    {
        private readonly IArchiveAppService _archiveAppService;
        public ArchivesController(IArchiveAppService archiveAppService)
        {
            _archiveAppService = archiveAppService;
        }

        [HttpPost("create")]
        [SwaggerOperation(summary: "创建档案", Tags = new[] { "Archives" })]
        public async Task<ArchiveDto> CreateAsync(CreateArchiveDto input)
        {
            return await _archiveAppService.CreateAsync(input);
        }
        [HttpPost("delete")]
        [SwaggerOperation(summary: "删除档案", Tags = new[] { "Archives" })]
        public async Task DeleteAsync(CreateArchiveDto input)
        {
             await _archiveAppService.DeleteAsync(input);
        }
        [HttpPost("update")]
        [SwaggerOperation(summary: "编辑档案", Tags = new[] { "Archives" })]
        public async Task<ArchiveDto> UpdateAsync(CreateArchiveDto input)
        {
            return await _archiveAppService.UpdateAsync(input);
        }
        [HttpPost("page")]
        [SwaggerOperation(summary: "查询档案", Tags = new[] { "Archives" })]
        public async Task<PagedResultDto<ArchiveDto>> PageAsync(PagingArchiveListInput input)
        {
            return await _archiveAppService.PageAsync(input);
        }
    }
}
