namespace Ecs.Dispatch
{
    public enum EnumDispatchJobState
    {
        Created = 0,
        WaitingDo,
        PreJudge,
        SendCmd,
        WaitingDone,
        Done,
        ForceDone,
        Canceled
    }
}