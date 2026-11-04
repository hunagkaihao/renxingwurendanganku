using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lion.AbpPro.Users.Dtos;
using Lion.AbpPro.Extension.Customs.Dtos;
using Lion.AbpPro.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Volo.Abp;
using Volo.Abp.Account;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Identity;
using Volo.Abp.Users;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.MultiTenancy;

namespace Lion.AbpPro.Users
{
    [Authorize(Policy = IdentityPermissions.Users.Default)]
    public class UserAppService : AbpProAppService, IUserAppService
    {
        private readonly IIdentityUserAppService _identityUserAppService;
        private readonly IdentityUserManager _userManager;
        private readonly IIdentityUserRepository _identityUserRepository;
        /// <summary>数据库中的用户与人脸、指静脉编号关联。</summary>
        private readonly IRepository<UserBiometricBinding, Guid> _biometricBindings;
       
        public UserAppService(
            IIdentityUserAppService identityUserAppService,
            IdentityUserManager userManager,
            IIdentityUserRepository userRepository,
            IRepository<UserBiometricBinding, Guid> biometricBindings
            )
        {
            _identityUserAppService = identityUserAppService;
            _userManager = userManager;
            _identityUserRepository = userRepository;
            _biometricBindings = biometricBindings;
        }

        /// <summary>
        /// 分页查询用户
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PagedResultDto<IdentityUserDto>> ListAsync(PagingUserListInput input)
        {
            var request = new GetIdentityUsersInput
            {
                Filter = input.Filter?.Trim(),
                MaxResultCount = input.PageSize,
                SkipCount = input.SkipCount,
                Sorting = " LastModificationTime desc"
            };
     
            long count = await _identityUserRepository.GetCountAsync(request.Filter)
                .ConfigureAwait(continueOnCapturedContext: false);
            List<Volo.Abp.Identity.IdentityUser> source = await _identityUserRepository
                .GetListAsync(request.Sorting, request.MaxResultCount, request.SkipCount, request.Filter)
                .ConfigureAwait(continueOnCapturedContext: false);

            return new PagedResultDto<IdentityUserDto>(count,
                base.ObjectMapper.Map<List<Volo.Abp.Identity.IdentityUser>, List<IdentityUserDto>>(source));
        }

        /// <summary>
        /// 新增用户
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [Authorize(IdentityPermissions.Users.Create)]
        public async Task<IdentityUserDto> CreateAsync(IdentityUserCreateDto input)
        {
            // abp 5.0 之后新增字段,是否运行用户登录，默认设置为true
            input.IsActive = true;
            return await _identityUserAppService.CreateAsync(input);
        }

        /// <summary>
        /// 更新用户
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [Authorize(IdentityPermissions.Users.Update)]
        public virtual async Task<IdentityUserDto> UpdateAsync(UpdateUserInput input)
        {
            input.UserInfo.IsActive = true;
            return await _identityUserAppService.UpdateAsync(input.UserId, input.UserInfo);
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        [Authorize(IdentityPermissions.Users.Delete)]
        public virtual async Task DeleteAsync(IdInput input)
        {
            await _identityUserAppService.DeleteAsync(input.Id);
        }

        /// <summary>
        /// 获取用户角色信息
        /// </summary>
        /// <returns></returns>
        public async Task<ListResultDto<IdentityRoleDto>> GetRoleByUserId(IdInput input)
        {
            return await _identityUserAppService.GetRolesAsync(input.Id);
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<bool> ChangePasswordAsync(ChangePasswordInput input)
        {
            var identityUser = await _userManager.GetByIdAsync(base.CurrentUser.GetId());
            IdentityResult result;
            if (identityUser.PasswordHash == null)
            {
                result = await _userManager.AddPasswordAsync(identityUser, input.NewPassword);
            }
            else
            {
                result = await _userManager.ChangePasswordAsync(identityUser, input.CurrentPassword, input.NewPassword);
            }

            return !result.Succeeded
                ? throw new UserFriendlyException(result?.Errors?.FirstOrDefault()?.Description)
                : result.Succeeded;
        }

        /// <summary>
        /// 锁定用户
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [Authorize(AbpProPermissions.SystemManagement.UserEnable)]
        public async Task LockAsync(LockUserInput input)
        {
            var identityUser = await _userManager.GetByIdAsync(input.UserId);
            identityUser.SetIsActive(input.Locked);
            await _userManager.UpdateAsync(identityUser);
        }

        /// <summary>查询指定用户当前租户内的所有绑定，包括已停用记录。</summary>
        public async Task<List<UserBiometricBindingDto>> GetBiometricBindingsAsync(Guid userId)
        {
            await EnsureBiometricUserAsync(userId);
            var bindings = await _biometricBindings.GetListAsync(binding => binding.UserId == userId
                && binding.TenantId == CurrentTenant.Id);
            return bindings.OrderBy(binding => binding.CreationTime).Select(ToBiometricBindingDto).ToList();
        }

        /// <summary>维护用户绑定；复用用户编辑权限，检查正整数、并发标记和编号唯一性。</summary>
        /// <param name="userId">当前租户中已存在的 WMS 用户 GUID。</param>
        /// <param name="input">新增时 Id 为空；修改时必须携带查询得到的并发标记。</param>
        [Authorize(IdentityPermissions.Users.Update)]
        public async Task<UserBiometricBindingDto> SaveBiometricBindingAsync(Guid userId, SaveUserBiometricBindingInput input)
        {
            await EnsureBiometricUserAsync(userId);
            UserBiometricBinding binding;
            if (input.Id.HasValue)
            {
                binding = await _biometricBindings.GetAsync(input.Id.Value);
                if (binding.UserId != userId || binding.TenantId != CurrentTenant.Id)
                    throw new UserFriendlyException("绑定记录不属于当前用户。");
                if (string.IsNullOrWhiteSpace(input.ConcurrencyStamp) || binding.ConcurrencyStamp != input.ConcurrencyStamp)
                    throw new UserFriendlyException("绑定已被修改，请刷新后重试。");
                binding.UpdateIdentifiers(input.FaceId, input.FingerId, input.IsEnabled);
            }
            else
            {
                binding = new UserBiometricBinding(GuidGenerator.Create(), userId, CurrentTenant.Id,
                    input.FaceId, input.FingerId, input.IsEnabled);
            }

            // 身份库编号全局唯一；仅检查是否冲突，不向调用方暴露其他租户或人员信息。
            using (DataFilter.Disable<IMultiTenant>())
            {
                if (await _biometricBindings.AnyAsync(other => other.Id != binding.Id
                    && ((input.FaceId.HasValue && other.FaceId == input.FaceId)
                        || (input.FingerId.HasValue && other.FingerId == input.FingerId))))
                    throw new UserFriendlyException("人脸编号或指静脉编号已被绑定，请先解除原绑定。");
            }

            if (input.Id.HasValue) await _biometricBindings.UpdateAsync(binding, autoSave: true);
            else await _biometricBindings.InsertAsync(binding, autoSave: true);
            return ToBiometricBindingDto(binding);
        }

        /// <summary>解除指定用户的一组绑定；设备样本保留，可重新建立正确的用户关联。</summary>
        [Authorize(IdentityPermissions.Users.Update)]
        public async Task DeleteBiometricBindingAsync(Guid userId, Guid bindingId)
        {
            await EnsureBiometricUserAsync(userId);
            var binding = await _biometricBindings.GetAsync(bindingId);
            if (binding.UserId != userId || binding.TenantId != CurrentTenant.Id)
                throw new UserFriendlyException("绑定记录不属于当前用户。");
            await _biometricBindings.DeleteAsync(binding, autoSave: true);
        }

        /// <summary>仅允许为当前租户的有效用户记录维护绑定，禁止关联已删除或跨租户账号。</summary>
        private async Task EnsureBiometricUserAsync(Guid userId)
        {
            var user = userId == Guid.Empty ? null : await _userManager.FindByIdAsync(userId.ToString());
            if (user == null || user.IsDeleted || user.TenantId != CurrentTenant.Id)
                throw new UserFriendlyException("指定的 WMS 用户不存在或不属于当前租户。");
        }

        /// <summary>将绑定转换为管理接口响应，不输出照片、模板或用户密码。</summary>
        private static UserBiometricBindingDto ToBiometricBindingDto(UserBiometricBinding binding)
        {
            return new UserBiometricBindingDto
            {
                Id = binding.Id, UserId = binding.UserId,
                FaceId = binding.FaceId, FingerId = binding.FingerId,
                IsEnabled = binding.IsEnabled, ConcurrencyStamp = binding.ConcurrencyStamp
            };
        }
    }
}
