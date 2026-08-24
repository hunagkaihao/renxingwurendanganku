using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using PlcServer.Defines;
using PlcServer.Defines.Enum;
using PlcServer.Jobs.Dtos;
using Shared.Config;
using Shared.Logger.ILogger;
using Shared.Redis.IRedisCli;
using System.Drawing;

namespace PlcServer.Jobs
{
    public class WcsLmJob : IHostedService, IDisposable
    {
        private JobHelper _jobHelper;
        private ILog _logger;
        private IRedisClient _redisClient;
        public WcsLmJob(JobHelper jobHelper, ILog logger, IRedisClient redisClient)
        {
            _jobHelper = jobHelper;
            _logger = logger;
            _redisClient = redisClient;
            _redisClient.Build(Settings.ConfigData.RedisConnString, Settings.ConfigData.RedisDBNumForSimPlc);
        }

        public void Dispose()
        {
            
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await Task.Delay(2000).ConfigureAwait(false);
            Task t = DoWork();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private const string CmdTagName = "Lm_Cmd";
        private const string ResponseTagName = "Lm_Response";
        private const string LmStateTagName = "Lm_State";
        private const string LmZeroTagName = "Lm_Zero";
        private const string LmSafePosTagName = "Lm_SafePos";
        private const string RedisKey = "WcsLmJob";
        private const string LastCmdTagValField = "LastLmCmd";

        private async Task SetErrorInfo(string err)
        {
            await _redisClient.SetHashValueAsync(RedisKey, "ErrInfo", err).ConfigureAwait(false);
        }

        private async Task DoWork()
        {
            //Do something
            await Task.Run(async () =>
            {
                //初始化龙门在原点，空闲状态
                await _jobHelper.WritePlcTagAsync("Plc1", LmStateTagName, "1").ConfigureAwait(false);//进入空闲状态
                await _jobHelper.WritePlcTagAsync("Plc1", LmZeroTagName, "1").ConfigureAwait(false);//龙门回到原点
                await _jobHelper.WritePlcTagAsync("Plc1", LmSafePosTagName, "1").ConfigureAwait(false);//龙门回到避让位
                while (true)
                {
                    await Task.Delay(200);

                    try
                    {
                        PlcTagValue? lmCmdTag = await _jobHelper.ReadPlcTagAsync("Plc1", CmdTagName).ConfigureAwait(false);
                        if(lmCmdTag == null || lmCmdTag.Quality == EnumQuality.Bad)
                        {
                            await SetErrorInfo($"读取{CmdTagName}失败").ConfigureAwait(false);
                        }
                        else
                        {
                            //当前的命令值
                            byte[] cmd = System.Text.Encoding.GetEncoding(28591).GetBytes(lmCmdTag.Value);
                            if(cmd.Length != 26)
                            {
                                await SetErrorInfo($"读到的Lm_Cmd值包含的字节数应为26，但实际为{cmd.Length}").ConfigureAwait(false);
                                continue;
                            }
                            LmCmdDto cmdDto = new LmCmdDto();
                            cmdDto.CmdVal = (ushort)((cmd[0] << 8) | cmd[1]);  //命令值，开门命令为10
                            cmdDto.TaskId = (ushort)((cmd[2] << 8) | cmd[3]);  //Job的ID
                            cmdDto.RowVal = (ushort)((cmd[4] << 8) | cmd[5]);
                            cmdDto.ColVal = (ushort)((cmd[6] << 8) | cmd[7]);
                            cmdDto.LayerVal = (ushort)((cmd[8] << 8) | cmd[9]);
                            cmdDto.CacheNo = (ushort)((cmd[10] << 8) | cmd[11]);
                            cmdDto.DoorNo = (ushort)((cmd[12] << 8) | cmd[13]);
                            cmdDto.Reserve1 = (ushort)((cmd[14] << 8) | cmd[15]);
                            cmdDto.Reserve2 = (ushort)((cmd[16] << 8) | cmd[17]);
                            cmdDto.Reserve3 = (ushort)((cmd[18] << 8) | cmd[19]);
                            cmdDto.BarcodeH = (ushort)((cmd[20] << 8) | cmd[21]);
                            cmdDto.BarcodeL = (ushort)((cmd[22] << 8) | cmd[23]);
                            cmdDto.Crc = (ushort)((cmd[24] << 8) | cmd[25]);

                            //上一次的命令值
                            LmCmdDto lastCmdDto = new LmCmdDto();
                            string? lastCmdStrVal = await _redisClient.GetHashValueAsync(RedisKey, LastCmdTagValField).ConfigureAwait(false);
                            if (string.IsNullOrEmpty(lastCmdStrVal))
                            {
                                lastCmdStrVal = JsonConvert.SerializeObject(lastCmdDto);
                                await _redisClient.SetHashValueAsync(RedisKey, LastCmdTagValField, lastCmdStrVal);
                            }
                            else
                                lastCmdDto = JsonConvert.DeserializeObject<LmCmdDto>(lastCmdStrVal) ?? throw new Exception("上一次LmCmd的值转换成LmCmdDto失败");


                            if (cmdDto.CmdVal == 2 || cmdDto.CmdVal == 3 || cmdDto.CmdVal == 7 || cmdDto.CmdVal == 8 || 
                                cmdDto.CmdVal == 11 || cmdDto.CmdVal == 12) //当前命令值：出入库取放货、移库取放货
                            {
                                if (cmdDto.CmdVal != lastCmdDto.CmdVal || cmdDto.TaskId != lastCmdDto.TaskId)
                                {
                                    await _jobHelper.WritePlcTagAsync("Plc1", LmStateTagName, "0").ConfigureAwait(false);//进入运行状态
                                    await _jobHelper.WritePlcTagAsync("Plc1", LmZeroTagName, "0").ConfigureAwait(false);//龙门离开原点
                                    await _jobHelper.WritePlcTagAsync("Plc1", LmSafePosTagName, "0").ConfigureAwait(false);//龙门离开避让位

                                    List<byte> list = new List<byte>();
                                    list.Add((byte)((cmdDto.CmdVal & 0xFF00) >> 8));
                                    list.Add((byte)(cmdDto.CmdVal & 0xFF));
                                    list.Add((byte)((cmdDto.TaskId & 0xFF00) >> 8));
                                    list.Add((byte)(cmdDto.TaskId & 0xFF));
                                    list.Add(0); //执行状态
                                    list.Add(1); //执行状态 1：正在执行
                                    list.Add(0); //条码高16位
                                    list.Add(0); //条码高16位
                                    list.Add(0); //条码低16位
                                    list.Add(0); //条码低16位
                                    list.Add(0); //crc
                                    list.Add(0); //crc 
                                    string strLmCmdResponse = System.Text.Encoding.GetEncoding(28591).GetString(list.ToArray());
                                    bool r = await _jobHelper.WritePlcTagAsync("Plc1", ResponseTagName, strLmCmdResponse).ConfigureAwait(false);
                                    if (!r)
                                    {
                                        await SetErrorInfo($"向Plc变量{ResponseTagName}第一次写值失败").ConfigureAwait(false);
                                        continue;
                                    }
                                    await Task.Delay(Settings.ConfigData.DakSimInterval).ConfigureAwait(false);

                                    list[5] = 2; //2：执行完成

                                    strLmCmdResponse = System.Text.Encoding.GetEncoding(28591).GetString(list.ToArray());
                                    r = await _jobHelper.WritePlcTagAsync("Plc1", ResponseTagName, strLmCmdResponse).ConfigureAwait(false);
                                    if (!r)
                                    {
                                        await SetErrorInfo($"向Plc变量{ResponseTagName}第二次写值失败").ConfigureAwait(false);
                                        continue;
                                    }

                                    string strCmdDto = JsonConvert.SerializeObject(cmdDto);
                                    await _redisClient.SetHashValueAsync(RedisKey, LastCmdTagValField, strCmdDto).ConfigureAwait(false);

                                    await _jobHelper.WritePlcTagAsync("Plc1", LmStateTagName, "1").ConfigureAwait(false);//进入空闲状态
                                }
                            }
                            else if (cmdDto.CmdVal == 1 || cmdDto.CmdVal == 5)
                            {
                                if (cmdDto.CmdVal != lastCmdDto.CmdVal || cmdDto.TaskId != lastCmdDto.TaskId)
                                {
                                    await _jobHelper.WritePlcTagAsync("Plc1", LmStateTagName, "0").ConfigureAwait(false);//进入运行状态

                                    List<byte> list = new List<byte>();
                                    list.Add((byte)((cmdDto.CmdVal & 0xFF00) >> 8));
                                    list.Add((byte)(cmdDto.CmdVal & 0xFF));
                                    list.Add((byte)((cmdDto.TaskId & 0xFF00) >> 8));
                                    list.Add((byte)(cmdDto.TaskId & 0xFF));
                                    list.Add(0); //执行状态
                                    list.Add(1); //执行状态 1：正在执行
                                    list.Add(0); //条码高16位
                                    list.Add(0); //条码高16位
                                    list.Add(0); //条码低16位
                                    list.Add(0); //条码低16位
                                    list.Add(0); //crc
                                    list.Add(0); //crc 
                                    string strLmCmdResponse = System.Text.Encoding.GetEncoding(28591).GetString(list.ToArray());
                                    bool r = await _jobHelper.WritePlcTagAsync("Plc1", ResponseTagName, strLmCmdResponse).ConfigureAwait(false);
                                    if (!r)
                                    {
                                        await SetErrorInfo($"向Plc变量{ResponseTagName}第一次写值失败").ConfigureAwait(false);
                                        continue;
                                    }

                                    await Task.Delay(Settings.ConfigData.DakSimInterval).ConfigureAwait(false);

                                    list[5] = 2; //2：执行完成

                                    strLmCmdResponse = System.Text.Encoding.GetEncoding(28591).GetString(list.ToArray());
                                    r = await _jobHelper.WritePlcTagAsync("Plc1", ResponseTagName, strLmCmdResponse).ConfigureAwait(false);
                                    if (!r)
                                    {
                                        await SetErrorInfo($"向Plc变量{ResponseTagName}第二次写值失败").ConfigureAwait(false);
                                        continue;
                                    }

                                    string strCmdDto = JsonConvert.SerializeObject(cmdDto);
                                    await _redisClient.SetHashValueAsync(RedisKey, LastCmdTagValField, strCmdDto).ConfigureAwait(false);

                                    await _jobHelper.WritePlcTagAsync("Plc1", LmStateTagName, "1").ConfigureAwait(false);//进入空闲状态
                                    await _jobHelper.WritePlcTagAsync("Plc1", LmSafePosTagName, "1").ConfigureAwait(false);//进入避让位
                                    if(cmdDto.CmdVal == 1)
                                        await _jobHelper.WritePlcTagAsync("Plc1", LmZeroTagName, "1").ConfigureAwait(false);//进入原点
                                }
                            }
                            else if (cmdDto.CmdVal == 4) //读库位信息
                            {
                                if (cmdDto.CmdVal != lastCmdDto.CmdVal || cmdDto.TaskId != lastCmdDto.TaskId)
                                {
                                    await _jobHelper.WritePlcTagAsync("Plc1", LmStateTagName, "0").ConfigureAwait(false);//进入运行状态
                                    await _jobHelper.WritePlcTagAsync("Plc1", LmZeroTagName, "0").ConfigureAwait(false);//龙门离开原点
                                    await _jobHelper.WritePlcTagAsync("Plc1", LmSafePosTagName, "0").ConfigureAwait(false);//龙门离开避让位

                                    List<byte> list = new List<byte>();
                                    list.Add((byte)((cmdDto.CmdVal & 0xFF00) >> 8));
                                    list.Add((byte)(cmdDto.CmdVal & 0xFF));
                                    list.Add((byte)((cmdDto.TaskId & 0xFF00) >> 8));
                                    list.Add((byte)(cmdDto.TaskId & 0xFF));
                                    list.Add(0); //执行状态
                                    list.Add(1); //执行状态 1：正在执行
                                    list.Add(0); //条码高16位
                                    list.Add(0); //条码高16位
                                    list.Add(0); //条码低16位
                                    list.Add(0); //条码低16位
                                    list.Add(0); //crc
                                    list.Add(0); //crc 
                                    string strLmCmdResponse = System.Text.Encoding.GetEncoding(28591).GetString(list.ToArray());
                                    bool r = await _jobHelper.WritePlcTagAsync("Plc1", ResponseTagName, strLmCmdResponse).ConfigureAwait(false);
                                    if (!r)
                                    {
                                        await SetErrorInfo($"向Plc变量{ResponseTagName}第一次写值失败").ConfigureAwait(false);
                                        continue;
                                    }

                                    await Task.Delay(Settings.ConfigData.DakSimInterval).ConfigureAwait(false);

                                    await _jobHelper.WritePlcTagAsync("Plc1", "SectionNoChked", cmdDto.LayerVal.ToString());
                                    await _jobHelper.WritePlcTagAsync("Plc1", "ColNoChked", cmdDto.CacheNo.ToString());
                                    await _jobHelper.WritePlcTagAsync("Plc1", "BarcodeChked", "20000012");
                                    var cellChkFinishedTag = await _jobHelper.ReadPlcTagAsync("Plc1", "CellChkFinished");
                                    int v = cellChkFinishedTag?.Value == null ? 1 : int.Parse(cellChkFinishedTag.Value) + 1;
                                    if (v == 101)
                                        v = 1;
                                    await _jobHelper.WritePlcTagAsync("Plc1", "CellChkFinished", v.ToString());

                                    await Task.Delay(20);

                                    await _jobHelper.WritePlcTagAsync("Plc1", "SectionNoChked", cmdDto.Reserve3.ToString());
                                    await _jobHelper.WritePlcTagAsync("Plc1", "ColNoChked", cmdDto.BarcodeH.ToString());
                                    await _jobHelper.WritePlcTagAsync("Plc1", "BarcodeChked", "20000013");
                                    v++;
                                    if (v == 101)
                                        v = 1;
                                    await _jobHelper.WritePlcTagAsync("Plc1", "CellChkFinished", v.ToString());

                                    await Task.Delay(10);

                                    var allCheckFinishedTag = await _jobHelper.ReadPlcTagAsync("Plc1", "AllCheckFinished");
                                    v = allCheckFinishedTag?.Value == null ? 1 : int.Parse(allCheckFinishedTag.Value) + 1;
                                    if (v == 101)
                                        v = 1;
                                    await _jobHelper.WritePlcTagAsync("Plc1", "AllCheckFinished", v.ToString());

                                    //await Task.Delay(Settings.ConfigData.DakSimInterval).ConfigureAwait(false);

                                    //list[5] = 2; //2：执行完成
                                    //list[6] = (byte)((cmdDto.TaskId & 0x0000ff00) >> 8);
                                    //list[7] = (byte)(cmdDto.TaskId & 0x000000ff);
                                    //list[8] = (byte)((cmdDto.TaskId & 0xff000000) >> 24);
                                    //list[8] = (byte)((cmdDto.TaskId & 0x00ff0000) >> 16);

                                    //strLmCmdResponse = System.Text.Encoding.GetEncoding(28591).GetString(list.ToArray());
                                    //r = await _jobHelper.WritePlcTagAsync("Plc1", ResponseTagName, strLmCmdResponse).ConfigureAwait(false);
                                    //if (!r)
                                    //{
                                    //    await SetErrorInfo($"向Plc变量{ResponseTagName}第二次写值失败").ConfigureAwait(false);
                                    //    continue;
                                    //}

                                    string strCmdDto = JsonConvert.SerializeObject(cmdDto);
                                    await _redisClient.SetHashValueAsync(RedisKey, LastCmdTagValField, strCmdDto).ConfigureAwait(false);

                                    await _jobHelper.WritePlcTagAsync("Plc1", LmStateTagName, "1").ConfigureAwait(false);//进入空闲状态
                                }
                            }
                            else
                            {
                                await SetErrorInfo($"无效的命令{cmdDto.CmdVal}").ConfigureAwait(false);
                                continue;
                            }
                        }
                    }
                    catch (Exception ex) 
                    {
                        Console.WriteLine(ex.Message);
                        _logger.Error(ex.Message);
                    }                    
                }
            });
        }
    }
}