using System;
using Volo.Abp.Application.Dtos;

namespace Lion.AbpPro.Users.Dtos
{
    /// <summary>一组用户生物识别绑定，不包含照片、特征模板或密码。</summary>
    public class UserBiometricBindingDto : EntityDto<Guid>
    {
        /// <summary>关联的 WMS 用户 GUID。</summary>
        public Guid UserId { get; set; }
        /// <summary>人脸人员编号，未绑定时为空。</summary>
        public int? FaceId { get; set; }
        /// <summary>指静脉设备模板编号，未绑定时为空。</summary>
        public int? FingerId { get; set; }
        /// <summary>本组编号是否允许用于登录。</summary>
        public bool IsEnabled { get; set; }
        /// <summary>修改时必须原样提交的并发标记，避免覆盖其他管理员的更改。</summary>
        public string ConcurrencyStamp { get; set; }
    }
}
