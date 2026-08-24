using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Volo.Abp.Application.Dtos;
using WarehouseManagement.Cells;
using WarehouseManagement.RfidCodes.Dto;

namespace WarehouseManagement.RfidCodes
{
    [Route("Rfid")]
    public class RfidCodesController : WarehouseManagementController,IRfidCodeAppService
    {
        private readonly IRfidCodeAppService _rfidCodeAppService;
        public RfidCodesController(IRfidCodeAppService rfidCodeAppService)
        {
            _rfidCodeAppService = rfidCodeAppService;
        }
        [HttpPost("create")]
        [SwaggerOperation(summary: "创建标签", Tags = new[] { "Rfid" })]
        public async Task<RfidCodeDto> CreateAsync(CreateRfidCodeDto input)
        {
            return await _rfidCodeAppService.CreateAsync(input);
        }
        [HttpPost("createMany")]
        [SwaggerOperation(summary: "批量创建标签", Tags = new[] { "Rfid" })]
        public async Task CreateManyAsync(List<CreateRfidCodeDto> input)
        {
             await _rfidCodeAppService.CreateManyAsync(input);
        }
        [HttpPost("delete")]
        [SwaggerOperation(summary: "删除标签", Tags = new[] { "Rfid" })]
        public async Task DeleteAsync(CreateRfidCodeDto input)
        {
             await _rfidCodeAppService.DeleteAsync(input);
        }
        [HttpPost("page")]
        [SwaggerOperation(summary: "查找标签", Tags = new[] { "Rfid" })]
        public async Task<PagedResultDto<RfidCodeDto>> PageAsync(PagingRfidListInput input)
        {
            return await _rfidCodeAppService.PageAsync(input);
        }
    }
}
