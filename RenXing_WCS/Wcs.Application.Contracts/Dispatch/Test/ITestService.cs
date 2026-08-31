using Volo.Abp.Application.Services;

namespace Wcs.Dispatch;

public interface ITestService : IApplicationService
{
    public ResponseDto StartTest();
    public ResponseDto StopTest();
    public ResponseDto RestartTest();
}