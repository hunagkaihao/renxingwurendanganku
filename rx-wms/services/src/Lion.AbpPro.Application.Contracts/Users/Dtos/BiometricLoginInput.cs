using System.ComponentModel.DataAnnotations;
using Volo.Abp.Auditing;

namespace Lion.AbpPro.Users.Dtos
{
    /// <summary>一体机生物识别登录请求；识别结果由 WMS 向配置的设备服务获取。</summary>
    public class BiometricLoginInput
    {
        /// <summary>识别方式：face 为人脸，vein 为指静脉。</summary>
        [Required]
        [RegularExpression("^(face|vein)$")]
        public string Method { get; set; }

        /// <summary>人脸照片的 Base64 内容，仅 face 方式必填；不写入审计日志。</summary>
        [StringLength(5_600_000)]
        [DisableAuditing]
        public string ImageData { get; set; }
    }
}
