using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Volo.Abp;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Wcs.LogTool;
using Volo.Abp.Uow;
using System.Collections.Generic;
using System.Linq;
using Wcs.Dispatch;
using Wcs.Nodes.Models;

namespace Wcs.Nodes;

public class NodeManager : ISingletonDependency
{
    private readonly ILogger<NodeManager> _logger;
    private readonly IRepository<DispatchNode, int> _nodeRepository;
    private readonly IRepository<DispatchNodeType, int> _nodeTypeRepository;
    private readonly IRepository<DispatchNodeCmd, int> _nodeCmdRepository;
    private readonly IUnitOfWorkManager _unitOfWorkManager;

    public NodeManager(
        IRepository<DispatchNode, int> nodeRepository,
        IRepository<DispatchNodeType, int> nodeTypeRepository,
        IRepository<DispatchNodeCmd, int> nodeCmdRepository,
        IUnitOfWorkManager unitOfWorkManager,
        ILogger<NodeManager> logger)
    {
        _nodeRepository = nodeRepository;
        _nodeTypeRepository = nodeTypeRepository;
        _nodeCmdRepository = nodeCmdRepository;
        _unitOfWorkManager = unitOfWorkManager;
        _logger = logger;
    }

    public async Task<bool> AddNodeTypeAsync(DispatchNodeType nodeType)
    {
        try
        {
            Check.NotNullOrEmpty(nodeType.NodeTypeName, nameof(nodeType.NodeTypeName));
            Check.NotNullOrEmpty(nodeType.NodeTypeCode, nameof(nodeType.NodeTypeCode));

            var types = await _nodeTypeRepository.GetListAsync(
                o => o.NodeTypeCode == nodeType.NodeTypeCode ||
                o.NodeTypeName == nodeType.NodeTypeName)
                .ConfigureAwait(false);
            if (types.Count > 0)
                throw new Exception($"类型码{nodeType.NodeTypeCode}或类型名{nodeType.NodeTypeName}已存在");

            await _nodeTypeRepository.InsertAsync(nodeType).ConfigureAwait(false);
            return true;
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            return false;
        }
    }

    [UnitOfWork]
    public async Task<bool> DelNodeTypeAsync(string nodeTypeCode)
    {
        try
        {
            var types = await _nodeTypeRepository.GetListAsync(o => o.NodeTypeCode == nodeTypeCode).ConfigureAwait(false);
            if (types.Count == 0) return true; //原本没有，默认删除成功

            using (var unit = _unitOfWorkManager.Begin(isTransactional: true))
            {
                foreach (var type in types)
                {
                    await _nodeTypeRepository.DeleteAsync(type).ConfigureAwait(false);
                }
                await unit.CompleteAsync().ConfigureAwait(false);
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            return false;
        }
    }

    public async Task<bool> DelAllNodeTypesAsync()
    {
        try
        {
            var types = await _nodeTypeRepository.GetListAsync().ConfigureAwait(false);
            if (types.Count == 0) return true; //原本没有，默认删除成功

            using (var unit = _unitOfWorkManager.Begin(isTransactional: true))
            {
                foreach (var type in types)
                {
                    await _nodeTypeRepository.DeleteAsync(type).ConfigureAwait(false);
                }
                await unit.CompleteAsync().ConfigureAwait(false);
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            return false;
        }
    }

    public async Task<DispatchNodeType> GetNodeTypeAsync(string nodeTypeCode)
    {
        try
        {
            var types = await _nodeTypeRepository.GetListAsync(o => o.NodeTypeCode == nodeTypeCode).ConfigureAwait(false);
            if (types.Count == 0) return null;

            if (types.Count > 1)
                throw new Exception($"类型码为{nodeTypeCode}的设备节点类型多于1个");

            return types[0];
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            return null;
        }
    }

    public async Task<List<DispatchNodeType>> GetAllNodeTypesAsync()
    {
        try
        {
            var types = await _nodeTypeRepository.GetListAsync().ConfigureAwait(false);
            if (types.Count == 0) return new List<DispatchNodeType>();

            return types.OrderBy(o => o.NodeTypeCode).ToList();
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            return new List<DispatchNodeType>();
        }
    }



    public async Task<bool> AddNodeCmdAsync(DispatchNodeCmd nodeCmd)
    {
        try
        {
            Check.NotNullOrEmpty(nodeCmd.NodeTypeCode, nameof(nodeCmd.NodeTypeCode));
            Check.NotNullOrEmpty(nodeCmd.NodeCmdName, nameof(nodeCmd.NodeCmdName));
            Check.Positive(nodeCmd.NodeCmdValue, nameof(nodeCmd.NodeCmdValue));

            var types = await _nodeTypeRepository.GetListAsync(
                o => o.NodeTypeCode == nodeCmd.NodeTypeCode)
                .ConfigureAwait(false);
            if (types.Count == 0)
                throw new Exception($"类型码{nodeCmd.NodeTypeCode}不存在");

            var cmds = await _nodeCmdRepository.GetListAsync(o =>
                o.NodeTypeCode == nodeCmd.NodeTypeCode && o.NodeCmdName == nodeCmd.NodeCmdName ||
                o.NodeTypeCode == nodeCmd.NodeTypeCode && o.NodeCmdValue == nodeCmd.NodeCmdValue
            ).ConfigureAwait(false);
            if (cmds.Count > 0)
                throw new Exception($"类型码为{nodeCmd.NodeTypeCode}的节点类型已存在命令名为{nodeCmd.NodeCmdName}或命令值为{nodeCmd.NodeCmdValue}的节点命令");

            await _nodeCmdRepository.InsertAsync(nodeCmd).ConfigureAwait(false);
            return true;
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            return false;
        }
    }

    public async Task<bool> DelNodeCmdByCmdNameAsync(string nodeCmdName)
    {
        try
        {
            var cmds = await _nodeCmdRepository.GetListAsync(o => o.NodeCmdName == nodeCmdName).ConfigureAwait(false);
            if (cmds.Count == 0) return true; //原本没有，默认删除成功

            using (var unit = _unitOfWorkManager.Begin(isTransactional: true))
            {
                foreach (var cmd in cmds)
                {
                    await _nodeCmdRepository.DeleteAsync(cmd).ConfigureAwait(false);
                }
                await unit.CompleteAsync().ConfigureAwait(false);
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            return false;
        }
    }

    public async Task<bool> DelNodeCmdByCmdValueAsync(int nodeCmdValue)
    {
        try
        {
            var cmds = await _nodeCmdRepository.GetListAsync(o => o.NodeCmdValue == nodeCmdValue).ConfigureAwait(false);
            if (cmds.Count == 0) return true; //原本没有，默认删除成功

            using (var unit = _unitOfWorkManager.Begin(isTransactional: true))
            {
                foreach (var cmd in cmds)
                {
                    await _nodeCmdRepository.DeleteAsync(cmd).ConfigureAwait(false);
                }
                await unit.CompleteAsync().ConfigureAwait(false);
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            return false;
        }
    }

    public async Task<bool> DelNodeCmdByNodeTypeAsync(string nodeTypeCode)
    {
        try
        {
            var cmds = await _nodeCmdRepository.GetListAsync(o => o.NodeTypeCode == nodeTypeCode).ConfigureAwait(false);
            if (cmds.Count == 0) return true; //原本没有，默认删除成功

            using (var unit = _unitOfWorkManager.Begin(isTransactional: true))
            {
                foreach (var cmd in cmds)
                {
                    await _nodeCmdRepository.DeleteAsync(cmd).ConfigureAwait(false);
                }
                await unit.CompleteAsync().ConfigureAwait(false);
            }
            return true;
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            return false;
        }
    }

    public async Task<bool> DelAllNodeCmdsAsync()
    {
        try
        {
            var cmds = await _nodeCmdRepository.GetListAsync().ConfigureAwait(false);
            if (cmds.Count == 0) return true; //原本没有，默认删除成功

            using (var unit = _unitOfWorkManager.Begin(isTransactional: true))
            {
                foreach (var cmd in cmds)
                    await _nodeCmdRepository.DeleteAsync(cmd).ConfigureAwait(false);

                await unit.CompleteAsync().ConfigureAwait(false);
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            return false;
        }
    }

    public async Task<DispatchNodeCmd> GetNodeCmdAsync(string nodeTypeCode, string nodeCmdName)
    {
        try
        {
            var cmds = await _nodeCmdRepository.GetListAsync(o =>
                o.NodeTypeCode == nodeTypeCode &&
                o.NodeCmdName == nodeCmdName
            ).ConfigureAwait(false);

            if (cmds.Count == 0) return null;
            if (cmds.Count > 1) throw new Exception($"节点类型为{nodeTypeCode}，命令名称为{nodeCmdName}的节点命令多于1个");

            return cmds[0];
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            return null;
        }
    }

    public async Task<List<DispatchNodeCmd>> GetAllNodeCmdsAsync()
    {
        try
        {
            var cmds = await _nodeCmdRepository.GetListAsync().ConfigureAwait(false);

            if (cmds.Count == 0) return new List<DispatchNodeCmd>();

            return cmds.OrderBy(o => o.NodeTypeCode).ThenBy(o => o.NodeCmdValue).ToList();
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            return new List<DispatchNodeCmd>();
        }
    }


    public async Task<bool> AddNodeAsync(DispatchNode node)
    {
        try
        {
            Check.NotNullOrEmpty(node.NodeCode, nameof(node.NodeCode));
            Check.NotNullOrEmpty(node.NodeName, nameof(node.NodeName));
            Check.NotNullOrEmpty(node.NodeTypeCode, nameof(node.NodeTypeCode));
            Check.NotNullOrEmpty(node.DASpecs, nameof(node.DASpecs));
            Check.NotNullOrEmpty(node.CmdTagName, nameof(node.CmdTagName));
            Check.NotNullOrEmpty(node.ResponseTagName, nameof(node.ResponseTagName));

            var nodes = await _nodeRepository.GetListAsync(o => o.NodeCode == node.NodeCode).ConfigureAwait(false);
            if (nodes.Count > 0)
                throw new Exception($"NodeCode为{node.NodeCode}的节点已经存在");

            await _nodeRepository.InsertAsync(node).ConfigureAwait(false);
            return true;
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            return false;
        }
    }

    public async Task<bool> DelNodeByNodeCodeAsync(string nodeCode)
    {
        try
        {
            var nodes = await _nodeRepository.GetListAsync(o => o.NodeCode == nodeCode).ConfigureAwait(false);
            if (nodes.Count == 0) return true; //原本没有，默认删除成功

            using (var unit = _unitOfWorkManager.Begin(isTransactional: true))
            {
                foreach (var node in nodes)
                {
                    await _nodeRepository.DeleteAsync(node).ConfigureAwait(false);
                }
                await unit.CompleteAsync().ConfigureAwait(false);
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            return false;
        }
    }

    public async Task<bool> DelAllNodesAsync()
    {
        try
        {
            var nodes = await _nodeRepository.GetListAsync().ConfigureAwait(false);
            if (nodes.Count == 0) return true; //原本没有，默认删除成功

            using (var unit = _unitOfWorkManager.Begin(isTransactional: true))
            {
                foreach (var node in nodes)
                {
                    await _nodeRepository.DeleteAsync(node).ConfigureAwait(false);
                }
                await unit.CompleteAsync().ConfigureAwait(false);
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            return false;
        }
    }

    /// <summary>
    /// 修改节点的数据，包括节点状态、占用此节点的调度任务Id
    /// </summary>
    /// <param name="nodeCodes">被修改的节点Code集合</param>
    /// <param name="newState">新的状态</param>
    /// <param name="taskIdOwnIt">占用此节点的调度任务ID，若没有占用，值为-1</param>
    /// <returns>true：成功，false：失败，null：发生错误</returns>
    public async Task<bool?> UpdateNodeDataAsync(List<string> nodeCodes, EnumDispatchNodeState newState, int taskIdOwnIt)
    {
        if (nodeCodes.Count == 0)
            return false;

        using (var unit = _unitOfWorkManager.Begin(isTransactional: true))
        {
            try
            {
                List<DispatchNode> nodes = new List<DispatchNode>();

                foreach (var code in nodeCodes)
                {
                    //同一个调度任务下的某一个命令只能有一个
                    List<DispatchNode> nds = await _nodeRepository.GetListAsync(o => o.NodeCode == code).ConfigureAwait(false);

                    if (nds == null || nds.Count == 0) //没有找到
                        throw new Exception($"NodeCode为{code}的节点不存在");

                    if (nds.Count > 1)
                        throw new Exception($"NodeCode为{code}的节点多于1个");

                    nds[0].NodeState = newState;
                    nds[0].TaskIdOwnIt = taskIdOwnIt;
                    nodes.Add(nds[0]);
                }

                foreach (var node in nodes)
                {
                    await _nodeRepository.UpdateAsync(node).ConfigureAwait(false);
                }

                await unit.CompleteAsync().ConfigureAwait(false);

                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                return null;
            }
        }
    }

    /// <summary>
    /// 根据节点码查询对应的节点信息
    /// </summary>
    /// <param name="nodeCode"></param>
    /// <returns></returns>
    public async Task<DispatchNode> GetNodeByNodeCodeAsync(string nodeCode)
    {
        try
        {
            var nodes = await _nodeRepository.GetListAsync(o => o.NodeCode == nodeCode).ConfigureAwait(false);
            if (nodes == null || nodes.Count == 0)
                return null;

            return nodes[0];
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            return null;
        }
    }

    /// <summary>
    /// 查询被指定调度任务占用的节点信息
    /// </summary>
    /// <param name="taskIdOwnIt"></param>
    /// <returns></returns>
    public async Task<List<DispatchNode>> GetNodesOccupiedByTaskAsync(int taskIdOwnIt)
    {
        try
        {
            var ret = await _nodeRepository.GetListAsync(o => o.TaskIdOwnIt == taskIdOwnIt).ConfigureAwait(false);
            return ret.OrderBy(o => o.Id).ToList();
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            return null;
        }
    }

    /// <summary>
    /// 获取所有的节点数据
    /// </summary>
    /// <returns>Key：NodeCode，Value：DispatchNode 键值对集合</returns>
    public async Task<Dictionary<string, DispatchNode>> GetAllNodesAsync()
    {
        try
        {
            var nodes = await _nodeRepository.GetListAsync().ConfigureAwait(false);
            Dictionary<string, DispatchNode> nodeDic = new Dictionary<string, DispatchNode>();
            foreach (var n in nodes)
            {
                nodeDic.Add(n.NodeCode, n);
            }

            return nodeDic;
        }
        catch (Exception e)
        {
            _logger.Error(e.Message);
            return null;
        }
    }

}