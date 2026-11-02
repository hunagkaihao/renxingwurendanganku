using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientApi.Model
{
    /// <summary>旧版人脸识别兼容请求。</summary>
    public class RecognitionInputDto
    {
        /// <summary>旧版操作码；当前接口未使用。</summary>
        public int RequestValue { get; set; }
        /// <summary>旧版密码字段；当前接口未使用。</summary>
        public string pass { get; set; }
        /// <summary>Base64 编码的图片内容，当前接口用于解码图片。</summary>
        public string image_content { get; set; }
        /// <summary>旧版图片类型字段；当前接口未使用。</summary>
        public string image_type { get; set; }
        /// <summary>旧版图片质量控制字段；当前接口未使用。</summary>
        public string quality_control { get; set; }
    }
}
