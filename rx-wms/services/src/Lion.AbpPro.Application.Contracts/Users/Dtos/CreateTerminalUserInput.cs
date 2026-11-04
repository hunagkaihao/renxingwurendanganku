using System.ComponentModel.DataAnnotations;
using Volo.Abp.Auditing;
using Volo.Abp.Identity;
using Volo.Abp.Validation;

namespace Lion.AbpPro.Users.Dtos
{
    /// <summary>一体机新增人员资料；邮箱选填，账号、姓名、初始密码和角色必须提供。</summary>
    public class CreateTerminalUserInput
    {
        /// <summary>可用于登录的账号，唯一性及允许字符沿用 Identity 规则。</summary>
        [Required, DynamicStringLength(typeof(IdentityUserConsts), nameof(IdentityUserConsts.MaxUserNameLength))]
        public string UserName { get; set; }

        /// <summary>人员姓名，用于下拉选择及录入目标确认。</summary>
        [Required, DynamicStringLength(typeof(IdentityUserConsts), nameof(IdentityUserConsts.MaxNameLength))]
        public string Name { get; set; }

        /// <summary>邮箱的内部存储值；未填写或仅有空格时为空。</summary>
        private string _email;

        /// <summary>选填邮箱；填写时校验格式和长度，空白输入统一视为未填写。</summary>
        [EmailAddress(ErrorMessage = "请输入有效的邮箱地址。")]
        [DynamicStringLength(typeof(IdentityUserConsts), nameof(IdentityUserConsts.MaxEmailLength))]
        public string Email
        {
            get => _email;
            set => _email = string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        }

        /// <summary>初始密码，复杂度沿用当前租户设置，禁止写入审计日志。</summary>
        [DisableAuditing, Required]
        [DynamicStringLength(typeof(IdentityUserConsts), nameof(IdentityUserConsts.MaxPasswordLength))]
        public string Password { get; set; }

        /// <summary>至少分配一个当前租户中存在的角色，角色有效性由 Identity 检查。</summary>
        public string[] RoleNames { get; set; }
    }
}
