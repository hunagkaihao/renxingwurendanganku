using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NetEscapades.Configuration.Yaml;
using Serilog.Extensions.Logging;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Wcs;

public class Program
{
    public async static Task<int> Main(string[] args)
    {
        var loggingInitialized = false;
        try
        {
            Console.OutputEncoding = new UTF8Encoding(false);
            Serilog.Log.Logger = WcsConsoleLogging.CreateLogger();
            loggingInitialized = true;
            Serilog.Log.Information("正在启动 WCS 服务，准备加载 YAML 配置。");

            var builder = WebApplication.CreateBuilder(args);
            builder.Logging.ClearProviders();
            // 保留默认 ILoggerFactory，后续 SQLite provider 仍接收原始 EventId 和日志级别。
            builder.Logging.AddProvider(new SerilogLoggerProvider(Serilog.Log.Logger, dispose: false));
            // .NET 6 exposes Sources through IConfigurationBuilder explicitly.
            var configurationSources = ((IConfigurationBuilder)builder.Configuration).Sources;
            // Replace only appsettings sources in place to preserve configuration precedence.
            for (var i = 0; i < configurationSources.Count; i++)
            {
                if (configurationSources[i] is JsonConfigurationSource source &&
                    (source.Path == "appsettings.json" ||
                     source.Path == $"appsettings.{builder.Environment.EnvironmentName}.json"))
                {
                    configurationSources[i] = new YamlConfigurationSource
                    {
                        Path = Path.ChangeExtension(source.Path, ".yaml"),
                        FileProvider = source.FileProvider,
                        Optional = source.Optional,
                        ReloadOnChange = false
                    };
                }
            }

            builder.Host.UseSystemd();
            builder.Host.UseAutofac();
            await builder.AddApplicationAsync<WcsHttpApiHostModule>();

            var configuration = builder.Services.GetConfiguration();
            string[] urls = configuration["Wcs:BaseUrl"].Split(",", System.StringSplitOptions.RemoveEmptyEntries);
            builder.WebHost.UseUrls(urls);

            var app = builder.Build();
            // app.UseMiddleware<ApiLogMidware>();
            await app.InitializeApplicationAsync();
            app.MapHub<WcsSignalHub>("/hub");

            using var startedRegistration = app.Lifetime.ApplicationStarted.Register(() =>
            {
                Serilog.Log.Information("WCS 服务已启动。按 Ctrl+C 停止服务。");
                Serilog.Log.Information("运行环境：{EnvironmentName}；内容根目录：{ContentRootPath}",
                    app.Environment.EnvironmentName, app.Environment.ContentRootPath);
                Serilog.Log.Information("监听地址：{Addresses}", string.Join(", ", app.Urls));
            });
            using var stoppingRegistration = app.Lifetime.ApplicationStopping.Register(() =>
                Serilog.Log.Information("WCS 服务正在停止，请等待后台任务退出。"));
            using var stoppedRegistration = app.Lifetime.ApplicationStopped.Register(() =>
                Serilog.Log.Information("WCS 服务已停止。"));

            await app.RunAsync();
            return 0;
        }
        catch(Exception ex)
        {
            // 保留完整异常及内部异常链，不翻译第三方异常，便于定位依赖注入和连接失败。
            if (loggingInitialized)
                Serilog.Log.Fatal(ex, "WCS 服务启动或运行失败，请查看以下异常及内部异常信息。");
            else
                Console.Error.WriteLine("WCS 日志初始化失败：{0}", ex);
            return 1;
        }
        finally
        {
            Serilog.Log.CloseAndFlush();
        }
    }
}
