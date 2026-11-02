using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientApi.Model
{
    /// <summary>人脸录入请求。</summary>
    public class FaceAddDto
    {
        /// <summary>Base64 编码的人脸照片。</summary>
        public string ImageData { get; set; }

        /// <summary>待录入的业务用户编号。</summary>
        public string UserId { get; set; }

    }
}
