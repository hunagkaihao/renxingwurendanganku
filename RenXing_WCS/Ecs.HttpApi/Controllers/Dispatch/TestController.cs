using Ecs.Controllers;
using Ecs.WMS;

using Microsoft.AspNetCore.Mvc;

namespace Ecs.Dispatch;

[Route("ecs/test")]
[ApiController]
public class TestController : EcsController, ITestService
{
    private readonly ITestService _testService;
    private readonly IWMSService _wMSService;

    public TestController(ITestService testService,IWMSService wMSService)
    {
        _testService = testService;
        _wMSService = wMSService;
    }

    [HttpPost("restart")]
    public ResponseDto RestartTest()
    {
        return _testService.RestartTest();
    }

    [HttpPost("start")]
    public ResponseDto StartTest()
    {
        return _testService.StartTest();
    }

    [HttpPost("stop")]
    public ResponseDto StopTest()
    {
        return _testService.StopTest();
    }

    [HttpPost("test11")]
    public async void Test()
    {
        TaskStatusDto taskStatusDto = new TaskStatusDto();  
        await _wMSService.SendTaskStatus(taskStatusDto);
    }
}