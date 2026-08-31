using System;
using System.Collections.Generic;
using System.Linq;
using Wcs.Caches;
using Wcs.Caches.Models;
using Wcs.Cells;
using Wcs.ConfigTool;
using Wcs.Dispatch;
using Wcs.Jobs.JobWorker;
using Wcs.Orders;
using Wcs.Orders.Models;
using Wcs.RedisTool;
using Wcs.Tasks;
using Wcs.Tasks.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Volo.Abp.DependencyInjection;

namespace Wcs.Jobs.JobCmds
{
    public class AllocateCacheCmd : IJobCmd, ITransientDependency
    {
        public bool JudgeResult { get; set; } = true;
        public IJobWorker Owner { get; set; }
        public string JobCmdNameCHS { get; set; } = string.Empty;

        private string PlateSpecs = string.Empty;
        private readonly OrderManager _orderManager;
        private readonly TaskManager _taskManager;
        private readonly CacheManager _cacheManager;
        private readonly ICellRepository _cellRepository;
        private readonly IOptions<ConfigOptions> _options;
        private readonly IRedisClient _redisClient;

        public AllocateCacheCmd(
            OrderManager orderManager,
            TaskManager taskManager,
            CacheManager cacheManager,
            ICellRepository cellRepository,
            IOptions<ConfigOptions> options,
            IRedisClient redisClient)
        {
            _orderManager = orderManager;
            _taskManager = taskManager;
            _cacheManager = cacheManager;
            _cellRepository = cellRepository;
            _options = options;
            _redisClient = redisClient;
            _redisClient.Build(_options.Value.RedisConnStr, _options.Value.DefaultRedisNo);
        }

        public OpResultInDispatchSvc GenerateCmdValue()
        {
            //获取对应的Job，并读取job的Id
            if (Owner == null)
                return new OpResultInDispatchSvc() { IsOK = false, Message = $"当前命令没有指定所属的JobWorker" };

            if (Owner.MyJob == null)
                return new OpResultInDispatchSvc() { IsOK = false, Message = $"当前命令所属Job为空" };

            DispatchOrder order = _orderManager.GetDispatchOrderByOrderCodeAsync(Owner.MyJob.OrderCode).GetAwaiter().GetResult();
            if (order == null)
                return new OpResultInDispatchSvc() { IsOK = false, Message = $"当前命令对应订单号为{Owner.MyJob.OrderCode}，但查询不到订单信息" };
            if (string.IsNullOrEmpty(order.PlateSpecs))
                return new OpResultInDispatchSvc() { IsOK = false, Message = $"当前命令对应订单号为{Owner.MyJob.OrderCode}，查询到的档案盒规格为空" };

            PlateSpecs = order.PlateSpecs;

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
                    return new OpResultInDispatchSvc() { IsOK = false, Message = "当前命令没有指定所属的JobWorker信息" };

                if (Owner.MyJob == null)
                    return new OpResultInDispatchSvc() { IsOK = false, Message = "当前命令所属Job为空" };

                Dictionary<int, DispatchTask> tasksDoing = GetTasksOfServer();
                if (tasksDoing == null || tasksDoing.Count == 0)
                    return new OpResultInDispatchSvc() { IsOK = false, Message = "查询当前正在执行的调度任务失败，或当前不存在调度任务" };

                if (tasksDoing.Count == 1)
                {
                    bool? ret = _taskManager.UpdateDispatchTaskCachePosAsync(Owner.MyJob.TaskId, 0).GetAwaiter().GetResult();
                    if (ret != true)
                        return new OpResultInDispatchSvc() { IsOK = false, Message = $"更新对应调度任务的缓存位失败" };

                    return new OpResultInDispatchSvc() { IsOK = true, Message = $"系统当前只执行一个调度任务，不需要分配缓存" };
                }

                DispatchOrder order = _orderManager.GetDispatchOrderByOrderCodeAsync(Owner.MyJob.OrderCode).GetAwaiter().GetResult();
                if (order == null)
                    return new OpResultInDispatchSvc() { IsOK = false, Message = $"当前命令对应订单号为{Owner.MyJob.OrderCode}，但查询不到订单信息" };

                if (order.OrderType == EnumDispatchOrderType.Move)
                {
                    var startCell = _cellRepository.FindByCellCodeAsync(order.StartNode).GetAwaiter().GetResult();
                    if (startCell == null)
                        throw new Exception($"当前命令属于移库任务，但移库任务的起始库位{order.StartNode}不存在");

                    var endCell = _cellRepository.FindByCellCodeAsync(order.EndNode).GetAwaiter().GetResult();
                    if (endCell == null)
                        throw new Exception($"当前命令属于移库任务，但移库任务的终止库位{order.EndNode}不存在");

                    if (startCell.Row == endCell.Row)
                    {
                        bool? ret = _taskManager.UpdateDispatchTaskCachePosAsync(Owner.MyJob.TaskId, 0).GetAwaiter().GetResult();
                        if (ret != true)
                            return new OpResultInDispatchSvc() { IsOK = false, Message = $"更新对应调度任务的缓存位失败" };

                        return new OpResultInDispatchSvc() { IsOK = true, Message = $"当前命令属于移库任务，且在同一排移库，不需要分配缓存" };
                    }

                    int startRowOfWms = _options.Value.WmsFirstRowNo;
                    if (startRowOfWms % 2 == 1) //密集架起始排为奇数
                    {
                        if (((startCell.Row % 2 == 0) && (endCell.Row == startCell.Row + 1)) ||
                            ((endCell.Row % 2 == 0) && (startCell.Row == endCell.Row + 1)))
                        {
                            bool? ret = _taskManager.UpdateDispatchTaskCachePosAsync(Owner.MyJob.TaskId, 0).GetAwaiter().GetResult();
                            if (ret != true)
                                return new OpResultInDispatchSvc() { IsOK = false, Message = $"更新对应调度任务的缓存位失败" };

                            return new OpResultInDispatchSvc() { IsOK = true, Message = $"当前命令属于移库任务，且在同一过道中移库，不需要分配缓存" };
                        }
                    }
                    else //密集架起始排为偶数
                    {
                        if (((startCell.Row % 2 == 1) && (endCell.Row == startCell.Row + 1)) ||
                            ((endCell.Row % 2 == 1) && (startCell.Row == endCell.Row + 1)))
                        {
                            bool? ret = _taskManager.UpdateDispatchTaskCachePosAsync(Owner.MyJob.TaskId, 0).GetAwaiter().GetResult();
                            if (ret != true)
                                return new OpResultInDispatchSvc() { IsOK = false, Message = $"更新对应调度任务的缓存位失败" };

                            return new OpResultInDispatchSvc() { IsOK = true, Message = $"当前命令属于移库任务，且在同一过道中移库，不需要分配缓存" };
                        }
                    }
                }

                //int curTaskIndex = -1;
                //for(int i = 0; i < tasksDoing.Keys.Count; i++)
                //{
                //    if (Owner.MyJob.TaskId == tasksDoing.Keys.ElementAt(i))
                //    {
                //        curTaskIndex = i;
                //        break;
                //    }
                //}

                //if (curTaskIndex == -1)
                //    return new OpResultInDispatchSvc() { IsOK = false, Message = "系统执行的调度任务中，查询不到当前正在执行的任务" };

                DispatchCache cacheOccupied = _cacheManager.GetFirstIdleCacheWithSpecsAndOccupyIt(PlateSpecs, Owner.MyJob.TaskId);
                if (cacheOccupied == null)
                    return new OpResultInDispatchSvc() { IsOK = false, Message = $"当前命令尚未分配到缓存位" };

                bool? r = _taskManager.UpdateDispatchTaskCachePosAsync(Owner.MyJob.TaskId, cacheOccupied.CachePos).GetAwaiter().GetResult();
                if (r != true)
                    return new OpResultInDispatchSvc() { IsOK = false, Message = $"更新对应调度任务的缓存位失败" };

                return new OpResultInDispatchSvc() { IsOK = true, Message = $"分配到缓存，缓存位：{cacheOccupied.CachePos}" };
            }
            catch (Exception ex)
            {
                return new OpResultInDispatchSvc() { IsOK = false, Message = ex.Message };
            }
        }


        public Dictionary<int, DispatchTask> GetTasksOfServer()
        {
            string json = _redisClient.GetStringValue(WcsConsts.DispatchTasksDoing);
            return JsonConvert.DeserializeObject<Dictionary<int, DispatchTask>>(json);
        }

    }
}