using System.ComponentModel.DataAnnotations;
using System;

namespace Lion.AbpPro.Users.Dtos
{
    /// <summary>管理员为已存在的 WMS 人员采集一种特征，并在成功后启用账号绑定。</summary>
    public class EnrollUserBiometricInput
    {
        /// <summary>本次录入的随机操作标识，用于查询采集进度；旧调用方可省略，重试应使用新标识。</summary>
        public Guid? OperationId { get; set; }

        /// <summary>录入方式：face 为人脸，vein 为指静脉。</summary>
        [Required, RegularExpression("^(face|vein)$")]
        public string Method { get; set; }

        /// <summary>兼容旧客户端的编号字段；人脸和指静脉均由服务端自动分配，此传入值不使用。</summary>
        public int? Identifier { get; set; }

        /// <summary>当次人脸照片的 Base64 内容；指静脉由设备采集，无需传入。</summary>
        [StringLength(6_000_000)]
        public string ImageData { get; set; }
    }
}
