using Wcs.Backups;
using Wcs.Caches.Etos;
using Wcs.Dispatch;
using Wcs.Jobs.Etos;
using Wcs.Jobs.Models;
using Wcs.LogTool;
using Wcs.Nodes.Etos;
using Wcs.Tasks.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.EventBus.Local;
using Volo.Abp.Uow;

namespace Wcs.Tasks;

public class TaskManager : ISingletonDependency
{
    private readonly IRepository<DispatchTask, int> _taskRepository;
    private readonly IRepository<DispatchTaskId, int> _taskIdRepository;
    private readonly IRepository<DispatchJob, int> _jobRepository;
    private readonly ILogger<TaskManager> _logger;
    private readonly BackupManager _backupManager;
    private readonly ILocalEventBus _eventBus;
    private readonly IUnitOfWorkManager _uowManager;

    public TaskManager(
        IRepository<DispatchTask, int> taskRepository,
        IRepository<DispatchTaskId, int> taskIdRepository,
        IRepository<DispatchJob, int> jobRepository,
        BackupManager backupHelper,
        ILocalEventBus eventBus,
        IUnitOfWorkManager uowManager,
        ILogger<TaskManager> logger)
    {
        _taskRepository = taskRepository;
        _taskIdRepository = taskIdRepository;
        _jobRepository = jobRepository;
        _backupManager = backupHelper;
        _eventBus = eventBus;
        _uowManager = uowManager;
        _logger = logger;
    }

    /// <summary>
    /// 添加调度任务
    /// </summary>
    /// <param name="task"></param>
    /// <returns></returns>
    /// <exception cref="WcsDomainException"></exception>
    public async Task<bool> AddDispatchTaskAsync(DispatchTask task)
    {
        try
        {
            List<DispatchTask> tasks = await _taskRepository.GetListAsync(o => o.OrderCode == task.OrderCode).ConfigureAwait(false);
            if (tasks.Count > 0) //没有查询到Id为taskId的任务
                throw new Exception($"OrderCode为{task.OrderCode}的调度任务已存在");

            await _taskRepository.InsertAsync(task).ConfigureAwait(false);

            await _backupManager.UpdateTaskInfoOfOrderInRedisAsync(task.OrderCode).ConfigureAwait(false);

            return true;
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            throw new WcsDomainException(ex.Message);
        }
    }
    /// <summary>
    /// 删除调度任务
    /// </summary>
    /// <param name="orderCode"></param>
    /// <returns></returns>
    /// <exception cref="WcsDomainException"></exception>
    public async Task<bool> RemoveDispatchTaskAsync(string orderCode)
    {
        using (IUnitOfWork unit = _uowManager.Begin(isTransactional: true))
        {
            try
            {
                List<DispatchTask> tasks = await _taskRepository.GetListAsync(o => o.OrderCode == orderCode).ConfigureAwait(false);
                if (tasks.Count == 0) //没有查询到Id为taskId的任务，默认删除成功
                    return true;
                if (tasks.Count > 1)
                    throw new Exception($"OrderCode为{orderCode}的调度任务不止1个，数据错误");

                await _eventBus.PublishAsync(new ReleaseNodesOccupiedEvent() { TaskIdOwnNodes = tasks[0].Id });
                await _eventBus.PublishAsync(new ReleaseCacheOccupiedEvent() { TaskIdOwnCache = tasks[0].Id });
                await _eventBus.PublishAsync(new RemoveJobsOfTaskEvent() { TaskId = tasks[0].Id });

                await _taskRepository.DeleteAsync(tasks[0]).ConfigureAwait(false);

                await unit.CompleteAsync().ConfigureAwait(false);
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                throw new WcsDomainException(ex.Message);
            }
        }
    }

    /// <summary>
    /// 更新任务状态
    /// </summary>
    /// <param name="taskId"></param>
    /// <param name="state"></param>
    /// <returns></returns>
    /// <exception cref="WcsDomainException"></exception>
    public async Task<bool?> UpdateDispatchTaskStateAsync(int taskId, EnumDispatchTaskState state)
    {
        try
        {
            List<DispatchTask> tasks = await _taskRepository.GetListAsync(o => o.Id == taskId).ConfigureAwait(false);
            if (tasks.Count == 0) //没有查询到Id为taskId的任务
                throw new Exception($"Id为{taskId}的调度任务不存在");

            tasks[0].State = state;
            await _taskRepository.UpdateAsync(tasks[0]).ConfigureAwait(false);

            await _backupManager.UpdateTaskStateOfOrderInRedisAsync(tasks[0].OrderCode, state.ToString()).ConfigureAwait(false);

            return true;
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            throw new WcsDomainException(ex.Message);
        }
    }

    [UnitOfWork]
    public async Task<bool?> UpdateDispatchTaskStateAsync(int taskId, EnumDispatchTaskState taskState, EnumDispatchJobState jobState)
    {
        try
        {
            List<DispatchTask> tasks = await _taskRepository.GetListAsync(o => o.Id == taskId).ConfigureAwait(false);
            if (tasks.Count == 0) //没有查询到Id为taskId的任务
                throw new Exception($"Id为{taskId}的调度任务不存在");

            List<DispatchJob> jobs = await _jobRepository.GetListAsync(o => o.TaskId == taskId).ConfigureAwait(false);
            if (jobs.Count == 0) //没有查询到TaskId为taskId的Jobs
                throw new Exception($"Id为{taskId}的调度任务没有Job");

            tasks[0].State = taskState;
            await _taskRepository.UpdateAsync(tasks[0]).ConfigureAwait(false);

            foreach (var j in jobs)
            {
                j.State = jobState;
                await _jobRepository.UpdateAsync(j).ConfigureAwait(false);
            }

            await _backupManager.UpdateTaskStateOfOrderInRedisAsync(tasks[0].OrderCode, taskState.ToString()).ConfigureAwait(false);
            foreach (var j in jobs)
            {
                await _backupManager.UpdateJobStateOfOrderInRedisAsync(tasks[0].OrderCode, j.Id, jobState.ToString()).ConfigureAwait(false);
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            return null;
        }
    }

    /// <summary>
    /// 更新任务缓存位置
    /// </summary>
    /// <param name="taskId"></param>
    /// <param name="cachePos"></param>
    /// <returns></returns>
    public async Task<bool?> UpdateDispatchTaskCachePosAsync(int taskId, int cachePos)
    {
        try
        {
            List<DispatchTask> tasks = await _taskRepository.GetListAsync(o => o.Id == taskId).ConfigureAwait(false);
            if (tasks.Count == 0) //没有查询到Id为taskId的任务
                throw new Exception($"Id为{taskId}的调度任务不存在");

            tasks[0].CachePos = cachePos;
            await _taskRepository.UpdateAsync(tasks[0]).ConfigureAwait(false);

            await _backupManager.UpdateCachePosOfOrderInRedisAsync(tasks[0].OrderCode, cachePos).ConfigureAwait(false);

            return true;
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            return null;
        }
    }

    /// <summary>
    /// 获取所有任务
    /// </summary>
    /// <returns></returns>
    public async Task<List<DispatchTask>> GetAllDispatchTasksAsync()
    {
        try
        {
            var tasks = await _taskRepository.GetListAsync().ConfigureAwait(false);
            return tasks.OrderBy(o => o.Id).ToList();
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            return null;
        }
    }

    /// <summary>
    /// 根据任务ID获取任务
    /// </summary>
    /// <param name="taskId"></param>
    /// <returns></returns>
    /// <exception cref="WcsDomainException"></exception>
    public async Task<DispatchTask> GetDispatchTaskByTaskIdAsync(int taskId)
    {
        try
        {
            List<DispatchTask> tasks = await _taskRepository.GetListAsync(o => o.Id == taskId).ConfigureAwait(false);
            if (tasks.Count == 0)
                return null;

            if (tasks.Count > 1)
                throw new Exception("Id为{taskId}的调度任务不止1个");

            return tasks[0];
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            throw new WcsDomainException(ex.Message);
        }
    }
    /// <summary>
    /// 根据订单号获取任务
    /// </summary>
    /// <param name="orderCode"></param>
    /// <returns></returns>
    public async Task<List<DispatchTask>> GetDispatchTasksByOrderCodeAsync(string orderCode)
    {
        try
        {
            List<DispatchTask> tasks = await _taskRepository.GetListAsync(o => o.OrderCode == orderCode).ConfigureAwait(false);
            return tasks.OrderBy(o => o.Id).ToList();
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            return null;
        }
    }

    /// <summary>
    /// 获取指定状态的任务
    /// </summary>
    /// <param name="state"></param>
    /// <returns></returns>
    public async Task<List<DispatchTask>> GetDispatchTasksWithStateAsync(EnumDispatchTaskState state)
    {
        try
        {
            var tasks = await _taskRepository.GetListAsync(o => o.State == state).ConfigureAwait(false);
            return tasks.OrderBy(o => o.Id).ToList();
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            return null;
        }
    }

    /// <summary>
    /// 获取指定状态的调度任务数量
    /// </summary>
    /// <param name="state"></param>
    /// <returns>若发生错误，返回0</returns>
    public async Task<int> NumberOfDispatchTasksWithState(EnumDispatchTaskState state)
    {
        try
        {
            var tasks = await _taskRepository.GetListAsync(o => o.State == state).ConfigureAwait(false);
            return tasks.Count;
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            return 0;
        }
    }

    /// <summary>
    /// 获取下一个task的id
    /// </summary>
    /// <returns>若发生错误，返回-1</returns>
    public async Task<int> GetNextTaskIdAsync()
    {
        try
        {
            List<DispatchTaskId> Ids = await _taskIdRepository.GetListAsync().ConfigureAwait(false);
            if (Ids.Count == 0)
            {
                await _taskIdRepository.InsertAsync(new DispatchTaskId() { TaskId = 1 }).ConfigureAwait(false);
                return 1;
            }
            else
            {
                int taskId = Ids[0].TaskId;
                if (taskId + 1 == int.MaxValue)
                    taskId = 1;
                else
                    taskId = taskId + 1;

                Ids[0].TaskId = taskId;
                await _taskIdRepository.UpdateAsync(Ids[0]).ConfigureAwait(false);
                return taskId;
            }
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            return -1;
        }
    }

}