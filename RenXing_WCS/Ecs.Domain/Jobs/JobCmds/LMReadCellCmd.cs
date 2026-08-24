using Ecs.Dispatch;
using Ecs.Jobs.JobWorker;
using Ecs.Jobs.Models;
using Ecs.LogTool;
using Ecs.Nodes;
using Ecs.Notifiers;
using Ecs.PlcTool;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using Volo.Abp.DependencyInjection;

namespace Ecs.Jobs.JobCmds
{
    public class LMReadCellCmd : IJobCmd, ITransientDependency
    {
        public IJobWorker Owner { get; set; }
        public bool JudgeResult { get; set; } = true;
        public string JobCmdNameCHS { get; set; } = string.Empty;

        private readonly ILogger<LMReadCellCmd> _logger;
        private readonly PlcHelper _plcHelper;
        private readonly NodeManager _nodeManager;
        private readonly JobCmdHelper _jobCmdHelper;
        private readonly NotifierManager _notifierManager;
        private readonly CheckMsgQHelper _checkMsgQHelper;

        private byte[] mCmdValue;
        private byte MjjColNo = 255;
        private byte MjjZYNo = 255;
        private ushort PlcRow = 255; //Plc定义的盘点排
        private ushort PlcLayer = 255; //Plc定义的盘点层     

        private bool StartCheckThread = false;

        public LMReadCellCmd(
            ILogger<LMReadCellCmd> logger,
            PlcHelper plcHelper,
            JobCmdHelper jobCmdHelper,
            NodeManager nodeManager,
            NotifierManager notifierManager,
            CheckMsgQHelper checkMsgQHelper)
        {
            _logger = logger;
            _plcHelper = plcHelper;
            _jobCmdHelper = jobCmdHelper;
            _nodeManager = nodeManager;
            _notifierManager = notifierManager;
            _checkMsgQHelper = checkMsgQHelper;
            mCmdValue = null;
            StartCheckThread = false;
        }

        public OpResultInDispatchSvc GenerateCmdValue()
        {
            if (Owner == null)
                return new OpResultInDispatchSvc() { IsOK = false, Message = $"当前命令没有指定所属的JobWorker" };

            if (Owner.MyJob == null)
                return new OpResultInDispatchSvc() { IsOK = false, Message = $"当前命令所属Job为空" };

            //获取对应的Cmd定义，并获取命令值
            var cmd = _nodeManager.GetNodeCmdAsync("13", EcsConsts.NodeType_ReadCell).GetAwaiter().GetResult();
            if (cmd == null)
                return new OpResultInDispatchSvc() { IsOK = false, Message = $"查询龙门读库位命令信息失败" };
            ushort cmdNo = (ushort)cmd.NodeCmdValue;

            bool ret = _jobCmdHelper.GetPlcCellXYZOfCheckTask(
                Owner.MyJob.TaskId,
                out ushort sRow, out ushort sLayer, out ushort sSect, out ushort sColInSect, out int sSpecsVal,
                out ushort eRow, out ushort eLayer, out ushort eSect, out ushort eColInSect, out int eSpecsVal,
                out string failedReason);
            if (!ret)
                return new OpResultInDispatchSvc() { IsOK = false, Message = failedReason };

            List<byte> list = new List<byte>
            {
                (byte)((cmdNo & 0xFF00) >> 8),
                (byte)(cmdNo & 0xFF),
                (byte)((Owner.MyJob.Id & 0xFF00) >> 8),
                (byte)(Owner.MyJob.Id & 0xFF),
                (byte)((sRow & 0xFF00) >> 8),
                (byte)(sRow & 0xFF),
                (byte)((sLayer & 0xFF00) >> 8),
                (byte)(sLayer & 0xFF),
                (byte)((sSect & 0xFF00) >> 8),
                (byte)(sSect & 0xFF),
                (byte)((sColInSect & 0xFF00) >> 8),
                (byte)(sColInSect & 0xFF),
                (byte)((sSpecsVal & 0xFF00) >> 8),
                (byte)(sSpecsVal & 0xFF),
                (byte)((eRow & 0xFF00) >> 8),
                (byte)(eRow & 0xFF),
                (byte)((eLayer & 0xFF00) >> 8),
                (byte)(eLayer & 0xFF),
                (byte)((eSect & 0xFF00) >> 8),
                (byte)(eSect & 0xFF),
                (byte)((eColInSect & 0xFF00) >> 8),
                (byte)(eColInSect & 0xFF),
                (byte)((eSpecsVal & 0xFF00) >> 8),
                (byte)(eSpecsVal & 0xFF)
            };
            // ushort crc = CrcHelper.CreateCrc16Code(list.ToArray(), 40961);
            ushort crc = 0;
            list.Add((byte)((crc & 0xFF00) >> 8));
            list.Add((byte)(crc & 0xFF));

            mCmdValue = list.ToArray();

            PlcRow = sRow; //盘点第起始库位和终止库位在同一排，同一层上
            PlcLayer = sLayer;

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
                    return new OpResultInDispatchSvc() { IsOK = false, Message = "当前命令没有指定所属的JobWorker信息" };

                if (Owner.MyJob == null)
                    return new OpResultInDispatchSvc() { IsOK = false, Message = "当前命令所属Job为空" };

                //获取命令地址
                DispatchJob job = Owner.MyJob;
                bool ret = _jobCmdHelper.GetPlcCmdTagAddrOfNode(job.NodeCode, out string plcName, out string cmdTagName, out string failedReason);
                if (!ret)
                    return new OpResultInDispatchSvc() { IsOK = false, Message = failedReason };

                if (mCmdValue == null || mCmdValue.Length == 0)
                    return new OpResultInDispatchSvc() { IsOK = false, Message = "\"库位盘点\"命令盘点命令值空" };

                string command = System.Text.Encoding.GetEncoding(28591).GetString(mCmdValue);
                string cmdForLog = string.Join(',', mCmdValue);

                _plcHelper.IsPlcTagValueChange("Plc1", "CellChkFinished"); //预判断库位盘点是否完成，确保没有误触发
                _plcHelper.IsPlcTagValueChange("Plc1", "AllCheckFinished"); //预判断库位盘点是否全部完成，确保没有误触发
                _notifierManager.IsNotifierValChanged(EcsConsts.StopCheckOrderNotifierName); //与判断有没有收到停止盘点通知，确保没有误通知

                ret = _plcHelper.WritePlcTag(plcName, cmdTagName, command);
                if (!ret)
                    return new OpResultInDispatchSvc() { IsOK = false, Message = $"向{plcName}.{cmdTagName}发送指令{cmdForLog}失败" };

                string log = Owner.GenerateLog($"向{plcName}.{cmdTagName}发送库位盘点指令{cmdForLog}成功");
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
                    return new OpResultInDispatchSvc() { IsOK = false, Message = "当前命令没有指定所属的JobWorker信息" };

                if (Owner.MyJob == null)
                    return new OpResultInDispatchSvc() { IsOK = false, Message = "当前命令所属Job为空" };

                if (!StartCheckThread)
                {
                    bool r = _checkMsgQHelper.SendMessage(Owner.MyJob.OrderCode, PlcRow, PlcLayer, out string reason);
                    if (!r)
                        return new OpResultInDispatchSvc() { IsOK = false, Message = reason };
                    StartCheckThread = true;
                }

                if (null != _checkMsgQHelper.GetMessage())
                    return new OpResultInDispatchSvc() { IsOK = false, Message = "等待盘点结果" };
                else
                    return new OpResultInDispatchSvc() { IsOK = true, Message = null };

                // while(true)
                // {
                //     Thread.Sleep(50);

                //     if(_plcHelper.IsPlcTagValueChange("Plc1", "CellChkFinished"))
                //     {
                //         //读取节号
                //         var sectionNoTag = _plcHelper.ReadPlcTag("Plc1", "SectionNoChked");
                //         if(sectionNoTag == null || sectionNoTag.Quality == EnumQuality.Bad)
                //         {                            
                //             _logger.Error("收到PLC的单库位盘点完成信号，但从PLC读取变量SectionNoChked失败");                            
                //             continue;
                //         }
                //         if(!int.TryParse(sectionNoTag.Value, out int sectionNo))
                //         {
                //             _logger.Error($"收到PLC的单库位盘点完成信号，但从PLC读取的变量SectionNoChked值为{sectionNoTag.Value}, 无法转换为int");
                //             continue;
                //         }

                //         //读取节中的列号
                //         var colNoChkedTag = _plcHelper.ReadPlcTag("Plc1", "ColNoChked");
                //         if(colNoChkedTag == null || colNoChkedTag.Quality == EnumQuality.Bad)
                //         {
                //             _logger.Error($"收到PLC的单库位盘点完成信号，但从PLC读取变量ColNoChked失败");                            
                //             continue;
                //         }
                //         if(!int.TryParse(colNoChkedTag.Value, out int colNoInSection))
                //         {
                //             _logger.Error($"收到PLC的单库位盘点完成信号，但从PLC读取的变量ColNoChked值为{colNoChkedTag.Value}, 无法转换为int");                            
                //             continue;
                //         }

                //         //读取档案盒码
                //         var barcodeChkedTag = _plcHelper.ReadPlcTag("Plc1", "BarcodeChked");
                //         if(barcodeChkedTag == null || barcodeChkedTag.Quality == EnumQuality.Bad)
                //         {
                //             _logger.Error($"收到PLC的单库位盘点完成信号，但从PLC读取变量BarcodeChked失败");                            
                //             continue;
                //         }
                //         if(!int.TryParse(barcodeChkedTag.Value, out int barcode))
                //         {
                //             _logger.Error($"收到PLC的单库位盘点完成信号，但从PLC读取的变量BarcodeChked值为{barcodeChkedTag.Value}, 无法转换为int");
                //             continue;
                //         }

                //         //查询库位
                //         var cell = _cellRepository.FindByPlcCellXYZAsync(PlcRow, PlcLayer, sectionNo, colNoInSection).GetAwaiter().GetResult();
                //         if(cell == null)
                //         {
                //             _logger.Error($"收到PLC的单库位盘点完成信号，但根据{PlcRow}排，{PlcLayer}层，{sectionNo}节，{colNoInSection}列查询不到库位");
                //             continue;
                //         }

                //         //更新库位的盘点结果
                //         bool ret = _orderManager.UpdatePlateCodeOfChkOrderRsltAsync(
                //             Owner.MyJob.OrderCode, cell.CellCode, barcode.ToString()).Result;
                //         if(!ret)
                //         {
                //             _logger.Error($"收到PLC的单库位盘点完成信号，但更新OrderCode为{Owner.MyJob.OrderCode}，CellCode为{cell.CellCode}的盘点结果为{barcode}失败");
                //             continue;
                //         }

                //         _logger.Info($"收到PLC的单库位盘点完成信号，成功更新OrderCode为{Owner.MyJob.OrderCode}，CellCode为{cell.CellCode}的盘点结果为{barcode}");
                //     }

                //     if(_plcHelper.IsPlcTagValueChange("Plc1", "AllCheckFinished"))
                //     {
                //         _logger.Info($"收到PLC的全部盘点完成信号，盘点订单{Owner.MyJob.OrderCode}盘点结束");
                //         break;
                //     }

                //     if(true == _notifierManager.IsNotifierValChanged(EcsConsts.StopCheckOrderNotifierName))
                //     {
                //         _logger.Info($"收到停止盘点通知，盘点订单{Owner.MyJob.OrderCode}盘点结束");
                //         break;
                //     }
                // }

                // return new OpResultInDispatchSvc() { IsOK = true, Message = null };

                // DispatchJob job = Owner.MyJob;

                // //获取反馈变量地址
                // bool ret = _jobCmdHelper.GetPlcResponseTagAddrOfNode(job.NodeCode, out string plcName, out string respTagName, out string failedReason);
                // if(!ret)
                //     return new OpResultInDispatchSvc() { IsOK = false, Message = failedReason };

                // PlcTagValue responseTagValue = _plcHelper.ReadPlcTag(plcName, respTagName);
                // if(responseTagValue == null || responseTagValue.Quality == EnumQuality.Bad)
                //     return new OpResultInDispatchSvc() { IsOK = false, Message = $"读取反馈变量{plcName}.{respTagName}失败" };

                // byte[] response = System.Text.Encoding.GetEncoding(28591).GetBytes(responseTagValue.Value);
                // if(response.Length != 12)
                //     return new OpResultInDispatchSvc() { IsOK = false, Message = $"PLC反馈数据包含的字节数量错误，应为12个Byte，实际为{response.Length}个" };

                // ushort cmdNo = (ushort)((response[0] << 8) | response[1]);  //命令值，开门命令为10
                // ushort taskNo = (ushort)((response[2] << 8) | response[3]); //Job的ID
                // ushort taskState = (ushort)((response[4] << 8) | response[5]);  //0：初始状态  1：正在执行  2：执行完成
                // ushort lowCode = (ushort)((response[6] << 8) | response[7]); //plc反馈前两个字节为条码低16位
                // ushort highCode = (ushort)((response[8] << 8) | response[9]); //plc反馈后两个字节为条码高16位                
                // ushort crc = (ushort)((response[10] << 8) | response[11]);

                // var nodeCmd = _nodeManager.GetNodeCmdAsync("13", EcsConsts.NodeType_ReadCell).GetAwaiter().GetResult();
                // if(nodeCmd == null)
                //     return new OpResultInDispatchSvc() { IsOK = false, Message = $"查询龙门读库位命令信息失败" };
                // ushort nodeCmdNo = (ushort)nodeCmd.NodeCmdValue;

                // if(cmdNo != nodeCmdNo)
                //     return new OpResultInDispatchSvc() { IsOK = false, Message = $"设备反馈当前执行的指令值为{cmdNo}，而非\"库位盘点\"指令{nodeCmdNo}" };

                // if(taskState == 0)
                //     return new OpResultInDispatchSvc() { IsOK = false, Message = $"设备反馈尚未执行\"库位盘点\"指令" };
                // else if(taskState == 1)
                // {
                //     if(taskNo != job.Id)
                //         return new OpResultInDispatchSvc() { IsOK = false, Message = $"设备反馈正在执行JobID为{taskNo}的\"库位盘点\"指令，而不是ID为{job.Id}的当前Job的\"库位盘点\"指令" };
                //     else
                //         return new OpResultInDispatchSvc() { IsOK = false, Message = null };
                // }
                // else if(taskState == 2)
                // {
                //     if(taskNo != job.Id)
                //         return new OpResultInDispatchSvc() { IsOK = false, Message = $"设备反馈JobID为{taskNo}的\"库位盘点\"指令已执行完成，而不是当前执行的JobID:{job.Id}" };
                //     else
                //     {
                //         string barcode = string.Concat(highCode.ToString().PadLeft(4, '0'), lowCode.ToString().PadLeft(4, '0'));
                //         if(barcode == "00000000")
                //             barcode = "empty";
                //         ret = _orderManager.UpdatePlateCodeOfChkOrderRsltAsync(job.OrderCode, "", barcode).Result;
                //         if(!ret)
                //             return new OpResultInDispatchSvc() { IsOK = false, Message = $"设备反馈JobID为{taskNo}的\"库位盘点\"指令已执行完成，但更新OrderCode为{job.OrderCode}的盘点结果为{barcode}失败" };
                //         string plcResponse = string.Join(",", response);
                //         return new OpResultInDispatchSvc() { IsOK = true, Message = $"收到Plc反馈：{plcResponse}" };
                //     }
                // }
                // else
                //     return new OpResultInDispatchSvc() { IsOK = false, Message = $"未知的任务执行状态{taskState}" };               

            }
            catch (Exception ex)
            {
                return new OpResultInDispatchSvc() { IsOK = false, Message = ex.Message };
            }
        }
    }
}