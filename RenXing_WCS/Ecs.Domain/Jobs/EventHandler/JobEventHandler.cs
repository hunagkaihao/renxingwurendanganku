using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Ecs.Jobs;
using Ecs.Jobs.Etos;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus;
using Volo.Abp.Uow;

namespace Ecs.Jobs.EventHandler;

public class JobEventHandler :
    ILocalEventHandler<RemoveJobsOfTaskEvent>,
    ITransientDependency

{
    private readonly JobManager _jobManager;

    public JobEventHandler(
        JobManager jobManager)
    {
        _jobManager = jobManager;
    }

    [UnitOfWork]
    public virtual async Task HandleEventAsync(RemoveJobsOfTaskEvent eventData)
    {
        int taskId = eventData.TaskId;
        bool ret = await _jobManager.RemoveJobsOfTaskAsync(taskId).ConfigureAwait(false);
        if (!ret)
            throw new Exception($"{nameof(RemoveJobsOfTaskEvent)}事件执行失败");
    }
}