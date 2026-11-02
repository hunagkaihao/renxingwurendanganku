
using System.Threading.Tasks;
using System.Threading;
using Lion.AbpPro.Users.Dtos;
using Volo.Abp.Application.Services;



namespace Lion.AbpPro.Users
{
    public interface IAccountAppService: IApplicationService
    {
        Task<LoginOutput> LoginAsync(LoginInput input);
        /// <summary>通过真实人脸或指静脉识别结果解析绑定账号并登录。</summary>
        Task<LoginOutput> BiometricLoginAsync(BiometricLoginInput input, CancellationToken cancellationToken = default);
        Task<LoginOutput> FaceLoginAsync(string input);


        Task<LoginOutput> StsLoginAsync(string accessToken);
  
    }
}
