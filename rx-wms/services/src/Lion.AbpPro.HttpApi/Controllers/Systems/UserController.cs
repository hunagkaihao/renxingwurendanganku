using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using Lion.AbpPro.Users;
using Lion.AbpPro.Users.Dtos;
using Lion.AbpPro.Extension.Customs.Dtos;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Volo.Abp.Account;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Identity;

namespace Lion.AbpPro.Controllers.Systems
{
    [Route("api/Users")]
    public class UserContoller : AbpProController, IUserAppService
    {
        private readonly IUserAppService _userAppService;

        public UserContoller(IUserAppService userAppService)
        {
            _userAppService = userAppService;
        }

        [HttpPost("page")]
        [SwaggerOperation(summary: "分页获取用户信息", Tags = new[] { "Users" })]
        public Task<PagedResultDto<IdentityUserDto>> ListAsync(PagingUserListInput input)
        {
            return _userAppService.ListAsync(input);
        }

        [HttpPost("create")]
        [SwaggerOperation(summary: "创建用户", Tags = new[] { "Users" })]
        public Task<IdentityUserDto> CreateAsync(IdentityUserCreateDto input)
        {
            return _userAppService.CreateAsync(input);
        }

        [HttpPost("update")]
        [SwaggerOperation(summary: "编辑用户", Tags = new[] { "Users" })]
        public Task<IdentityUserDto> UpdateAsync(UpdateUserInput input)
        {
            return _userAppService.UpdateAsync(input);
        }

        [HttpPost("delete")]
        [SwaggerOperation(summary: "删除用户", Tags = new[] { "Users" })]
        public Task DeleteAsync(IdInput input)
        {
            return _userAppService.DeleteAsync(input);
        }


        [HttpPost("role")]
        [SwaggerOperation(summary: "获取用户角色信息", Tags = new[] { "Users" })]
        public Task<ListResultDto<IdentityRoleDto>> GetRoleByUserId(IdInput input)
        {
            return _userAppService.GetRoleByUserId(input);
        }

        [HttpPost("changePassword")]
        [SwaggerOperation(summary: "修改当前用户密码", Tags = new[] { "Users" })]
        public Task<bool> ChangePasswordAsync(ChangePasswordInput input)
        {
            return _userAppService.ChangePasswordAsync(input);
        }

        [HttpPost("lock")]
        [SwaggerOperation(summary: "锁定用户", Tags = new[] { "Users" })]
        public Task LockAsync(LockUserInput input)
        {
            return _userAppService.LockAsync(input);
        }

        /// <summary>获取当前租户指定用户的全部人脸、指静脉绑定。</summary>
        [HttpGet("{userId:guid}/biometric-bindings")]
        [SwaggerOperation(summary: "查询用户生物识别绑定", Tags = new[] { "Users" })]
        public Task<List<UserBiometricBindingDto>> GetBiometricBindingsAsync(Guid userId)
        {
            return _userAppService.GetBiometricBindingsAsync(userId);
        }

        /// <summary>新增或修改用户的一组编号绑定，需要用户编辑权限；修改时携带并发标记。</summary>
        [HttpPost("{userId:guid}/biometric-bindings")]
        [SwaggerOperation(summary: "保存用户生物识别绑定", Tags = new[] { "Users" })]
        public Task<UserBiometricBindingDto> SaveBiometricBindingAsync(Guid userId, [FromBody] SaveUserBiometricBindingInput input)
        {
            return _userAppService.SaveBiometricBindingAsync(userId, input);
        }

        /// <summary>解除用户的一组绑定，需要用户编辑权限；不删除设备中的样本或模板。</summary>
        [HttpDelete("{userId:guid}/biometric-bindings/{bindingId:guid}")]
        [SwaggerOperation(summary: "解除用户生物识别绑定", Tags = new[] { "Users" })]
        public Task DeleteBiometricBindingAsync(Guid userId, Guid bindingId)
        {
            return _userAppService.DeleteBiometricBindingAsync(userId, bindingId);
        }
    }
}
