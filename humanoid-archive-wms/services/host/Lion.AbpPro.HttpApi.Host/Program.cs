using System;
using System.IO;
using Lion.AbpPro.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
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
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureKestrel((context, options) => { options.Limits.MaxRequestBodySize = 1024 * 50; });
                    webBuilder.UseStartup<Startup>()                   
                    ////使用多IP
                    //.UseUrls("http://*:5000"); 
                    ;

                })
            //   .UseSerilog((ctx, config) => config
            //.ReadFrom.Configuration(ctx.Configuration))
               //暂时屏蔽ES
               .UseSerilog((context, loggerConfiguration) =>
               {
                   SerilogToEsExtensions.SetSerilogConfiguration(
                       loggerConfiguration,
                       context.Configuration);
               })
               .UseAutofac();
    }
}
