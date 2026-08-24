using Ecs.Backups;
using Ecs.Cells;
using Ecs.Cells.Models;
using Ecs.ConfigTool;
using Ecs.Errors;
using Ecs.Jobs;
using Ecs.Jobs.JobCmds;
using Ecs.Jobs.JobWorker;
using Ecs.Jobs.Models;
using Ecs.LogTool;
using Ecs.Nodes;
using Ecs.Nodes.Models;
using Ecs.Notifiers;
using Ecs.Orders;
using Ecs.Orders.Models;
using Ecs.Processes;
using Ecs.RedisTool;
using Ecs.SignalRTool;
using Ecs.Tasks;
using Ecs.Tasks.Models;
using Ecs.WMS;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Newtonsoft.Json;

namespace Ecs.Dispatch;

/// <summary>
/// 调度服务
/// </summary>
public class DispatchCoreJob : IHostedService, IDisposable
{
    private readonly ILogger<DispatchCoreJob> _logger;
    private readonly IOptions<ConfigOptions> _options;
    private readonly OrderManager _orderManager;
    private readonly TaskManager _taskManager;
    private readonly JobManager _jobManager;
    private readonly ProcessManager _processManager;
    private readonly NodeManager _nodeManager;
    private readonly ICellRepository _cellRepository;
    private readonly BackupManager _backupManager;
    private readonly ErrorManager _errorManager;
    private readonly NotifierManager _notifierManager;
    private readonly JobWorkerFactory _jobWorkerFactory;
    private readonly JobCmdFactory _jobCmdFactory;
    private readonly IRedisClient _ecsRedisClient;
    private readonly HubMsgQHelper _hubHelper;
    private readonly IWMSService _wmsService;

    private object mLocker = new object();

    private string mDispatchServerName = string.Empty;
    public string DispatchServerName
    {
        get => mDispatchServerName;
        set => mDispatchServerName = value;
    }

    /// <summary>
    /// 调度系统的状态，如运行中，暂停中
    /// </summary>
    private string serverState = string.Empty;
    public string ServerState
    {
        get
        {
            lock (mLocker)
            {
                return serverState;
            }
        }
        set
        {
            lock (mLocker)
            {
                serverState = value;
            }
        }
    }

    /// <summary>
    /// 设备的工作状态，如入库中，出库中，空闲中等
    /// </summary>
    private EnumDispatchDeviceState deviceState = EnumDispatchDeviceState.Idle;
    public EnumDispatchDeviceState DeviceState
    {
        get
        {
            lock (mLocker)
            {
                return deviceState;
            }
        }
        set
        {
            lock (mLocker)
            {
                deviceState = value;
            }
        }
    }


    /// <summary>
    /// 调度同时处理的最大任务数量
    /// </summary>
    private int mMaxTaskHandlingNum;


    /// <summary>
    /// 排序后的调度任务Id集合
    /// </summary>
    private List<int> mTaskIdSequence; //排序后的调度任务Id


    private readonly object mTaskDicLocker = new object();
    /// <summary>
    /// key：调度任务Id，value：调度任务
    /// </summary>
    private Dictionary<int, DispatchTask> mTaskDic;

    private Dictionary<int, DispatchTask> TaskDic
    {
        get
        {
            lock (mTaskDicLocker)
            {
                return mTaskDic;
            }
        }
        set
        {
            lock (mTaskDicLocker)
            {
                mTaskDic = value;
                string json = JsonConvert.SerializeObject(mTaskDic);
                _ecsRedisClient.SetStringValue(EcsConsts.DispatchTasksDoing, json);
            }
        }
    }

    private void TaskDicAdd(DispatchTask task)
    {
        lock (mTaskDicLocker)
        {
            mTaskDic.Add(task.Id, task);
            string json = JsonConvert.SerializeObject(mTaskDic);
            _ecsRedisClient.SetStringValue(EcsConsts.DispatchTasksDoing, json);
        }
    }

    private void TaskDicRemove(int id)
    {
        lock (mTaskDicLocker)
        {
            mTaskDic.Remove(id);
            string json = JsonConvert.SerializeObject(mTaskDic);
            _ecsRedisClient.SetStringValue(EcsConsts.DispatchTasksDoing, json);
        }
    }

    private void TaskDicRemoveAll()
    {
        lock (mTaskDicLocker)
        {
            mTaskDic.Clear();
            string json = JsonConvert.SerializeObject(mTaskDic);
            _ecsRedisClient.SetStringValue(EcsConsts.DispatchTasksDoing, json);
        }
    }


    /// <summary>
    /// key：调度任务Id，value：属于该任务的按顺序排列的JobId与Job键值对集合
    /// </summary>
    private Dictionary<int, Dictionary<int, DispatchJob>> mJobsDic;


    /// <summary>
    /// key：调度任务Id，value：调度任务当前执行的JobWorker
    /// </summary>
    private Dictionary<int, IJobWorker> mWorkerDic;


    private CancellationTokenSource mCancelSource;
    private CancellationToken mCancelToken;

    public DispatchCoreJob(
        ILogger<DispatchCoreJob> logger,
        IOptions<ConfigOptions> options,
        OrderManager orderManager,
        BackupManager backupManager,
        TaskManager taskManager,
        NodeManager nodeManager,
        JobManager jobManager,
        ProcessManager processManager,
        ICellRepository cellRepository,
        JobWorkerFactory jobWorkerFactory,
        JobCmdFactory jobCmdFactory,
        IRedisClient redisClient,
        ErrorManager errorManager,
        NotifierManager notifierManager,
        HubMsgQHelper hubHelper,
        IWMSService wmsService)
    {
        _logger = logger;
        _options = options;
        _orderManager = orderManager;
        _backupManager = backupManager;
        _taskManager = taskManager;
        _nodeManager = nodeManager;
        _jobManager = jobManager;
        _processManager = processManager;
        _cellRepository = cellRepository;
        _jobWorkerFactory = jobWorkerFactory;
        _jobCmdFactory = jobCmdFactory;
        _errorManager = errorManager;
        _notifierManager = notifierManager;
        _hubHelper = hubHelper;
        _ecsRedisClient = redisClient;
        _wmsService = wmsService;
        _ecsRedisClient.Build(_options.Value.RedisConnStr, _options.Value.DefaultRedisNo);

        mMaxTaskHandlingNum = _options.Value.DispatchTaskMaxHandlingNum;
        mTaskIdSequence = new List<int>();
        TaskDic = new Dictionary<int, DispatchTask>();
        mJobsDic = new Dictionary<int, Dictionary<int, DispatchJob>>();
        mWorkerDic = new Dictionary<int, IJobWorker>();

        _errorManager.RemoveAllErrInfoOfDispatchSvr();
        DispatchServerName = _options.Value.DispatchServerName;

        mCancelSource = new CancellationTokenSource();
        mCancelToken = mCancelSource.Token;
    }

    public void Dispose()
    {

    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        Task.Run(async () =>
        {
            await Task.Delay(3000).ConfigureAwait(false);
            //autofac不能在创建对象过程中再次创建别的对象
            await Initialize().ConfigureAwait(false);
            await Execute().ConfigureAwait(false);
        });
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        StopExecute();
        return Task.CompletedTask;
    }

    /// <summary>
    /// 执行任务调度
    /// </summary>
    /// <returns></returns>
    public async Task Execute()
    {
        await Task.Run(async () =>
        {
            while (true)
            {
                if (mCancelToken.IsCancellationRequested)
                    break;

                await Task.Delay(100).ConfigureAwait(false);

                await ListenToForceDoneOrderRequest().ConfigureAwait(false);

                await ListenToCancelTaskRequest().ConfigureAwait(false);

                ServerState = await GetServerState().ConfigureAwait(false);
                if (ServerState == "Pause")
                {
                    await _errorManager.RemoveAllErrInfoOfDispatchSvrAsync().ConfigureAwait(false);
                    continue;
                }

                //[1]
                //查询新的订单，转换成调度任务后保存
                await OrderToTask().ConfigureAwait(false);
                //将新增的调度任务分解成Jobs并保存，即使没有加载新的任务，也执行，防止前一次Job创建出现错误导致有些Job没有创建出来    
                int num = await TaskToJobs().ConfigureAwait(false);
                //当新增了Job时，将新增的任务和Jobs加载到内存，并对调度任务进行重新排序，新加载的task以及第一个Job状态为WaitingDo
                if (num > 0) await LoadAndSortTask().ConfigureAwait(false);

                //[2]
                //没有任务，返回
                if (mTaskIdSequence.Count == 0)
                {
                    await _errorManager.RemoveAllErrInfoOfDispatchSvrAsync().ConfigureAwait(false);
                    continue;
                }
                //若调度任务数量超过最大处理量时，多余的任务暂时不处理，防止太多无效的操作
                int handlingNum = mTaskIdSequence.Count > mMaxTaskHandlingNum ? mMaxTaskHandlingNum : mTaskIdSequence.Count;

                //[3]
                //按照顺序为每个JobWorker分配资源
                AllocatedResult[] allocRslts = new AllocatedResult[handlingNum];
                bool errHappened = false;
                for (int i = 0; i < handlingNum; i++)
                {
                    //可分配的资源，即所有的节点
                    Dictionary<string, DispatchNode> nodeDic = await _nodeManager.GetAllNodesAsync().ConfigureAwait(false);
                    if (nodeDic == null)
                    {
                        errHappened = true;
                        await _errorManager.UpdateErrInfoOfDispatchSvrAsync($"AllocateResource.{i}", "未配置节点资源").ConfigureAwait(false);
                        continue;
                    }
                    int taskId = mTaskIdSequence[i];
                    allocRslts[i] = await AllocateResource(taskId, nodeDic).ConfigureAwait(false);
                    if (!string.IsNullOrEmpty(allocRslts[i].ErrInfo))
                    {
                        errHappened = true;
                        await _errorManager.UpdateErrInfoOfDispatchSvrAsync($"AllocateResource.{i}", allocRslts[i].ErrInfo!).ConfigureAwait(false);
                    }
                }
                if (!errHappened)
                {
                    for (int i = 0; i < handlingNum; i++)
                        await _errorManager.RemoveErrInfoOfDispatchSvrAsync($"AllocateResource.{i}").ConfigureAwait(false);
                }

                //[4]
                //执行任务
                Task[] ts = new Task[handlingNum];
                for (int i = 0; i < handlingNum; i++)
                {
                    if (!mWorkerDic.ContainsKey(mTaskIdSequence[i]))
                        continue;
                    ts[i] = mWorkerDic[mTaskIdSequence[i]].Execute();
                }
                Task.WaitAll(ts);

                //[5]
                //执行任务后，判断是否需要切换下一个Job，以及释放资源和刷新Task和Job的状态
                Task<string>[] tasks = new Task<string>[handlingNum];
                for (int i = 0; i < handlingNum; i++)
                {
                    tasks[i] = UpdateStateAndToNextJob(mTaskIdSequence[i]);
                }
                Task.WaitAll(tasks);

                errHappened = false;
                for (int i = 0; i < tasks.Length; i++)
                {
                    if (tasks[i].Result != string.Empty)
                    {
                        errHappened = true;
                        await _errorManager.UpdateErrInfoOfDispatchSvrAsync($"UpdateStateAndToNextJob.{i}", tasks[i].Result).ConfigureAwait(false);
                    }
                }
                if (!errHappened)
                {
                    for (int i = 0; i < tasks.Length; i++)
                        await _errorManager.RemoveErrInfoOfDispatchSvrAsync($"UpdateStateAndToNextJob.{i}").ConfigureAwait(false);
                }
            }
        });
    }

    /// <summary>
    /// 初始化调度任务，加载所有的任务
    /// </summary>
    /// <returns></returns>
    private async Task Initialize()
    {
        //初始化订单查询信息
        await _backupManager.InitializeOrdersInRedisAsync().ConfigureAwait(false);

        List<DispatchTask> tasks = await _taskManager.GetAllDispatchTasksAsync().ConfigureAwait(false);
        if (tasks == null || tasks.Count == 0)
            return;

        List<int> taskIdsToRemove = new List<int>();
        foreach (var task in tasks)
        {
            List<DispatchJob> jobs = await _jobManager.GetAllJobsOfTaskAsync(task.Id).ConfigureAwait(false);
            if (jobs == null || jobs.Count == 0) //没有jobs的Task不做处理，待删除
            {
                taskIdsToRemove.Add(task.Id);
                string info = $"OrderCode:{task.OrderCode}，TaskId:{task.Id}，该任务没有Job数据，此任务即将被移除";
                _logger.Info(info);
                continue;
            }
            Dictionary<int, DispatchJob> jobDic = new Dictionary<int, DispatchJob>();
            foreach (var j in jobs)
            {
                jobDic.Add(j.Id, j);
            }

            //寻找第一个没有完成的Job，生成Worker
            DispatchJob firstUnDoneJob = null;
            foreach (var job in jobs)
            {
                if (job.State != EnumDispatchJobState.Done &&
                    job.State != EnumDispatchJobState.ForceDone &&
                    job.State != EnumDispatchJobState.Canceled)
                {
                    firstUnDoneJob = job;
                    break;
                }
            }
            if (firstUnDoneJob == null) //所有Job都已完成，任务待删除
            {
                taskIdsToRemove.Add(task.Id);
                string info = $"OrderCode:{task.OrderCode}，TaskId:{task.Id}，该任务所有Job都已完成，此任务即将被移除";
                _logger.Info(info);
                continue;
            }

            string workerName = await _jobManager.GetJobWorkerClassNameAsync(firstUnDoneJob.JobWorkerId).ConfigureAwait(false);
            if (string.IsNullOrEmpty(workerName)) //配置问题
            {
                string info = $"OrderCode:{task.OrderCode}，TaskId:{task.Id}，Id为{firstUnDoneJob.JobWorkerId}的Job没有正确配置JobWorkerName，该任务无法初始化";
                _logger.Error(info);
                continue;
            }

            IJobWorker worker = _jobWorkerFactory.CreateJobWorker(workerName);
            if (worker == null)
            {
                string info = $"OrderCode:{task.OrderCode}，TaskId:{task.Id}，Id为{firstUnDoneJob.JobWorkerId}的Job创建JobWorker失败，该任务无法初始化";
                _logger.Error(info);
                continue;
            }

            // string cmdName = await _dispatchService.GetJobCmdNameAsync(firstUnDoneJob.JobCmdVal).ConfigureAwait(false);
            DispatchJobCmd jobCmd = await _jobManager.GetJobCmdAsync(firstUnDoneJob.JobCmdId).ConfigureAwait(false);
            if (jobCmd == null) //配置问题
            {
                string info = $"OrderCode:{task.OrderCode}，TaskId:{task.Id}，Id为{firstUnDoneJob.JobWorkerId}的Job没有正确配置JobCmd，该任务无法初始化";
                _logger.Error(info);
                continue;
            }

            IJobCmd jobCmdObj = _jobCmdFactory.CreateStep(jobCmd.JobCmdClassName);
            if (jobCmdObj == null)
            {
                string info = $"OrderCode:{task.OrderCode}，TaskId:{task.Id}，Id为{firstUnDoneJob.JobWorkerId}的Job创建JobCmd对象失败，该任务无法初始化";
                _logger.Error(info);
                continue;
            }

            worker.MyJob = new DispatchJob(firstUnDoneJob);
            jobCmdObj.JobCmdNameCHS = jobCmd.Describe ?? "";
            worker.MyJobCmd = jobCmdObj;
            jobCmdObj.Owner = worker;
            TaskDicAdd(task);
            mJobsDic.Add(task.Id, jobDic);
            mWorkerDic.Add(task.Id, worker);
        }

        // mTaskIdSequence = await _dispatchStrategy.SortDispatchTasks(mTaskDic.Keys.ToList()).ConfigureAwait(false)
        mTaskIdSequence = TaskDic.Keys.ToList().OrderBy(o => o).ToList();

        //TaskDic也重新排序
        Dictionary<int, DispatchTask> temp = new Dictionary<int, DispatchTask>();
        foreach (var item in TaskDic)
        {
            temp.Add(item.Key, item.Value);
        }
        TaskDicRemoveAll();
        foreach (int id in mTaskIdSequence)
        {
            TaskDicAdd(temp[id]);
        }
    }

    /// <summary>
    /// 查询新增的调度订单，转换成调度任务并保存，被转换的订单状态=>Readed，新生成的调度任务状态=>Created
    /// </summary>
    /// <returns>下载的调度任务数量</returns>
    private async Task<int> OrderToTask()
    {
        List<DispatchOrder> orders = null;
        string strategy = _options.Value.DiaptchStrategy;
        if (strategy == "Mix") //各种任务类型混合执行
        {
            //查询新增的的调度任务
            orders = await _orderManager.GetAllDispatchOrdersToDoAsync().ConfigureAwait(false);
        }
        else if (strategy == "Apart") //任务类型分开执行
        {
            if (TaskDic.Count == 0) //当前没有任务在执行，则寻找第一个需要执行的任务的类型，并查询该类型的所有订单
            {
                DispatchOrder order = await _orderManager.GetFirstDispatchOrderToDoAsync().ConfigureAwait(false);
                if (order == null)
                    return 0;
                orders = await _orderManager.GetAllDispatchOrdersToDoWithTypeAsync(order.OrderType).ConfigureAwait(false);
            }
            else //当前存在执行任务，根据执行任务类型查询订单
            {
                string orderCode = TaskDic.First().Value.OrderCode;
                DispatchOrder order = await _orderManager.GetDispatchOrderByOrderCodeAsync(orderCode).ConfigureAwait(false);
                if (order == null)
                {
                    _errorManager.UpdateErrInfoOfDispatchSvr("OrderToTask", $"当前正在执行的第一个任务对应订单号为{orderCode}，但根据此订单号，查询不到订单信息");
                    return 0;
                }
                orders = await _orderManager.GetAllDispatchOrdersToDoWithTypeAsync(order.OrderType).ConfigureAwait(false);
            }
        }
        else
        {
            _errorManager.UpdateErrInfoOfDispatchSvr("OrderToTask", $"调度策略设置错误，当前为{strategy}，应为Mix或Apart");
            return 0;
        }


        if (orders == null || orders.Count == 0)   //没有新增的订单
            return 0;

        int numLoaded = 0;
        foreach (DispatchOrder o in orders)
        {
            bool ret = await OrderToTaskAndSaveAsync(o).ConfigureAwait(false);
            if (ret == false)
            {
                _errorManager.UpdateErrInfoOfDispatchSvr("OrderToTask", $"OrderCode为{o.OrderCode}的调度订单转换成调度任务并保存失败，具体信息请查看错误日志");
                return numLoaded;
            }
            string info = $"接收到订单{o.OrderCode}（物流起点为{o.StartNode}，物流终点为{o.EndNode}，物料载体码为{o.PlateCode}），已成功转换成调度任务";
            _logger.Info(info);
            ++numLoaded;
        }

        _errorManager.RemoveErrInfoOfDispatchSvr("OrderToTask"); //没有发生错误，删除该错误标记

        return numLoaded;
    }

    /// <summary>
    /// 将新增的调度任务转换成jobs，并保存，被转换的调度任务状态=>ToJobs，新增的Job状态=>Created
    /// </summary>
    /// <returns>成功转换成Jobs并保存jobs的调度任务数量</returns>
    private async Task<int> TaskToJobs()
    {
        List<DispatchTask> tasks = await _taskManager.GetDispatchTasksWithStateAsync(EnumDispatchTaskState.Created).ConfigureAwait(false);
        if (tasks == null || tasks.Count == 0)
            return 0;

        int taskNumToJobs = 0;
        foreach (var t in tasks)
        {
            bool ret = await TaskToJobsAndSaveAsync(t).ConfigureAwait(false);
            if (ret == false)
            {
                _errorManager.UpdateErrInfoOfDispatchSvr("TaskToJobs", $"Id为{t.Id}的调度任务转换成Jobs并保存失败，具体信息请查看错误日志");
                return taskNumToJobs;
            }
            string info = $"调度任务{t.Id}（对应订单{t.OrderCode}，起点{t.StartNode}，终点{t.EndNode}，物料载体码{t.PlateCode}，过程{t.ProcessId}，优先级{t.Priority}），已成功分解出Jobs";
            _logger.Info(info);
            ++taskNumToJobs;
        }

        _errorManager.RemoveErrInfoOfDispatchSvr("TaskToJobs"); //没有发生错误，删除该错误标记

        return taskNumToJobs;
    }

    /// <summary>
    /// 载入新增的调度任务和Jobs，并重新排序，载入后的调度任务状态=>WaitingDo，载入后的第一个Job状态=>WaitingDo，其余为Created
    /// </summary>
    /// <returns></returns>
    private async Task LoadAndSortTask()
    {
        var tasks = await _taskManager.GetDispatchTasksWithStateAsync(EnumDispatchTaskState.ToJobs).ConfigureAwait(false);
        if (tasks == null || tasks.Count == 0)  //没有可以下载的任务
            return;

        int numLoaded = 0;
        foreach (var t in tasks)
        {
            if (TaskDic.Keys.Contains(t.Id)) //调度任务已经加载
            {
                string info = $"调度任务{t.Id}（对应订单{t.OrderCode}），已加载，但任务状态仍为ToJobs，而非WaitingDo";
                _errorManager.UpdateErrInfoOfDispatchSvr("LoadAndSortTask", info);
                break;
            }

            var jobs = await _jobManager.GetAllJobsOfTaskAsync(t.Id).ConfigureAwait(false);
            if (jobs == null || jobs.Count == 0) //没有查到调度任务相应的Jobs
            {
                string info = $"调度任务{t.Id}（对应订单{t.OrderCode}）没有Jobs数据";
                _errorManager.UpdateErrInfoOfDispatchSvr("LoadAndSortTask", info);
                break;
            }

            DispatchJob firstJob = jobs[0];

            string workerName = await _jobManager.GetJobWorkerClassNameAsync(firstJob.JobWorkerId).ConfigureAwait(false);
            if (string.IsNullOrEmpty(workerName)) //配置问题
            {
                string info = $"调度任务{t.Id}（对应订单{t.OrderCode}），查询该任务下第一个Job的Worker名称失败，无法创建Worker";
                _errorManager.UpdateErrInfoOfDispatchSvr("LoadAndSortTask", info);
                break;
            }

            IJobWorker worker = _jobWorkerFactory.CreateJobWorker(workerName);
            if (worker == null)
            {
                string info = $"调度任务{t.Id}（对应订单{t.OrderCode}），创建该任务下第一个Job的Worker失败，Worker名为{workerName}";
                _errorManager.UpdateErrInfoOfDispatchSvr("LoadAndSortTask", info);
                break;
            }

            //string cmdName = await _dispatchService.GetJobCmdNameAsync(firstJob.JobCmdVal).ConfigureAwait(false);
            DispatchJobCmd jobCmd = await _jobManager.GetJobCmdAsync(firstJob.JobCmdId).ConfigureAwait(false);
            if (jobCmd == null) //配置问题
            {
                string info = $"调度任务{t.Id}（对应订单{t.OrderCode}），查询该任务下第一个Job的JobCmd失败，无法创建Step对象";
                _errorManager.UpdateErrInfoOfDispatchSvr("LoadAndSortTask", info);
                break;
            }

            IJobCmd jobCmdObj = _jobCmdFactory.CreateStep(jobCmd.JobCmdClassName);
            if (jobCmdObj == null)
            {
                string info = $"调度任务{t.Id}（对应订单{t.OrderCode}），创建该任务下第一个Job的JobCmd失败，JobCmd类名为{jobCmd.JobCmdClassName}";
                _errorManager.UpdateErrInfoOfDispatchSvr("LoadAndSortTask", info);
                break;
            }

            bool? ret = await SetFirstJobOfSomeTaskWaitingToDoAsync(t.Id, firstJob.Id).ConfigureAwait(false);
            if (ret == null || ret == false)
            {
                string info = $"调度任务{t.Id}（对应订单{t.OrderCode}），更新该任务以及该任务下的第一个Job状态为WaitingDo失败，第一个Job的Id为{firstJob.Id}";
                _errorManager.UpdateErrInfoOfDispatchSvr("LoadAndSortTask", info);
                break;
            }

            t.State = EnumDispatchTaskState.WaitingDo;
            TaskDic.Add(t.Id, t);

            jobs[0].State = EnumDispatchJobState.WaitingDo;
            Dictionary<int, DispatchJob> jobPairs = new Dictionary<int, DispatchJob>();
            foreach (var job in jobs)
            {
                jobPairs.Add(job.Id, job);
            }
            mJobsDic.Add(t.Id, jobPairs);

            worker.MyJob = new DispatchJob(jobs[0]);
            jobCmdObj.JobCmdNameCHS = jobCmd.Describe ?? "";
            worker.MyJobCmd = jobCmdObj;
            jobCmdObj.Owner = worker;
            mWorkerDic.Add(t.Id, worker);

            await _orderManager.UpdateExecStepOfDispatchOrderAsync(worker.MyJob.OrderCode, jobCmdObj.JobCmdNameCHS).ConfigureAwait(false);

            string log = $"调度任务{t.Id}（对应订单{t.OrderCode}，起点{t.StartNode}，终点{t.EndNode}，物料载体码{t.PlateCode}，过程{t.ProcessId}，优先级{t.Priority}），已进入执行队列";
            _logger.Info(log);

            ++numLoaded;
        }

        if (numLoaded == tasks.Count) //没有发生错误
            _errorManager.RemoveErrInfoOfDispatchSvr("LoadAndSortTask");

        if (numLoaded == 0)   //没有载入新调度任务，不需要再进行排序
            return;

        //重新排序
        //mTaskIdSequence = await _dispatchStrategy.SortDispatchTasks(mTaskDic.Keys.ToList()).ConfigureAwait(false);
        mTaskIdSequence = TaskDic.Keys.ToList().OrderBy(o => o).ToList();

        //TaskDic也重新排序
        Dictionary<int, DispatchTask> temp = new Dictionary<int, DispatchTask>();
        foreach (var item in TaskDic)
        {
            temp.Add(item.Key, item.Value);
        }
        TaskDicRemoveAll();
        foreach (int id in mTaskIdSequence)
        {
            TaskDicAdd(temp[id]);
        }
    }

    /// <summary>
    /// 刷新调度任务及其Jobs的状态，以及切换Job
    /// </summary>
    /// <returns>发生的错误信息，若无错误，返回空字符串</returns>
    private async Task<string> UpdateStateAndToNextJob(int taskId)
    {
        if (!mWorkerDic.ContainsKey(taskId) ||
            !mJobsDic.ContainsKey(taskId) ||
            !TaskDic.ContainsKey(taskId)) //taskId无效
            return $"调度任务{taskId}无法识别，更新状态失败";

        IJobWorker currentWorker = mWorkerDic[taskId];
        DispatchJob jobInWorker = currentWorker.MyJob;
        if (jobInWorker == null)   //无效的JobWorker
            return $"调度任务{taskId}当前Worker对应的Job信息为Null";

        Dictionary<int, DispatchJob> jobDic = mJobsDic[taskId];
        if (jobDic.Count == 0) //Job不存在的错误
            return $"调度任务{taskId}没有Job数据";

        int index = jobDic.Keys.ToList().IndexOf(jobInWorker.Id);
        if (index == -1) //JobWorker非法
            return $"调度任务{taskId}当前Worker对应的Job非法";

        DispatchJob jobInJobDic = jobDic[jobInWorker.Id];

        if (jobInWorker.State == jobInJobDic.State) //没有发生状态变化
            return string.Empty;

        if (index == 0 && jobInJobDic.State == EnumDispatchJobState.WaitingDo) //表示此调度任务的第一个Job开始执行
        {
            bool? r = await _taskManager.UpdateDispatchTaskStateAsync(taskId, EnumDispatchTaskState.Doing).ConfigureAwait(false);
            if (r != true)
                return $"调度任务{taskId}更改状态为Doing失败";
            //通知WMS订单开始执行
            //1.盘点任务
            if (jobInWorker.ProcessId == 17)
            {
                ChkStatusDto chkStatusDto = new ChkStatusDto()
                {
                    orderCode = jobInWorker.OrderCode,
                    execState = "EXECUTING",
                };
                bool flag = await _wmsService.SendChkStatus(chkStatusDto);
                //_logger.Info($"盘点任务{jobInWorker.OrderCode},通知WMS盘点任务执行中反馈{flag}");
            }
            else//出入库
            {
                TaskStatusDto taskStatusDto = new TaskStatusDto()
                {
                    ExecState = "Executing",
                    OrderCode = jobInWorker.OrderCode,
                };
                bool flag = await _wmsService.SendTaskStatus(taskStatusDto);

                //_logger.Info($"订单{jobInWorker.OrderCode}通知WMS出入库任务在执行中反馈{flag}");

            }

        }

        //状态发生变化，且新的状态为"未完成"
        if (jobInWorker.State != EnumDispatchJobState.Done &&
            jobInWorker.State != EnumDispatchJobState.ForceDone &&
            jobInWorker.State != EnumDispatchJobState.Canceled)
        {
            mJobsDic[taskId][jobInWorker.Id].State = jobInWorker.State;
            return string.Empty;
        }


        //状态发生变化，且新的状态为"已完成"，将该调度任务下的所有job都重新读取一遍（某些Job可能会影响其它的Job状态）
        List<DispatchJob> jobs = await _jobManager.GetAllJobsOfTaskAsync(taskId).ConfigureAwait(false);
        if (jobs == null || jobs.Count == 0) //没有jobs的Task不做处理，待删除
            return $"调度任务{taskId}查询不到Job信息";
        if (jobDic.Count != jobs.Count)
            return $"调度任务{taskId}重新查询后得到的Job数量与缓存mJobsDic中的数量不一致";
        for (int i = 0; i < jobs.Count; i++)
        {
            if (jobs[i].Id != jobDic.Keys.ElementAt(i))
            {
                return $"调度任务{taskId}重新查询后得到的第{i}个Job与缓存mJobsDic中的不一致";
            }
        }
        jobDic.Clear();
        foreach (var j in jobs)
        {
            jobDic.Add(j.Id, j);
        }
        // mJobsDic[taskId] = jobDic;

        string orderCode = TaskDic[taskId].OrderCode;//该任务对应的OderCode
        int jobDoneId = jobInWorker.Id; //该任务刚完成的job的Id
        int indexOfJobDone = index; //该完成的job在所有Jobs中的索引

        if (indexOfJobDone == jobDic.Count - 1) //当前完成的Worker是任务的最后一个Job，表示该任务已全部完成
        {
            DispatchTask finishedTask = TaskDic[taskId];
            finishedTask.State = EnumDispatchTaskState.Done;
            FinishedTaskHandleResult r = await HandleFinishedTask(finishedTask).ConfigureAwait(false);
            if (!r.handleResult)
                return $"调度任务{taskId}（对应订单{orderCode}）已全部完成，删除该任务失败，{r.handleInfo}";

            string info = $"调度任务{taskId}（对应订单{orderCode}）已全部完成，删除该任务成功";
            _logger.Info(info);
            //通知上层系统任务状态
            if (finishedTask.ProcessId == 17)
            {
                var results = await _backupManager.GetChkResltsByOrderCodeInRedisAsync(orderCode).ConfigureAwait(false);
                ResultsDto ret = new ResultsDto();
                foreach (var res in results)
                {
                    ret.cells.Add(new Cell() { cellCode = res.CellCode, orderCode = res.OrderCode, plateCode = res.PlateCode });
                }

                ChkStatusDto chkStatusDto = new ChkStatusDto()
                {
                    orderCode = orderCode,
                    execState = "WAITING_CONFIRM",
                    resultsDto= ret,
                };
                bool flag = await _wmsService.SendChkStatus(chkStatusDto);
                _logger.Info($"盘点任务{jobInWorker.OrderCode},通知WMS盘点任务完成反馈{flag}");
            }
            else
            {
                TaskStatusDto taskStatusDto = new TaskStatusDto()
                {
                    OrderCode = orderCode,
                    ExecState = "Completed",
                };
                bool flag = await _wmsService.SendTaskStatus(taskStatusDto);

                _logger.Info($"出入库订单{jobInWorker.OrderCode}通知WMS出入库任务完成反馈：{flag}");
            }

            return string.Empty;
        }
        else //还存在下一个Job
        {
            //有些Job完成后会将剩余某些Job的状态改为Done，所以下一个执行Job需要遍历剩下的第一个状态为Created的Job
            int nextJobIndex = -1;
            for (int i = indexOfJobDone + 1; i < jobDic.Count; i++)
            {
                if (jobDic.ElementAt(i).Value.State != EnumDispatchJobState.Done &&
                jobDic.ElementAt(i).Value.State != EnumDispatchJobState.ForceDone &&
                jobDic.ElementAt(i).Value.State != EnumDispatchJobState.Canceled)
                {
                    nextJobIndex = i;
                    break;
                }
            }

            if (nextJobIndex == -1) //后续所有Job都已完成
            {
                DispatchTask finishedTask = TaskDic[taskId];
                finishedTask.State = EnumDispatchTaskState.Done;
                FinishedTaskHandleResult r = await HandleFinishedTask(finishedTask).ConfigureAwait(false);
                if (!r.handleResult)
                    return $"调度任务{taskId}（对应订单{orderCode}）已全部完成，删除该任务失败，{r.handleInfo}";

                string info = $"调度任务{taskId}（对应订单{orderCode}）已全部完成，删除该任务成功";
                _logger.Info(info);
                return string.Empty;
            }

            DispatchJob nextJob = jobDic.ElementAt(nextJobIndex).Value;

            if (nextJob.State != EnumDispatchJobState.Created)
                return $"调度任务{taskId}（对应订单{orderCode}）下Id为{jobDoneId}的Job已完成，下一个Id为{nextJob.Id}的Job状态非Created，而是{nextJob.State.ToString()}，状态错误";

            string workerName = await _jobManager.GetJobWorkerClassNameAsync(nextJob.JobWorkerId).ConfigureAwait(false);
            if (string.IsNullOrEmpty(workerName)) //配置问题
                return $"调度任务{taskId}（对应订单{orderCode}）下Id为{jobDoneId}的Job已完成，下一个Id为{nextJob.Id}的Job没有正确配置JobWorkerName，无法执行下一个Job";

            IJobWorker nextWorker = _jobWorkerFactory.CreateJobWorker(workerName);
            if (nextWorker == null)
                return $"调度任务{taskId}（对应订单{orderCode}）下Id为{jobDoneId}的Job已完成，下一个Id为{nextJob.Id}的Job创建JobWorker失败，无法执行下一个Job";

            //string cmdName = await _dispatchService.GetJobCmdNameAsync(nextJob.JobCmdVal).ConfigureAwait(false);
            DispatchJobCmd jobCmd = await _jobManager.GetJobCmdAsync(nextJob.JobCmdId).ConfigureAwait(false);
            if (jobCmd == null) //配置问题
                return $"调度任务{taskId}（对应订单{orderCode}），查询Id为{nextJob.Id}的Job的JobCmd失败，无法执行下一个Job";

            IJobCmd jobCmdObj = _jobCmdFactory.CreateStep(jobCmd.JobCmdClassName);
            if (jobCmdObj == null)
                return $"调度任务{taskId}（对应订单{orderCode}）下Id为{nextJob.Id}的Job创建JobCmd失败，JobCmd名为{jobCmd.JobCmdClassName}";

            bool? ret = await _jobManager.UpdateJobStateAsync(nextJob.Id, EnumDispatchJobState.WaitingDo).ConfigureAwait(false);
            if (ret == null || ret == false)
                return $"调度任务{taskId}（对应订单{orderCode}）下Id为{jobDoneId}的Job已完成，修改下一个Id为{nextJob.Id}的Job状态为{EnumDispatchJobState.WaitingDo.ToString()}失败";

            jobDic[nextJob.Id].State = EnumDispatchJobState.WaitingDo;
            mJobsDic[taskId] = jobDic;
            nextWorker.MyJob = new DispatchJob(jobDic[nextJob.Id]);
            jobCmdObj.JobCmdNameCHS = jobCmd.Describe ?? "";
            nextWorker.MyJobCmd = jobCmdObj;
            jobCmdObj.Owner = nextWorker;
            mWorkerDic[taskId] = nextWorker;
            await _orderManager.UpdateExecStepOfDispatchOrderAsync(nextWorker.MyJob.OrderCode, jobCmdObj.JobCmdNameCHS).ConfigureAwait(false);

            return string.Empty;
        }
    }

    /// <summary>
    /// 资源分配结果
    /// </summary>
    private struct AllocatedResult
    {
        public bool IsAllocated { get; set; }
        public string ErrInfo { get; set; }
    }

    /// <summary>
    /// 给指定调度任务当前的JobWorker分配资源
    /// </summary>
    /// <param name="taskId">分配资源的调度任务ID</param>
    /// <param name="resDic">可分配的资源，Key：NodeCode Value：DispatchNode键值对集合</param>
    /// <returns>true：已分配到资源 false：未分配到资源</returns>
    private async Task<AllocatedResult> AllocateResource(int taskId, Dictionary<string, DispatchNode> resDic)
    {
        if (!mWorkerDic.ContainsKey(taskId) ||
            !TaskDic.ContainsKey(taskId))  //该taskId非法
        {
            return new AllocatedResult() { IsAllocated = false, ErrInfo = $"调度任务{taskId}非法" };
        }

        if (resDic.Count == 0)
        {
            return new AllocatedResult() { IsAllocated = false, ErrInfo = $"没有可分配的资源" };
        }

        IJobWorker worker = mWorkerDic[taskId];
        if (worker.MyJob == null) //该taskId当前的JobWorker非法
            return new AllocatedResult() { IsAllocated = false, ErrInfo = $"调度任务{taskId}当前JobWorker非法" };

        string resource = await _processManager.GetResourceOfProcessStepAsync(TaskDic[taskId].ProcessId,
                worker.MyJob.ProcessSequence).ConfigureAwait(false);
        if (string.IsNullOrEmpty(resource)) //该taskId当前的JobWorker没有指定资源，这是错误的配置，资源至少需要有本身的节点
        {
            AllocatedResult result = new AllocatedResult();
            result.IsAllocated = false;
            result.ErrInfo = $"调度任务{taskId}当前执行的过程节点没有配置所需资源（过程：{TaskDic[taskId].ProcessId}，节点：{worker.MyJob.ProcessSequence}）";
            return result;
        }

        //如果资源为0，且当前的Job对应的节点为虚拟节点，代表这个Job用于释放资源
        if (resource == "0" && worker.MyJob.NodeCode.StartsWith("18"))
        {
            //释放本身原先占用的资源
            List<DispatchNode> nds = await _nodeManager.GetNodesOccupiedByTaskAsync(taskId).ConfigureAwait(false);
            if (nds == null) //本身肯定是占用资源的
            {
                AllocatedResult result = new AllocatedResult();
                result.IsAllocated = false;
                result.ErrInfo = $"查询被调度任务{taskId}占用的节点失败";
                return result;
            }
            if (nds.Count > 0)
            {
                List<string> nodeCodes = new List<string>();
                foreach (var n in nds)
                {
                    nodeCodes.Add(n.NodeCode);
                }
                await _nodeManager.UpdateNodeDataAsync(nodeCodes, EnumDispatchNodeState.Idle, -1);
            }

            return new AllocatedResult() { IsAllocated = true, ErrInfo = string.Empty };   //已经全部被本任务占用 
        }

        string[] resArray = resource.Split(","); //每一个resource均为Node
        if (resArray.Length == 0) //没有指定资源，属于配置错误，至少该worker对应的节点本身必须作为资源
        {
            AllocatedResult result = new AllocatedResult();
            result.IsAllocated = false;
            result.ErrInfo = $"调度任务{taskId}当前执行的过程节点配置的资源为空（过程：{TaskDic[taskId].ProcessId}，节点：{worker.MyJob.ProcessSequence}）";
            return result;
        }

        bool isAllResValid = true;
        string invalidResource = string.Empty;
        foreach (var res in resArray)
        {
            if (!resDic.ContainsKey(res))
            {
                isAllResValid = false;
                invalidResource = res;
                break;
            }
        }
        if (!isAllResValid) //存在无效的资源配置
        {
            AllocatedResult result = new AllocatedResult();
            result.IsAllocated = false;
            result.ErrInfo = $"调度任务{taskId}当前执行的过程节点存在无效的资源配置{invalidResource}（过程：{TaskDic[taskId].ProcessId}，节点：{worker.MyJob.ProcessSequence}）";
            return result;
        }

        bool isSomeResOwnedByOtherTaskOrDisabled = false; //是否存在某个资源被其它任务占用，或处于禁用状态
        foreach (var res in resArray)
        {
            if ((resDic[res].NodeState == EnumDispatchNodeState.Working &&
                resDic[res].TaskIdOwnIt != taskId) ||
                resDic[res].NodeState == EnumDispatchNodeState.Disabled)
            {
                isSomeResOwnedByOtherTaskOrDisabled = true;
            }
        }
        if (isSomeResOwnedByOtherTaskOrDisabled) //资源中存在被别的调度任务占据的节点，或存在被禁用的节点，该任务不能占用
        {
            AllocatedResult result = new AllocatedResult();
            result.IsAllocated = false;
            result.ErrInfo = string.Empty;
            return result;
        }

        //准备占用新的资源，本身原先占用的资源先释放
        List<DispatchNode> nodes = await _nodeManager.GetNodesOccupiedByTaskAsync(taskId).ConfigureAwait(false);

        //2024.08.08：增加虚拟节点在空步骤时不占用任何资源的功能

        //if(nodes == null) //本身肯定是占用资源的
        //{
        //    AllocatedResult result = new AllocatedResult();
        //    result.IsAllocated = false;
        //    result.ErrInfo = $"查询被调度任务{taskId}占用的节点失败";
        //    return result;
        //}

        if (nodes != null && nodes.Count > 0)
        {
            List<string> nodeCodes = new List<string>();
            foreach (var n in nodes)
            {
                nodeCodes.Add(n.NodeCode);
            }
            await _nodeManager.UpdateNodeDataAsync(nodeCodes, EnumDispatchNodeState.Idle, -1);
        }

        await _nodeManager.UpdateNodeDataAsync(resArray.ToList(), EnumDispatchNodeState.Working, taskId).ConfigureAwait(false);  //所有资源都可用的情况下，为该任务占用资源

        return new AllocatedResult() { IsAllocated = true, ErrInfo = string.Empty };   //已经全部被本任务占用    
    }


    private struct FinishedTaskHandleResult
    {
        public bool handleResult;
        public string handleInfo;
    }
    /// <summary>
    /// 处理已完成的调度任务
    /// </summary>
    /// <param name="Finishedtask">待处理的已完成任务，任务的状态（Done，Canceled，ForceDone）必须是正确的，因为需要通知到上层系统</param>
    /// <returns></returns>
    private async Task<FinishedTaskHandleResult> HandleFinishedTask(DispatchTask Finishedtask)
    {
        OpResultInDispatchSvc res = await _orderManager.FinishDispatchOrderAsync(Finishedtask.OrderCode).ConfigureAwait(false);
        if (res.IsOK == false)
            return new FinishedTaskHandleResult() { handleResult = false, handleInfo = res.Message ?? "" };

        TaskDicRemove(Finishedtask.Id);
        mJobsDic.Remove(Finishedtask.Id);
        mWorkerDic.Remove(Finishedtask.Id);
        mTaskIdSequence.Remove(Finishedtask.Id);
        return new FinishedTaskHandleResult() { handleResult = true, handleInfo = "" };
    }

    private async Task<bool> OrderToTaskAndSaveAsync(DispatchOrder order)
    {
        try
        {
            var od = await _orderManager.GetDispatchOrderByOrderCodeAsync(order.OrderCode).ConfigureAwait(false);
            if (od == null) //order不存在
                throw new Exception($"订单号{order.OrderCode}对应的订单信息不存在");

            var ts = await _taskManager.GetDispatchTasksByOrderCodeAsync(order.OrderCode).ConfigureAwait(false);
            if (ts.Count > 0)  //该order对应的task已存在
                throw new Exception($"订单{order.OrderCode}对应的调度任务已经存在");

            //order的StartNode和EndNode可能是库位，若是库位需要转换成对应的设备节点，才能定位过程
            //若是库位，则格式为：zz-xx-yy，若是设备节点，则为数字，根据这些特征来判断StartNode和EndNode的类型
            string startNode = string.Empty, endNode = string.Empty;

            if (int.TryParse(order.StartNode, out int iStartNode)) //符合设备节点的特点
                startNode = order.StartNode;
            else
            {
                string[] sections = order.StartNode.Split("-");

                if (sections.Length != 3) //不符合库位格式
                    throw new Exception($"订单{order.OrderCode}的起始节点为{order.StartNode}，既不是设备节点，也不是库位");

                if (!int.TryParse(sections[0], out int row))
                    throw new Exception($"订单{order.OrderCode}的起始节点{order.StartNode}是库位，但其中排{sections[0]}无法转换成int");

                if (!int.TryParse(sections[1], out int col))
                    throw new Exception($"订单{order.OrderCode}的起始节点{order.StartNode}是库位，但其中列{sections[1]}无法转换成int");

                if (!int.TryParse(sections[2], out int layer))
                    throw new Exception($"订单{order.OrderCode}的起始节点{order.StartNode}是库位，但其中层{sections[2]}无法转换成int");

                DispatchCell cell = await _cellRepository.FindByCellCodeAsync(order.StartNode).ConfigureAwait(false);
                if (cell == null)
                    throw new Exception($"订单{order.OrderCode}的起始节点{order.StartNode}是库位，但此库位未定义");

                if (od.OrderType == EnumDispatchOrderType.CheckDown ||
                    od.OrderType == EnumDispatchOrderType.StockIn ||
                    od.OrderType == EnumDispatchOrderType.StockOut)
                    startNode = "15001";
                else if (od.OrderType == EnumDispatchOrderType.Move)
                    startNode = "13001";
                else
                    throw new Exception($"未知的订单类型{od.OrderType}");
                //startNode = cell.RelativeNode;
            }

            if (int.TryParse(order.EndNode, out int iEndNode)) //符合设备节点的特点
                endNode = order.EndNode;
            else
            {
                string[] sections = order.EndNode.Split("-");

                if (sections.Length != 3) //不符合库位格式
                    throw new Exception($"订单{order.OrderCode}的终止节点为{order.EndNode}，既不是设备节点，也不是库位");

                if (!int.TryParse(sections[0], out int row))
                    throw new Exception($"订单{order.OrderCode}的终止节点{order.EndNode}是库位，但其中排{sections[0]}无法转换成int");

                if (!int.TryParse(sections[1], out int col))
                    throw new Exception($"订单{order.OrderCode}的终止节点{order.EndNode}是库位，但其中列{sections[1]}无法转换成int");

                if (!int.TryParse(sections[2], out int layer))
                    throw new Exception($"订单{order.OrderCode}的终止节点{order.EndNode}是库位，但其中层{sections[2]}无法转换成int");

                DispatchCell cell = await _cellRepository.FindByCellCodeAsync(order.EndNode).ConfigureAwait(false);
                if (cell == null)
                    throw new Exception($"订单{order.OrderCode}的终止节点{order.EndNode}是库位，但此库位未定义");

                if (od.OrderType == EnumDispatchOrderType.CheckDown ||
                    od.OrderType == EnumDispatchOrderType.StockIn ||
                    od.OrderType == EnumDispatchOrderType.StockOut)
                    endNode = "15001";
                else if (od.OrderType == EnumDispatchOrderType.Move)
                    endNode = "13001";
                else
                    throw new Exception($"未知的订单类型{od.OrderType}");
                //endNode = cell.RelativeNode;
            }

            var process = await _processManager.GetDispatchProcessAsync(startNode, endNode).ConfigureAwait(false);
            if (process == null) //该order指定的起止点间不存在过程
                throw new Exception($"订单{order.OrderCode}指定的起止点间不存在可执行的过程");

            int taskId = await _taskManager.GetNextTaskIdAsync().ConfigureAwait(false);
            if (taskId == -1)
                throw new Exception("获取下一个调度任务Id失败");

            DispatchTask task = new DispatchTask(taskId)
            {
                OrderCode = order.OrderCode,
                PlateCode = order.PlateCode,
                StartNode = order.StartNode,
                EndNode = order.EndNode,
                CachePos = -1,
                ProcessId = process.Id,
                Priority = order.Priority,
                LastChkOrder = order.LastCheckOrder,
                State = EnumDispatchTaskState.Created,
                CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            };

            await _taskManager.AddDispatchTaskAsync(task).ConfigureAwait(false);
            od.SetOrderState(EnumDispatchOrderState.Doing);
            await _orderManager.UpdateDispatchOrderStateAsync(od.OrderCode, EnumDispatchOrderState.Doing).ConfigureAwait(false);

            return true;
        }
        catch (EcsDomainException) //该类型的错误是已经记录在日志中的
        {
            return false;
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            return false;
        }
    }

    private async Task<bool> TaskToJobsAndSaveAsync(DispatchTask task)
    {
        try
        {
            var t = await _taskManager.GetDispatchTaskByTaskIdAsync(task.Id).ConfigureAwait(false);
            if (t == null) //没有查询到task
                throw new Exception($"Id为{task.Id}的调度任务不存在，无法分解出Job");

            var steps = await _processManager.GetDispatchProcessStepsAsync(task.ProcessId).ConfigureAwait(false);
            steps = steps.OrderBy(o => o.Sequence).ToList();

            if (steps.Count == 0) //没有找到调度过程的节点信息
                throw new Exception($"Id为{task.ProcessId}的过程没有节点信息");

            foreach (var d in steps)
            {
                if (steps.Where(o => o.Sequence == d.Sequence).ToList().Count > 1)
                    throw new Exception($"过程{task.ProcessId}中存在重复的Step");
            }

            List<DispatchJob> jobs = new List<DispatchJob>();
            foreach (var d in steps)
            {
                int jobId = await _jobManager.GetNextJobIdAsync().ConfigureAwait(false);
                if (jobId == -1)
                    throw new Exception("获取下一个Job的Id失败");

                DispatchJob job = new DispatchJob(jobId)
                {
                    JobCmdId = d.JobCmdId,
                    JobWorkerId = d.JobWorkerId,
                    ProcessId = d.ProcessId,
                    ProcessSequence = d.Sequence,
                    NextTrueStep = d.NextTrueStep,
                    NextFalseStep = d.NextFalseStep,
                    NodeCode = d.NodeCode,
                    State = EnumDispatchJobState.Created,
                    TaskId = task.Id,
                    OrderCode = task.OrderCode,
                    Priority = task.Priority,
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                };
                jobs.Add(job);
            }

            foreach (var job in jobs)
            {
                //同一个调度任务下的某一个命令只能有一个
                var js = await _jobManager.GetDispatchJobAsync(job.TaskId, job.ProcessSequence).ConfigureAwait(false);
                if (js != null) //重复了
                    throw new Exception($"调度任务Id为{job.TaskId}，过程步骤为{job.ProcessSequence}的Job已存在");
            }

            foreach (var job in jobs)
                await _jobManager.AddDispatchJobAsync(job).ConfigureAwait(false);

            await _taskManager.UpdateDispatchTaskStateAsync(task.Id, EnumDispatchTaskState.ToJobs).ConfigureAwait(false);

            return true;
        }
        catch (EcsDomainException)
        {
            return false;
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            return false;
        }
    }

    private async Task<bool?> SetFirstJobOfSomeTaskWaitingToDoAsync(int taskId, int firstJobId)
    {
        try
        {
            var task = await _taskManager.GetDispatchTaskByTaskIdAsync(taskId).ConfigureAwait(false);
            if (task == null) //没有查询到Id为taskId的任务
                throw new Exception($"Id为{taskId}的调度任务不存在");

            List<DispatchJob> jobs = await _jobManager.GetAllJobsOfTaskAsync(taskId).ConfigureAwait(false);
            if (jobs.Count == 0)
                throw new Exception($"Id为{taskId}的调度任务不存在Job");

            if (jobs[0].Id != firstJobId)
                throw new Exception($"Id为{taskId}的调度任务的第一个Job的ID不是{firstJobId}，而是{jobs[0].Id}");

            await _taskManager.UpdateDispatchTaskStateAsync(taskId, EnumDispatchTaskState.WaitingDo).ConfigureAwait(false);
            await _jobManager.UpdateJobStateAsync(firstJobId, EnumDispatchJobState.WaitingDo).ConfigureAwait(false);

            return true;
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            return null;
        }
    }

    public void StopExecute()
    {
        mCancelSource.Cancel();
    }

    public async Task<string> GetServerState()
    {
        string state = await GetDispatchSvrStateAsync().ConfigureAwait(false);
        if (state != "Running" && state != "Pause")
        {
            await UpdateDispatchSvrStateAsync("Running").ConfigureAwait(false);
            state = "Running";
        }

        if (state == "Running")
        {
            if (true == _notifierManager.IsNotifierValChanged(EcsConsts.PauseDispatcherSvrNotifierName))
            {
                await UpdateDispatchSvrStateAsync("Pause").ConfigureAwait(false);
                state = "Pause";
            }
        }
        else
        {
            if (true == _notifierManager.IsNotifierValChanged(EcsConsts.RunDispatcherSvrNotifierName))
            {
                await UpdateDispatchSvrStateAsync("Running").ConfigureAwait(false);
                state = "Running";
            }
        }

        return state;
    }

    public async Task ListenToForceDoneOrderRequest()
    {
        if (true != _notifierManager.IsNotifierValWithParaChanged(EcsConsts.DispatchOrderForceDoneNotifierName, out string orderCode))
            return;

        if (ServerState == "Running")
        {
            _notifierManager.NotifyDispatchSvrWithPara(EcsConsts.DispatchOrderForceDoneRespNotifierName, "服务处于运行状态，不能执行强制完成操作");
            _logger.Error($"收到强制完成调度订单请求，但当前服务处于运行状态，不能执行强制完成操作");
            return;
        }

        DispatchOrder order = await _orderManager.GetDispatchOrderByOrderCodeAsync(orderCode).ConfigureAwait(false);
        if (order == null)
        {
            _notifierManager.NotifyDispatchSvrWithPara(EcsConsts.DispatchOrderForceDoneRespNotifierName, $"订单码为{orderCode}的订单不存在");
            _logger.Error($"收到强制完成调度订单请求，但订单码为{orderCode}的订单不存在");
            return;
        }

        if (order.State != EnumDispatchOrderState.Created && order.State != EnumDispatchOrderState.Doing)
        {
            _notifierManager.NotifyDispatchSvrWithPara(EcsConsts.DispatchOrderForceDoneRespNotifierName, $"订单码为{orderCode}的订单已结束，无需强制完成");
            _logger.Error($"收到强制完成调度订单请求，但订单码为{orderCode}的订单已结束，无需强制完成");
            return;
        }

        List<DispatchTask> tasks = await _taskManager.GetDispatchTasksByOrderCodeAsync(orderCode).ConfigureAwait(false);

        OpResultInDispatchSvc result = await _orderManager.ForceDoneDispatchOrderAsync(orderCode).ConfigureAwait(false);
        if (result.IsOK != true)
        {
            _notifierManager.NotifyDispatchSvrWithPara(EcsConsts.DispatchOrderForceDoneRespNotifierName, $"强制完成失败");
            _logger.Error($"收到强制完成调度订单请求，订单码为{orderCode}，但强制完成失败 {result.Message}");
            return;
        }

        _notifierManager.NotifyDispatchSvr(EcsConsts.StopCheckOrderNotifierName);//通知盘点Job停止

        if (tasks != null && tasks.Count > 0)
        {
            foreach (DispatchTask task in tasks)
            {
                mTaskIdSequence.Remove(task.Id);
                TaskDicRemove(task.Id);
                mJobsDic.Remove(task.Id);
                mWorkerDic.Remove(task.Id);
            }
        }

        _notifierManager.NotifyDispatchSvrWithPara(EcsConsts.DispatchOrderForceDoneRespNotifierName, string.Empty);
        _logger.Info($"收到强制完成调度订单请求，订单码为{orderCode}，强制完成成功");
    }

    public async Task ListenToCancelTaskRequest()
    {
        if (true != _notifierManager.IsNotifierValWithParaChanged(EcsConsts.DispatchOrderCancelNotifierName, out string orderCode))
            return;

        if (ServerState == "Running")
        {
            _notifierManager.NotifyDispatchSvrWithPara(EcsConsts.DispatchOrderCancelRespNotifierName, $"服务处于运行状态，不能执行取消操作");
            _logger.Error($"收到取消调度订单请求，但当前服务处于运行状态，不能执行取消操作");
            return;
        }

        DispatchOrder order = await _orderManager.GetDispatchOrderByOrderCodeAsync(orderCode).ConfigureAwait(false);
        if (order == null)
        {
            _notifierManager.NotifyDispatchSvrWithPara(EcsConsts.DispatchOrderCancelRespNotifierName, $"订单码为{orderCode}的订单不存在");
            _logger.Error($"收到取消调度订单请求，但订单码为{orderCode}的订单不存在");
            return;
        }

        if (order.State != EnumDispatchOrderState.Created)
        {
            _notifierManager.NotifyDispatchSvrWithPara(EcsConsts.DispatchOrderCancelRespNotifierName, $"订单码为{orderCode}的订单已经执行或已经完成，无法取消");
            _logger.Error($"收到取消调度订单请求，但订单码为{orderCode}的订单已经执行或已经完成，无法取消");
            return;
        }

        List<DispatchTask> tasks = await _taskManager.GetDispatchTasksByOrderCodeAsync(orderCode).ConfigureAwait(false);

        OpResultInDispatchSvc result = await _orderManager.CancelDispatchOrderAsync(orderCode).ConfigureAwait(false);
        if (result.IsOK != true)
        {
            _notifierManager.NotifyDispatchSvrWithPara(EcsConsts.DispatchOrderCancelRespNotifierName, $"取消失败");
            _logger.Error($"收到取消调度订单请求，订单码为{orderCode}，但取消失败 {result.Message}");
            return;
        }

        if (tasks != null && tasks.Count > 0)
        {
            foreach (DispatchTask task in tasks)
            {
                mTaskIdSequence.Remove(task.Id);
                TaskDicRemove(task.Id);
                mJobsDic.Remove(task.Id);
                mWorkerDic.Remove(task.Id);
            }
        }

        _notifierManager.NotifyDispatchSvrWithPara(EcsConsts.DispatchOrderCancelRespNotifierName, string.Empty);
        _logger.Info($"收到取消调度订单请求，订单码为{orderCode}，取消成功");
    }

    public void UpdateDispatchSvrState(string state)
    {
        try
        {
            _ecsRedisClient.SetStringValue(EcsConsts.DispatchSvrStateChannel, state);
            NotifyClientsToUpdateSvrStatusAsync(state);
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
        }
    }

    public async Task UpdateDispatchSvrStateAsync(string state)
    {
        try
        {
            await _ecsRedisClient.SetStringValueAsync(EcsConsts.DispatchSvrStateChannel, state).ConfigureAwait(false);
            NotifyClientsToUpdateSvrStatusAsync(state);
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
        }
    }

    public string GetDispatchSvrState()
    {
        try
        {
            return _ecsRedisClient.GetStringValue(EcsConsts.DispatchSvrStateChannel);
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            return null;
        }
    }

    public async Task<string> GetDispatchSvrStateAsync()
    {
        try
        {
            return await _ecsRedisClient.GetStringValueAsync(EcsConsts.DispatchSvrStateChannel);
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            return null;
        }
    }

    public void NotifyClientsToUpdateSvrStatusAsync(string state)
    {
        try
        {
            _hubHelper.SendMessage(_options.Value.HubCliMethod_UpdateWcsStatus, state);
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
        }
    }

}