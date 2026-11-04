using System.Threading.Tasks;
using System.Threading;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Auditing;
using Lion.AbpPro.Users;
using Lion.AbpPro.Users.Dtos;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Lion.AbpPro.Controllers.Systems
{
    public class AccountController : AbpProController,IAccountAppService
    {
        private readonly IAccountAppService _accountAppService;

        public AccountController(IAccountAppService accountAppService)
        {
            _accountAppService = accountAppService;
        }


        /// <summary>
        /// 使用账号和密码登录，并签发前端调用业务接口所需的访问令牌。
        /// </summary>
        [HttpPost("/api/app/account/login")]
        [SwaggerOperation(summary: "登录", Tags = new[] {"Account"})]
        public Task<LoginOutput> LoginAsync(LoginInput input)
        {
            return _accountAppService.LoginAsync(input);
        }

        /// <summary>一体机人脸或指静脉登录；识别编号通过数据库绑定关联 WMS 用户。</summary>
        [AllowAnonymous]
        [DisableAuditing]
        [RequestSizeLimit(6_000_000)]
        [HttpPost("/api/app/account/biometric-login")]
        [SwaggerOperation(summary: "人脸或指静脉登录", Tags = new[] { "Account" })]
        public Task<LoginOutput> BiometricLoginAsync([FromBody] BiometricLoginInput input, CancellationToken cancellationToken = default)
        {
            return _accountAppService.BiometricLoginAsync(input, cancellationToken);
        }

        [HttpPost("/api/app/account/facelogin")]
        [SwaggerOperation(summary: "用户ID登录", Tags = new[] { "Account" })]
        public Task<LoginOutput> FaceLoginAsync(string input)
        {
            return _accountAppService.FaceLoginAsync(input);
        }

        [SwaggerOperation(summary: "登录", Tags = new[] {"Account"})]
        [HttpPost("/api/app/account/login/Sts")]
        public Task<LoginOutput> StsLoginAsync(string accessToken)
        {
            return _accountAppService.StsLoginAsync(accessToken);
        }
    }
}
