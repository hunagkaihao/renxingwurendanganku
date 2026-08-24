using Volo.Abp.Application.Services;

namespace Ecs.Dispatch;

public interface ITestService : IApplicationService
{
    public ResponseDto StartTest();
    public ResponseDto StopTest();
    public ResponseDto RestartTest();
}