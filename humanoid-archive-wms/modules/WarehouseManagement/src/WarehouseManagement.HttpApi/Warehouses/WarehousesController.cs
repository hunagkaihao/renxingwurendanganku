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
using WarehouseManagement.Warehouses.Dto;

namespace WarehouseManagement.Warehouses
{
    [Route("Warehouses")]
    public class WarehousesController : WarehouseManagementController, IWarehouseAppService
    {
        private readonly IWarehouseAppService _warehouseAppService;

        public WarehousesController(IWarehouseAppService warehouseAppService)
        {
            _warehouseAppService = warehouseAppService;
        }

        [HttpPost("create")]
        [SwaggerOperation(summary: "创建仓库", Tags = new[] { "Warehouses" })]
        public async Task<WarehouseDto> CreateAsync(CreateWarehouseDto input)
        {
            return await _warehouseAppService.CreateAsync(input);
        }     
        [HttpPost("delete")]
        [SwaggerOperation(summary: "删除仓库", Tags = new[] { "Warehouses" })]
        public async Task DeleteAsync(IdIntInput input)
        {
            await _warehouseAppService.DeleteAsync(input);
        }
        [HttpPost("page")]
        [SwaggerOperation(summary: "获取仓库清单", Tags = new[] { "Warehouses" })]
        public async Task<PagedResultDto<WarehouseDto>> GetPagingListAsync(PagingWarehouseListInput input)
        {
            return await _warehouseAppService.GetPagingListAsync(input); ;
        }
        [HttpPost("update")]
        [SwaggerOperation(summary: "更新仓库", Tags = new[] { "Warehouses" })]
        public async Task<WarehouseDto> UpdateAsync(UpdateWarehouseDto input)
        {
            return await _warehouseAppService.UpdateAsync(input);
        }


        [HttpPost("createArea")]
        [SwaggerOperation(summary: "创建仓库区域", Tags = new[] { "Warehouses" })]
        public async Task<WarehouseAreaDto> CreateAreaAsync(CreateWarehouseAreaDto input)
        {
            return await _warehouseAppService.CreateAreaAsync(input);
        }
        [HttpPost("deleteArea")]
        [SwaggerOperation(summary: "删除仓库区域", Tags = new[] { "Warehouses" })]
        public async Task DeleteAreaAsync(IdIntInput input)
        {
            await _warehouseAppService.DeleteAreaAsync(input);
        }
        [HttpPost("pageArea")]
        [SwaggerOperation(summary: "获取仓库区域清单", Tags = new[] { "Warehouses" })]
        public async Task<PagedResultDto<WarehouseAreaDto>> GetAreaPagingListAsync(PagingWarehouseAreaListInput input)
        {
            return await _warehouseAppService.GetAreaPagingListAsync(input); ;
        }
        [HttpPost("updateArea")]
        [SwaggerOperation(summary: "更新仓库区域", Tags = new[] { "Warehouses" })]
        public async Task<WarehouseAreaDto> UpdateAreaAsync(UpdateWarehouseAreaDto input)
        {
            return await _warehouseAppService.UpdateAreaAsync(input);
        }
    }
}
