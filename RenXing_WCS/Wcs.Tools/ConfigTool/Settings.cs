using Microsoft.Extensions.Configuration;
using System;

namespace Wcs.ConfigTool;

public static class Settings
{
    public static ConfigOptions Options = new ConfigOptions();

    static Settings()
    {
        IConfigurationRoot root = new ConfigurationBuilder()
                .AddYamlFile($@"{AppDomain.CurrentDomain.BaseDirectory}appsettings.yaml", optional: false, reloadOnChange: false).Build();

        root.GetSection("Wcs").Bind(Options);
    }
}
