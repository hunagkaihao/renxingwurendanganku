using Wcs.Dispatch;
using Wcs.Jobs.JobWorker;

namespace Wcs.Jobs.JobCmds;

/// <summary>
/// 每个工作包含的命令，具备生成命令值，下发命令的功能
/// </summary>
public interface IJobCmd
{
    /// <summary>
    /// 命令命令中文名
    /// </summary>
    /// <value></value>
    public string JobCmdNameCHS { get; set; }

    /// <summary>
    /// 此字段只用于判断命令，判断成立，返回true，不成立，返回false
    /// </summary>
    /// <value></value>
    public bool JudgeResult { get; set; }

    /// <summary>
    /// 该命令命令所属JobWorker，命令与Job一一对应
    /// </summary>
    /// <value></value>
    public IJobWorker Owner { get; set; }

    /// <summary>
    /// 生成设备指令
    /// </summary>
    /// <returns>OpResultInDispatchSvc.IsOK：是否生成成功，OpResultInDispatchSvc.Message：反馈错误信息</returns>
    public OpResultInDispatchSvc GenerateCmdValue();

    /// <summary>
    /// 向设备发送指令
    /// </summary>
    /// <returns>OpResultInDispatchSvc.IsOK：命令是否发送成功，OpResultInDispatchSvc.Message：错误信息</returns>
    public OpResultInDispatchSvc SendCmdValue();

    /// <summary>
    /// 判断设备指令是否执行完成
    /// </summary>
    /// <returns>OpResultInDispatchSvc.IsOK：命令是否执行完成，OpResultInDispatchSvc.Message：错误信息</returns>
    public OpResultInDispatchSvc IsCmdFinished();
}