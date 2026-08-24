using System;
using System.Threading.Tasks;
using Ecs.Tasks;
using Ecs.Tasks.Etos;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus;
using Volo.Abp.Uow;

namespace Ecs.Tasks.EventHandler;

public class TaskEventsHandler :
    ILocalEventHandler<RemoveTaskOfOrderEvent>,
    ITransientDependency

{
    private TaskManager _taskManager;

    public TaskEventsHandler(
        TaskManager taskManager)
    {
        _taskManager = taskManager;
    }

    public virtual async Task HandleEventAsync(RemoveTaskOfOrderEvent eventData)
    {
        string orderCode = eventData.OrderCode;
        bool ret = await _taskManager.RemoveDispatchTaskAsync(orderCode).ConfigureAwait(false);
        if (!ret)
            throw new Exception($"{nameof(RemoveTaskOfOrderEvent)}事件执行失败"); //这里需要抛出事件，告诉事件发布者，事件执行失败
    }
}