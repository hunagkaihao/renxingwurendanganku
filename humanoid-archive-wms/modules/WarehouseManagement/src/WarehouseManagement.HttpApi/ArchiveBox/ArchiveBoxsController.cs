using Lion.AbpPro.Extension.Customs.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using WarehouseManagement.ArchiveBoxs.Dto;

namespace WarehouseManagement.ArchiveBoxs
{
    [Route("ArchiveBoxs")]
    public class ArchiveBoxsController : WarehouseManagementController, IArchiveBoxAppService
    {
        private readonly IArchiveBoxAppService _archiveBoxAppService;
        public ArchiveBoxsController(IArchiveBoxAppService archiveBoxAppService)
        {
            _archiveBoxAppService = archiveBoxAppService;
        }
        [HttpPost("create")]
        [SwaggerOperation(summary: "创建档案盒", Tags = new[] { "ArchiveBoxs" })]
        public async Task<ArchiveBoxDto> CreateAsync(CreateArchiveBoxDto input)
        {
            return await _archiveBoxAppService.CreateAsync(input);
        }
        [HttpPost("update")]
        [SwaggerOperation(summary: "编辑档案盒", Tags = new[] { "ArchiveBoxs" })]
        public async Task<ArchiveBoxDto> UpdateAsync(CreateArchiveBoxDto input)
        {
            return await _archiveBoxAppService.UpdateAsync(input);
        }
        [HttpPost("delete")]
        [SwaggerOperation(summary: "删除档案盒", Tags = new[] { "ArchiveBoxs" })]
        public async Task DeleteAsync(CreateArchiveBoxDto input)
        {
             await _archiveBoxAppService.DeleteAsync(input);
        }
        [HttpPost("page")]
        [SwaggerOperation(summary: "获取档案盒数据", Tags = new[] { "ArchiveBoxs" })]
        public async Task<PagedResultDto<ArchiveBoxDto>> PageAsync(PagingArchiveBoxListInput input)
        {
            return await _archiveBoxAppService.PageAsync(input);
        }
        [HttpPost("pageDetail")]
        [SwaggerOperation(summary: "获取档案盒数据明细", Tags = new[] { "ArchiveBoxs" })]
        public async Task<PagedResultDto<ArchiveBoxDetailDto>> DetailAsync(PagingArchiveBoxDetailInput input)
        {
            return await _archiveBoxAppService.DetailAsync(input);
        }
        [HttpPost("bindRfid")]
        [SwaggerOperation(summary: "档案盒绑定标签", Tags = new[] { "ArchiveBoxs" })]
        public async Task<ArchiveBoxDto> BindRfid(CreateArchiveBoxDto input)
        {
            return await _archiveBoxAppService.BindRfid(input);
        }
        [HttpPost("bindArchive")]
        [SwaggerOperation(summary: "档案盒绑定档案", Tags = new[] { "ArchiveBoxs" })]
        public async Task<bool> BindArchive(string ArchiveBoxRfid, string ArchiveRfid)
        {
            return await _archiveBoxAppService.BindArchive(ArchiveBoxRfid, ArchiveRfid);
        }

    }
}
