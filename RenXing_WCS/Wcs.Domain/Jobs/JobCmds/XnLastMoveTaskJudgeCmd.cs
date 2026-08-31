using Wcs.ConfigTool;
using Wcs.Dispatch;
using Wcs.Jobs.JobWorker;
using Wcs.Orders;
using Wcs.Orders.Models;
using Wcs.RedisTool;
using Wcs.Tasks.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Volo.Abp.DependencyInjection;

namespace Wcs.Jobs.JobCmds
{
    internal class XnLastMoveTaskJudgeCmd : IJobCmd, ITransientDependency
    {
        public bool JudgeResult { get; set; } = true;
        public IJobWorker Owner { get; set; }
        public string JobCmdNameCHS { get; set; } = string.Empty;

        private readonly OrderManager _orderManager;

        private readonly JobManager _jobManager;

        private readonly IOptions<ConfigOptions> _options;

        private readonly IRedisClient _redisClient;

        public XnLastMoveTaskJudgeCmd(OrderManager orderManager,
            JobManager jobManager,
            IOptions<ConfigOptions> options,
            IRedisClient redisClient)
        {
            _orderManager = orderManager;
            _jobManager = jobManager;
            _options = options;
            _redisClient = redisClient;
            _redisClient.Build(_options.Value.RedisConnStr, _options.Value.DefaultRedisNo);
        }

        public OpResultInDispatchSvc GenerateCmdValue()
        {
            //获取对应的Job，并读取job的Id
            if (Owner == null)
                return new OpResultInDispatchSvc() { IsOK = false, Message = $"\"最后一个移库任务判断\"命令没有指定所属的JobWorker" };

            if (Owner.MyJob == null)
                return new OpResultInDispatchSvc() { IsOK = false, Message = $"\"最后一个移库任务判断\"命令所属Job为空" };

            return new OpResultInDispatchSvc() { IsOK = true, Message = null };
        }

        public OpResultInDispatchSvc SendCmdValue()
        {
            try
            {
                OpResultInDispatchSvc r = GenerateCmdValue();
                if (!r.IsOK)
                    return r;

                return new OpResultInDispatchSvc() { IsOK = true, Message = null };
            }
            catch (Exception ex)
            {
                return new OpResultInDispatchSvc() { IsOK = false, Message = ex.Message };
            }
        }

        public OpResultInDispatchSvc IsCmdFinished()
        {
            try
            {
                if (Owner == null)
                    return new OpResultInDispatchSvc() { IsOK = false, Message = "\"最后一个移库任务判断\"命令没有指定所属的JobWorker信息" };

                if (Owner.MyJob == null)
                    return new OpResultInDispatchSvc() { IsOK = false, Message = "\"最后一个移库任务判断\"命令所属Job为空" };


                //该命令用于判断所属的调度任务是否是最后一个入库任务，该调度任务本身必须是入库任务
                DispatchOrder order = _orderManager.GetDispatchOrderByOrderCodeAsync(Owner.MyJob.OrderCode).GetAwaiter().GetResult();
                if (order == null)
                    return new OpResultInDispatchSvc() { IsOK = false, Message = $"当前命令对应订单号为{Owner.MyJob.OrderCode}，但查询不到订单信息" };

                if (order.OrderType != EnumDispatchOrderType.Move)
                    return new OpResultInDispatchSvc() { IsOK = false, Message = "\"最后一个移库任务判断\"命令所属调度任务非移库任务" };


                //查询当前正在执行的所有入库任务
                string json = _redisClient.GetStringValue(WcsConsts.DispatchTasksDoing);
                Dictionary<int, DispatchTask> tasksDoing = JsonConvert.DeserializeObject<Dictionary<int, DispatchTask>>(json);

                if (tasksDoing == null || tasksDoing.Count == 0)
                    return new OpResultInDispatchSvc() { IsOK = false, Message = "查询当前正在执行的调度任务失败，或当前不存在调度任务" };

                Dictionary<int, DispatchTask> moveTasksDoing = new Dictionary<int, DispatchTask>();
                foreach (var item in tasksDoing)
                {
                    order = _orderManager.GetDispatchOrderByOrderCodeAsync(item.Value.OrderCode).GetAwaiter().GetResult();
                    if (order == null) continue;
                    if (order.OrderType == EnumDispatchOrderType.Move)
                        moveTasksDoing.Add(item.Key, item.Value);
                }

                if (moveTasksDoing.Count == 0)
                    return new OpResultInDispatchSvc() { IsOK = false, Message = "当前正在执行的调度任务中查询不到移库任务" };


                //判断当前移库任务是否在其中
                int curTaskIndex = -1;
                for (int i = 0; i < moveTasksDoing.Keys.Count; i++)
                {
                    if (Owner.MyJob.TaskId == moveTasksDoing.Keys.ElementAt(i))
                    {
                        curTaskIndex = i;
                        break;
                    }
                }

                if (curTaskIndex == -1)
                    return new OpResultInDispatchSvc() { IsOK = false, Message = "当前正在执行的移库任务中查询不到当前任务" };



                //判断当前移库任务是否是最后一个移库库任务
                if (moveTasksDoing.Count == 1)
                    JudgeResult = true;
                else
                {
                    moveTasksDoing.Remove(Owner.MyJob.TaskId);
                    JudgeResult = true;
                    foreach (var item in moveTasksDoing)
                    {
                        var jobs = _jobManager.GetAllJobsOfTaskAsync(item.Key).GetAwaiter().GetResult();
                        if (jobs == null || jobs.Count == 0) continue;
                        foreach (var job in jobs)
                        {
                            if (job.State != EnumDispatchJobState.Done &&
                                job.State != EnumDispatchJobState.ForceDone &&
                                job.State != EnumDispatchJobState.Canceled &&
                                job.ProcessSequence <= Owner.MyJob.ProcessSequence)
                            {
                                JudgeResult = false;
                                break;
                            }
                        }
                        if (JudgeResult == false)
                            break;
                    }
                }

                //JudgeResult = curTaskIndex == (moveTasksDoing.Count - 1);

                return new OpResultInDispatchSvc() { IsOK = true, Message = null };
            }
            catch (Exception ex)
            {
                return new OpResultInDispatchSvc() { IsOK = false, Message = ex.Message };
            }
        }
    }
}
