using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Uow;
using Wcs.ConfigTool;
using Wcs.Dispatch;
using Wcs.Nodes.Models;
using Wcs.Processes.Models;
using Wcs.Processes.ProcessTemplates;

namespace Wcs.Nodes;

/// <summary>只补齐配置门及缺失流程；不修改已有业务步骤，不删除停用门或历史记录。</summary>
public class DoorConfigurationInitializer : ITransientDependency
{
    private readonly DoorConfiguration _doors;
    private readonly IRepository<DispatchNode, int> _nodes;
    private readonly IRepository<DispatchProcess, int> _processes;
    private readonly IRepository<DispatchProcessStep, int> _steps;
    private readonly IRepository<DispatchProcessStepPrecondition, int> _conditions;
    private readonly IRepository<DispatchProcessStepResource, int> _resources;
    private readonly IUnitOfWorkManager _uow;

    public DoorConfigurationInitializer(DoorConfiguration doors, IRepository<DispatchNode, int> nodes,
        IRepository<DispatchProcess, int> processes, IRepository<DispatchProcessStep, int> steps,
        IRepository<DispatchProcessStepPrecondition, int> conditions,
        IRepository<DispatchProcessStepResource, int> resources, IUnitOfWorkManager uow)
    {
        _doors = doors; _nodes = nodes; _processes = processes; _steps = steps;
        _conditions = conditions; _resources = resources; _uow = uow;
    }

    public async Task EnsureAsync()
    {
        using var unit = _uow.Begin(requiresNew: true, isTransactional: true);
        var nodes = await _nodes.GetListAsync();
        var processes = await _processes.GetListAsync();
        var steps = await _steps.GetListAsync();
        var conditions = await _conditions.GetListAsync();
        var resources = await _resources.GetListAsync();
        int nextNodeId = Math.Max(1000, nodes.Select(n => n.Id).DefaultIfEmpty(0).Max() + 1);
        int nextProcessId = processes.Select(p => p.Id).DefaultIfEmpty(0).Max() + 1;
        int nextStepId = steps.Select(s => s.Id).DefaultIfEmpty(0).Max() + 1;

        foreach (string code in _doors.Codes)
        {
            var node = nodes.SingleOrDefault(n => n.NodeCode == code);
            if (node != null && node.NodeTypeCode != "12")
                throw new InvalidOperationException($"柜门配置 {code} 与现有非柜门节点冲突。");
            if (node == null)
            {
                node = new DispatchNode(nextNodeId++)
                {
                    NodeCode = code, NodeName = $"取档口{DoorConfiguration.Number(code)}", NodeTypeCode = "12",
                    DASpecs = DoorConfiguration.DefaultSpecs(code),
                    CmdTagName = $"Plc1.{DoorConfiguration.CommandTag(code)}",
                    ResponseTagName = $"Plc1.{DoorConfiguration.ResponseTag(code)}",
                    TaskIdOwnIt = -1, NodeState = EnumDispatchNodeState.Idle
                };
                await _nodes.InsertAsync(node);
                nodes.Add(node);
            }

            // 空库首次部署仍沿用已有基础数据初始化流程；基础节点就绪后才补柜门过程。
            if (!new[] { "13001", "15001", "18001" }.All(c => nodes.Any(n => n.NodeCode == c))) continue;
            foreach (bool inbound in new[] { true, false })
            {
                string start = inbound ? code : "15001", end = inbound ? "15001" : code;
                if (processes.Any(p => p.StartNodeCode == start && p.EndNodeCode == end)) continue;

                // 优先复用同规格柜门的已配置业务过程，包括现场增加的开门授权工步。
                var peer = processes.Where(p => inbound ? p.EndNodeCode == "15001" : p.StartNodeCode == "15001")
                    .Where(p => _doors.Contains(inbound ? p.StartNodeCode : p.EndNodeCode))
                    .OrderByDescending(p => nodes.Any(n => n.NodeCode == (inbound ? p.StartNodeCode : p.EndNodeCode) && n.DASpecs == node.DASpecs))
                    .ThenBy(p => p.Id).FirstOrDefault();
                int processId = nextProcessId++;
                List<DispatchProcessStep> sourceSteps;
                List<DispatchProcessStepPrecondition> sourceConditions;
                List<DispatchProcessStepResource> sourceResources;
                string peerDoor = peer == null ? code : inbound ? peer.StartNodeCode : peer.EndNodeCode;
                if (peer != null)
                {
                    sourceSteps = steps.Where(s => s.ProcessId == peer.Id).ToList();
                    sourceConditions = conditions.Where(c => c.ProcessId == peer.Id).ToList();
                    sourceResources = resources.Where(r => r.ProcessId == peer.Id).ToList();
                    if (sourceSteps.Count == 0)
                        throw new InvalidOperationException($"参考柜门流程 {peer.Id} 没有工步，不能为 {code} 自动复制。");
                }
                else
                {
                    BaseTemplate template = inbound ? new RK_FixMJJ_Cache() : new CK_FixMJJ_Cache();
                    template.ProcessId = processId; template.StartNode = start; template.EndNode = end;
                    template.Build();
                    sourceSteps = template.Details; sourceConditions = template.Preconditions; sourceResources = template.Resources;
                }
                string Remap(string value) => value?.Replace($"Door{DoorConfiguration.Number(peerDoor)}_", $"Door{DoorConfiguration.Number(code)}_")
                    .Replace(peerDoor, code);
                var process = new DispatchProcess(processId, start, end);
                await _processes.InsertAsync(process); processes.Add(process);
                foreach (var s in sourceSteps)
                {
                    var copy = new DispatchProcessStep(nextStepId++)
                    {
                        ProcessId = processId, Sequence = s.Sequence, NodeCode = Remap(s.NodeCode),
                        JobWorkerId = s.JobWorkerId, JobCmdId = s.JobCmdId,
                        NextTrueStep = s.NextTrueStep, NextFalseStep = s.NextFalseStep, Describe = s.Describe
                    };
                    await _steps.InsertAsync(copy); steps.Add(copy);
                }
                foreach (var c in sourceConditions)
                {
                    var copy = new DispatchProcessStepPrecondition
                    {
                        ProcessId = processId, Sequence = c.Sequence, ConditionName = Remap(c.ConditionName),
                        ConditionValue = c.ConditionValue, Describe = c.Describe
                    };
                    await _conditions.InsertAsync(copy); conditions.Add(copy);
                }
                foreach (var r in sourceResources)
                {
                    var copy = new DispatchProcessStepResource { ProcessId = processId, Sequence = r.Sequence, Resource = Remap(r.Resource) };
                    await _resources.InsertAsync(copy); resources.Add(copy);
                }
            }
        }
        await unit.CompleteAsync();
    }
}
