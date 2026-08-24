using Lion.AbpPro.Jobs;
using Lion.AbpPro.Localization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Lion.AbpPro
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddApplication<AbpProHttpApiHostModule>();
            services.AddHostedService<ScheduledDemoJob>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            app.InitializeApplication();
            app.InitializeLocalization();
        }
        
    }
}
