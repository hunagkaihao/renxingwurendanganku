using System;
using System.Collections.Generic;
using Wcs.PlcTool;
using Wcs.LogTool;
using Microsoft.Extensions.Logging;
using Volo.Abp.DependencyInjection;
using Microsoft.Extensions.Options;
using Wcs.ConfigTool;
using System.Threading;
using System.Xml.Linq;
using Wcs.Nodes;
using Wcs.Nodes.Models;
using Wcs.Jobs;
using Wcs.Dispatch;
using Wcs.Jobs.JobWorker;
using Wcs.Jobs.Models;
using Wcs.Orders;

namespace Wcs.Jobs.JobCmds
{
    public class OpenDoorCmd : IJobCmd, ITransientDependency
    {
        public bool JudgeResult { get; set; } = true;
        public IJobWorker Owner { get; set; }
        public string JobCmdNameCHS { get; set; } = string.Empty;

        private readonly ILogger<OpenDoorCmd> _logger;
        private readonly IOptions<ConfigOptions> _options;
        private readonly PlcHelper _plcHelper;
        private readonly JobManager _jobManager;
        private readonly OrderManager _orderManager;
        private readonly NodeManager _nodeManager;
        private readonly JobCmdHelper _jobCmdHelper;

        private byte[] mCmdValue;

        public OpenDoorCmd(
            ILogger<OpenDoorCmd> logger,
            IOptions<ConfigOptions> options,
            PlcHelper plcHelper,
            JobManager jobManager,
            OrderManager orderManager,
            NodeManager nodeManager,
            JobCmdHelper jobCmdHelper)
        {
            _logger = logger;
            _options = options;
            _plcHelper = plcHelper;
            _jobManager = jobManager;
            _orderManager = orderManager;
            _nodeManager = nodeManager;
            _jobCmdHelper = jobCmdHelper;
            mCmdValue = null;
        }

        public OpResultInDispatchSvc GenerateCmdValue()
        {
            if (Owner == null)
                return new OpResultInDispatchSvc() { IsOK = false, Message = $"当前命令没有指定所属的JobWorker" };

            if (Owner.MyJob == null)
                return new OpResultInDispatchSvc() { IsOK = false, Message = $"当前命令所属Job为空" };

            //获取命令值
            var cmd = _nodeManager.GetNodeCmdAsync("12", WcsConsts.NodeType_DoorOpen).GetAwaiter().GetResult();
            if (cmd == null)
                return new OpResultInDispatchSvc() { IsOK = false, Message = $"查询取档口打开命令信息失败" };
            ushort cmdNo = (ushort)cmd.NodeCmdValue;

            List<byte> list = new List<byte>();
            list.Add((byte)((cmdNo & 0xFF00) >> 8));
            list.Add((byte)(cmdNo & 0xFF));
            list.Add((byte)((Owner.MyJob.Id & 0xFF00) >> 8));
            list.Add((byte)(Owner.MyJob.Id & 0xFF));
            list.Add(0);
            list.Add(0);
            list.Add(0);
            list.Add(0);
            list.Add(0);
            list.Add(0);
            // ushort crc = CrcHelper.CreateCrc16Code(list.ToArray(), 40961);
            ushort crc = 0;
            list.Add((byte)((crc & 0xFF00) >> 8));
            list.Add((byte)(crc & 0xFF));

            mCmdValue = list.ToArray();
            return new OpResultInDispatchSvc() { IsOK = true, Message = null };
        }

        public OpResultInDispatchSvc SendCmdValue()
        {
            try
            {
                OpResultInDispatchSvc r = GenerateCmdValue();
                if (!r.IsOK)
                    return r;

                if (Owner == null)
                    return new OpResultInDispatchSvc() { IsOK = false, Message = $"当前命令没有指定所属的JobWorker" };

                if (Owner.MyJob == null)
                    return new OpResultInDispatchSvc() { IsOK = false, Message = $"当前命令所属Job为空" };

                if (_options.Value.OpenDoorAfterWmsAllowed)
                {
                    var order = _orderManager.GetDispatchOrderByOrderCodeAsync(Owner.MyJob.OrderCode).Result;
                    if (order == null)
                        return new OpResultInDispatchSvc() { IsOK = false, Message = $"当前命令对应订单{Owner.MyJob.OrderCode}不存在" };

                    if (!order.CanOpenDoorImmediate)
                        return new OpResultInDispatchSvc() { IsOK = false, Message = $"等待Wms下发允许开门指令" };
                }

                DispatchJob job = Owner.MyJob;
                bool ret = _jobCmdHelper.GetPlcCmdTagAddrOfNode(job.NodeCode, out string plcName, out string cmdTagName, out string failedReason);
                if (!ret)
                    return new OpResultInDispatchSvc() { IsOK = false, Message = failedReason };

                if (mCmdValue == null || mCmdValue.Length == 0)
                    return new OpResultInDispatchSvc() { IsOK = false, Message = "OpenDoorCmd命令命令值空" };

                string command = System.Text.Encoding.GetEncoding(28591).GetString(mCmdValue);
                string cmdForLog = string.Join(',', mCmdValue);

                ret = _plcHelper.WritePlcTag(plcName, cmdTagName, command);
                if (!ret)
                    return new OpResultInDispatchSvc() { IsOK = false, Message = $"向{plcName}.{cmdTagName}发送指令{cmdForLog}失败" };

                Thread.Sleep(10); //经常发生 PLC收不到开门命令 的情况，这里尝试读取 开门命令 看看，是否写入成功

                PlcTagValue cmdRtn = _plcHelper.ReadPlcTag(plcName, cmdTagName);
                if (cmdRtn == null || cmdRtn.Quality == EnumQuality.Bad)
                    return new OpResultInDispatchSvc() { IsOK = false, Message = $"读取刚发送给PLC的OpenDoorCmd指令失败" };

                byte[] cmdRtn_bytes = System.Text.Encoding.GetEncoding(28591).GetBytes(cmdRtn.Value);
                if (cmdRtn_bytes[0] != mCmdValue[0] || cmdRtn_bytes[1] != mCmdValue[1] ||
                    cmdRtn_bytes[2] != mCmdValue[2] || cmdRtn_bytes[3] != mCmdValue[3])
                    return new OpResultInDispatchSvc() { IsOK = false, Message = "刚发送给PLC的OpenDoorCmd指令未发送成功" };


                _logger.Info(Owner.GenerateLog($"向{plcName}.{cmdTagName}发送开门指令{cmdForLog}成功"));
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
                    return new OpResultInDispatchSvc() { IsOK = false, Message = "OpenDoorCmd命令没有指定所属的JobWorker信息" };

                if (Owner.MyJob == null)
                    return new OpResultInDispatchSvc() { IsOK = false, Message = "OpenDoorCmd命令所属Job为空" };

                DispatchJobCmd jobCmd = _jobManager.GetJobCmdAsync(Owner.MyJob.JobCmdId).Result;

                if (jobCmd == null)
                    return new OpResultInDispatchSvc() { IsOK = false, Message = $"根据OpenDoorCmd命令所属Job指定的JobCmdId({Owner.MyJob.JobCmdId})无法查询到JobCmd信息" };

                if (jobCmd.JobCmdClassName != GetType().Name)
                    return new OpResultInDispatchSvc() { IsOK = false, Message = $"根据OpenDoorCmd命令所属Job指定的JobCmdId({Owner.MyJob.JobCmdId})查询到的JobCmd类名称为{jobCmd.JobCmdClassName}，而非{GetType().Name}" };

                var nodeCmd = _nodeManager.GetNodeCmdAsync("12", WcsConsts.NodeType_DoorOpen).GetAwaiter().GetResult();
                if (nodeCmd == null)
                    return new OpResultInDispatchSvc() { IsOK = false, Message = $"查询取档口打开命令信息失败" };
                ushort nodeCmdNo = (ushort)nodeCmd.NodeCmdValue;

                DispatchJob job = Owner.MyJob;

                DispatchNode node = _nodeManager.GetNodeByNodeCodeAsync(job.NodeCode).Result;
                if (node == null)
                    return new OpResultInDispatchSvc() { IsOK = false, Message = $"OpenDoorCmd所对应的执行设备{job.NodeCode}不存在" };

                string[] sects = node.ResponseTagName.Split(".");
                if (sects.Length != 2 || sects[0] == "" || sects[1] == "")
                    return new OpResultInDispatchSvc() { IsOK = false, Message = $"设备{node.NodeCode}的指令反馈地址设置错误，应为\"plcName.tagName\"，但实际为{node.ResponseTagName}" };

                PlcTagValue responseTagValue = _plcHelper.ReadPlcTag(sects[0], sects[1]);
                if (responseTagValue == null || responseTagValue.Quality == EnumQuality.Bad)
                    return new OpResultInDispatchSvc() { IsOK = false, Message = $"读取OpenDoorCmd指令的反馈变量{node.ResponseTagName}失败" };

                byte[] response = System.Text.Encoding.GetEncoding(28591).GetBytes(responseTagValue.Value);
                if (response.Length != 12)
                    return new OpResultInDispatchSvc() { IsOK = false, Message = $"OpenDoorCmd指令的反馈数据包含的字节数量错误，应为12个Byte，实际为{response.Length}个" };

                ushort cmdNo = (ushort)(response[0] << 8 | response[1]);  //命令值，开门命令为10
                ushort taskNo = (ushort)(response[2] << 8 | response[3]); //Job的ID
                ushort taskState = (ushort)(response[4] << 8 | response[5]);  //0：初始状态  1：正在执行  2：执行完成
                ushort reserve1 = (ushort)(response[6] << 8 | response[7]);
                ushort reserve2 = (ushort)(response[8] << 8 | response[9]);
                ushort crc = (ushort)(response[10] << 8 | response[11]);

                if (cmdNo != nodeCmdNo)
                    return new OpResultInDispatchSvc() { IsOK = false, Message = $"设备反馈当前执行的指令值为{cmdNo}，而非开门指令{nodeCmdNo}" };

                if (taskState == 0)
                    return new OpResultInDispatchSvc() { IsOK = false, Message = $"设备反馈尚未执行开门指令" };
                else if (taskState == 1)
                {
                    if (taskNo != job.Id)
                        return new OpResultInDispatchSvc() { IsOK = false, Message = $"设备反馈正在执行JobID为{taskNo}的开门指令，而不是ID为{job.Id}的当前Job的开门指令" };
                    else
                        return new OpResultInDispatchSvc() { IsOK = false, Message = null };
                }
                else if (taskState == 2)
                {
                    if (taskNo != job.Id)
                        return new OpResultInDispatchSvc() { IsOK = false, Message = $"设备反馈JobID为{taskNo}的开门指令已执行完成，而不是当前执行的JobID:{job.Id}" };
                    else
                    {
                        //此处不需要写日志，反馈的信息会在JobWorker中写入日志
                        string plcResponse = string.Join(",", response);
                        return new OpResultInDispatchSvc() { IsOK = true, Message = $"收到Plc反馈：{plcResponse}" };
                    }
                }
                else
                    return new OpResultInDispatchSvc() { IsOK = false, Message = $"未知的任务执行状态{taskState}" };

            }
            catch (Exception ex)
            {
                return new OpResultInDispatchSvc() { IsOK = false, Message = ex.Message };
            }
        }
    }
}