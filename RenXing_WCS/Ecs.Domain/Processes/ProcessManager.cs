using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ecs.Conditions;
using Ecs.Dispatch;
using Ecs.LogTool;
using Ecs.Nodes;
using Ecs.Nodes.Models;
using Ecs.Processes.Models;
using Ecs.Processes.ProcessTemplates;
using Microsoft.Extensions.Logging;
using Volo.Abp;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Uow;

namespace Ecs.Processes;

public class ProcessManager : ISingletonDependency
{
    private readonly ILogger<ProcessManager> _logger;
    private readonly IRepository<DispatchProcess, int> _processRepository;
    private readonly IRepository<DispatchProcessStep, int> _processStepRepository;
    private readonly IRepository<DispatchProcessStepPrecondition, int> _preconditionRepository;
    private readonly IRepository<DispatchProcessStepResource, int> _resourceRepository;
    private readonly NodeManager _nodeManager;
    private readonly ConditionManager _conditionManager;

    public ProcessManager(
        ILogger<ProcessManager> logger,
        IRepository<DispatchProcess, int> processRepository,
        IRepository<DispatchProcessStep, int> processStepRepository,
        IRepository<DispatchProcessStepPrecondition, int> preconditionRepository,
        IRepository<DispatchProcessStepResource, int> sourceRepository,
        NodeManager nodeManager,
        ConditionManager conditionManager
        )
    {
        _logger = logger;
        _processRepository = processRepository;
        _processStepRepository = processStepRepository;
        _preconditionRepository = preconditionRepository;
        _resourceRepository = sourceRepository;
        _nodeManager = nodeManager;
        _conditionManager = conditionManager;
    }

    /// <summary>
    /// 添加过程种子，包含前提、资源等
    /// </summary>
    /// <param name="templates"></param>
    /// <returns></returns>
    [UnitOfWork]
    public async Task<bool> AddProcessSeedAsync(BaseTemplate templates)
    {
        try
        {
            Check.NotNull(templates.Process, nameof(templates.Process));
            Check.NotNullOrEmpty(templates.Details, nameof(templates.Details));
            Check.NotNullOrEmpty(templates.Preconditions, nameof(templates.Preconditions));
            Check.NotNullOrEmpty(templates.Resources, nameof(templates.Resources));

            List<DispatchProcess> paths = await _processRepository.GetListAsync(o => o.Id == templates.Process.Id).ConfigureAwait(false);
            if (paths.Count > 0)
                throw new Exception($"Id为{templates.Process.Id}的过程已经存在，重复添加");

            paths = await _processRepository.GetListAsync(
                o => o.StartNodeCode == templates.Process.StartNodeCode
                && o.EndNodeCode == templates.Process.EndNodeCode)
                .ConfigureAwait(false);
            if (paths.Count > 0)
                throw new Exception($"起始节点为{templates.Process.StartNodeCode}，终止节点为{templates.Process.EndNodeCode}的过程已经存在，重复添加");

            await _processRepository.InsertAsync(templates.Process).ConfigureAwait(false);

            foreach (var detail in templates.Details)
                await _processStepRepository.InsertAsync(detail).ConfigureAwait(false);

            foreach (var condition in templates.Preconditions)
                await _preconditionRepository.InsertAsync(condition).ConfigureAwait(false);

            foreach (var reource in templates.Resources)
                await _resourceRepository.InsertAsync(reource).ConfigureAwait(false);

            return true;
        }
        catch (Exception e)
        {
            _logger.Error(e.Message);
            return false;
        }
    }

    [UnitOfWork]
    public async Task<bool> DelAllProcessesAsync()
    {
        try
        {
            await _resourceRepository.DeleteAsync(o => o.Id > 0).ConfigureAwait(false);
            await _preconditionRepository.DeleteAsync(o => o.Id > 0).ConfigureAwait(false);
            await _processStepRepository.DeleteAsync(o => o.Id > 0).ConfigureAwait(false);
            await _processRepository.DeleteAsync(o => o.Id > 0).ConfigureAwait(false);

            return true;
        }
        catch (Exception e)
        {
            _logger.Error(e.Message);
            return false;
        }
    }

    /// <summary>
    /// 查询所有的调度过程
    /// </summary>
    /// <returns></returns>
    public async Task<List<DispatchProcess>> GetAllDispatchProcessesAsync()
    {
        try
        {
            var ret = await _processRepository.GetListAsync().ConfigureAwait(false);
            if (ret == null || ret.Count == 0)
                return ret;

            return ret.OrderBy(o => o.Id).ToList();
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            return null;
        }
    }

    /// <summary>
    /// 根据起点和终点，查询调度过程
    /// </summary>
    /// <param name="startNodeCode">起点</param>
    /// <param name="endNodeCode">终点</param>
    /// <returns></returns>
    public async Task<DispatchProcess> GetDispatchProcessAsync(string startNodeCode, string endNodeCode)
    {
        try
        {
            var paths = await _processRepository.GetListAsync(
                o => o.StartNodeCode == startNodeCode && o.EndNodeCode == endNodeCode)
                .ConfigureAwait(false);

            if (paths == null || paths.Count == 0)
                return null;

            return paths[0];
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            return null;
        }
    }

    /// <summary>
    /// 根据过程Id，查询调度过程
    /// </summary>
    /// <param name="processId"></param>
    /// <returns></returns>
    public async Task<DispatchProcess> GetDispatchProcessAsync(int processId)
    {
        try
        {
            return await _processRepository.GetAsync(processId).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            return null;
        }
    }

    /// <summary>
    /// 根据过程ID查询过程详细信息
    /// </summary>
    /// <param name="processId">过程ID</param>
    /// <returns></returns>
    public async Task<List<DispatchProcessStep>> GetDispatchProcessStepsAsync(int processId)
    {
        try
        {
            var steps = await _processStepRepository.GetListAsync(o => o.ProcessId == processId).ConfigureAwait(false);
            if (steps == null || steps.Count == 0)
                return steps;

            return steps.OrderBy(o => o.Id).ToList();
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            return null;
        }
    }

    /// <summary>
    /// 查询指定过程下的某个命令需要的资源
    /// </summary>
    /// <param name="processId">指定过程Id</param>
    /// <param name="sequence">过程中的指定命令</param>
    /// <returns>资源数据，各资源间以逗号分隔</returns>async
    public async Task<string> GetResourceOfProcessStepAsync(int processId, int sequence)
    {
        try
        {
            var resources = await _resourceRepository.GetListAsync(o => o.ProcessId == processId && o.Sequence == sequence).ConfigureAwait(false);
            if (resources == null || resources.Count == 0)
                return null;

            return resources[0].Resource;
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            return null;
        }
    }

    /// <summary>
    /// 判断某个任务的某个节点是否占用了需要的所有资源
    /// </summary>
    /// <param name="taskId">调度任务</param>
    /// <param name="processId">调度任务对应的过程ID</param>
    /// <param name="sequence">过程节点</param>
    /// <returns>true：被自己占用，false：未被自己占用，null：发生错误</returns>
    public async Task<bool?> IsResourcesOccupiedBySelf(int taskId, int processId, int sequence)
    {
        try
        {
            var resources = await _resourceRepository.GetListAsync(o => o.ProcessId == processId && o.Sequence == sequence).ConfigureAwait(false);

            if (resources == null || resources.Count == 0)
                throw new Exception($"过程ID：{processId}，命令：{sequence}，对应的资源定义查询失败");

            if (resources[0].Resource == "0") //表示释放资源，不需要占用任何资源
                return true;

            string[] resArray = resources[0].Resource.Split(","); //每一个resource均为Node
            if (resArray.Length == 0) //没有指定资源，属于配置错误，至少节点本身必须作为资源
                throw new Exception($"过程ID：{processId}，命令：{sequence}，对应的资源为空，配置错误");

            bool occupiedBySelf = true;
            foreach (string res in resArray)
            {
                DispatchNode node = await _nodeManager.GetNodeByNodeCodeAsync(res).ConfigureAwait(false);
                if (node == null)
                    throw new Exception($"过程ID：{processId}，命令：{sequence}，对应的其中一个资源{res}没有定义");
                if (node.NodeState != EnumDispatchNodeState.Working || node.TaskIdOwnIt != taskId)
                {
                    occupiedBySelf = false;
                    break;
                }
            }
            return occupiedBySelf;
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            return null;
        }
    }


    public async Task<List<DispatchProcessStepPrecondition>> GetPreconditionOfSomePathNode(int processId, int sequence)
    {
        try
        {
            var preConditions = await _preconditionRepository.GetListAsync(o => o.ProcessId == processId && o.Sequence == sequence).ConfigureAwait(false);
            return preConditions.OrderBy(o => o.Id).ToList();
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            return null;
        }
    }

    public async Task<OpResultInDispatchSvc> IsAllPreconditionSatisfied(int processId, int sequence)
    {
        try
        {
            var conds = await _preconditionRepository.GetListAsync(c => c.ProcessId == processId && c.Sequence == sequence).ConfigureAwait(false);
            conds = conds.OrderBy(o => o.Id).ToList();

            if (conds.Count == 0) //没有前提条件，默认满足
                return new OpResultInDispatchSvc() { IsOK = true, Message = string.Empty };

            bool bIsSatisfied = true;
            string reason = null;
            foreach (var c in conds)
            {
                string value = await _conditionManager.GetConditionValueAsync(c.ConditionName).ConfigureAwait(false);
                if (value == null)
                {
                    bIsSatisfied = false;
                    reason = $"{c.ConditionName}为null";
                    break;
                }
                if (value != c.ConditionValue)
                {
                    bIsSatisfied = false;
                    reason = $"{c.ConditionName}为{value}，非目标值{c.ConditionValue}";
                    break;
                }
            }

            return new OpResultInDispatchSvc() { IsOK = bIsSatisfied, Message = reason };
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            return new OpResultInDispatchSvc() { IsOK = false, Message = ex.Message };
        }
    }

}