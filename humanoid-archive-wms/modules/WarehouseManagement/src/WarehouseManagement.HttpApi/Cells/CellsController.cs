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
using WarehouseManagement.Cells.Dto;

namespace WarehouseManagement.Cells
{
    [Route("Cells")]
    public class CellsController : WarehouseManagementController, ICellAppService
    {
        private readonly ICellAppService _cellAppService;

        public CellsController(ICellAppService cellAppService)
        {
            _cellAppService = cellAppService;
        }

        [HttpPost("create")]
        [SwaggerOperation(summary: "创建库位", Tags = new[] { "Cells" })]
        public async Task<CellDto> CreateAsync(CreateCellDto input)
        {
            return await _cellAppService.CreateAsync(input);
        }
        [HttpPost("initCreateCell")]
        [SwaggerOperation(summary: "批量创建库位", Tags = new[] { "Cells" })]
        public async Task InitCreateCell(CellInitDto input)
        {
            await _cellAppService.InitCreateCell(input);
        }
        [HttpPost("customCreate")]
        [SwaggerOperation(summary: "创建人工库位", Tags = new[] { "Cells" })]
        public async Task<CellDto> CustomCreateAsync(CustomCreateCellDto input)
        {
            return await _cellAppService.CustomCreateAsync(input);
        }
        [HttpPost("createStation")]
        [SwaggerOperation(summary: "创建工作站", Tags = new[] { "Cells" })]
        public async Task<CellDto> CreateStationAsync(CreateStationDto input)
        {
            return await _cellAppService.CreateStationAsync(input);
        }
        [HttpPost("getByCode")]
        [SwaggerOperation(summary: "通过库位编码查询", Tags = new[] { "Cells" })]
        public async Task<CellDto> GetByCodeAsync(CreateCellDto input)
        {
            return await _cellAppService.GetByCodeAsync(input);
        }        
        [HttpPost("delete")]
        [SwaggerOperation(summary: "删除库位", Tags = new[] { "Cells" })]
        public async Task DeleteAsync(IdIntInput input)
        {
            await _cellAppService.DeleteAsync(input);
        }
        [HttpPost("all")]
        [SwaggerOperation(summary: "获取所有库位", Tags = new[] { "Cells" })]
        public async Task<ListResultDto<CellDto>> AllListAsync()
        {
            return await _cellAppService.AllListAsync();
        }
        [HttpPost("page")]
        [SwaggerOperation(summary: "获取库位清单", Tags = new[] { "Cells" })]
        public async Task<PagedResultDto<CellDto>> GetPagingListAsync(PagingCellListInput input)
        {
            return await _cellAppService.GetPagingListAsync(input); ;
        }
        [HttpPost("getCellsByZ")]
        [SwaggerOperation(summary: "通过排获取库位清单", Tags = new[] { "Cells" })]
        public async Task<ListResultDto<CellDto>> GetCellListByZAsync(PagingCellListInput input)
        {
            return await _cellAppService.GetCellListByZAsync(input);
        }
        [HttpPost("update")]
        [SwaggerOperation(summary: "更新库位", Tags = new[] { "Cells" })]
        public async Task<CellDto> UpdateAsync(UpdateCellDto input)
        {
            return await _cellAppService.UpdateAsync(input);
        }
        [HttpPost("setCellEnable")]
        [SwaggerOperation(summary: "设置库位可用", Tags = new[] { "Cells" })]
        public async Task SetCellEnable(UpdateCellDto input)
        {
             await _cellAppService.SetCellEnable(input);
        }
        [HttpPost("setCellDisable")]
        [SwaggerOperation(summary: "设置库位不可用", Tags = new[] { "Cells" })]
        public async Task SetCellDisable(UpdateCellDto input)
        {
            await _cellAppService.SetCellDisable(input);
        }
    }
}
