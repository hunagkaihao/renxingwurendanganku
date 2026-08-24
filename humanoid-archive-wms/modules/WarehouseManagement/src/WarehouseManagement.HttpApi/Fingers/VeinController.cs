using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WarehouseManagement.Archives.Dto;
using Swashbuckle.AspNetCore.Annotations;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using WarehouseManagement.Faces.Dto;
using WarehouseManagement.Faces;
using WarehouseManagement.Fingers;
using WarehouseManagement.Fingers.Dto;

namespace WarehouseManagement.Face
{
    [Route("Veins")]
    public class VeinController : WarehouseManagementController
    {
        private readonly IVeinAppService _veinAppService;
        public VeinController(IVeinAppService veinAppService)
        {
            _veinAppService = veinAppService;

        }

        /*
        [HttpPost("addvein")]
        [SwaggerOperation(summary: "添加指静脉", Tags = new[] { "Vein" })]
        public async Task<VeinDto> CreateAsync(AddVeinDto vein)
        {
            return await _veinAppService.VeinAddAsync(vein);
        }
        */

        [HttpPost("deletevein")]
        [SwaggerOperation(summary: "删除指静脉", Tags = new[] { "Vein" })]
        public async Task<VeinDto> DeleteAsync(string userId)
        {
            return await _veinAppService.VeinDeleteAsync(userId);
        }
        

        [HttpPost("addvein")]
        [SwaggerOperation(summary: "添加指静脉", Tags = new[] { "Vein" })]
        public async Task<VeinDto> CreateAsync([FromBody] AddVeinDto vein)
        {
           return  await _veinAppService.VeinAddAsync(vein);
        }


        //        GetVeinListByUserIdAsync

        [HttpGet("getvein")]
        [SwaggerOperation(summary: "获取指静脉", Tags = new[] { "Vein" })]
        public async Task<List<AddVeinDto>> GetVeinListByUserIdAsync(string userId)
        {
            return await _veinAppService.GetVeinListByUserIdAsync(userId);
        }

    }
}
