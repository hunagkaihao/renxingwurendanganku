using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Wcs.Dispatch;

public interface ICoreService : IApplicationService
{
    public ResponseDto PauseDispatcherSvr();

    public ResponseDto RestartDispatcherSvr();

    public Task<string> GetDispatchSvrStateAsync();

}