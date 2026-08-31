using System;
using System.Collections.Generic;
using Wcs.Dispatch;
using Wcs.Jobs.JobWorker;
using Wcs.Jobs.Models;
using Wcs.LogTool;
using Wcs.Nodes;
using Wcs.PlcTool;
using Microsoft.Extensions.Logging;
using Volo.Abp.DependencyInjection;

namespace Wcs.Jobs.JobCmds
{
    public class LMToSafePosCmd : IJobCmd, ITransientDependency
    {
        public bool JudgeResult { get; set; } = true;
        public string JobCmdNameCHS { get; set; } = string.Empty;
        public IJobWorker Owner { get; set; }

        private readonly ILogger<LMToSafePosCmd> _logger;
        private readonly PlcHelper _plcHelper;
        private readonly NodeManager _nodeManager;
        private readonly JobCmdHelper _jobCmdHelper;

        private byte[] mCmdValue;

        public LMToSafePosCmd(
            ILogger<LMToSafePosCmd> logger,
            PlcHelper plcHelper,
            NodeManager nodeManager,
            JobCmdHelper jobCmdHelper)
        {
            _logger = logger;
            _plcHelper = plcHelper;
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
            var cmd = _nodeManager.GetNodeCmdAsync("13", WcsConsts.NodeType_LMToSafePos).GetAwaiter().GetResult();
            if (cmd == null)
                return new OpResultInDispatchSvc() { IsOK = false, Message = $"查询龙门回避让位命令信息失败" };
            ushort cmdNo = (ushort)cmd.NodeCmdValue;

            List<byte> list = new List<byte>
            {
                (byte)((cmdNo & 0xFF00) >> 8),
                (byte)(cmdNo & 0xFF),
                (byte)((Owner.MyJob.Id & 0xFF00) >> 8),
                (byte)(Owner.MyJob.Id & 0xFF),
                0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0
            };
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

                if (mCmdValue == null || mCmdValue.Length == 0)
                    return new OpResultInDispatchSvc() { IsOK = false, Message = "\"龙门回避让位\"命令命令值为空" };

                DispatchJob job = Owner.MyJob;
                bool ret = _jobCmdHelper.GetPlcCmdTagAddrOfNode(job.NodeCode, out string plcName, out string cmdTagName, out string failedReason);
                if (!ret)
                    return new OpResultInDispatchSvc() { IsOK = false, Message = failedReason };

                string command = System.Text.Encoding.GetEncoding(28591).GetString(mCmdValue);
                string cmdForLog = string.Join(',', mCmdValue);

                ret = _plcHelper.WritePlcTag(plcName, cmdTagName, command);
                if (!ret)
                    return new OpResultInDispatchSvc() { IsOK = false, Message = $"向{plcName}.{cmdTagName}发送指令{cmdForLog}失败" };

                _logger.Info(Owner.GenerateLog($"向{plcName}.{cmdTagName}发送龙门回避让位指令{cmdForLog}成功"));
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
                    return new OpResultInDispatchSvc() { IsOK = false, Message = $"当前命令没有指定所属的JobWorker" };

                if (Owner.MyJob == null)
                    return new OpResultInDispatchSvc() { IsOK = false, Message = $"当前命令所属Job为空" };

                DispatchJob job = Owner.MyJob;

                bool ret = _jobCmdHelper.GetPlcResponseTagAddrOfNode(job.NodeCode, out string plcName, out string respTagName, out string failedReason);
                if (!ret)
                    return new OpResultInDispatchSvc() { IsOK = false, Message = failedReason };

                PlcTagValue responseTagValue = _plcHelper.ReadPlcTag(plcName, respTagName);
                if (responseTagValue == null || responseTagValue.Quality == EnumQuality.Bad)
                    return new OpResultInDispatchSvc() { IsOK = false, Message = $"读取反馈变量{plcName}.{respTagName}失败" };

                byte[] response = System.Text.Encoding.GetEncoding(28591).GetBytes(responseTagValue.Value);
                if (response.Length != 12)
                    return new OpResultInDispatchSvc() { IsOK = false, Message = $"PLC反馈数据包含的字节数量错误，应为12个Byte，实际为{response.Length}个" };

                ushort cmdNo = (ushort)(response[0] << 8 | response[1]);  //命令值，开门命令为10
                ushort jobNo = (ushort)(response[2] << 8 | response[3]); //Job的ID
                ushort jobState = (ushort)(response[4] << 8 | response[5]);  //0：初始状态  1：正在执行  2：执行完成
                ushort highCode = (ushort)(response[6] << 8 | response[7]);
                ushort lowCode = (ushort)(response[8] << 8 | response[9]);
                ushort crc = (ushort)(response[10] << 8 | response[11]);

                var nodeCmd = _nodeManager.GetNodeCmdAsync("13", WcsConsts.NodeType_LMToSafePos).GetAwaiter().GetResult();
                if (nodeCmd == null)
                    return new OpResultInDispatchSvc() { IsOK = false, Message = $"查询龙门回避让位命令信息失败" };
                ushort nodeCmdNo = (ushort)nodeCmd.NodeCmdValue;

                if (cmdNo != nodeCmdNo)
                    return new OpResultInDispatchSvc() { IsOK = false, Message = $"设备反馈当前执行的指令值为{cmdNo}，而非\"龙门回避让位\"指令{nodeCmdNo}" };

                if (jobState == 0)
                    return new OpResultInDispatchSvc() { IsOK = false, Message = $"设备反馈尚未执行\"龙门回避让位\"指令" };
                else if (jobState == 1)
                {
                    if (jobNo != job.Id)
                        return new OpResultInDispatchSvc() { IsOK = false, Message = $"设备反馈正在执行JobID为{jobNo}的\"龙门回避让位\"指令，而不是ID为{job.Id}的当前Job的\"龙门回避让位\"指令" };
                    else
                        return new OpResultInDispatchSvc() { IsOK = false, Message = "设备正在执行\"龙门回避让位\"指令" };
                }
                else if (jobState == 2)
                {
                    if (jobNo != job.Id)
                        return new OpResultInDispatchSvc() { IsOK = false, Message = $"设备反馈JobID为{jobNo}的\"龙门回避让位\"指令已执行完成，而不是当前执行的JobID:{job.Id}" };
                    else
                    {
                        string plcResponse = string.Join(",", response);
                        return new OpResultInDispatchSvc() { IsOK = true, Message = $"收到Plc反馈：{plcResponse}" };
                    }
                }
                else
                    return new OpResultInDispatchSvc() { IsOK = false, Message = $"未知的任务执行状态{jobState}" };

            }
            catch (Exception ex)
            {
                return new OpResultInDispatchSvc() { IsOK = false, Message = ex.Message };
            }
        }
    }
}