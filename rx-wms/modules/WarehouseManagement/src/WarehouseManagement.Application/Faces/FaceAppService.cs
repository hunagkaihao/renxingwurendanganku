using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Serilog;
using Volo.Abp;
using WarehouseManagement.Faces.Aggregates;
using WarehouseManagement.Faces.Dto;
using static System.Net.Mime.MediaTypeNames;

namespace WarehouseManagement.Faces
{
    public class FaceAppService : WarehouseManagementAppService, IFaceAppService
    {
        private readonly IFaceRepository _faceRepository;

        public FaceAppService(IFaceRepository faceRepository)
        {
            _faceRepository = faceRepository;
        }
        //public async Task<FaceDto> FaceAddAsync(AddFaceDto faces)
        //{
        //    try
        //    {
        //        byte[] imageDataBytes = Convert.FromBase64String(faces.ImageDate);

        //        Face face = new Face
        //        {
        //            UserId = faces.UserId,
        //            ImageDate = imageDataBytes
        //        };

        //        await _faceRepository.InsertAsync(face);

        //        return new FaceDto() { Success = true, Error = "" };
        //    }
        //    catch (Exception ex)
        //    {
        //        return new FaceDto() { Success = false, Error = ex.Message };
        //    }


        //}
        //string FileOriginName = faces.UserId.ToString() + ".jpg";
        public async Task<FaceDto> FaceAddAsync(AddFaceDto faces)
        {
            try
            {
                //string FileOriginName = faces.UserId.ToString() + ".jpg";
                //string fileSaveRootDir = "UploadFile";
                //string absoluteFileDir = fileSaveRootDir + "/User/" + "/" + faces.UserId.ToString();

                //文件保存的路径(应用的工作目录+文件夹相对路径);
                //string fileSavePath = Environment.CurrentDirectory + "/wwwroot/" + absoluteFileDir;
                //byte[] imageDataBytes = Convert.FromBase64String(faces.ImageDate);
                //if (!Directory.Exists(fileSavePath))
                //{
                //    Directory.CreateDirectory(fileSavePath);
                //}
                //生成文件的名称
                //string Extension = Path.GetExtension(FileOriginName);//获取文件的源后缀
                //if (string.IsNullOrEmpty(Extension))
                //{
                //   throw new UserFriendlyException("文件上传的原始名称好像不对哦，没有找到文件后缀");
                // }
                //string fileName = Guid.NewGuid().ToString() + Extension;//通过uuid和原始后缀生成新的文件名
                //最终生成的文件的相对路径（xxx/xxx/xx.xx）
                //string finalyFilePath = fileSavePath + "/" + fileName;
                //将指定的字符串（它将二进制数据编码为 Base64 数字）转换为等效的 8 位无符号整数数组
                //MemoryStream stream = new MemoryStream(Convert.FromBase64String(faces.ImageDate));
                //开始保存拷贝文件
                //FileStream targetFileStream = new FileStream(finalyFilePath, FileMode.OpenOrCreate);
                //await stream.CopyToAsync(targetFileStream);
                Face userPhoto = new();
                userPhoto.UserId = faces.UserId;
                //userPhoto.ImageDate = "http://192.168.1.188:21021/" + absoluteFileDir + "/" + fileName;
                userPhoto.ImageDate = faces.ImageDate;
                await _faceRepository.InsertAsync(userPhoto);


                return new FaceDto() { Success = true, Error = "" };
            }
            catch (Exception ex)
            {
                return new FaceDto() { Success = false, Error = ex.Message };
            }


        }
        public async Task<FaceDto> FaceDeleteAsync(string userid)
        {
            try
            {
                Face face1= await _faceRepository.FindByIdAsync(userid);

                if (face1 == null)
                {
                    return new FaceDto() { Success = false, Error = "没查询到该用户的人脸信息" };
                }
                await _faceRepository.DeleteAsync(face1.Id);

                return new FaceDto() { Success = true, Error = "" };
            }
            catch (Exception ex)
            {
                return new FaceDto() { Success = false, Error = ex.Message };
            }
        }

        public async Task<GetFaceDto> GetFaceByUserIdAsync(string userid)
        {
            Face face1 = await _faceRepository.FindByIdAsync(userid);
            if (face1 == null)
            {
                return new GetFaceDto() { Success=false,Error="没查询到该用户的人脸信息",Face=null};
            }
            else
            {
                GetFaceDto getFaceDto = new GetFaceDto();
                getFaceDto.Success = true;
                getFaceDto.Error = "";
                getFaceDto.Face = new face(); 
                getFaceDto.Face.UserId = face1.UserId;
                getFaceDto.Face.ImageDate = face1.ImageDate;
                return getFaceDto;
            }
        }



    }
}
