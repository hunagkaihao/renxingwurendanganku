using Volo.Abp.Data;

namespace Wcs;

public static class WcsConsts
{
    public const string DbTablePrefix = "";

    public const string DbSchema = null;

    public const string MonitorChannelName = "Plc.Monitor"; //Plc变量监控通道名称

    public const string StationNotifierChannelName = "Station.Notifier"; //站点通知器通道名称

    public const string StationNotifierTmpChannelName = "Station.NotifierTemp"; //站点通知器暂存通道名称

    public const string MjjCmdChannel = "Mjj.Cmd"; //向密集架服务发送指令的通道名称

    public const string MjjStatusChannel = "Mjj.Status"; //获取密集架服务状态的通道名称

    public const string UnFinishedDispatchOrderCodeChannel = "DispatchSvr.OrderCodeNotDone"; //未完成的订单号

    public const string DispatchOrderChannel = "DispatchSvr.Order"; //订单信息，包括未完成和已完成的

    public const string DispatchChkOdResultChannel = "DispatchSvr.ChkOdResult"; //订单信息，包括未完成和已完成的

    public const string DispatchConditionChannel = "DispatchSvr.Condition"; //存放实时的条件值

    public const string DispatchSvrErrChannel = "DispatchSvr.Error"; //存放调度系统后台Job的错误信息

    public const string DispatchSvrStateChannel = "DispatchSvr.State"; //存放调度系统后台Job的状态信息

    public const string DispatchSvrNotifyChannel = "DispatchSvr.Notifier"; //存放调度系统后台Job的通知器

    public const string DispatchSvrNotifyTempChannel = "DispatchSvr.NotifierTemp"; //存放调度系统后台Job的通知器缓存

    public const string DispatchSvrNotifyWithParaChannel = "DispatchSvr.ParaNotifier"; //存放调度系统后台Job的带参数通知器

    public const string DispatchSvrNotifyTempWithParaChannel = "DispatchSvr.ParaNotifierTemp"; //存放调度系统后台Job的带参数通知器缓存

    public const string DispatchOrderCancelNotifierName = "CancelOrder"; //通知调度系统取消订单通知器名称

    public const string DispatchOrderCancelRespNotifierName = "CancelOrderResp"; //调度系统取消订单操作结果反馈通知器名称

    public const string DispatchOrderForceDoneNotifierName = "ForceDone"; //通知调度系统强制完成订单的通知器名称

    public const string DispatchOrderForceDoneRespNotifierName = "ForceDoneResp"; //调度系统强制完成订单操作结果反馈通知器名称

    public const string PauseDispatcherSvrNotifierName = "PauseDispatchSvr"; //通知调度系统暂停执行的通知器名称

    public const string RunDispatcherSvrNotifierName = "RunDispatchSvr"; //通知调度系统继续执行的通知器名称

    public const string StopCheckOrderNotifierName = "StopCheckNotifier"; //通知调度系统停止盘点的通知器名称

    public const string DispatchTasksDoing = "DispatchSvr.TasksDoing"; //调度系统当前正在执行的调度任务

    public const string NodeType_DoorOpen = "DoorOpen";
    public const string NodeType_LMToZeroPos = "LMToZeroPos";
    public const string NodeType_LMToSafePos = "LMToSafePos";
    public const string NodeType_ReadCell = "LMReadCell";
    public const string NodeType_LMInPick = "LMInPick";
    public const string NodeType_LMInPlace = "LMInPlace";
    public const string NodeType_LMOutPick = "LMOutPick";
    public const string NodeType_LMOutPlace = "LMOutPlace";
    public const string NodeType_LMMovePick = "LMMovePick";
    public const string NodeType_LMMovePlace = "LMMovePlace";
}
