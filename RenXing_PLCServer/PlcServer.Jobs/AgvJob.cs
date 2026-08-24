using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Linq;
using PlcServer.Defines;
using Shared.Config;
using Shared.Logger.ILogger;

namespace PlcServer.Jobs
{
    public class AgvJob : IHostedService, IDisposable
    {
        private JobHelper _jobHelper;
        private ILog _logger;
        public AgvJob(JobHelper jobHelper, ILog logger)
        {
            _jobHelper = jobHelper;
            _logger = logger;
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

        private async Task DoWork()
        {
            //Do something
            await Task.Run(async () =>
            {
                List<AgvJobConfig> configs = Settings.ConfigData.AgvJobConfigs;

                while (true)
                {
                    await Task.Delay(200);

                    try
                    {
                        foreach (var config in configs)
                        {
                            string[] sections = config.Trigger.Split(".");
                            if (sections.Length != 2)
                            {
                                Console.WriteLine($"触发信号{config.Trigger}配置错误，正确格式为：plcName.tagName");
                                _logger.Error($"触发信号{config.Trigger}配置错误，正确格式为：plcName.tagName");
                                continue;
                            }


                            if (_jobHelper.IsPlcTagValueChange(sections[0], sections[1]))
                            {
                                string startPoint = config.StartPoint;
                                string endPoint = config.EndPoint;
                                int iStartPoint = 0;
                                int iEndPoint = 0;


                                if (!int.TryParse(startPoint, out iStartPoint))
                                {
                                    sections = startPoint.Split(".");
                                    if (sections.Length != 2)
                                    {
                                        Console.WriteLine($"收到触发信号{config.Trigger}，但起始位置{config.StartPoint}配置错误，应为数字，或PLC点位：plcName.tagName");
                                        _logger.Error($"收到触发信号{config.Trigger}，但起始位置{config.StartPoint}配置错误，应为数字，或PLC点位：plcName.tagName");
                                        continue;
                                    }
                                    PlcTagValue? value = _jobHelper.ReadPlcTag(sections[0], sections[1]);
                                    if (value == null || value.Quality == Defines.Enum.EnumQuality.Bad)
                                    {
                                        Console.WriteLine($"收到触发信号{config.Trigger}，但从PLC读取起始位置{config.StartPoint}失败");
                                        _logger.Error($"收到触发信号{config.Trigger}，但从PLC读取起始位置{config.StartPoint}失败");
                                        continue;
                                    }
                                    if (!int.TryParse(value.Value, out iStartPoint))
                                    {
                                        Console.WriteLine($"收到触发信号{config.Trigger}，但从PLC读取起始位置{config.StartPoint}的值为{value.Value}，无法转换为整数");
                                        _logger.Error($"收到触发信号{config.Trigger}，但从PLC读取起始位置{config.StartPoint}的值为{value.Value}，无法转换为整数");
                                        continue;
                                    }
                                }


                                if (!int.TryParse(endPoint, out iEndPoint))
                                {
                                    sections = endPoint.Split(".");
                                    if (sections.Length != 2)
                                    {
                                        Console.WriteLine($"收到触发信号{config.Trigger}，但目标位置{config.EndPoint}配置错误，应为数字，或PLC点位：plcName.tagName");
                                        _logger.Error($"收到触发信号{config.Trigger}，但目标位置{config.EndPoint}配置错误，应为数字，或PLC点位：plcName.tagName");
                                        continue;
                                    }
                                    PlcTagValue? value = _jobHelper.ReadPlcTag(sections[0], sections[1]);
                                    if (value == null || value.Quality == Defines.Enum.EnumQuality.Bad)
                                    {
                                        Console.WriteLine($"收到触发信号{config.Trigger}，但从PLC读取目标位置{config.EndPoint}失败");
                                        _logger.Error($"收到触发信号{config.Trigger}，但从PLC读取目标位置{config.EndPoint}失败");
                                        continue;
                                    }
                                    if (!int.TryParse(value.Value, out iEndPoint))
                                    {
                                        Console.WriteLine($"收到触发信号{config.Trigger}，但从PLC读取目标位置{config.EndPoint}的值为{value.Value}，无法转换为整数");
                                        _logger.Error($"收到触发信号{config.Trigger}，但从PLC读取目标位置{config.EndPoint}的值为{value.Value}，无法转换为整数");
                                        continue;
                                    }
                                }

                                //在此调用RCS接口
                                Console.WriteLine($"收到触发信号{config.Trigger}，起始位置：{iStartPoint}，终止位置：{iEndPoint}");
                                _logger.Info($"收到触发信号{config.Trigger}，起始位置：{iStartPoint}，终止位置：{iEndPoint}");

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