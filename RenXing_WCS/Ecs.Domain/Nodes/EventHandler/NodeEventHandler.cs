using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ecs.Dispatch;
using Ecs.Nodes;
using Ecs.Nodes.Etos;
using Ecs.Nodes.Models;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus;

namespace Ecs.Nodes.EventHandler;

public class NodeEventsHandler :
    ILocalEventHandler<ReleaseNodesOccupiedEvent>,
    ITransientDependency

{
    private NodeManager _nodeManager;

    public NodeEventsHandler(
        NodeManager nodeManager)
    {
        _nodeManager = nodeManager;
    }

    public virtual async Task HandleEventAsync(ReleaseNodesOccupiedEvent eventData)
    {
        // 1. 获取要释放节点的任务ID
        int taskIdOwnNodes = eventData.TaskIdOwnNodes;

        // 2. 查询该任务占用的所有节点
        var nodes = await _nodeManager.GetNodesOccupiedByTaskAsync(taskIdOwnNodes)
            .ConfigureAwait(false);

        // 3. 如果没有占用节点，直接返回
        if (nodes.Count == 0)
            return;

        // 4. 收集所有需要释放的节点编码
        List<string> nodeCodes = new List<string>();
        foreach (DispatchNode node in nodes)
        {
            nodeCodes.Add(node.NodeCode);
        }

        // 5. 更新节点状态为空闲，并解除与任务的关联
        bool? ret = await _nodeManager.UpdateNodeDataAsync(
            nodeCodes,           // 节点编码列表
            EnumDispatchNodeState.Idle,  // 新状态：空闲
            -1                   // 任务ID设为-1，表示未分配任务
        ).ConfigureAwait(false);

        // 6. 如果更新失败，抛出异常
        if (ret != true)
            throw new Exception($"{nameof(ReleaseNodesOccupiedEvent)}事件执行失败");
    }
}