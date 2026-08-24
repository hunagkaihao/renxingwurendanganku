using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using PlcServer.Defines;
using PlcServer.Defines.Enum;
using PlcServer.Jobs.Dtos;
using Shared.Config;
using Shared.Logger.ILogger;
using Shared.Redis.IRedisCli;

namespace PlcServer.Jobs
{
    public class WcsDoor5Job : IHostedService, IDisposable
    {
        private JobHelper _jobHelper;
        private ILog _logger;
        private IRedisClient _redisClient;
        public WcsDoor5Job(JobHelper jobHelper, ILog logger, IRedisClient redisClient)
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

        private const string CmdTagName = "Door5_Cmd";
        private const string ResponseTagName = "Door5_Response";
        private const string RedisKey = "WcsDoor5Job";
        private const string LastCmdTagValField = "LastDoor5Cmd";

        private async Task SetErrorInfo(string err)
        {
            await _redisClient.SetHashValueAsync(RedisKey, "ErrInfo", err).ConfigureAwait(false);
        }

        private async Task DoWork()
        {
            //Do something
            await Task.Run(async () =>
            {
                while (true)
                {
                    await Task.Delay(200);

                    try
                    {
                        PlcTagValue? cmdTag = await _jobHelper.ReadPlcTagAsync("Plc1", CmdTagName).ConfigureAwait(false);
                        if(cmdTag == null || cmdTag.Quality == EnumQuality.Bad)
                        {
                            await SetErrorInfo($"读取Plc变量{CmdTagName}失败").ConfigureAwait(false);
                            continue;
                        }
                        else
                        {
                            byte[] cmdTagValue = System.Text.Encoding.GetEncoding(28591).GetBytes(cmdTag.Value);
                            if(cmdTagValue.Length != 12)
                            {
                                await SetErrorInfo($"Plc变量{CmdTagName}的值应包含12个字节，但实际为{cmdTagValue.Length}个字节").ConfigureAwait(false);
                                continue;
                            }

                            DoorCmdDto curCmdDto = new DoorCmdDto();
                            curCmdDto.CmdVal = (ushort)((cmdTagValue[0] << 8) | cmdTagValue[1]);  //命令值，开门命令为10
                            curCmdDto.TaskId = (ushort)((cmdTagValue[2] << 8) | cmdTagValue[3]);  //Job的ID
                            curCmdDto.Reserve1 = (ushort)((cmdTagValue[4] << 8) | cmdTagValue[5]);
                            curCmdDto.Reserve2 = (ushort)((cmdTagValue[6] << 8) | cmdTagValue[7]);
                            curCmdDto.Reserve3 = (ushort)((cmdTagValue[8] << 8) | cmdTagValue[9]);
                            curCmdDto.Crc = (ushort)((cmdTagValue[10] << 8) | cmdTagValue[11]);

                            DoorCmdDto lastCmdDto = new DoorCmdDto();
                            string? lastCmdStrVal = await _redisClient.GetHashValueAsync(RedisKey, LastCmdTagValField).ConfigureAwait(false);
                            if(string.IsNullOrEmpty(lastCmdStrVal))
                            {
                                lastCmdStrVal = JsonConvert.SerializeObject(lastCmdDto);
                                await _redisClient.SetHashValueAsync(RedisKey, LastCmdTagValField, lastCmdStrVal);
                            }
                            else
                                lastCmdDto = JsonConvert.DeserializeObject<DoorCmdDto>(lastCmdStrVal) ?? throw new Exception($"Plc变量{CmdTagName}上一次的值转换成DoorCmdDto失败");

                            if(curCmdDto.CmdVal != lastCmdDto.CmdVal || curCmdDto.TaskId != lastCmdDto.TaskId)
                            {
                                await Task.Delay(200).ConfigureAwait(false);

                                List<byte> list = new List<byte>();
                                list.Add((byte)((curCmdDto.CmdVal & 0xFF00) >> 8));
                                list.Add((byte)(curCmdDto.CmdVal & 0xFF));
                                list.Add((byte)((curCmdDto.TaskId & 0xFF00) >> 8));
                                list.Add((byte)(curCmdDto.TaskId & 0xFF));
                                list.Add(0); //执行状态
                                list.Add(1); //执行状态 1：正在执行
                                list.Add(0); //备用16位
                                list.Add(0); //备用16位
                                list.Add(0); //备用16位
                                list.Add(0); //备用16位
                                list.Add(0); //crc
                                list.Add(0); //crc
                                                        
                                string strDoorCmdResponse = System.Text.Encoding.GetEncoding(28591).GetString(list.ToArray());
                                bool r = await _jobHelper.WritePlcTagAsync("Plc1", ResponseTagName, strDoorCmdResponse).ConfigureAwait(false);
                                if(!r)
                                {
                                    await SetErrorInfo($"向Plc变量{ResponseTagName}第一次写值失败").ConfigureAwait(false);
                                    continue;
                                }

                                await Task.Delay(Settings.ConfigData.DakSimInterval).ConfigureAwait(false);

                                list[5] = 2; //2：执行完成

                                strDoorCmdResponse = System.Text.Encoding.GetEncoding(28591).GetString(list.ToArray());
                                r = await _jobHelper.WritePlcTagAsync("Plc1", ResponseTagName, strDoorCmdResponse).ConfigureAwait(false);
                                if(!r)
                                {
                                    await SetErrorInfo($"向Plc变量{ResponseTagName}第二次写值失败").ConfigureAwait(false);
                                    continue;
                                }

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