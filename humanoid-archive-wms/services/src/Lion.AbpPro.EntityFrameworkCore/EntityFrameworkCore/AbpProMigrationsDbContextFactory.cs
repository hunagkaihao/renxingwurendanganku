using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using NetEscapades.Configuration.Yaml;

namespace Lion.AbpPro.EntityFrameworkCore
{
    /* This class is needed for EF Core console commands
     * (like Add-Migration and Update-Database commands) */
    public class AbpProMigrationsDbContextFactory : IDesignTimeDbContextFactory<AbpProDbContext>
    {
        public AbpProDbContext CreateDbContext(string[] args)
        {
            AbpProEfCoreEntityExtensionMappings.Configure();

            var configuration = BuildConfiguration();
            //string conn = "Data Source=localhost;port=3306;Database=daktuta;uid=root;pwd=123456;charset=utf8mb4;Allow User Variables=true;AllowLoadLocalInfile=true";//configuration.GetConnectionString("Default");
            var builder = new DbContextOptionsBuilder<AbpProDbContext>()
                .UseMySql(configuration.GetConnectionString("Default"), MySqlServerVersion.LatestSupportedServerVersion);

            return new AbpProDbContext(builder.Options);
        }

        private static IConfigurationRoot BuildConfiguration()
        {
            var builder = new ConfigurationBuilder()
                // 设计时配置统一读取主 HTTP Host 的普通 YAML 配置文件。
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../../host/Lion.AbpPro.HttpApi.Host/"))
                .AddYamlFile("appsettings.yaml", optional: false, reloadOnChange: false);

            return builder.Build();
        }
    }
}
