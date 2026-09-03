using System;
using System.IO;
using Lion.AbpPro.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Hosting;
using NetEscapades.Configuration.Yaml;
using Serilog;
using Serilog.Events;

namespace Lion.AbpPro
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
          
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostContext, configuration) =>
                {
                    // 用 YAML 替换默认的 appsettings JSON 配置源，保留环境配置的覆盖顺序。
                    var sources = configuration.Sources;
                    for (var i = 0; i < sources.Count; i++)
                    {
                        if (sources[i] is JsonConfigurationSource source &&
                            (source.Path == "appsettings.json" ||
                             source.Path == $"appsettings.{hostContext.HostingEnvironment.EnvironmentName}.json"))
                        {
                            sources[i] = new YamlConfigurationSource
                            {
                                Path = Path.ChangeExtension(source.Path, ".yaml"),
                                FileProvider = source.FileProvider,
                                Optional = source.Optional,
                                ReloadOnChange = false
                            };
                        }
                    }
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureKestrel((context, options) => { options.Limits.MaxRequestBodySize = 1024 * 50; });
                    webBuilder.UseStartup<Startup>()                   
                    ////ʹ�ö�IP
                    //.UseUrls("http://*:5000"); 
                    ;

                })
            //   .UseSerilog((ctx, config) => config
            //.ReadFrom.Configuration(ctx.Configuration))
               //��ʱ����ES
               .UseSerilog((context, loggerConfiguration) =>
               {
                   SerilogToEsExtensions.SetSerilogConfiguration(
                       loggerConfiguration,
                       context.Configuration);
               })
               .UseAutofac();
    }
}
