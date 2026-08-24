using Ecs.Processes.Models;
using Volo.Abp.DependencyInjection;

namespace Ecs.Processes.ProcessTemplates
{
    public class Move_FixMJJ_Cache : BaseTemplate, ITransientDependency
    {
        public override void Build()
        {
            Process = new DispatchProcess(ProcessId, StartNode, EndNode);

            DispatchProcessStep detail1 = new DispatchProcessStep(ProcessId * 20 + 1)
            {
                ProcessId = Process.Id,
                Sequence = 1,
                NodeCode = "17001",
                JobWorkerId = 1,
                JobCmdId = 19,
                NextTrueStep = 2, //下一步：2
                NextFalseStep = 0,
                Describe = "分配缓存"
            };
            DispatchProcessStep detail2 = new DispatchProcessStep(ProcessId * 20 + 2)
            {
                ProcessId = Process.Id,
                Sequence = 2,
                NodeCode = "18001",
                JobWorkerId = 2,
                JobCmdId = 20,
                NextTrueStep = 3,
                NextFalseStep = 8,
                Describe = "是否分配到缓存判断"
            };
            DispatchProcessStep detail3 = new DispatchProcessStep(Process.Id * 20 + 3)
            {
                ProcessId = Process.Id,
                Sequence = 3,
                NodeCode = "13001",
                JobWorkerId = 1,
                JobCmdId = 11,
                NextTrueStep = 4,
                NextFalseStep = 0,
                Describe = "龙门移库取货"
            };
            DispatchProcessStep detail4 = new DispatchProcessStep(Process.Id * 20 + 4)
            {
                ProcessId = Process.Id,
                Sequence = 4,
                NodeCode = "18002",
                JobWorkerId = 1,
                JobCmdId = 18,
                NextTrueStep = 5,
                NextFalseStep = 0,
                Describe = "空步骤"
            };
            DispatchProcessStep detail5 = new DispatchProcessStep(Process.Id * 20 + 5)
            {
                ProcessId = Process.Id,
                Sequence = 5,
                NodeCode = "13001",
                JobWorkerId = 1,
                JobCmdId = 12,
                NextTrueStep = 6,
                NextFalseStep = 0,
                Describe = "龙门移库放货"
            };
            DispatchProcessStep detail6 = new DispatchProcessStep(Process.Id * 20 + 6)
            {
                ProcessId = Process.Id,
                Sequence = 6,
                NodeCode = "18003",
                JobWorkerId = 2,
                JobCmdId = 17,
                NextTrueStep = 7,
                NextFalseStep = 0,
                Describe = "最后一个移库任务判断"
            };
            DispatchProcessStep detail7 = new DispatchProcessStep(Process.Id * 20 + 7)
            {
                ProcessId = Process.Id,
                Sequence = 7,
                NodeCode = "13001",
                JobWorkerId = 1,
                JobCmdId = 2,
                NextTrueStep = 0,
                NextFalseStep = 0,
                Describe = "龙门回原点"
            };
            DispatchProcessStep detail8 = new DispatchProcessStep(Process.Id * 20 + 8)
            {
                ProcessId = Process.Id,
                Sequence = 8,
                NodeCode = "13001",
                JobWorkerId = 1,
                JobCmdId = 11,
                NextTrueStep = 9,
                NextFalseStep = 0,
                Describe = "龙门移库取货"
            };
            DispatchProcessStep detail9 = new DispatchProcessStep(Process.Id * 20 + 9)
            {
                ProcessId = Process.Id,
                Sequence = 9,
                NodeCode = "13001",
                JobWorkerId = 1,
                JobCmdId = 12,
                NextTrueStep = 10,
                NextFalseStep = 0,
                Describe = "龙门移库放货"
            };
            DispatchProcessStep detail10 = new DispatchProcessStep(Process.Id * 20 + 10)
            {
                ProcessId = Process.Id,
                Sequence = 10,
                NodeCode = "18003",
                JobWorkerId = 2,
                JobCmdId = 17,
                NextTrueStep = 11,
                NextFalseStep = 0,
                Describe = "最后一个移库任务判断"
            };
            DispatchProcessStep detail11 = new DispatchProcessStep(Process.Id * 20 + 11)
            {
                ProcessId = Process.Id,
                Sequence = 11,
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
            Details.Add(detail4);
            Details.Add(detail5);
            Details.Add(detail6);
            Details.Add(detail7);
            Details.Add(detail8);
            Details.Add(detail9);
            Details.Add(detail10);
            Details.Add(detail11);

            DispatchProcessStepPrecondition precondition1 = new DispatchProcessStepPrecondition()
            {
                ProcessId = Process.Id,
                Sequence = 3,
                ConditionName = "Plc1.Lm_State",
                ConditionValue = "1",
                Describe = "龙门移库取货前，龙门须处于空闲状态"
            };
            DispatchProcessStepPrecondition precondition2 = new DispatchProcessStepPrecondition()
            {
                ProcessId = Process.Id,
                Sequence = 5,
                ConditionName = "Plc1.Lm_State",
                ConditionValue = "1",
                Describe = "龙门移库放货前，龙门须处于空闲状态"
            };
            DispatchProcessStepPrecondition precondition3 = new DispatchProcessStepPrecondition()
            {
                ProcessId = Process.Id,
                Sequence = 7,
                ConditionName = "Plc1.Lm_State",
                ConditionValue = "1",
                Describe = "龙门回原点前，龙门须处于空闲状态"
            };

            DispatchProcessStepPrecondition precondition4 = new DispatchProcessStepPrecondition()
            {
                ProcessId = Process.Id,
                Sequence = 8,
                ConditionName = "Plc1.Lm_State",
                ConditionValue = "1",
                Describe = "龙门移库取货前，龙门须处于空闲状态"
            };
            DispatchProcessStepPrecondition precondition5 = new DispatchProcessStepPrecondition()
            {
                ProcessId = Process.Id,
                Sequence = 9,
                ConditionName = "Plc1.Lm_State",
                ConditionValue = "1",
                Describe = "龙门移库放货前，龙门须处于空闲状态"
            };
            DispatchProcessStepPrecondition precondition6 = new DispatchProcessStepPrecondition()
            {
                ProcessId = Process.Id,
                Sequence = 11,
                ConditionName = "Plc1.Lm_State",
                ConditionValue = "1",
                Describe = "龙门回原点前，龙门须处于空闲状态"
            };

            Preconditions.Add(precondition1);
            Preconditions.Add(precondition2);
            Preconditions.Add(precondition3);
            Preconditions.Add(precondition4);
            Preconditions.Add(precondition5);
            Preconditions.Add(precondition6);

            DispatchProcessStepResource resource1 = new DispatchProcessStepResource()
            {
                ProcessId = Process.Id,
                Sequence = 1,
                Resource = $"17001"
            };
            DispatchProcessStepResource resource2 = new DispatchProcessStepResource()
            {
                ProcessId = Process.Id,
                Sequence = 2,
                Resource = $"18001"
            };
            DispatchProcessStepResource resource3 = new DispatchProcessStepResource()
            {
                ProcessId = Process.Id,
                Sequence = 3,
                Resource = $"13001"
            };
            DispatchProcessStepResource resource4 = new DispatchProcessStepResource()
            {
                ProcessId = Process.Id,
                Sequence = 4,
                Resource = $"0" //不占用资源
            };
            DispatchProcessStepResource resource5 = new DispatchProcessStepResource()
            {
                ProcessId = Process.Id,
                Sequence = 5,
                Resource = $"13001"
            };
            DispatchProcessStepResource resource6 = new DispatchProcessStepResource()
            {
                ProcessId = Process.Id,
                Sequence = 6,
                Resource = $"18003,13001"
            };
            DispatchProcessStepResource resource7 = new DispatchProcessStepResource()
            {
                ProcessId = Process.Id,
                Sequence = 7,
                Resource = $"13001"
            };
            DispatchProcessStepResource resource8 = new DispatchProcessStepResource()
            {
                ProcessId = Process.Id,
                Sequence = 8,
                Resource = $"13001"
            };
            DispatchProcessStepResource resource9 = new DispatchProcessStepResource()
            {
                ProcessId = Process.Id,
                Sequence = 9,
                Resource = $"13001"
            };
            DispatchProcessStepResource resource10 = new DispatchProcessStepResource()
            {
                ProcessId = Process.Id,
                Sequence = 10,
                Resource = $"18003,13001"
            };
            DispatchProcessStepResource resource11 = new DispatchProcessStepResource()
            {
                ProcessId = Process.Id,
                Sequence = 11,
                Resource = $"13001"
            };

            Resources.Add(resource1);
            Resources.Add(resource2);
            Resources.Add(resource3);
            Resources.Add(resource4);
            Resources.Add(resource5);
            Resources.Add(resource6);
            Resources.Add(resource7);
            Resources.Add(resource8);
            Resources.Add(resource9);
            Resources.Add(resource10);
            Resources.Add(resource11);
        }
    }
}
