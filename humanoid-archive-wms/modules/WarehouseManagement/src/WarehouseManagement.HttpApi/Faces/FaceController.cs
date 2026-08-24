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

namespace WarehouseManagement.Face
{
    [Route("Faces")]
    public class FaceController : WarehouseManagementController
    {
        private readonly IFaceAppService _faceAppService;
        public FaceController(IFaceAppService faceAppService)
        {
            _faceAppService = faceAppService;
        }

        [HttpPost("add")]
        [SwaggerOperation(summary: "添加人脸", Tags = new[] { "Faces" })]
        public async Task<FaceDto> CreateAsync([FromBody] AddFaceDto face)
        {
            return await _faceAppService.FaceAddAsync(face);
        }


        [HttpPost("delete")]
        [SwaggerOperation(summary: "删除人脸", Tags = new[] { "Faces" })]
        public async Task<FaceDto> DeleteAsync(string userId)
        {
          return  await _faceAppService.FaceDeleteAsync(userId);
        }

        [HttpGet("getImage")]
        [SwaggerOperation(summary: "获取人脸", Tags = new[] { "Faces" })]
        public async Task<GetFaceDto> GetFaceByUserIdAsync(string userId)
        {
            return await _faceAppService.GetFaceByUserIdAsync(userId);
        }



    }
}
