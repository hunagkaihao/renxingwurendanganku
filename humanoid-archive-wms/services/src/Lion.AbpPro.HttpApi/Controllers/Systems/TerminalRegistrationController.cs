using System;
using System.Threading.Tasks;
using Lion.AbpPro.Users;
using Lion.AbpPro.Users.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Identity;

namespace Lion.AbpPro.Controllers.Systems
{
    /// <summary>一体机人员注册接口，仅允许管理员使用，由 WMS 调用设备服务完成录入。</summary>
    [Route("api/terminal-registration")]
    // 入口要求登录；管理员角色由应用服务统一通过 ABP CurrentUser 校验，避免两套角色声明解析不一致。
    [Authorize]
    public class TerminalRegistrationController : AbpProController
    {
        /// <summary>执行具有管理员权限检查的注册应用服务。</summary>
        private readonly ITerminalRegistrationAppService _registration;

        /// <summary>注入人员注册应用服务。</summary>
        public TerminalRegistrationController(ITerminalRegistrationAppService registration) { _registration = registration; }

        /// <summary>检查是否允许进入人员注册页面。</summary>
        [HttpGet("access")]
        public Task<bool> CheckAccessAsync() => _registration.CheckAccessAsync();

        /// <summary>分页查询已有人员。</summary>
        [HttpPost("users/search")]
        public Task<PagedResultDto<IdentityUserDto>> GetUsersAsync([FromBody] PagingUserListInput input)
            => _registration.GetUsersAsync(input);

        /// <summary>查询新建账号可分配的角色。</summary>
        [HttpGet("roles")]
        public Task<ListResultDto<IdentityRoleDto>> GetRolesAsync() => _registration.GetRolesAsync();

        /// <summary>创建新的 WMS 人员账号，复用现有密码、权限和唯一性校验。</summary>
        [HttpPost("users")]
        public Task<IdentityUserDto> CreateUserAsync([FromBody] IdentityUserCreateDto input)
            => _registration.CreateUserAsync(input);

        /// <summary>录入人脸或连续采集六次指静脉，成功后启用与所选账号的绑定。</summary>
        [HttpPost("users/{userId:guid}/enroll")]
        [RequestSizeLimit(7_000_000)]
        public Task<UserBiometricBindingDto> EnrollAsync(Guid userId, [FromBody] EnrollUserBiometricInput input)
            => _registration.EnrollAsync(userId, input);

        /// <summary>查询所选人员本次指静脉采集进度，保留与录入一致的管理员权限检查。</summary>
        [HttpGet("users/{userId:guid}/enroll-progress/{operationId:guid}")]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public Task<UserBiometricEnrollmentProgressDto> GetEnrollmentProgressAsync(Guid userId, Guid operationId)
            => _registration.GetEnrollmentProgressAsync(userId, operationId);
    }
}
