using Wcs.Processes.Models;
using Volo.Abp.DependencyInjection;

namespace Wcs.Processes.ProcessTemplates
{
    public class Chk_FixMJJ_Cache : BaseTemplate, ITransientDependency
    {
        public override void Build()
        {
            Process = new DispatchProcess(ProcessId, StartNode, EndNode);

            DispatchProcessStep detail1 = new DispatchProcessStep(ProcessId * 20 + 1)
            {
                ProcessId = Process.Id,
                Sequence = 1,
                NodeCode = "13001",
                JobWorkerId = 1,
                JobCmdId = 4,
                NextTrueStep = 2, //下一步：2
                NextFalseStep = 0,
                Describe = "龙门读库位信息"
            };
            DispatchProcessStep detail2 = new DispatchProcessStep(Process.Id * 20 + 2)
            {
                ProcessId = Process.Id,
                Sequence = 2,
                NodeCode = "18001",
                JobWorkerId = 2,
                JobCmdId = 14,
                NextTrueStep = 3,
                NextFalseStep = 0,
                Describe = "最后一个盘点任务判断"
            };
            DispatchProcessStep detail3 = new DispatchProcessStep(Process.Id * 20 + 3)
            {
                ProcessId = Process.Id,
                Sequence = 3,
                NodeCode = "13001",
                JobWorkerId = 1,
                JobCmdId = 2,
                NextTrueStep = 0,
                NextFalseStep = 0,
                Describe = "龙门回原点"
            };

            Details.Add(detail1);
            Details.Add(detail2);
            Details.Add(detail3);

            DispatchProcessStepPrecondition precondition1 = new DispatchProcessStepPrecondition()
            {
                ProcessId = Process.Id,
                Sequence = 1,
                ConditionName = "Plc1.Lm_State",
                ConditionValue = "1",
                Describe = "龙门读库位信息前，龙门须处于空闲状态"
            };
            DispatchProcessStepPrecondition precondition2 = new DispatchProcessStepPrecondition()
            {
                ProcessId = Process.Id,
                Sequence = 3,
                ConditionName = "Plc1.Lm_State",
                ConditionValue = "1",
                Describe = "龙门回原点前，龙门须处于空闲状态"
            };

            Preconditions.Add(precondition1);
            Preconditions.Add(precondition2);

            DispatchProcessStepResource resource1 = new DispatchProcessStepResource()
            {
                ProcessId = Process.Id,
                Sequence = 1,
                Resource = $"13001"
            };
            DispatchProcessStepResource resource2 = new DispatchProcessStepResource()
            {
                ProcessId = Process.Id,
                Sequence = 2,
                Resource = $"13001,18001"
            };
            DispatchProcessStepResource resource3 = new DispatchProcessStepResource()
            {
                ProcessId = Process.Id,
                Sequence = 3,
                Resource = $"13001"
            };

            Resources.Add(resource1);
            Resources.Add(resource2);
            Resources.Add(resource3);
        }
    }
}
