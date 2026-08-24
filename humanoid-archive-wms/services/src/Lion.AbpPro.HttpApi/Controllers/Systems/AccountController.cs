using System.Threading.Tasks;
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


        [SwaggerOperation(summary: "登录", Tags = new[] {"Account"})]
        public Task<LoginOutput> LoginAsync(LoginInput input)
        {
            return _accountAppService.LoginAsync(input);
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