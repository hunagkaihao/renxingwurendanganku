using Ecs.Dispatch;
using Ecs.Jobs.JobWorker;
using Ecs.Orders.Models;
using Ecs.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Ecs.Caches;
using Ecs.Caches.Models;

namespace Ecs.Jobs.JobCmds
{
    internal class XnCacheAllocatedJudgeCmd : IJobCmd, ITransientDependency
    {
        public bool JudgeResult { get; set; } = true;
        public IJobWorker Owner { get; set; }
        public string JobCmdNameCHS { get; set; } = string.Empty;

        private readonly OrderManager _orderManager;

        private readonly CacheManager _cacheManager;

        public XnCacheAllocatedJudgeCmd(
            OrderManager orderManager,
            CacheManager cacheManager)
        {
            _orderManager = orderManager;
            _cacheManager = cacheManager;
        }

        public OpResultInDispatchSvc GenerateCmdValue()
        {
            //获取对应的Job，并读取job的Id
            if (Owner == null)
                return new OpResultInDispatchSvc() { IsOK = false, Message = $"\"是否分配了缓存\"命令没有指定所属的JobWorker" };

            if (Owner.MyJob == null)
                return new OpResultInDispatchSvc() { IsOK = false, Message = $"\"是否分配了缓存\"命令所属Job为空" };

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
                    return new OpResultInDispatchSvc() { IsOK = false, Message = "\"是否分配了缓存\"命令没有指定所属的JobWorker信息" };

                if (Owner.MyJob == null)
                    return new OpResultInDispatchSvc() { IsOK = false, Message = "\"是否分配了缓存\"命令所属Job为空" };

                bool ret = _cacheManager.GetCacheByTaskId(Owner.MyJob.TaskId, out DispatchCache cache);
                if (!ret)
                    throw new Exception($"查询调度任务{Owner.MyJob.TaskId}占用的缓存失败");
                
                JudgeResult = cache != null;

                return new OpResultInDispatchSvc() { IsOK = true, Message = null };
            }
            catch (Exception ex)
            {
                return new OpResultInDispatchSvc() { IsOK = false, Message = ex.Message };
            }
        }

    }
}
