using System;
using System.Collections.Generic;
using Ecs.ConfigTool;
using Ecs.PlcTool;
using Ecs.LogTool;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Volo.Abp.DependencyInjection;
using Ecs.Nodes;
using Ecs.Dispatch;
using Ecs.Jobs.JobWorker;
using Ecs.Jobs.Models;
using Ecs.Tasks;
using Ecs.Tasks.Models;

namespace Ecs.Jobs.JobCmds
{
    public class LMInPlaceCmd : IJobCmd, ITransientDependency
    {
        public IJobWorker Owner { get; set; }
        public bool JudgeResult { get; set; } = true;
        public string JobCmdNameCHS { get; set; } = string.Empty;

        private readonly ILogger<LMInPlaceCmd> _logger;
        private readonly PlcHelper _plcHelper;
        private readonly NodeManager _nodeManager;
        private readonly TaskManager _taskManager;
        private readonly JobCmdHelper _jobCmdHelper;

        private byte[] mCmdValue;
        private byte MjjColNo = 255;
        private byte MjjZYNo = 255;
        private byte CachePos = 255;

        public LMInPlaceCmd(
            ILogger<LMInPlaceCmd> logger,
            PlcHelper plcHelper,
            NodeManager nodeManager,
            TaskManager taskManager,
            JobCmdHelper jobCmdHelper)
        {
            _logger = logger;
            _plcHelper = plcHelper;
            _nodeManager = nodeManager;
            _taskManager = taskManager;
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
            var cmd = _nodeManager.GetNodeCmdAsync("13", EcsConsts.NodeType_LMInPlace).GetAwaiter().GetResult();
            if (cmd == null)
                return new OpResultInDispatchSvc() { IsOK = false, Message = $"查询龙门的入库放货命令信息失败" };
            ushort cmdNo = (ushort)cmd.NodeCmdValue;

            //获取按电气定义的目标库位排、列、层，以及取档口
            bool ret = _jobCmdHelper.GetPlcCellXYZOfStockInTask(
                Owner.MyJob.TaskId, out ushort usRow, out ushort usLayer, out ushort usSectNo,
                out ushort usColInSect, out int iSpecsValue, out ushort doorCode, out string failedReason);

            if (!ret)
                return new OpResultInDispatchSvc() { IsOK = false, Message = failedReason };

            //获取被当前调度任务占用的缓存位
            ret = _jobCmdHelper.GetCacheOccupiedByJob(Owner.MyJob, out CachePos, out failedReason);
            if (!ret)
                return new OpResultInDispatchSvc() { IsOK = false, Message = failedReason };

            //获取档案盒编号
            DispatchTask task = _taskManager.GetDispatchTaskByTaskIdAsync(Owner.MyJob.TaskId).Result;
            string palletCode = task.PlateCode; //对于无人库来讲，该托盘号其实为档案盒编号，是数字
            if (!int.TryParse(palletCode, out int iPalletCode))
                return new OpResultInDispatchSvc() { IsOK = false, Message = $"搬运的档案盒编号为{palletCode}，无法转换为int" };

            ushort hightCode = (ushort)((iPalletCode & 0xffff0000) >> 16);
            ushort lowCode = (ushort)(iPalletCode & 0xffff);

            List<byte> list = new List<byte>
            {
                (byte)((cmdNo & 0xFF00) >> 8),
                (byte)(cmdNo & 0xFF),
                (byte)((Owner.MyJob.Id & 0xFF00) >> 8),
                (byte)(Owner.MyJob.Id & 0xFF),
                (byte)((usRow & 0xFF00) >> 8),
                (byte)(usRow & 0xFF),
                (byte)((usLayer & 0xFF00) >> 8),
                (byte)(usLayer & 0xFF),
                (byte)((usSectNo & 0xFF00) >> 8),
                (byte)(usSectNo & 0xFF),
                (byte)((usColInSect & 0xFF00) >> 8),
                (byte)(usColInSect & 0xFF),
                (byte)((iSpecsValue & 0xFF00) >> 8),
                (byte)(iSpecsValue & 0xFF),
                (byte)((CachePos & 0xFF00) >> 8),
                (byte)(CachePos & 0xFF),
                (byte)((doorCode & 0xFF00) >> 8),
                (byte)(doorCode & 0xFF),
                0, 0,
                (byte)((hightCode & 0xFF00) >> 8),
                (byte)(hightCode & 0xFF),
                (byte)((lowCode & 0xFF00) >> 8),
                (byte)(lowCode & 0xFF)
            };
            // ushort crc = CrcHelper.CreateCrc16Code(list.ToArray(), 40961);
            ushort crc = 0; //电气没有用到校验
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

                //获取向PLC发送命令的变量地址
                DispatchJob job = Owner.MyJob;
                bool ret = _jobCmdHelper.GetPlcCmdTagAddrOfNode(job.NodeCode, out string plcName, out string tagName, out string failedReason);
                if (!ret)
                    return new OpResultInDispatchSvc() { IsOK = false, Message = failedReason };

                if (mCmdValue == null || mCmdValue.Length == 0)
                    return new OpResultInDispatchSvc() { IsOK = false, Message = "\"龙门入库放货\"命令值空" };

                string command = System.Text.Encoding.GetEncoding(28591).GetString(mCmdValue);
                string cmdForLog = string.Join(',', mCmdValue);

                ret = _plcHelper.WritePlcTag(plcName, tagName, command);
                if (!ret)
                    return new OpResultInDispatchSvc() { IsOK = false, Message = $"向{plcName}.{tagName}发送指令{cmdForLog}失败" };

                string log = Owner.GenerateLog($"向{plcName}.{tagName}发送龙门入库放货指令{cmdForLog}成功");
                _logger.Info(log);
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
                ushort taskNo = (ushort)(response[2] << 8 | response[3]); //Job的ID
                ushort taskState = (ushort)(response[4] << 8 | response[5]);  //0：初始状态  1：正在执行  2：执行完成
                ushort highCode = (ushort)(response[6] << 8 | response[7]);
                ushort lowCode = (ushort)(response[8] << 8 | response[9]);
                ushort crc = (ushort)(response[10] << 8 | response[11]);

                var cmd = _nodeManager.GetNodeCmdAsync("13", EcsConsts.NodeType_LMInPlace).GetAwaiter().GetResult();
                if (cmd == null)
                    return new OpResultInDispatchSvc() { IsOK = false, Message = $"查询龙门入库放货命令信息失败" };
                ushort nodeCmdNo = (ushort)cmd.NodeCmdValue;

                if (cmdNo != nodeCmdNo)
                    return new OpResultInDispatchSvc() { IsOK = false, Message = $"设备反馈当前执行的指令值为{cmdNo}，而非\"龙门入库放货\"指令{nodeCmdNo}" };

                if (taskState == 0)
                    return new OpResultInDispatchSvc() { IsOK = false, Message = $"设备反馈尚未执行\"龙门入库放货\"指令" };
                else if (taskState == 1)
                {
                    if (taskNo != job.Id)
                        return new OpResultInDispatchSvc() { IsOK = false, Message = $"设备反馈正在执行JobID为{taskNo}的\"龙门入库放货\"指令，而不是ID为{job.Id}的当前Job的\"龙门入库放货\"指令" };
                    else
                        return new OpResultInDispatchSvc() { IsOK = false, Message = null };
                }
                else if (taskState == 2)
                {
                    if (taskNo != job.Id)
                        return new OpResultInDispatchSvc() { IsOK = false, Message = $"设备反馈JobID为{taskNo}的\"龙门入库放货\"指令已执行完成，而不是当前执行的JobID:{job.Id}" };
                    else
                    {
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