using System;
using Ecs.LogTool;
using Microsoft.Extensions.Logging;

namespace Ecs.Dispatch;

public class TestService : EcsAppService, ITestService
{
    private readonly TestMsgHelper _testMsgHelper;
    private readonly ILogger<TestService> _logger;

    public TestService(TestMsgHelper testMsgHelper, ILogger<TestService> logger)
    {
        _testMsgHelper = testMsgHelper;
        _logger = logger;
    }

    public ResponseDto RestartTest()
    {
        try
        {
            if(!_testMsgHelper.SendMessage(EnumTestMessageCmd.Restart, out string errInfo))
                throw new Exception($"发送重新测试命令失败，{errInfo}");
            
            return new ResponseDto(){ success = true, message = "已接收重新测试命令" };
        }
        catch(Exception ex)
        {
            _logger.Error(ex.Message);
            return new ResponseDto(){ success = false, message = ex.Message };
        }
    }

    public ResponseDto StartTest()
    {
        try
        {
            if(!_testMsgHelper.SendMessage(EnumTestMessageCmd.Start, out string errInfo))
                throw new Exception($"发送测试命令失败，{errInfo}");
            
            return new ResponseDto(){ success = true, message = "已接收测试命令" };
        }
        catch(Exception ex)
        {
            _logger.Error(ex.Message);
            return new ResponseDto(){ success = false, message = ex.Message };
        }
    }

    public ResponseDto StopTest()
    {
        try
        {
            if(!_testMsgHelper.SendMessage(EnumTestMessageCmd.Stop, out string errInfo))
                throw new Exception($"发送停止测试命令失败，{errInfo}");
            
            return new ResponseDto(){ success = true, message = "已接收停止测试命令" };
        }
        catch(Exception ex)
        {
            _logger.Error(ex.Message);
            return new ResponseDto(){ success = false, message = ex.Message };
        }
    }
}