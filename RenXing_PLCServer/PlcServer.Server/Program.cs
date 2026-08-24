using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PlcServer.Cache;
using PlcServer.Core;
using PlcServer.Devices.DeviceServices.DeviceServiceByFile;
using PlcServer.Devices.DeviceServices.DeviceServiceByMySql;
using PlcServer.Devices.IDeviceServices;
using PlcServer.Driver.Siemens;
using PlcServer.Driver.Simulation;
using PlcServer.Jobs;
using Shared.Config;
using Shared.Logger.LogRegister;
using Shared.Redis.RedisCliReg;
using System.Reflection;

namespace PlcServer.Server
{
    internal class Program
    {
        static void Main(string[] args)
       {
            IHost host = Host.CreateDefaultBuilder(args).UseSystemd().ConfigureServices((context, services) =>
            {
                services = services.RegisterLogger();
                services = services.RegisterRedisClient();
                services = services.AddTransient<ICache, CacheInRedis>();
                services = services.AddTransient<SiemensPlc>();
                services = services.AddTransient<SimulationPlc>();
                services = services.AddScoped<PlcCore>();
                services = services.AddTransient<JobHelper>();

                //配置PLC设备
                if (Settings.ConfigData.PlcSettingSource == "File")
                    services = services.AddTransient<IDeviceService, DeviceServiceInFile>();
                else if (Settings.ConfigData.PlcSettingSource == "MySQL")
                    services = services.AddTransient<IDeviceService, DeviceServiceInMySql>();

                //启动后台任务
                List<BackGroundJob> bgJobs = Settings.ConfigData.BackGroundJobs;
                Assembly assembly = Assembly.Load("PlcServer.Jobs");
                foreach (BackGroundJob job in bgJobs)
                {
                    Type extType = typeof(ServiceCollectionHostedServiceExtensions);
                    Type? type = assembly.GetType($"PlcServer.Jobs.{job.BGJobName}");
                    if (type != null)
                    {
                        MethodInfo? method = extType.GetMethod("AddHostedService", new Type[] { typeof(IServiceCollection) });
                        if (method != null)
                        {
                            method = method.MakeGenericMethod(type);
                            method.Invoke(null, new object[] { services });
                        }
                    }
                }

            }).Build();

            PlcCore plcCore = host.Services.GetRequiredService<PlcCore>();
            Task task = plcCore.WorkAsync();

            host.Run();
        }
    }
}