using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using WarehouseManagement.Archives.Dto;
using WarehouseManagement.Faces.Dto;

namespace WarehouseManagement.Faces
{
    public interface IFaceAppService : IApplicationService
    {

        //添加人脸信息
        Task<FaceDto> FaceAddAsync(AddFaceDto faces);

        //删除人脸信息
        Task<FaceDto> FaceDeleteAsync(string  userid);

        //获取照片
        Task<GetFaceDto> GetFaceByUserIdAsync(string userid);


        




    }
}
