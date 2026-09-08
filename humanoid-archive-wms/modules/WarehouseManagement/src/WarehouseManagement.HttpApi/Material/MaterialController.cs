using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WarehouseManagement.Material.Dto;
using Swashbuckle.AspNetCore.Annotations;
using Volo.Abp.Application.Dtos;

namespace WarehouseManagement.Material
{
    [Route("Material")]
    public class MaterialController : WarehouseManagementController, IMaterialAppService
    {
        private readonly IMaterialAppService _materialAppService;
        public MaterialController(IMaterialAppService materialAppService)
        {
            _materialAppService = materialAppService;
        }

        [HttpPost("create")]
        [SwaggerOperation(summary: "创建物料", Tags = new[] { "Material" })]
        public async Task<MaterialDto> CreateAsync(CreateMaterialDto input)
        {
            return await _materialAppService.CreateAsync(input);
        }
        [HttpPost("delete")]
        [SwaggerOperation(summary: "删除物料", Tags = new[] { "Material" })]
        public async Task DeleteAsync(CreateMaterialDto input)
        {
             await _materialAppService.DeleteAsync(input);
        }
        [HttpPost("update")]
        [SwaggerOperation(summary: "编辑物料", Tags = new[] { "Material" })]
        public async Task<MaterialDto> UpdateAsync(CreateMaterialDto input)
        {
            return await _materialAppService.UpdateAsync(input);
        }
        [HttpPost("page")]
        [SwaggerOperation(summary: "查询物料页", Tags = new[] { "Material" })]
        public async Task<PagedResultDto<MaterialDto>> PageAsync(PagingMaterialListInput input)
        {
            return await _materialAppService.PageAsync(input);
        }
    }
}
