using Wcs.ConfigTool;
using Wcs.PlcTool;
using Wcs.RedisTool;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using Volo.Abp.Modularity;

namespace Wcs;

[DependsOn(
)]
public class WcsToolsModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.Configure<ConfigOptions>(option =>
        {
            IConfigurationRoot root = new ConfigurationBuilder()
                .AddYamlFile($@"{AppDomain.CurrentDomain.BaseDirectory}appsettings.yaml", optional: false, reloadOnChange: false).Build();

            root.GetSection("Wcs").Bind(option);
        });
        context.Services.AddTransient<IRedisClient, RedisClientByStaEx>();
    }
}
