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
using WarehouseManagement.MaterialBoxs.Dto;

namespace WarehouseManagement.MaterialBoxs
{
    [Route("MaterialBoxs")]
    public class MaterialBoxsController : WarehouseManagementController, IMaterialBoxAppService
    {
        private readonly IMaterialBoxAppService _materialBoxAppService;
        public MaterialBoxsController(IMaterialBoxAppService materialBoxAppService)
        {
            _materialBoxAppService = materialBoxAppService;
        }
        [HttpPost("create")]
        [SwaggerOperation(summary: "创建物料容器", Tags = new[] { "MaterialBoxs" })]
        public async Task<MaterialBoxDto> CreateAsync(CreateMaterialBoxDto input)
        {
            return await _materialBoxAppService.CreateAsync(input);
        }
        [HttpPost("update")]
        [SwaggerOperation(summary: "编辑物料容器", Tags = new[] { "MaterialBoxs" })]
        public async Task<MaterialBoxDto> UpdateAsync(CreateMaterialBoxDto input)
        {
            return await _materialBoxAppService.UpdateAsync(input);
        }
        [HttpPost("delete")]
        [SwaggerOperation(summary: "删除物料容器", Tags = new[] { "MaterialBoxs" })]
        public async Task DeleteAsync(CreateMaterialBoxDto input)
        {
             await _materialBoxAppService.DeleteAsync(input);
        }
        [HttpPost("page")]
        [SwaggerOperation(summary: "获取物料容器数据", Tags = new[] { "MaterialBoxs" })]
        public async Task<PagedResultDto<MaterialBoxDto>> PageAsync(PagingMaterialBoxListInput input)
        {
            return await _materialBoxAppService.PageAsync(input);
        }
        [HttpPost("pageDetail")]
        [SwaggerOperation(summary: "获取物料容器数据明细", Tags = new[] { "MaterialBoxs" })]
        public async Task<PagedResultDto<MaterialBoxDetailDto>> DetailAsync(PagingMaterialBoxDetailInput input)
        {
            return await _materialBoxAppService.DetailAsync(input);
        }
        [HttpPost("bindRfid")]
        [SwaggerOperation(summary: "物料容器绑定标签", Tags = new[] { "MaterialBoxs" })]
        public async Task<MaterialBoxDto> BindRfid(CreateMaterialBoxDto input)
        {
            return await _materialBoxAppService.BindRfid(input);
        }
        [HttpPost("bindMaterial")]
        [SwaggerOperation(summary: "物料容器绑定物料", Tags = new[] { "MaterialBoxs" })]
        public async Task<bool> BindArchive(string MaterialBoxRfid, string MaterialRfid)
        {
            return await _materialBoxAppService.BindArchive(MaterialBoxRfid, MaterialRfid);
        }

    }
}
