using System;
using Ecs.Conditions;
using Ecs.Dispatch;
using Ecs.Jobs.JobWorker;
using Volo.Abp.DependencyInjection;

namespace Ecs.Jobs.JobCmds
{
    public class LMAtSafePosJudgeCmd : IJobCmd, ITransientDependency
    {
        public bool JudgeResult { get; set; } = true;
        public IJobWorker Owner { get; set; }
        public string JobCmdNameCHS { get; set; } = string.Empty;

        private readonly ConditionManager _conditionManager;

        public LMAtSafePosJudgeCmd(ConditionManager conditionManager)
        {
            _conditionManager = conditionManager;
        }

        public OpResultInDispatchSvc GenerateCmdValue()
        {
            //获取对应的Job，并读取job的Id
            if (Owner == null)
                return new OpResultInDispatchSvc() { IsOK = false, Message = $"\"龙门避让位判断\"命令没有指定所属的JobWorker" };

            if (Owner.MyJob == null)
                return new OpResultInDispatchSvc() { IsOK = false, Message = $"\"龙门避让位判断\"命令所属Job为空" };

            return new OpResultInDispatchSvc() { IsOK = true, Message = null };
        }

        public OpResultInDispatchSvc SendCmdValue()
        {
            try
            {
                OpResultInDispatchSvc r = GenerateCmdValue();
                if (!r.IsOK)
                    return r;

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
                    return new OpResultInDispatchSvc() { IsOK = false, Message = "\"龙门避让位判断\"命令没有指定所属的JobWorker信息" };

                if (Owner.MyJob == null)
                    return new OpResultInDispatchSvc() { IsOK = false, Message = "\"龙门避让位判断\"命令所属Job为空" };

                string value = _conditionManager.GetConditionValueAsync("Plc1.Lm_SafePos").Result;
                if (value == null)
                    return new OpResultInDispatchSvc() { IsOK = false, Message = "龙门避让位信号读取失败" };

                JudgeResult = value == "1";

                return new OpResultInDispatchSvc() { IsOK = true, Message = null };
            }
            catch (Exception ex)
            {
                return new OpResultInDispatchSvc() { IsOK = false, Message = ex.Message };
            }
        }

    }
}