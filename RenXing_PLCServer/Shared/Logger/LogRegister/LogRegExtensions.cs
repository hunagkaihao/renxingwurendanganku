using Microsoft.Extensions.DependencyInjection;
using Shared.Logger.ILogger;
using Shared.Logger.LogByLog4Net;
using Shared.Config;
using System.Reflection;

namespace Shared.Logger.LogRegister
{
    public static class LogRegExtensions
    {
        public static IServiceCollection RegisterLogger(this IServiceCollection services)
        {
            var assembly = Assembly.Load("Shared");
            var typeList = assembly.GetTypes().Where(t => t.Name == Settings.ConfigData.LoggerClass).ToList();
            Type? loggerType = typeList.FirstOrDefault();
            if (loggerType == null)
            {
                loggerType = typeof(Log4NetLogger);
            }
            services.AddSingleton(typeof(ILog), loggerType);
            return services;
        }
    }
}
