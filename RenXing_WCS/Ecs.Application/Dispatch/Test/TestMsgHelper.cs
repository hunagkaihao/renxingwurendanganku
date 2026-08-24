using System.Collections;
using Volo.Abp.DependencyInjection;

namespace Ecs.Dispatch;

public enum EnumTestMessageCmd
{
    Start,
    Restart,
    Stop
}

public class TestMessage
{
    public EnumTestMessageCmd Command { get; set; }

    public TestMessage()
    {
        Command = EnumTestMessageCmd.Stop;
    }

    public TestMessage(EnumTestMessageCmd cmd)
    {
        Command = cmd;
    }
}

public class TestMsgHelper : ISingletonDependency
{
    private readonly Queue mQueue;
    private readonly object mlocker;

    public TestMsgHelper()
    {
        mQueue = new Queue();
        mlocker = new object();
    }
    
    public bool SendMessage(EnumTestMessageCmd cmd, out string errMsg)
    {
        lock(mlocker)
        {
            errMsg = string.Empty;

            if(mQueue.Count > 0)
            {
                errMsg = "当前还存在指令未处理";
                return false;
            }

            mQueue.Enqueue(new TestMessage(cmd));
            return true;            
        }        
    }

    public TestMessage GetMessage()
    {
        lock(mlocker)
        {
            if(mQueue.Count > 0)
                return (TestMessage)mQueue.ToArray()[0];
            return null;
        }
    }

    public void DequeueMessage()
    {
        lock(mlocker)
        {
            mQueue.Dequeue();
        }
    }
}