using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Ecs.LogTool;
using Ecs.RedisTool;
using Ecs.ConfigTool;
using Newtonsoft.Json;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Ecs.SignalRTool;
using Ecs.Dispatch;
using Ecs.Nodes.Models;
using Ecs.Jobs.Models;
using Ecs.Tasks.Models;
using Ecs.Orders.Models;

namespace Ecs.Backups;

public class BackupManager : ISingletonDependency
{
    private readonly ILogger<BackupManager> _logger;
    private readonly IOptions<ConfigOptions> _options;
    private readonly IRedisClient _ecsRedisClient;
    private readonly IRepository<DispatchOrder, int> _orderRepository;
    private readonly IRepository<DispatchTask, int> _taskRepository;
    private readonly IRepository<DispatchNode, int> _nodeRepository;
    private readonly IRepository<DispatchJob, int> _jobRepository;
    private readonly IRepository<DispatchJobCmd, int> _cmdRepository;
    private readonly HubMsgQHelper _hubHelper;

    public BackupManager(
        ILogger<BackupManager> logger,
        IOptions<ConfigOptions> options,
        IRedisClient redisClient,
        IRepository<DispatchOrder, int> orderRepository,
        IRepository<DispatchTask, int> taskRepository,
        IRepository<DispatchNode, int> nodeRepository,
        IRepository<DispatchJob, int> jobRepository,
        IRepository<DispatchJobCmd, int> cmdRepository,
        HubMsgQHelper hubHelper)
    {
        _logger = logger;
        _options = options;
        _ecsRedisClient = redisClient;
        _ecsRedisClient.Build(_options.Value.RedisConnStr, _options.Value.DefaultRedisNo);
        _orderRepository = orderRepository;
        _taskRepository = taskRepository;
        _nodeRepository = nodeRepository;
        _jobRepository = jobRepository;
        _cmdRepository = cmdRepository;
        _hubHelper = hubHelper;
    }

    /// <summary>
    /// 通知所有Hub客户端更新未完成订单信息
    /// </summary>
    /// <returns></returns>
    private async Task NotifyClientsToUpdateUndoneOrdersAsync()
    {
        try
        {
            List<OrderInRedis> orders = await GetUnFinishedOrdersInRedisAsync().ConfigureAwait(false);
            _hubHelper.SendMessage(_options.Value.HubCliMethod_UpdateUndoneOrders, orders);
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
        }
    }

    /// <summary>
    /// 初始化Redis内对订单的备份
    /// </summary>
    /// <returns></returns>
    public async Task InitializeOrdersInRedisAsync()
    {
        string[] unDoneOrderCodes = _ecsRedisClient.GetHashFields(EcsConsts.UnFinishedDispatchOrderCodeChannel);
        if (unDoneOrderCodes.Length != 0)
        {
            List<string> unDoneOdCodes = new List<string>();
            foreach (string code in unDoneOrderCodes)
            {
                if (code == null)
                    continue;
                unDoneOdCodes.Add(code);
            }

            await RemoveOrdersInRedisAsync(unDoneOdCodes).ConfigureAwait(false);
        }

        List<DispatchOrder> orders = await _orderRepository.GetListAsync(
            o => o.State == EnumDispatchOrderState.Created || o.State == EnumDispatchOrderState.Doing)
            .ConfigureAwait(false);

        if (orders != null && orders.Count > 0)
        {
            orders = orders.OrderBy(o => o.Id).ToList();
            List<OrderInRedis> orderInfos = new List<OrderInRedis>();

            foreach (DispatchOrder o in orders)
            {
                orderInfos.Add(new OrderInRedis(o));
            }
            orderInfos = orderInfos.OrderBy(o => o.createTime).ToList();
            foreach (OrderInRedis od in orderInfos)
                await SetOrderInfoInRedisAsync(od).ConfigureAwait(false);
            foreach (OrderInRedis od in orderInfos)
                await UpdateJobInfoOfOrderInRedisAsync(od.orderCode).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// 获取所有的订单信息
    /// </summary>
    /// <returns></returns>
    public async Task<List<OrderInRedis>> GetAllOrdersInRedisAsync()
    {
        try
        {
            List<OrderInRedis> result = new List<OrderInRedis>();

            KeyValuePair<string, string>[] pairs = await _ecsRedisClient.GetAllHashFieldValuePairsAsync(EcsConsts.DispatchOrderChannel).ConfigureAwait(false);
            foreach (var pair in pairs)
            {
                string value = pair.Value;
                if (value == null)
                    continue;

                OrderInRedis dto = JsonConvert.DeserializeObject<OrderInRedis>(value);
                if (dto == null)
                    continue;

                result.Add(dto);
            }

            return result.OrderBy(o => o.createTime).ToList();
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            return new List<OrderInRedis>();
        }
    }

    /// <summary>
    /// 将调度订单查询信息保存到Redis缓存
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public async Task SetOrderInfoInRedisAsync(OrderInRedis data)
    {
        try
        {
            if (data.orderState != EnumDispatchOrderState.Created.ToString() &&
                data.orderState != EnumDispatchOrderState.Doing.ToString()) //订单已完成
                await _ecsRedisClient.RemoveHashFieldsAsync(EcsConsts.UnFinishedDispatchOrderCodeChannel, new string[] { data.orderCode }).ConfigureAwait(false);
            else //订单未完成
                await _ecsRedisClient.SetHashValueAsync(EcsConsts.UnFinishedDispatchOrderCodeChannel, data.orderCode, string.Empty).ConfigureAwait(false);

            string strDto = JsonConvert.SerializeObject(data);
            await _ecsRedisClient.SetHashValueAsync(EcsConsts.DispatchOrderChannel, data.orderCode, strDto).ConfigureAwait(false);

            await NotifyClientsToUpdateUndoneOrdersAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
        }
    }

    public async Task<OrderInRedis> GetOrderWithOrderCodeInRedisAsync(string orderCode)
    {
        try
        {
            string strDto = await _ecsRedisClient.GetHashValueAsync(EcsConsts.DispatchOrderChannel, orderCode).ConfigureAwait(false);
            if (strDto == null)
                return null;

            return JsonConvert.DeserializeObject<OrderInRedis>(strDto);
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            return null;
        }
    }

    public async Task<List<OrderInRedis>> GetUnFinishedOrdersInRedisAsync()
    {
        try
        {
            List<OrderInRedis> result = new List<OrderInRedis>();

            string[] orderCodesUnDone = _ecsRedisClient.GetHashFields(EcsConsts.UnFinishedDispatchOrderCodeChannel);
            foreach (string code in orderCodesUnDone)
            {
                if (string.IsNullOrEmpty(code))
                    continue;

                string strOrder = await _ecsRedisClient.GetHashValueAsync(EcsConsts.DispatchOrderChannel, code).ConfigureAwait(false);
                if (string.IsNullOrEmpty(strOrder))
                    continue;

                OrderInRedis order = JsonConvert.DeserializeObject<OrderInRedis>(strOrder);
                if (order == null)
                    continue;

                result.Add(order);
            }

            return result.OrderBy(o => o.createTime).ToList();
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            return new List<OrderInRedis>();
        }
    }

    /// <summary>
    /// 更新指定调度订单查询信息的Task信息
    /// </summary>
    /// <param name="orderCode"></param>
    /// <param name="taskId"></param>
    /// <returns></returns>
    public async Task UpdateTaskInfoOfOrderInRedisAsync(string orderCode)
    {
        try
        {
            string strDto = await _ecsRedisClient.GetHashValueAsync(EcsConsts.DispatchOrderChannel, orderCode).ConfigureAwait(false);
            if (strDto == null)
                throw new Exception($"Redis数据库内没有订单号为{orderCode}的订单信息");

            OrderInRedis data = JsonConvert.DeserializeObject<OrderInRedis>(strDto);
            if (data == null)
                throw new Exception($"Redis数据库内订单号为{orderCode}的订单信息{strDto}无法转换成OrderInfo类型");

            List<DispatchTask> tasks = await _taskRepository.GetListAsync(o => o.OrderCode == orderCode).ConfigureAwait(false);

            if (tasks.Count == 0)
            {
                data.taskId = 0;
                data.pathId = 0;
                data.taskState = string.Empty;
            }
            else
            {
                data.taskId = tasks[0].Id;
                data.pathId = tasks[0].ProcessId;
                data.taskState = tasks[0].State.ToString();
            }

            strDto = JsonConvert.SerializeObject(data);
            await _ecsRedisClient.SetHashValueAsync(EcsConsts.DispatchOrderChannel, orderCode, strDto).ConfigureAwait(false);

            await NotifyClientsToUpdateUndoneOrdersAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
        }
    }

    /// <summary>
    /// 更新指定调度订单查询信息的Jobs信息
    /// </summary>
    /// <param name="orderCode"></param>
    /// <param name="jobs"></param>
    /// <returns></returns>
    public async Task UpdateJobInfoOfOrderInRedisAsync(string orderCode)
    {
        try
        {
            string strDto = await _ecsRedisClient.GetHashValueAsync(EcsConsts.DispatchOrderChannel, orderCode).ConfigureAwait(false);
            if (strDto == null)
                throw new Exception($"Redis数据库内没有订单号为{orderCode}的订单信息");

            OrderInRedis data = JsonConvert.DeserializeObject<OrderInRedis>(strDto);
            if (data == null)
                throw new Exception($"Redis数据库内订单号为{orderCode}的订单信息{strDto}无法转换成OrderInfo类型");

            List<DispatchTask> tasks = await _taskRepository.GetListAsync(o => o.OrderCode == orderCode).ConfigureAwait(false);
            if (tasks.Count == 0)
            {
                data.taskId = 0;
                data.pathId = 0;
                data.taskState = string.Empty;
                data.cachePos = -1;
                data.jobs = new List<JobInfo>();
            }
            else
            {
                data.taskId = tasks[0].Id;
                data.pathId = tasks[0].ProcessId;
                data.taskState = tasks[0].State.ToString();
                data.cachePos = tasks[0].CachePos;

                List<DispatchJob> jobList = await _jobRepository.GetListAsync(o => o.TaskId == tasks[0].Id).ConfigureAwait(false);
                jobList = jobList.OrderBy(o => o.Id).ToList();

                List<JobInfo> jobInfos = new List<JobInfo>();
                foreach (DispatchJob job in jobList)
                {
                    JobInfo jobDto = new JobInfo(job);

                    List<DispatchJobCmd> cmds = await _cmdRepository.GetListAsync(o => o.Id == job.JobCmdId).ConfigureAwait(false);
                    if (cmds.Count == 1)
                    {
                        if (cmds[0].Describe == null)
                            jobDto.cmdName = cmds[0].JobCmdClassName;
                        else
                            jobDto.cmdName = cmds[0].Describe ?? "";
                    }

                    string[] nodeCodes = job.NodeCode.Split(",");
                    if (nodeCodes.Length >= 1)
                    {
                        string nodeName = string.Empty;
                        foreach (var ndCode in nodeCodes)
                        {
                            List<DispatchNode> nodes = await _nodeRepository.GetListAsync(o => o.NodeCode == ndCode).ConfigureAwait(false);
                            if (nodes.Count == 1)
                                nodeName = nodeName + nodes[0].NodeName + ",";
                        }

                        jobDto.nodeName = nodeName.Substring(0, nodeName.Length - 1);
                    }

                    jobInfos.Add(jobDto);
                }

                data.jobs = jobInfos;
            }

            strDto = JsonConvert.SerializeObject(data);
            await _ecsRedisClient.SetHashValueAsync(EcsConsts.DispatchOrderChannel, orderCode, strDto).ConfigureAwait(false);

            await NotifyClientsToUpdateUndoneOrdersAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
        }
    }

    public async Task UpdateCachePosOfOrderInRedisAsync(string orderCode, int cachePos)
    {
        try
        {
            string strDto = await _ecsRedisClient.GetHashValueAsync(EcsConsts.DispatchOrderChannel, orderCode).ConfigureAwait(false);
            if (strDto == null)
                throw new Exception($"Redis数据库内没有订单号为{orderCode}的订单信息");

            OrderInRedis data = JsonConvert.DeserializeObject<OrderInRedis>(strDto);
            if (data == null)
                throw new Exception($"Redis数据库内订单号为{orderCode}的订单信息{strDto}无法转换成OrderInfoDto类型");

            if (data.cachePos == cachePos)
                return;

            data.cachePos = cachePos;
            strDto = JsonConvert.SerializeObject(data);
            await _ecsRedisClient.SetHashValueAsync(EcsConsts.DispatchOrderChannel, orderCode, strDto).ConfigureAwait(false);

            await NotifyClientsToUpdateUndoneOrdersAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
        }
    }

    /// <summary>
    /// 更新指定调度订单查询信息的订单状态
    /// </summary>
    /// <param name="orderCode"></param>
    /// <param name="newState"></param>
    /// <returns></returns>
    public async Task UpdateOrderStateOfOrderInRedisAsync(string orderCode, string newOrderState)
    {
        try
        {
            string strDto = await _ecsRedisClient.GetHashValueAsync(EcsConsts.DispatchOrderChannel, orderCode).ConfigureAwait(false);
            if (strDto == null)
                throw new Exception($"Redis数据库内没有订单号为{orderCode}的订单信息");

            OrderInRedis data = JsonConvert.DeserializeObject<OrderInRedis>(strDto);
            if (data == null)
                throw new Exception($"Redis数据库内订单号为{orderCode}的订单信息{strDto}无法转换成OrderInfoDto类型");

            if (data.orderState == newOrderState)
                return;

            data.orderState = newOrderState;
            strDto = JsonConvert.SerializeObject(data);
            await _ecsRedisClient.SetHashValueAsync(EcsConsts.DispatchOrderChannel, orderCode, strDto).ConfigureAwait(false);

            if (newOrderState != EnumDispatchOrderState.Created.ToString() &&
                newOrderState != EnumDispatchOrderState.Doing.ToString()) //订单已完成
                await _ecsRedisClient.RemoveHashFieldsAsync(EcsConsts.UnFinishedDispatchOrderCodeChannel, new string[] { orderCode }).ConfigureAwait(false);
            else //订单未完成
                await _ecsRedisClient.SetHashValueAsync(EcsConsts.UnFinishedDispatchOrderCodeChannel, orderCode, string.Empty).ConfigureAwait(false);

            await NotifyClientsToUpdateUndoneOrdersAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
        }
    }

    public async Task UpdateOpenDoorImmeOfOrderInRedisAsync(string orderCode, bool openDoorImme)
    {
        try
        {
            string strDto = await _ecsRedisClient.GetHashValueAsync(EcsConsts.DispatchOrderChannel, orderCode).ConfigureAwait(false);
            if (strDto == null)
                throw new Exception($"Redis数据库内没有订单号为{orderCode}的订单信息");
            OrderInRedis data = JsonConvert.DeserializeObject<OrderInRedis>(strDto);
            if (data == null)
                throw new Exception($"Redis数据库内订单号为{orderCode}的订单信息{strDto}无法转换成OrderInRedis类型");
            if (data.openDoorImme == openDoorImme)
                return;
            data.openDoorImme = openDoorImme;
            strDto = JsonConvert.SerializeObject(data);
            await _ecsRedisClient.SetHashValueAsync(EcsConsts.DispatchOrderChannel, orderCode, strDto).ConfigureAwait(false);

            await NotifyClientsToUpdateUndoneOrdersAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
        }
    }

    /// <summary>
    /// 更新指定订单查询信息的task状态
    /// </summary>
    /// <param name="orderCode"></param>
    /// <param name="newState"></param>
    /// <returns></returns>
    public async Task UpdateTaskStateOfOrderInRedisAsync(string orderCode, string newTaskState)
    {
        try
        {
            string strDto = await _ecsRedisClient.GetHashValueAsync(EcsConsts.DispatchOrderChannel, orderCode).ConfigureAwait(false);
            if (strDto == null)
                throw new Exception($"Redis数据库内没有订单号为{orderCode}的订单信息");

            OrderInRedis data = JsonConvert.DeserializeObject<OrderInRedis>(strDto);
            if (data == null)
                throw new Exception($"Redis数据库内订单号为{orderCode}的订单信息{strDto}无法转换成OrderInfoDto类型");

            if (data.taskState == newTaskState)
                return;
            data.taskState = newTaskState;
            strDto = JsonConvert.SerializeObject(data);
            await _ecsRedisClient.SetHashValueAsync(EcsConsts.DispatchOrderChannel, orderCode, strDto).ConfigureAwait(false);

            await NotifyClientsToUpdateUndoneOrdersAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
        }
    }

    /// <summary>
    /// 更新指定调度订单查询信息下的指定Job的状态
    /// </summary>
    /// <param name="orderCode"></param>
    /// <param name="jobId"></param>
    /// <param name="newState"></param>
    /// <returns></returns>
    public async Task UpdateJobStateOfOrderInRedisAsync(string orderCode, int jobId, string newJobState)
    {
        try
        {
            string strDto = await _ecsRedisClient.GetHashValueAsync(EcsConsts.DispatchOrderChannel, orderCode).ConfigureAwait(false);
            if (strDto == null)
                throw new Exception($"Redis数据库内没有订单号为{orderCode}的订单信息");

            OrderInRedis data = JsonConvert.DeserializeObject<OrderInRedis>(strDto);
            if (data == null)
                throw new Exception($"Redis数据库内订单号为{orderCode}的订单信息{strDto}无法转换成OrderInfoDto类型");

            for (int i = 0; i < data.jobs.Count; i++)
            {
                if (data.jobs[i].id == jobId)
                {
                    if (data.jobs[i].state == newJobState)
                        return;
                    data.jobs[i].state = newJobState;
                    break;
                }
            }
            strDto = JsonConvert.SerializeObject(data);
            await _ecsRedisClient.SetHashValueAsync(EcsConsts.DispatchOrderChannel, orderCode, strDto).ConfigureAwait(false);

            await NotifyClientsToUpdateUndoneOrdersAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
        }
    }

    public async Task UpdateJobExecInfoOfOrderInRedisAsync(string orderCode, int jobId, string execInfo)
    {
        try
        {
            string orderInfo = await _ecsRedisClient.GetHashValueAsync(EcsConsts.DispatchOrderChannel, orderCode).ConfigureAwait(false);
            if (orderInfo == null)
                throw new Exception($"Redis数据库内没有订单号为{orderCode}的订单信息");
            OrderInRedis data = JsonConvert.DeserializeObject<OrderInRedis>(orderInfo);
            if (data == null)
                throw new Exception($"Redis数据库内订单号为{orderCode}的订单信息{orderInfo}无法转换成OrderInfo类型");
            for (int i = 0; i < data.jobs.Count; i++)
            {
                if (data.jobs[i].id == jobId)
                {
                    if (data.jobs[i].execInfo == execInfo) //相同数据时，不更新
                        return;
                    data.jobs[i].execInfo = execInfo;
                    break;
                }
            }
            orderInfo = JsonConvert.SerializeObject(data);
            await _ecsRedisClient.SetHashValueAsync(EcsConsts.DispatchOrderChannel, orderCode, orderInfo).ConfigureAwait(false);

            await NotifyClientsToUpdateUndoneOrdersAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
        }
    }

    /// <summary>
    /// 更新指定调度订单查询信息下的task状态和所有Job状态
    /// </summary>
    /// <param name="orderCode"></param>
    /// <param name="newState"></param>
    /// <returns></returns>
    public async Task UpdateTaskAndJobsStateOfOrderInRedisAsync(string orderCode, string newState)
    {
        try
        {
            string strDto = await _ecsRedisClient.GetHashValueAsync(EcsConsts.DispatchOrderChannel, orderCode).ConfigureAwait(false);
            if (strDto == null)
                throw new Exception($"Redis数据库内没有订单号为{orderCode}的订单信息");

            OrderInRedis data = JsonConvert.DeserializeObject<OrderInRedis>(strDto);
            if (data == null)
                throw new Exception($"Redis数据库内订单号为{orderCode}的订单信息{strDto}无法转换成OrderInfoDto类型");

            data.taskState = newState;
            for (int i = 0; i < data.jobs.Count; i++)
            {
                data.jobs[i].state = newState;
            }
            strDto = JsonConvert.SerializeObject(data);
            await _ecsRedisClient.SetHashValueAsync(EcsConsts.DispatchOrderChannel, orderCode, strDto).ConfigureAwait(false);

            await NotifyClientsToUpdateUndoneOrdersAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
        }
    }

    /// <summary>
    /// 更新指定调度订单查询信息的执行信息
    /// </summary>
    /// <param name="orderCode"></param>
    /// <param name="execInfo"></param>
    /// <returns></returns>
    public async Task UpdateExecInfoOfOrderInRedisAsync(string orderCode, string execInfo)
    {
        try
        {
            string strDto = await _ecsRedisClient.GetHashValueAsync(EcsConsts.DispatchOrderChannel, orderCode).ConfigureAwait(false);
            if (strDto == null)
                throw new Exception($"Redis数据库内没有订单号为{orderCode}的订单信息");

            OrderInRedis data = JsonConvert.DeserializeObject<OrderInRedis>(strDto);
            if (data == null)
                throw new Exception($"Redis数据库内订单号为{orderCode}的订单信息{strDto}无法转换成OrderInfoDto类型");

            if (data.execInfo == execInfo)
                return;

            data.execInfo = execInfo;
            data.execUpdateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            strDto = JsonConvert.SerializeObject(data);
            await _ecsRedisClient.SetHashValueAsync(EcsConsts.DispatchOrderChannel, orderCode, strDto).ConfigureAwait(false);

            await NotifyClientsToUpdateUndoneOrdersAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
        }
    }

    /// <summary>
    /// 更新调度订单查询信息的执行命令信息
    /// </summary>
    /// <param name="orderCode"></param>
    /// <param name="execStep"></param>
    /// <returns></returns>
    public async Task UpdateExecStepOfOrderInRedisAsync(string orderCode, string execStep)
    {
        try
        {
            string strDto = await _ecsRedisClient.GetHashValueAsync(EcsConsts.DispatchOrderChannel, orderCode).ConfigureAwait(false);
            if (strDto == null)
                throw new Exception($"Redis数据库内没有订单号为{orderCode}的订单信息");

            OrderInRedis data = JsonConvert.DeserializeObject<OrderInRedis>(strDto);
            if (data == null)
                throw new Exception($"Redis数据库内订单号为{orderCode}的订单信息{strDto}无法转换成OrderInfoDto类型");

            if (data.execStep == execStep)
                return;

            data.execStep = execStep;
            data.execUpdateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            strDto = JsonConvert.SerializeObject(data);
            await _ecsRedisClient.SetHashValueAsync(EcsConsts.DispatchOrderChannel, orderCode, strDto).ConfigureAwait(false);

            await NotifyClientsToUpdateUndoneOrdersAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
        }
    }

    /// <summary>
    /// 更新调度订单查询信息中的是否发生错误
    /// </summary>
    /// <param name="orderCode"></param>
    /// <param name="hasError"></param>
    /// <returns></returns>
    public async Task UpdateErrorOfOrderInRedisAsync(string orderCode, bool hasError)
    {
        try
        {
            string strDto = await _ecsRedisClient.GetHashValueAsync(EcsConsts.DispatchOrderChannel, orderCode).ConfigureAwait(false);
            if (strDto == null)
                throw new Exception($"Redis数据库内没有订单号为{orderCode}的订单信息");

            OrderInRedis data = JsonConvert.DeserializeObject<OrderInRedis>(strDto);
            if (data == null)
                throw new Exception($"Redis数据库内订单号为{orderCode}的订单信息{strDto}无法转换成OrderInfoDto类型");

            if (data.hasError == hasError)
                return;

            data.hasError = hasError;
            data.execUpdateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            strDto = JsonConvert.SerializeObject(data);
            await _ecsRedisClient.SetHashValueAsync(EcsConsts.DispatchOrderChannel, orderCode, strDto).ConfigureAwait(false);

            await NotifyClientsToUpdateUndoneOrdersAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
        }
    }

    /// <summary>
    /// 删除指定订单号的调度订单查询信息
    /// </summary>
    /// <param name="orderCode"></param>
    /// <returns></returns>
    public async Task RemoveOrderInRedisAsync(string orderCode)
    {
        try
        {
            await _ecsRedisClient.RemoveHashFieldsAsync(EcsConsts.UnFinishedDispatchOrderCodeChannel, new string[] { orderCode }).ConfigureAwait(false);
            await _ecsRedisClient.RemoveHashFieldsAsync(EcsConsts.DispatchOrderChannel, new string[] { orderCode }).ConfigureAwait(false);

            await NotifyClientsToUpdateUndoneOrdersAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
        }
    }

    /// <summary>
    /// 删除指定订单号的多个调度订单查询信息
    /// </summary>
    /// <param name="orderCodes"></param>
    /// <returns></returns>
    public async Task RemoveOrdersInRedisAsync(List<string> orderCodes)
    {
        try
        {
            await _ecsRedisClient.RemoveHashFieldsAsync(EcsConsts.UnFinishedDispatchOrderCodeChannel, orderCodes.ToArray()).ConfigureAwait(false);
            await _ecsRedisClient.RemoveHashFieldsAsync(EcsConsts.DispatchOrderChannel, orderCodes.ToArray()).ConfigureAwait(false);

            await NotifyClientsToUpdateUndoneOrdersAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
        }
    }

    /// <summary>
    /// 从Redis缓存中删除所有的调度订单查询信息
    /// </summary>
    /// <returns></returns>
    public async Task RemoveAllOrdersInRedisAsync()
    {
        try
        {
            await _ecsRedisClient.RemoveKeyAsync(EcsConsts.UnFinishedDispatchOrderCodeChannel).ConfigureAwait(false);
            await _ecsRedisClient.RemoveKeyAsync(EcsConsts.DispatchOrderChannel).ConfigureAwait(false);

            await NotifyClientsToUpdateUndoneOrdersAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
        }
    }

    /// <summary>
    /// 备份盘点结果
    /// </summary>
    /// <param name="rslt"></param>
    /// <returns></returns>
    public async Task SetChkOrderRsltInRedisAsync(DispatchChkOrderRslt rslt)
    {
        try
        {
            string strDto = JsonConvert.SerializeObject(rslt);
            await _ecsRedisClient.SetHashValueAsync(
                EcsConsts.DispatchChkOdResultChannel,
                $"{rslt.OrderCode}.{rslt.CellCode}",
                strDto).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
        }
    }

    /// <summary>
    /// 删除盘点结果备份
    /// </summary>
    /// <param name="chkOdCode"></param>
    /// <returns></returns>
    public async Task RemoveOrderRsltInRedisAsync(string chkOdCode)
    {
        try
        {
            await _ecsRedisClient.RemoveHashFieldsAsync(EcsConsts.DispatchChkOdResultChannel, new string[] { chkOdCode }).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
        }
    }

    public async Task<List<DispatchChkOrderRslt>> GetChkResltsByOrderCodeInRedisAsync(string orderCode)
    {
        try
        {
            KeyValuePair<string, string>[] pairs = await _ecsRedisClient.GetAllHashFieldValuePairsAsync(EcsConsts.DispatchChkOdResultChannel).ConfigureAwait(false);
            if (pairs.Length == 0)
                return new List<DispatchChkOrderRslt>();

            List<DispatchChkOrderRslt> rslts = new List<DispatchChkOrderRslt>();

            foreach (var pair in pairs)
            {
                string value = pair.Value;
                if (value == null)
                    continue;

                DispatchChkOrderRslt rslt = JsonConvert.DeserializeObject<DispatchChkOrderRslt>(value);
                if (rslt == null)
                    continue;

                rslts.Add(rslt);
            }

            List<DispatchChkOrderRslt> rets = new List<DispatchChkOrderRslt>();
            foreach (var rslt in rslts)
            {
                if (rslt.OrderCode == orderCode)
                    rets.Add(rslt);
            }

            if (rets.Count == 0)
                return new List<DispatchChkOrderRslt>();

            return rets.OrderBy(r => r.Id).ToList();
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            return new List<DispatchChkOrderRslt>();
        }
    }

    public async Task<List<DispatchChkOrderRslt>> GetChkResultsByQueryCodeInRedisAsync(string queryCode)
    {
        try
        {
            KeyValuePair<string, string>[] pairs = await _ecsRedisClient.GetAllHashFieldValuePairsAsync(EcsConsts.DispatchChkOdResultChannel).ConfigureAwait(false);
            if (pairs.Length == 0)
                return new List<DispatchChkOrderRslt>();

            List<DispatchChkOrderRslt> rslts = new List<DispatchChkOrderRslt>();

            foreach (var pair in pairs)
            {
                string value = pair.Value;
                if (value == null)
                    continue;

                DispatchChkOrderRslt rslt = JsonConvert.DeserializeObject<DispatchChkOrderRslt>(value);
                if (rslt == null)
                    continue;

                rslts.Add(rslt);
            }

            List<DispatchChkOrderRslt> rets = new List<DispatchChkOrderRslt>();
            foreach (var rslt in rslts)
            {
                if (rslt.QueryCode == queryCode)
                    rets.Add(rslt);
            }

            if (rets.Count == 0)
                return new List<DispatchChkOrderRslt>();

            return rets.OrderBy(r => r.Id).ToList();
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            return new List<DispatchChkOrderRslt>();
        }
    }
}