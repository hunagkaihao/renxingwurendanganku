using System.Threading.Tasks;
using Ecs.Jobs.JobCmds;
using Ecs.Jobs.Models;

namespace Ecs.Jobs.JobWorker;

/// <summary>
/// 针对每个Job创建的执行器，能够判断执行条件，下发执行命令，反馈执行结果
/// </summary>
public interface IJobWorker
{
    public DispatchJob MyJob { get; set; }

    public IJobCmd MyJobCmd { get; set; }

    /// <summary>
    /// 执行工作
    /// </summary>
    /// <returns></returns>
    public Task Execute();

    /// <summary>
    /// 强制完成
    /// </summary>
    public void ForceDone();

    /// <summary>
    /// 重新执行当前命令
    /// </summary>
    public void RedoCurStep();

    /// <summary>
    /// 强制完成当前命令
    /// </summary>
    public void ForceDoneCurStep();

    /// <summary>
    /// 为日志加上任务相关信息
    /// </summary>
    /// <param name="logContent">记录信息</param>
    public string GenerateLog(string logContent);
}