
using System;
using Ecs.Dispatch;
using Ecs.Jobs;
using Ecs.Jobs.JobWorker;
using Ecs.Jobs.Models;
using Ecs.Nodes;
using Ecs.Nodes.Models;
using Volo.Abp.DependencyInjection;

namespace Ecs.Jobs.JobCmds
{
    public class NullCmd : IJobCmd, ITransientDependency
    {
        public bool JudgeResult { get; set; } = true;
        public IJobWorker Owner { get; set; }
        public string JobCmdNameCHS { get; set; } = string.Empty;

        private readonly JobManager _jobManager;
        private readonly NodeManager _nodeManager;

        // private byte[] mCmdValue;

        public NullCmd(
            JobManager jobManager,
            NodeManager nodeManager)
        {
            _jobManager = jobManager;
            _nodeManager = nodeManager;
            // mCmdValue = null;
        }

        public OpResultInDispatchSvc GenerateCmdValue()
        {
            //获取对应的Job，并读取job的Id
            if (Owner == null)
                return new OpResultInDispatchSvc() { IsOK = false, Message = $"NullCmd命令没有指定所属的JobWorker信息" };

            if (Owner.MyJob == null)
                return new OpResultInDispatchSvc() { IsOK = false, Message = "NullCmd命令所属Job为空" };

            //获取对应的Cmd定义，并获取命令值
            DispatchJobCmd jobCmd = _jobManager.GetJobCmdAsync(Owner.MyJob.JobCmdId).Result;
            if (jobCmd == null)
                return new OpResultInDispatchSvc() { IsOK = false, Message = $"根据NullCmd命令所属Job指定的JobCmdId({Owner.MyJob.JobCmdId})无法查询到JobCmd信息" };

            if (jobCmd.JobCmdClassName != GetType().Name)
                return new OpResultInDispatchSvc() { IsOK = false, Message = $"根据NullCmd命令所属Job指定的JobCmdId({Owner.MyJob.JobCmdId})查询到的JobCmd类名称为{jobCmd.JobCmdClassName}，而非{GetType().Name}" };

            return new OpResultInDispatchSvc() { IsOK = true, Message = null };
        }

        public OpResultInDispatchSvc SendCmdValue()
        {
            try
            {
                OpResultInDispatchSvc r = GenerateCmdValue();
                if (!r.IsOK)
                    return r;

                if (Owner == null)
                    return new OpResultInDispatchSvc() { IsOK = false, Message = "NullCmd命令没有指定所属的JobWorker信息" };

                if (Owner.MyJob == null)
                    return new OpResultInDispatchSvc() { IsOK = false, Message = "NullCmd命令所属Job为空" };

                DispatchJob job = Owner.MyJob;
                DispatchNode node = _nodeManager.GetNodeByNodeCodeAsync(job.NodeCode).Result;
                if (node == null)
                    return new OpResultInDispatchSvc() { IsOK = false, Message = $"NullCmd所对应的执行设备{job.NodeCode}不存在" };

                return new OpResultInDispatchSvc() { IsOK = true, Message = null };
            }
            catch (Exception ex)
            {
                return new OpResultInDispatchSvc() { IsOK = false, Message = ex.Message };
            }
        }

        public OpResultInDispatchSvc IsCmdFinished()
        {
            try
            {
                if (Owner == null)
                    return new OpResultInDispatchSvc() { IsOK = false, Message = "NullCmd命令没有指定所属的JobWorker信息" };

                if (Owner.MyJob == null)
                    return new OpResultInDispatchSvc() { IsOK = false, Message = "NullCmd命令所属Job为空" };

                DispatchJobCmd jobCmd = _jobManager.GetJobCmdAsync(Owner.MyJob.JobCmdId).Result;

                if (jobCmd == null)
                    return new OpResultInDispatchSvc() { IsOK = false, Message = $"根据NullCmd命令所属Job指定的JobCmdId({Owner.MyJob.JobCmdId})无法查询到JobCmd信息" };

                if (jobCmd.JobCmdClassName != GetType().Name)
                    return new OpResultInDispatchSvc() { IsOK = false, Message = $"根据NullCmd命令所属Job指定的JobCmdId({Owner.MyJob.JobCmdId})查询到的JobCmd类名称为{jobCmd.JobCmdClassName}，而非{GetType().Name}" };

                DispatchJob job = Owner.MyJob;

                DispatchNode node = _nodeManager.GetNodeByNodeCodeAsync(job.NodeCode).Result;
                if (node == null)
                    return new OpResultInDispatchSvc() { IsOK = false, Message = $"NullCmd所对应的执行设备{job.NodeCode}不存在" };

                return new OpResultInDispatchSvc() { IsOK = true, Message = null };

            }
            catch (Exception ex)
            {
                return new OpResultInDispatchSvc() { IsOK = false, Message = ex.Message };
            }
        }
    }
}