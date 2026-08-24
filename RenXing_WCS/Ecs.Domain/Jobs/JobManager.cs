using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using Ecs.Backups;
using Ecs.Dispatch;
using Ecs.Jobs.Models;
using Ecs.LogTool;
using Microsoft.Extensions.Logging;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Uow;

namespace Ecs.Jobs;

public class JobManager : ISingletonDependency
{
    private readonly IRepository<DispatchJob, int> _jobRepository;
    private readonly IRepository<DispatchJobId, int> _jobIdRepository;
    private readonly IRepository<DispatchJobCmd, int> _cmdRepository;
    private readonly IRepository<DispatchJobWorker, int> _workerRepository;
    private readonly ILogger<JobManager> _logger;
    private readonly BackupManager _backupManager;
    private readonly IUnitOfWorkManager _unitOfWorkManager;

    public JobManager(
        IRepository<DispatchJob, int> jobRepository,
        IRepository<DispatchJobId, int> jobIdRepository,
        IRepository<DispatchJobCmd, int> cmdRepository,
        IRepository<DispatchJobWorker, int> workerRepository,
        BackupManager backupManager,
        IUnitOfWorkManager unitOfWorkManager,
        ILogger<JobManager> logger)
    {
        _jobRepository = jobRepository;
        _jobIdRepository = jobIdRepository;
        _cmdRepository = cmdRepository;
        _workerRepository = workerRepository;
        _backupManager = backupManager;
        _unitOfWorkManager = unitOfWorkManager;
        _logger = logger;
    }

    /// <summary>
    /// 添加单个作业：
    /// </summary>
    /// <param name="job"></param>
    /// <returns></returns>
    /// <exception cref="EcsDomainException"></exception>
    public async Task<bool?> AddDispatchJobAsync(DispatchJob job)
    {
        try
        {
            //同一个调度任务下的某一个命令只能有一个
            var jobs = await _jobRepository.GetListAsync(
                o => o.TaskId == job.TaskId &&
                o.ProcessSequence == job.ProcessSequence)
                .ConfigureAwait(false);

            if (jobs.Count > 0) //重复了
                throw new Exception($"Id为{job.TaskId}的调度任务已存在PathStep为{job.ProcessSequence}的Job，重复添加");

            await _jobRepository.InsertAsync(job).ConfigureAwait(false);

            await _backupManager.UpdateJobInfoOfOrderInRedisAsync(job.OrderCode).ConfigureAwait(false);

            return true;
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            throw new EcsDomainException(ex.Message);
        }
    }

    /// <summary>
    /// 批量添加作业：
    /// </summary>
    /// <param name="jobs"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public async Task<bool?> AddDispatchJobsAsync(List<DispatchJob> jobs)
    {
        if (jobs.Count == 0)
            throw new ArgumentOutOfRangeException(nameof(jobs), "没有需要增加的Job");

        try
        {
            foreach (var job in jobs)
            {
                //同一个调度任务下的某一个命令只能有一个
                List<DispatchJob> js = jobs.Where(o => o.TaskId == job.TaskId && o.ProcessSequence == job.ProcessSequence).ToList();
                if (js.Count > 1) //重复了
                    throw new ArgumentException($"存在多个TaskId为{job.TaskId},PathStep为{job.ProcessSequence}的job");

                js = await _jobRepository.GetListAsync(o => o.TaskId == job.TaskId && o.ProcessSequence == job.ProcessSequence).ConfigureAwait(false);
                if (js.Count > 0) //重复了
                    throw new Exception($"数据库中已经存在TaskId为{job.TaskId},PathStep为{job.ProcessSequence}的job");
            }

            using (var unit = _unitOfWorkManager.Begin(isTransactional: true))
            {
                foreach (var job in jobs)
                    await _jobRepository.InsertAsync(job).ConfigureAwait(false);

                await unit.CompleteAsync().ConfigureAwait(false);
            }

            await _backupManager.UpdateJobInfoOfOrderInRedisAsync(jobs[0].OrderCode).ConfigureAwait(false);

            return true;
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            return null;
        }
    }

    /// <summary>
    /// 删除任务相关作业
    /// </summary>
    /// <param name="taskId"></param>
    /// <returns></returns>
    /// <exception cref="EcsDomainException"></exception>
    public async Task<bool> RemoveJobsOfTaskAsync(int taskId)
    {
        try
        {
            List<DispatchJob> jobs = await _jobRepository.GetListAsync(o => o.TaskId == taskId).ConfigureAwait(false);
            if (jobs.Count == 0) //没有对应的job，默认删除成功
                return true;

            using (var unit = _unitOfWorkManager.Begin(isTransactional: true))
            {
                foreach (var job in jobs)
                    await _jobRepository.DeleteAsync(job).ConfigureAwait(false);

                await unit.CompleteAsync().ConfigureAwait(false);
            }

            await _backupManager.UpdateJobInfoOfOrderInRedisAsync(jobs[0].OrderCode).ConfigureAwait(false);

            return true;
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            throw new EcsDomainException(ex.Message);
        }
    }


    /// <summary>
    /// 更新单个作业状态：
    /// </summary>
    /// <param name="jobId"></param>
    /// <param name="newState"></param>
    /// <returns></returns>
    public async Task<bool?> UpdateJobStateAsync(int jobId, EnumDispatchJobState newState)
    {
        try
        {
            List<DispatchJob> jobs = await _jobRepository.GetListAsync(o => o.Id == jobId).ConfigureAwait(false);

            if (jobs.Count == 0) //不存在jobId
                return false;

            jobs[0].State = newState;
            await _jobRepository.UpdateAsync(jobs[0]).ConfigureAwait(false);

            await _backupManager.UpdateJobStateOfOrderInRedisAsync(jobs[0].OrderCode, jobId, newState.ToString()).ConfigureAwait(false);

            return true;
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            return null;
        }
    }

    /// <summary>
    /// 更新指定任务中，所有流程步骤小于指定步骤的作业状态
    /// </summary>
    /// <param name="taskId"></param>
    /// <param name="pathStep"></param>
    /// <param name="newState"></param>
    /// <returns></returns>
    public async Task<bool?> UpdateJobsStateOfPathStepLessThanAsync(int taskId, int pathStep, EnumDispatchJobState newState)
    {
        try
        {
            var jobs = await _jobRepository.GetListAsync(o => o.TaskId == taskId && o.ProcessSequence < pathStep).ConfigureAwait(false);

            if (jobs.Count == 0) //taskId下不存在命令小于pathStep的Job
                throw new Exception("taskId为{taskId}，pathStep小于{pathStep}的Jobs不存在");

            using (var unit = _unitOfWorkManager.Begin(isTransactional: true))
            {
                foreach (DispatchJob job in jobs)
                {
                    job.State = newState;
                    await _jobRepository.UpdateAsync(job).ConfigureAwait(false);
                }
                await unit.CompleteAsync().ConfigureAwait(false);
            }

            foreach (DispatchJob job in jobs)
            {
                await _backupManager.UpdateJobStateOfOrderInRedisAsync(job.OrderCode, job.Id, newState.ToString()).ConfigureAwait(false);
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
    /// 批量更新作业状态：
    /// </summary>
    /// <param name="taskId"></param>
    /// <param name="state"></param>
    /// <returns></returns>
    public async Task<bool?> UpdateJobsStateAsync(int taskId, EnumDispatchJobState state)
    {
        try
        {
            List<DispatchJob> jobs = await _jobRepository.GetListAsync(o => o.TaskId == taskId).ConfigureAwait(false);

            if (jobs.Count == 0) //taskId下不存在Job
                return false;

            using (var unit = _unitOfWorkManager.Begin(isTransactional: true))
            {
                foreach (DispatchJob job in jobs)
                {
                    job.State = state;
                    await _jobRepository.UpdateAsync(job).ConfigureAwait(false);
                }
                await unit.CompleteAsync().ConfigureAwait(false);
            }

            foreach (DispatchJob job in jobs)
            {
                await _backupManager.UpdateJobStateOfOrderInRedisAsync(job.OrderCode, job.Id, state.ToString()).ConfigureAwait(false);
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
    /// 获取任务的所有作业：
    /// </summary>
    /// <param name="taskId"></param>
    /// <returns></returns>
    public async Task<List<DispatchJob>> GetAllJobsOfTaskAsync(int taskId)
    {
        try
        {
            var jobs = await _jobRepository.GetListAsync(o => o.TaskId == taskId).ConfigureAwait(false);
            return jobs.OrderBy(o => o.ProcessSequence).ToList();
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            return null;
        }
    }

    /// <summary>
    /// 获取特定作业：
    /// </summary>
    /// <param name="taskId"></param>
    /// <param name="processSequence"></param>
    /// <returns></returns>
    /// <exception cref="EcsDomainException"></exception>
    public async Task<DispatchJob> GetDispatchJobAsync(int taskId, int processSequence)
    {
        try
        {
            var jobs = await _jobRepository.GetListAsync(
                o => o.TaskId == taskId &&
                o.ProcessSequence == processSequence)
                .ConfigureAwait(false);

            if (jobs.Count == 0)
                return null;

            if (jobs.Count > 1)
                throw new Exception($"taskId为{taskId}, processSequence为{processSequence}的Job数量不止1个");

            return jobs[0];
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            throw new EcsDomainException(ex.Message);
        }
    }

    /// <summary>
    /// 添加JobCmd，一般用于种子数据 添加作业命令
    /// </summary>
    /// <param name="step"></param>
    /// <returns></returns> 
    public async Task<bool> AddJobCmdAsync(DispatchJobCmd jobCmd)
    {
        try
        {
            var steps = await _cmdRepository.GetListAsync(
                o => o.JobCmdClassName == jobCmd.JobCmdClassName).ConfigureAwait(false);
            if (steps.Count > 0)
                throw new Exception($"{jobCmd.JobCmdClassName}已经存在");
            await _cmdRepository.InsertAsync(jobCmd).ConfigureAwait(false);
            return true;
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            return false;
        }
    }

    public async Task<bool> DelAllJobCmdsAsync()
    {
        try
        {
            await _cmdRepository.DeleteAsync(o => o.Id > 0).ConfigureAwait(false);
            return true;
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            return false;
        }
    }

    public async Task<List<DispatchJobCmd>> GetAllJobCmdsAsync()
    {
        try
        {
            var cmds = await _cmdRepository.GetListAsync().ConfigureAwait(false);
            return cmds.OrderBy(o => o.Id).ToList();
        }
        catch (Exception e)
        {
            _logger.Error(e.Message);
            return new List<DispatchJobCmd>();
        }
    }

    /// <summary>
    /// 根据JobCmd的Id查询命令信息 获取命令信息
    /// </summary>
    /// <param name="jobCmdId"></param>
    /// <returns>未找到，返回string.Empty，发生错误，返回null</returns>
    public async Task<string> GetJobCmdClassNameAsync(int jobCmdId)
    {
        try
        {
            var step = await _cmdRepository.GetAsync(jobCmdId).ConfigureAwait(false);
            return step.JobCmdClassName;
        }
        catch (Exception e)
        {
            _logger.Error(e.Message);
            return null;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="jobCmdId"></param>
    /// <returns>未找到或发生错误，返回null</returns>
    public async Task<DispatchJobCmd> GetJobCmdAsync(int jobCmdId)
    {
        try
        {
            return await _cmdRepository.GetAsync(jobCmdId).ConfigureAwait(false);
        }
        catch (Exception e)
        {
            _logger.Error(e.Message);
            return null;
        }
    }

    /// <summary>
    /// 根据Id获取Worker名称
    /// </summary>
    /// <param name="jobWorkerId"></param>
    /// <returns>未找到，或发生错误，返回null</returns>
    public async Task<string> GetJobWorkerClassNameAsync(int jobWorkerId)
    {
        try
        {
            var worker = await _workerRepository.GetAsync(jobWorkerId).ConfigureAwait(false);
            return worker.JobWorkerClassName;
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            return null;
        }
    }

    /// <summary>
    /// 添加作业执行器：
    /// </summary>
    /// <param name="worker"></param>
    /// <returns></returns>
    public async Task<bool> AddJobWorkerAsync(DispatchJobWorker worker)
    {
        try
        {
            var workers = await _workerRepository.GetListAsync(o => o.JobWorkerClassName == worker.JobWorkerClassName).ConfigureAwait(false);
            if (workers.Count > 0)
                return false;
            await _workerRepository.InsertAsync(worker).ConfigureAwait(false);
            return true;
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            return false;
        }
    }

    public async Task<bool> DelAllJobWorkersAsync()
    {
        try
        {
            await _workerRepository.DeleteAsync(o => o.Id > 0).ConfigureAwait(false);
            return true;
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            return false;
        }
    }

    /// <summary>
    /// 获取执行器信息：
    /// </summary>
    /// <returns></returns>
    public async Task<List<DispatchJobWorker>> GetAllJobWorkersAsync()
    {
        try
        {
            var workers = await _workerRepository.GetListAsync().ConfigureAwait(false);
            return workers.OrderBy(o => o.Id).ToList();
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            return new List<DispatchJobWorker>();
        }
    }

    /// <summary>
    /// 获取下一个分配给Job的Id
    /// </summary>
    /// <returns>若发生错误，返回-1</returns>
    public async Task<int> GetNextJobIdAsync()
    {
        try
        {
            List<DispatchJobId> Ids = await _jobIdRepository.GetListAsync().ConfigureAwait(false);
            if (Ids.Count == 0)
            {
                await _jobIdRepository.InsertAsync(new DispatchJobId() { JobId = 1 }).ConfigureAwait(false);
                return 1;
            }
            else
            {
                int jobId = Ids[0].JobId;
                if (jobId + 1 == 60000) //PLC会发生溢出
                    jobId = 1;
                else
                    jobId = jobId + 1;

                Ids[0].JobId = jobId;
                await _jobIdRepository.UpdateAsync(Ids[0]).ConfigureAwait(false);
                return jobId;
            }
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            return -1;
        }
    }
}