using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing.Imaging;
using Newtonsoft.Json;
using System.Web;
using Microsoft.AspNetCore.Hosting;
using ClientApi.Model;
using Newtonsoft.Json.Linq;

namespace ClientApi.Controllers
{
    /// <summary>旧版用户管理兼容接口；当前未操作实际人脸身份库。</summary>
    [ApiController]
    [Route("[controller]")]
    //[Route("api/student")]
    public class UserManageController : ControllerBase
    {

        private readonly ILogger<UserManageController> _logger;
        public UserManageController(ILogger<UserManageController> logger)
        {
            _logger = logger;
            //_faceDictionary = ReadJson();
        }


        /// <summary>
        /// 按旧版操作码返回用户管理兼容响应。
        /// </summary>
        /// <param name="faceData">旧版请求；RequestValue 为 2817、2819、2824 分别表示新增、删除、查询列表。</param>
        /// <returns>JSON 文本响应；新增返回失败，删除返回兼容成功响应，查询返回空字符串。</returns>
        /// <remarks>实际人脸录入和删除请使用 /Face/FaceAdd 和 /Face/FaceDelete。</remarks>
        [HttpPost]
        public IActionResult Post([FromBody] UserManagerInputDto faceData)
        {
            //var fa= JsonConvert.SerializeObject(faceEncodingA);
            //var ss=  JsonConvert.SerializeObject(_faceDictionary,Formatting.Indented);
            try
            {
                //增加用户
                if (faceData.RequestValue == 2817)
                {
                    byte[] imageBytes = Convert.FromBase64String(faceData.image_content);

                        return Content(JsonConvert.SerializeObject(new AddUserOutputDto(1, "0", "Bad ImageData")));
                    
                }
                //删除用户
                else if (faceData.RequestValue == 2819)
                {

                        return Content(JsonConvert.SerializeObject(new DeleteUserOutputDto(0, faceData.user_id, "delete user successfully")));
                }
                //获取用户列表
                else if (faceData.RequestValue == 2824)
                {
                        return Content(JsonConvert.SerializeObject(""));
                }
                else
                {
                    return Content(JsonConvert.SerializeObject(new DeleteUserOutputDto(1, "0", "RequestValue 错误")));
                }
            }
            catch (Exception e)
            {
                return Content(JsonConvert.SerializeObject(new DeleteUserOutputDto(1, "0", "Exception"+e.Message)));
            }
        }


        private static Bitmap ToFormat24bpprgb(byte[] bs)
        {
            MemoryStream ms = new MemoryStream(bs);
            System.Drawing.Image imgr = System.Drawing.Image.FromStream(ms);
            Bitmap bmp = new Bitmap(imgr.Width, imgr.Height, PixelFormat.Format24bppRgb);
            Graphics g = Graphics.FromImage(bmp);
            g.DrawImage(imgr, new System.Drawing.Point(0, 0));
            g.Dispose();
            imgr.Dispose();
            ms.Dispose();
            return bmp;
        }



    }

}
