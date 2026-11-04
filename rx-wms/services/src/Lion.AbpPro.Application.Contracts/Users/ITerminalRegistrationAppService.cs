using System;
using System.Threading.Tasks;
using Lion.AbpPro.Users.Dtos;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Identity;

namespace Lion.AbpPro.Users
{
    /// <summary>一体机管理员人员注册入口，提供账号创建、人员选择和特征录入。</summary>
    public interface ITerminalRegistrationAppService : IApplicationService
    {
        /// <summary>验证当前会话具有管理员角色和人员编辑权限。</summary>
        Task<bool> CheckAccessAsync();

        /// <summary>分页查询当前租户已有的 WMS 人员。</summary>
        Task<PagedResultDto<IdentityUserDto>> GetUsersAsync(PagingUserListInput input);

        /// <summary>取得新账号可分配的角色。</summary>
        Task<ListResultDto<IdentityRoleDto>> GetRolesAsync();

        /// <summary>创建一体机人员账号，邮箱选填，账号、密码及角色规则由 Identity 验证。</summary>
        Task<IdentityUserDto> CreateUserAsync(CreateTerminalUserInput input);

        /// <summary>为指定人员录入特征，设备确认成功后启用登录绑定；失败保留停用的编号。</summary>
        Task<UserBiometricBindingDto> EnrollAsync(Guid userId, EnrollUserBiometricInput input);

        /// <summary>取得当前租户指定人员本次指静脉录入的实时操作提示和采集次数。</summary>
        Task<UserBiometricEnrollmentProgressDto> GetEnrollmentProgressAsync(Guid userId, Guid operationId);
    }
}
