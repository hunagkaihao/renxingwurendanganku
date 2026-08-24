using System.Collections;
using Volo.Abp.DependencyInjection;

namespace Ecs.Jobs.JobCmds;

public class CheckMessage
{
    public ushort PlcRow { get; set; }
    public ushort PlcLayer { get; set; }
    public string OrderCode { get; set; }

    public CheckMessage(ushort plcRow, ushort plcLayer, string orderCode)
    {
        PlcRow = plcRow;
        PlcLayer = plcLayer;
        OrderCode = orderCode;
    }
}

public class CheckMsgQHelper : ISingletonDependency
{
    private readonly Queue mQueue;
    private readonly object mlocker;

    public CheckMsgQHelper()
    {
        mQueue = new Queue();
        mlocker = new object();
    }

    public bool SendMessage(string orderCode, ushort plcRow, ushort plcLayer, out string errMsg)
    {
        lock (mlocker)
        {
            errMsg = string.Empty;

            if (string.IsNullOrEmpty(orderCode))
            {
                errMsg = "盘点消息中的orderCode不能为空";
                return false;
            }

            if (mQueue.Count > 0)
            {
                errMsg = "当前还存在盘点消息未处理完";
                return false;
            }

            mQueue.Enqueue(new CheckMessage(plcRow, plcLayer, orderCode));
            return true;
        }
    }

    public CheckMessage GetMessage()
    {
        lock (mlocker)
        {
            if (mQueue.Count > 0)
                return (CheckMessage)mQueue.ToArray()[0];
            return null;
        }
    }

    public void DequeueMessage()
    {
        lock (mlocker)
        {
            if (mQueue.Count > 0)
                mQueue.Dequeue();
        }
    }
}