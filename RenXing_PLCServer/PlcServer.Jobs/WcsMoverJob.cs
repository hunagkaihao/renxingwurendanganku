using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using PlcServer.Defines.Enum;
using PlcServer.Defines;
using PlcServer.Jobs.Dtos;
using Shared.Config;
using Shared.Redis.IRedisCli;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.Logger.ILogger;

namespace PlcServer.Jobs
{
    public class WcsMoverJob : IHostedService, IDisposable
    {
        private JobHelper _jobHelper;
        private ILog _logger;
        private IRedisClient _redisClient;
        public WcsMoverJob(JobHelper jobHelper, ILog logger, IRedisClient redisClient)
        {
            _jobHelper = jobHelper;
            _logger = logger;
            _redisClient = redisClient;
            _redisClient.Build(Settings.ConfigData.RedisConnString, Settings.ConfigData.RedisDBNumForSimPlc);
        }

        public void Dispose()
        {

        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Task t = DoWork();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private const string CmdTagName = "Mover_Cmd";
        private const string ResponseTagName = "Mover_Response";
        private const string RedisKey = "WcsMoverJob";
        private const string LastCmdTagValField = "LastMoverCmd";

        private async Task SetErrorInfo(string err)
        {
            await _redisClient.SetHashValueAsync(RedisKey, "ErrInfo", err).ConfigureAwait(false);
        }

        private async Task DoWork()
        {
            //Do something
            await Task.Run(async () =>
            {
                await _jobHelper.WritePlcTagAsync("Plc1", "Mover_Pos", "2").ConfigureAwait(false);//默认缩回状态
                while (true)
                {
                    await Task.Delay(200);
                    try
                    {
                        PlcTagValue? cmdTag = await _jobHelper.ReadPlcTagAsync("Plc1", CmdTagName).ConfigureAwait(false);
                        if (cmdTag == null || cmdTag.Quality == EnumQuality.Bad)
                        {
                            await SetErrorInfo($"读取Plc变量{CmdTagName}失败").ConfigureAwait(false);
                            continue;
                        }
                        else
                        {
                            byte[] cmdTagValue = System.Text.Encoding.GetEncoding(28591).GetBytes(cmdTag.Value);
                            if (cmdTagValue.Length != 8)
                            {
                                await SetErrorInfo($"Plc变量{CmdTagName}的值应包含8个字节，但实际为{cmdTagValue.Length}个字节").ConfigureAwait(false);
                                continue;
                            }

                            MoverCmdDto curCmdDto = new MoverCmdDto();
                            curCmdDto.CmdVal = (ushort)((cmdTagValue[0] << 8) | cmdTagValue[1]);  //命令值，开门命令为10
                            curCmdDto.TaskId = (ushort)((cmdTagValue[2] << 8) | cmdTagValue[3]);  //Job的ID
                            curCmdDto.Reserve1 = (ushort)((cmdTagValue[4] << 8) | cmdTagValue[5]);
                            curCmdDto.Crc = (ushort)((cmdTagValue[6] << 8) | cmdTagValue[7]);

                            if (curCmdDto.CmdVal != 11 && curCmdDto.CmdVal != 12)
                            {
                                await SetErrorInfo($"Plc变量{CmdTagName}的命令值应为11或12，但实际为{curCmdDto.CmdVal}").ConfigureAwait(false);
                                continue;
                            }

                            MoverCmdDto lastCmdDto = new MoverCmdDto();
                            string? lastCmdStrVal = await _redisClient.GetHashValueAsync(RedisKey, LastCmdTagValField).ConfigureAwait(false);
                            if (string.IsNullOrEmpty(lastCmdStrVal))
                            {
                                lastCmdStrVal = JsonConvert.SerializeObject(lastCmdDto);
                                await _redisClient.SetHashValueAsync(RedisKey, LastCmdTagValField, lastCmdStrVal);
                            }
                            else
                                lastCmdDto = JsonConvert.DeserializeObject<MoverCmdDto>(lastCmdStrVal) ?? throw new Exception($"Plc变量{CmdTagName}上一次的值转换成MoverCmdDto失败");

                            if (curCmdDto.CmdVal != lastCmdDto.CmdVal || curCmdDto.TaskId != lastCmdDto.TaskId)
                            {
                                await Task.Delay(200).ConfigureAwait(false);

                                List<byte> list = new List<byte>();
                                list.Add((byte)((curCmdDto.CmdVal & 0xFF00) >> 8));
                                list.Add((byte)(curCmdDto.CmdVal & 0xFF));
                                list.Add((byte)((curCmdDto.TaskId & 0xFF00) >> 8));
                                list.Add((byte)(curCmdDto.TaskId & 0xFF));
                                list.Add(0); //执行状态
                                list.Add(1); //执行状态 1：正在执行
                                list.Add(0); //crc
                                list.Add(0); //crc

                                string strDoorCmdResponse = System.Text.Encoding.GetEncoding(28591).GetString(list.ToArray());
                                bool r = await _jobHelper.WritePlcTagAsync("Plc1", ResponseTagName, strDoorCmdResponse).ConfigureAwait(false);
                                if (!r)
                                {
                                    await SetErrorInfo($"向Plc变量{ResponseTagName}第一次写值失败").ConfigureAwait(false);
                                    continue;
                                }

                                await Task.Delay(2700).ConfigureAwait(false);

                                list[5] = 2; //2：执行完成

                                strDoorCmdResponse = System.Text.Encoding.GetEncoding(28591).GetString(list.ToArray());
                                r = await _jobHelper.WritePlcTagAsync("Plc1", ResponseTagName, strDoorCmdResponse).ConfigureAwait(false);
                                if (!r)
                                {
                                    await SetErrorInfo($"向Plc变量{ResponseTagName}第二次写值失败").ConfigureAwait(false);
                                    continue;
                                }

                                if(curCmdDto.CmdVal == 11) //伸出命令完成
                                    await _jobHelper.WritePlcTagAsync("Plc1", "Mover_Pos", "1").ConfigureAwait(false);//伸出状态
                                else
                                    await _jobHelper.WritePlcTagAsync("Plc1", "Mover_Pos", "2").ConfigureAwait(false);//缩回状态

                                string strCmdDto = JsonConvert.SerializeObject(curCmdDto);
                                await _redisClient.SetHashValueAsync(RedisKey, LastCmdTagValField, strCmdDto).ConfigureAwait(false);
                            }
                            await SetErrorInfo("").ConfigureAwait(false);
                        }
                    }
                    catch (Exception ex)
                    {
                        await SetErrorInfo(ex.Message).ConfigureAwait(false);
                    }
                }
            });
        }
    }
}
