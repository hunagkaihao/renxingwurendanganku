using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Volo.Abp.DependencyInjection;
using Wcs.LogTool;
using System.Collections.Generic;
using Wcs.Backups;
using Wcs.Jobs;
using Wcs.Jobs.JobCmds;
using Wcs.Dispatch;
using Wcs.Jobs.Models;
using Wcs.Orders;
using Wcs.Processes;
using Wcs.Processes.Models;

namespace Wcs.Jobs.JobWorker;

public class DefaultJobWorker : IJobWorker, ITransientDependency
{
    private readonly JobManager _jobManager;
    private readonly BackupManager _backupManager;
    private readonly OrderManager _orderManager;
    private readonly ProcessManager _processManager;
    private readonly ILogger<DefaultJobWorker> _logger;

    private string JobCmdName;

    private DispatchJob myJob;
    public DispatchJob MyJob
    {
        get { return myJob; }
        set
        {
            if (value == null)
            {
                JobCmdName = string.Empty;
                myJob = null;
            }
            else
            {
                JobCmdName = _jobManager.GetJobCmdClassNameAsync(value.JobCmdId).Result;
                myJob = value;
            }
        }
    }

    public IJobCmd MyJobCmd { get; set; }


    public DefaultJobWorker(
        JobManager jobManager,
        BackupManager backupManager,
        OrderManager orderManager,
        ProcessManager processManager,
        ILogger<DefaultJobWorker> logger)
    {
        _jobManager = jobManager;
        _backupManager = backupManager;
        _orderManager = orderManager;
        _processManager = processManager;
        _logger = logger;
        MyJob = null;
        MyJobCmd = null;
    }

    public async Task Execute()
    {
        if (MyJob == null)
        {
            return;
        }

        EnumDispatchJobState state = MyJob.State;

        if (state == EnumDispatchJobState.Created)
        {
            string errInfo = "Job处于Create状态，状态异常";
            await _orderManager.UpdateExecInfoOfDispatchOrderAsync(MyJob.OrderCode, errInfo, true).ConfigureAwait(false);
            await _backupManager.UpdateJobExecInfoOfOrderInRedisAsync(MyJob.OrderCode, MyJob.Id, errInfo).ConfigureAwait(false);
            return;
        }

        if (state == EnumDispatchJobState.WaitingDo)
        {
            bool? ret = await _processManager.IsResourcesOccupiedBySelf(MyJob.TaskId, MyJob.ProcessId, MyJob.ProcessSequence).ConfigureAwait(false);
            if (ret == null)
            {
                string errInfo = "Job是否分配到资源未知";
                await _orderManager.UpdateExecInfoOfDispatchOrderAsync(MyJob.OrderCode, errInfo, true).ConfigureAwait(false);
                await _backupManager.UpdateJobExecInfoOfOrderInRedisAsync(MyJob.OrderCode, MyJob.Id, errInfo).ConfigureAwait(false);
                return;
            }
            else if (ret == false)
            {
                string execInfo = "Job尚未分配到资源";
                await _orderManager.UpdateExecInfoOfDispatchOrderAsync(MyJob.OrderCode, execInfo, false).ConfigureAwait(false);
                await _backupManager.UpdateJobExecInfoOfOrderInRedisAsync(MyJob.OrderCode, MyJob.Id, execInfo).ConfigureAwait(false);
                return;
            }
            else
            {
                bool? r = await _jobManager.UpdateJobStateAsync(MyJob.Id, EnumDispatchJobState.PreJudge).ConfigureAwait(false);
                if (r == null || r == false)
                {
                    string errInfo = "Job已分配到资源，但刷新状态为PreJudge失败";
                    await _orderManager.UpdateExecInfoOfDispatchOrderAsync(MyJob.OrderCode, errInfo, true).ConfigureAwait(false);
                    await _backupManager.UpdateJobExecInfoOfOrderInRedisAsync(MyJob.OrderCode, MyJob.Id, errInfo).ConfigureAwait(false);
                    return;
                }
                else
                {
                    string execInfo = "Job已分配到资源，开始前提条件判断";
                    await _orderManager.UpdateExecInfoOfDispatchOrderAsync(MyJob.OrderCode, execInfo, false).ConfigureAwait(false);
                    await _backupManager.UpdateJobExecInfoOfOrderInRedisAsync(MyJob.OrderCode, MyJob.Id, execInfo).ConfigureAwait(false);

                    MyJob.State = EnumDispatchJobState.PreJudge;
                    state = MyJob.State; //state也更新，使得下一个步骤能够直接执行*****************
                    string log = GenerateLog(execInfo);
                    _logger.Info(log);
                }
            }
        }

        if (state == EnumDispatchJobState.PreJudge) //还未执行=>前提条件判断
        {
            OpResultInDispatchSvc res = await _processManager.IsAllPreconditionSatisfied(MyJob.ProcessId, MyJob.ProcessSequence).ConfigureAwait(false);
            if (res.IsOK == false)
            {
                string errInfo = $"Job执行前提尚未满足，{res.Message}";
                await _orderManager.UpdateExecInfoOfDispatchOrderAsync(MyJob.OrderCode, errInfo, true).ConfigureAwait(false);
                await _backupManager.UpdateJobExecInfoOfOrderInRedisAsync(MyJob.OrderCode, MyJob.Id, errInfo).ConfigureAwait(false);
                return;
            }
            else
            {
                bool? r = await _jobManager.UpdateJobStateAsync(MyJob.Id, EnumDispatchJobState.SendCmd).ConfigureAwait(false);
                if (r == null || r == false)
                {
                    string errInfo = "Job执行前提已满足，但刷新状态为SendCmd失败";
                    await _orderManager.UpdateExecInfoOfDispatchOrderAsync(MyJob.OrderCode, errInfo, true).ConfigureAwait(false);
                    await _backupManager.UpdateJobExecInfoOfOrderInRedisAsync(MyJob.OrderCode, MyJob.Id, errInfo).ConfigureAwait(false);
                    return;
                }
                else
                {
                    string execInfo = "Job执行前提条件已满足，准备发送设备指令";
                    await _orderManager.UpdateExecInfoOfDispatchOrderAsync(MyJob.OrderCode, execInfo, false).ConfigureAwait(false);
                    await _backupManager.UpdateJobExecInfoOfOrderInRedisAsync(MyJob.OrderCode, MyJob.Id, execInfo).ConfigureAwait(false);

                    MyJob.State = EnumDispatchJobState.SendCmd;
                    state = MyJob.State; //state也更新，使得下一个步骤能够直接执行*****************
                    string log = GenerateLog(execInfo);
                    _logger.Info(log);
                }
            }
        }

        if (state == EnumDispatchJobState.SendCmd)
        {
            if (MyJobCmd == null)
            {
                string errInfo = "Job没有指定Command信息";
                await _orderManager.UpdateExecInfoOfDispatchOrderAsync(MyJob.OrderCode, errInfo, true).ConfigureAwait(false);
                await _backupManager.UpdateJobExecInfoOfOrderInRedisAsync(MyJob.OrderCode, MyJob.Id, errInfo).ConfigureAwait(false);
                return;
            }
            OpResultInDispatchSvc ret = MyJobCmd.SendCmdValue();
            if (!ret.IsOK)
            {
                string errInfo = $"Job发送cmd到设备失败，{ret.Message}";
                await _orderManager.UpdateExecInfoOfDispatchOrderAsync(MyJob.OrderCode, errInfo, true).ConfigureAwait(false);
                await _backupManager.UpdateJobExecInfoOfOrderInRedisAsync(MyJob.OrderCode, MyJob.Id, errInfo).ConfigureAwait(false);
                return;
            }
            else
            {
                bool? r = await _jobManager.UpdateJobStateAsync(MyJob.Id, EnumDispatchJobState.WaitingDone).ConfigureAwait(false);
                if (r == null || r == false)
                {
                    string errInfo = "Job已发送cmd到设备，刷新Job状态为WaitingDone失败";
                    await _orderManager.UpdateExecInfoOfDispatchOrderAsync(MyJob.OrderCode, errInfo, true).ConfigureAwait(false);
                    await _backupManager.UpdateJobExecInfoOfOrderInRedisAsync(MyJob.OrderCode, MyJob.Id, errInfo).ConfigureAwait(false);
                    return;
                }
                else
                {
                    string execInfo = "Job已发送cmd到设备，等待设备执行完成";
                    await _orderManager.UpdateExecInfoOfDispatchOrderAsync(MyJob.OrderCode, execInfo, false).ConfigureAwait(false);
                    await _backupManager.UpdateJobExecInfoOfOrderInRedisAsync(MyJob.OrderCode, MyJob.Id, execInfo).ConfigureAwait(false);

                    MyJob.State = EnumDispatchJobState.WaitingDone;
                    state = MyJob.State; //state也更新，使得下一个步骤能够直接执行*****************
                    string log = GenerateLog(execInfo);
                    _logger.Info(log);
                }
            }
        }

        if (state == EnumDispatchJobState.WaitingDone)
        {
            if (MyJobCmd == null)
            {
                string errInfo = "Job没有指定Command信息";
                await _orderManager.UpdateExecInfoOfDispatchOrderAsync(MyJob.OrderCode, errInfo, true).ConfigureAwait(false);
                await _backupManager.UpdateJobExecInfoOfOrderInRedisAsync(MyJob.OrderCode, MyJob.Id, errInfo).ConfigureAwait(false);
                return;
            }
            OpResultInDispatchSvc ret = MyJobCmd.IsCmdFinished();

            if (!ret.IsOK)
            {
                if (ret.Message != null)
                {
                    string errInfo = $"等待设备执行完成：{ret.Message}";
                    await _orderManager.UpdateExecInfoOfDispatchOrderAsync(MyJob.OrderCode, errInfo, true).ConfigureAwait(false);
                    await _backupManager.UpdateJobExecInfoOfOrderInRedisAsync(MyJob.OrderCode, MyJob.Id, errInfo).ConfigureAwait(false);
                }
                else
                {
                    string execInfo = $"等待设备执行完成";
                    await _orderManager.UpdateExecInfoOfDispatchOrderAsync(MyJob.OrderCode, execInfo, false).ConfigureAwait(false);
                    await _backupManager.UpdateJobExecInfoOfOrderInRedisAsync(MyJob.OrderCode, MyJob.Id, execInfo).ConfigureAwait(false);
                }
                return;
            }
            else
            {
                List<DispatchProcessStep> details = await _processManager.GetDispatchProcessStepsAsync(MyJob.ProcessId).ConfigureAwait(false);
                if (details == null)
                {
                    string errInfo = $"设备执行完成，但查询所属过程节点数据失败";
                    await _orderManager.UpdateExecInfoOfDispatchOrderAsync(MyJob.OrderCode, errInfo, true).ConfigureAwait(false);
                    await _backupManager.UpdateJobExecInfoOfOrderInRedisAsync(MyJob.OrderCode, MyJob.Id, errInfo).ConfigureAwait(false);
                    return;
                }

                if (details.Count < MyJob.ProcessSequence)
                {
                    string errInfo = $"设备执行完成，但查询所属过程节点数量为{details.Count}，小于当前节点顺序值{MyJob.ProcessSequence}";
                    await _orderManager.UpdateExecInfoOfDispatchOrderAsync(MyJob.OrderCode, errInfo, true).ConfigureAwait(false);
                    await _backupManager.UpdateJobExecInfoOfOrderInRedisAsync(MyJob.OrderCode, MyJob.Id, errInfo).ConfigureAwait(false);
                    return;
                }

                int nextStep = MyJob.NextTrueStep; //下一个步骤NextTrueStep，若下一个步骤为0，表示没有下一步，此调度任务完成，否则需要跳步
                if (nextStep == 0)
                {
                    bool? res = await _jobManager.UpdateJobsStateAsync(MyJob.TaskId, EnumDispatchJobState.Done).ConfigureAwait(false);
                    if (res == null || res == false)
                    {
                        string errInfo = $"设备执行完成，且没有下一步骤，刷新此任务的所有Job状态为Done失败";
                        await _orderManager.UpdateExecInfoOfDispatchOrderAsync(MyJob.OrderCode, errInfo, true).ConfigureAwait(false);
                        await _backupManager.UpdateJobExecInfoOfOrderInRedisAsync(MyJob.OrderCode, MyJob.Id, errInfo).ConfigureAwait(false);
                        return;
                    }
                    else
                    {
                        string execInfo = $"设备执行完成，且没有下一步骤，刷新此任务的所有Job状态为Done成功";
                        if (!string.IsNullOrEmpty(ret.Message))
                            execInfo = $"{execInfo}({ret.Message})";
                        await _orderManager.UpdateExecInfoOfDispatchOrderAsync(MyJob.OrderCode, string.Empty, false).ConfigureAwait(false);
                        await _backupManager.UpdateJobExecInfoOfOrderInRedisAsync(MyJob.OrderCode, MyJob.Id, string.Empty).ConfigureAwait(false);

                        MyJob.State = EnumDispatchJobState.Done;
                        string log = GenerateLog(execInfo);
                        _logger.Info(log);
                        return;
                    }
                }

                if (nextStep <= MyJob.ProcessSequence || nextStep > details.Count)
                {
                    string errInfo = $"设备执行完成，下一步骤为{nextStep}，不在范围{MyJob.ProcessSequence + 1}~{details.Count}内";
                    await _orderManager.UpdateExecInfoOfDispatchOrderAsync(MyJob.OrderCode, errInfo, true).ConfigureAwait(false);
                    await _backupManager.UpdateJobExecInfoOfOrderInRedisAsync(MyJob.OrderCode, MyJob.Id, errInfo).ConfigureAwait(false);
                    return;
                }

                bool? r = await _jobManager.UpdateJobsStateOfPathStepLessThanAsync(MyJob.TaskId, nextStep, EnumDispatchJobState.Done).ConfigureAwait(false);
                if (r == null || r == false)
                {
                    string errInfo = $"设备执行完成，刷新下一步骤{nextStep}前的所有Job状态为Done失败";
                    await _orderManager.UpdateExecInfoOfDispatchOrderAsync(MyJob.OrderCode, errInfo, true).ConfigureAwait(false);
                    await _backupManager.UpdateJobExecInfoOfOrderInRedisAsync(MyJob.OrderCode, MyJob.Id, errInfo).ConfigureAwait(false);
                    return;
                }
                else
                {
                    string execInfo = $"设备执行完成，刷新下一步骤{nextStep}前的所有Job状态为Done成功";
                    if (!string.IsNullOrEmpty(ret.Message))
                        execInfo = $"{execInfo}({ret.Message})";
                    await _orderManager.UpdateExecInfoOfDispatchOrderAsync(MyJob.OrderCode, string.Empty, false).ConfigureAwait(false);
                    await _backupManager.UpdateJobExecInfoOfOrderInRedisAsync(MyJob.OrderCode, MyJob.Id, string.Empty).ConfigureAwait(false);

                    MyJob.State = EnumDispatchJobState.Done;
                    string log = GenerateLog(execInfo);
                    _logger.Info(log);
                }
            }
        }
    }

    public void ForceDone()
    {
        throw new NotImplementedException();
    }

    public void ForceDoneCurStep()
    {
        throw new NotImplementedException();
    }

    public void RedoCurStep()
    {
        throw new NotImplementedException();
    }

    public string GenerateLog(string logContent)
    {
        return $"调度任务{MyJob?.TaskId}（对应订单{MyJob?.OrderCode}，当前Job：{MyJob?.Id}，执行设备：{MyJob?.NodeCode}，过程ID：{MyJob?.ProcessId}，过程节点：{MyJob?.ProcessSequence}，执行命令：{JobCmdName}），{logContent}";
    }
}