using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientApi.Model
{
    /// <summary>人脸识别请求。</summary>
    public class FaceMatchDto
    {
        /// <summary>Base64 编码的待识别人脸照片。</summary>
        public string ImageData { get; set; }

    }
}
