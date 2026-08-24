namespace Ecs.Dispatch
{
    public enum EnumDispatchTaskState
    {
        Created, //任务已创建
        ToJobs,  //任务已转换成Jobs
        WaitingDo, //调度系统已受理，并等待执行
        Doing, //正在执行，至少一个过程节点的命令已发送
        Done, //执行完成
        Canceled,  //已取消任务，Doing之前的状态可进行取消
        ForceDone //已强制结束执行
    }
}