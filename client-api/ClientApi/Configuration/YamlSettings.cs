using Microsoft.Extensions.Configuration.Json;
using NetEscapades.Configuration.Yaml;

namespace ClientApi.Configuration;

/// <summary>应用配置源的 YAML 迁移入口。</summary>
public static class YamlSettings
{
    /// <summary>以支持热重载的 YAML 替换默认应用 JSON，保留环境变量、命令行及用户机密的覆盖顺序。</summary>
    /// <param name="builder">尚未构建的 Web 应用构建器。</param>
    public static void UseYamlSettings(this WebApplicationBuilder builder)
    {
        var sources = builder.Configuration.Sources;
        for (var index = 0; index < sources.Count; index++)
        {
            if (sources[index] is not JsonConfigurationSource source ||
                (source.Path != "appsettings.json" && source.Path != $"appsettings.{builder.Environment.EnvironmentName}.json"))
                continue;

            sources[index] = new YamlConfigurationSource
            {
                Path = Path.ChangeExtension(source.Path, ".yaml"),
                FileProvider = builder.Environment.ContentRootFileProvider,
                Optional = source.Path != "appsettings.json",
                ReloadOnChange = true
            };
        }
    }
}
