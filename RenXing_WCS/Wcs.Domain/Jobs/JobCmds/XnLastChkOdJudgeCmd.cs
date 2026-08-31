using System;
using Wcs.Dispatch;
using Wcs.Jobs.JobWorker;
using Wcs.Orders;
using Wcs.Orders.Models;
using Volo.Abp.DependencyInjection;

namespace Wcs.Jobs.JobCmds
{
    public class XnLastChkOdJudgeCmd : IJobCmd, ITransientDependency
    {
        public bool JudgeResult { get; set; } = true;
        public IJobWorker Owner { get; set; }
        public string JobCmdNameCHS { get; set; } = string.Empty;

        private readonly OrderManager _orderManager;

        public XnLastChkOdJudgeCmd(OrderManager orderManager)
        {
            _orderManager = orderManager;
        }

        public OpResultInDispatchSvc GenerateCmdValue()
        {
            //获取对应的Job，并读取job的Id
            if (Owner == null)
                return new OpResultInDispatchSvc() { IsOK = false, Message = $"\"最后一个盘点订单判断\"命令没有指定所属的JobWorker" };

            if (Owner.MyJob == null)
                return new OpResultInDispatchSvc() { IsOK = false, Message = $"\"最后一个盘点订单判断\"命令所属Job为空" };

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
                    return new OpResultInDispatchSvc() { IsOK = false, Message = "\"最后一个盘点订单判断\"命令没有指定所属的JobWorker信息" };

                if (Owner.MyJob == null)
                    return new OpResultInDispatchSvc() { IsOK = false, Message = "\"最后一个盘点订单判断\"命令所属Job为空" };

                string orderCode = Owner.MyJob.OrderCode;
                DispatchOrder order = _orderManager.GetDispatchOrderByOrderCodeAsync(orderCode).Result;
                if (order == null)
                    return new OpResultInDispatchSvc() { IsOK = false, Message = $"\"最后一个盘点订单判断\"命令对应订单号为{orderCode}，但查询不到对应订单信息" };

                if (order.OrderType != EnumDispatchOrderType.CheckDown)
                    return new OpResultInDispatchSvc() { IsOK = false, Message = $"\"最后一个盘点订单判断\"命令对应订单号为{orderCode}，但该订单不是盘点订单" };

                if (order.LastCheckOrder == null)
                    return new OpResultInDispatchSvc() { IsOK = false, Message = $"\"最后一个盘点订单判断\"命令对应订单号为{orderCode}，但该订单没有是否为最后一个盘点订单的信息" };

                JudgeResult = order.LastCheckOrder == true;

                return new OpResultInDispatchSvc() { IsOK = true, Message = null };
            }
            catch (Exception ex)
            {
                return new OpResultInDispatchSvc() { IsOK = false, Message = ex.Message };
            }
        }

    }
}