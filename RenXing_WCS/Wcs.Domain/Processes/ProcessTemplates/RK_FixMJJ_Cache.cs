using Wcs.Processes.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace Wcs.Processes.ProcessTemplates
{
    public class RK_FixMJJ_Cache : BaseTemplate, ITransientDependency
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
                JobCmdId = 7,
                NextTrueStep = 2, //下一步：2
                NextFalseStep = 0,
                Describe = "龙门入库取货"
            };
            DispatchProcessStep detail2 = new DispatchProcessStep(Process.Id * 20 + 2)
            {
                ProcessId = Process.Id,
                Sequence = 2,
                NodeCode = "13001",
                JobWorkerId = 1,
                JobCmdId = 8,
                NextTrueStep = 3,
                NextFalseStep = 0,
                Describe = "龙门入库放货"
            };
            DispatchProcessStep detail3 = new DispatchProcessStep(Process.Id * 20 + 3)
            {
                ProcessId = Process.Id,
                Sequence = 3,
                NodeCode = "18001",
                JobWorkerId = 2,
                JobCmdId = 15,
                NextTrueStep = 4,
                NextFalseStep = 0,
                Describe = "判断是否是最后一个入库任务"
            };
            DispatchProcessStep detail4 = new DispatchProcessStep(Process.Id * 20 + 4)
            {
                ProcessId = Process.Id,
                Sequence = 4,
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

            DispatchProcessStepPrecondition precondition1 = new DispatchProcessStepPrecondition()
            {
                ProcessId = Process.Id,
                Sequence = 1,
                ConditionName = "Plc1.Lm_State",
                ConditionValue = "1",
                Describe = "龙门入库取货前，龙门须处于空闲状态"
            };
            DispatchProcessStepPrecondition precondition2 = new DispatchProcessStepPrecondition()
            {
                ProcessId = Process.Id,
                Sequence = 2,
                ConditionName = "Plc1.Lm_State",
                ConditionValue = "1",
                Describe = "龙门入库放货前，龙门须处于空闲状态"
            };
            DispatchProcessStepPrecondition precondition3 = new DispatchProcessStepPrecondition()
            {
                ProcessId = Process.Id,
                Sequence = 4,
                ConditionName = "Plc1.Lm_State",
                ConditionValue = "1",
                Describe = "龙门回原点前，龙门须处于空闲状态"
            };

            Preconditions.Add(precondition1);
            Preconditions.Add(precondition2);
            Preconditions.Add(precondition3);

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
                Resource = $"13001"
            };
            DispatchProcessStepResource resource3 = new DispatchProcessStepResource()
            {
                ProcessId = Process.Id,
                Sequence = 3,
                Resource = $"13001,18001"
            };
            DispatchProcessStepResource resource4 = new DispatchProcessStepResource()
            {
                ProcessId = Process.Id,
                Sequence = 4,
                Resource = $"13001"
            };
            
            Resources.Add(resource1);
            Resources.Add(resource2);
            Resources.Add(resource3);
            Resources.Add(resource4);
        }
    }
}
