using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Lion.AbpPro.ConfigurationOptions;
using Lion.AbpPro.Users.Dtos;
using IdentityModel;
using Lion.AbpPro.Extension.Customs.Dtos;
using Lion.AbpPro.Extension.Customs.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Volo.Abp;
using Volo.Abp.Identity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Security.Claims;

namespace Lion.AbpPro.Users
{
    public class AccountAppService : AbpProAppService, IAccountAppService
    {
        private readonly IdentityUserManager _userManager;
        private readonly JwtOptions _jwtOptions;

        private readonly Microsoft.AspNetCore.Identity.SignInManager<Volo.Abp.Identity.IdentityUser>
            _signInManager;

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ICurrentTenant _currentTenant;
        private readonly IHttpContextAccessor _contextAccessor;
        /// <summary>识别设备身份并解析数据库中的用户绑定。</summary>
        private readonly BiometricLoginService _biometricLoginService;


        public AccountAppService(
            IdentityUserManager userManager,
            IOptionsSnapshot<JwtOptions> jwtOptions,
            Microsoft.AspNetCore.Identity.SignInManager<IdentityUser> signInManager,
            IHttpClientFactory httpClientFactory, ICurrentTenant currentTenant,
            IHttpContextAccessor contextAccessor,
            BiometricLoginService biometricLoginService)
        {
            _userManager = userManager;
            _jwtOptions = jwtOptions.Value;
            _signInManager = signInManager;
            _httpClientFactory = httpClientFactory;
            _currentTenant = currentTenant;
            _contextAccessor = contextAccessor;
            _biometricLoginService = biometricLoginService;
        }


        public async Task<LoginOutput> LoginAsync(LoginInput input)
        {
            var result = await _signInManager.PasswordSignInAsync(input.Name, input.Password, false, true);
            if (result.IsNotAllowed)
            {
                throw new UserFriendlyException("当前用户已锁定");
            }

            if (!result.Succeeded)
            {
                throw new UserFriendlyException("用户名或者密码错误");
            }
            
            var user = await _userManager.FindByNameAsync(input.Name);
            return await BuildResult(user);
        }
        /// <summary>通过服务端真实识别和绑定账号登录，沿用账号状态、角色及 JWT 签发规则。</summary>
        public async Task<LoginOutput> BiometricLoginAsync(BiometricLoginInput input, CancellationToken cancellationToken = default)
        {
            var userId = await _biometricLoginService.RecognizeUserIdAsync(input, cancellationToken);
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user != null && user.TenantId != _currentTenant.Id)
                throw new UserFriendlyException("绑定账号与当前租户不一致，请联系管理员。");
            cancellationToken.ThrowIfCancellationRequested();
            return await BuildResult(user);
        }

        //人脸登录
        public async Task<LoginOutput> FaceLoginAsync(string input)
        {
            var user = await _userManager.FindByIdAsync(input);
            return await BuildResult(user);
        }


        public async Task<LoginOutput> StsLoginAsync(string accessToken)
        {
            // 通过access token 获取用户信息
            Dictionary<string, string> headers = new Dictionary<string, string>
                { { "Authorization", $"Bearer {accessToken}" } };
            var response =
                await _httpClientFactory.GetAsync<LoginStsOutput>(HttpClientNameConsts.Sts,
                    "connect/userinfo", headers);
            var user = await _userManager.FindByNameAsync(response.name);
            if (!user.IsActive)
            {
                throw new UserFriendlyException("当前用户已锁定");
            }
            return await BuildResult(user);
        }


        private async Task<LoginOutput> BuildResult(IdentityUser user)
        {
            if (user == null) throw new UserFriendlyException("绑定的 WMS 账号不存在，请联系管理员。");
            // LockoutEnabled 只表示允许锁定，实际锁定状态由 Identity 的结束时间判断。
            if (!user.IsActive || !await _signInManager.CanSignInAsync(user)
                || await _userManager.IsLockedOutAsync(user))
                throw new UserFriendlyException("当前用户已停用、锁定或不允许登录。");
            var roles = await _userManager.GetRolesAsync(user);
            if (roles == null || roles.Count == 0) throw new UserFriendlyException("当前用户未分配角色");
            var token = GenerateJwt(user.Id, user.UserName, user.Name, user.Email,
                user.TenantId.ToString(), roles.ToList());
            var loginOutput = ObjectMapper.Map<IdentityUser, LoginOutput>(user);
            loginOutput.Token = token;
            loginOutput.Roles = roles.ToList();
            return loginOutput;
        }

        /// <summary>
        /// 生成jwt token
        /// </summary>
        /// <returns></returns>
        private string GenerateJwt(Guid userId, string userName, string name, string email,
            string tenantId, List<string> roles)
        {
            var dateNow = DateTime.Now;
            var expirationTime = dateNow + TimeSpan.FromHours(_jwtOptions.ExpirationTime);
            var key = Encoding.ASCII.GetBytes(_jwtOptions.SecurityKey);

            var claims = new List<Claim>
            {
                new Claim(JwtClaimTypes.Audience, _jwtOptions.Audience),
                new Claim(JwtClaimTypes.Issuer, _jwtOptions.Issuer),
                new Claim(AbpClaimTypes.UserId, userId.ToString()),
                new Claim(AbpClaimTypes.Name, name ?? string.Empty),
                new Claim(AbpClaimTypes.UserName, userName),
                new Claim(AbpClaimTypes.Email, email ?? string.Empty),
                new Claim(AbpClaimTypes.TenantId, tenantId)
            };

            foreach (var item in roles)
            {
                claims.Add(new Claim(JwtClaimTypes.Role, item));
            }

            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expirationTime,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            var handler = new JwtSecurityTokenHandler();
            var token = handler.CreateToken(tokenDescriptor);
            return handler.WriteToken(token);
        }
    }
}
