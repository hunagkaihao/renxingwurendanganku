using Hangfire;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Volo.Abp;

namespace Lion.AbpPro.Jobs
{
    public static class RecurringJobsExtensions
    {
        public static void CreateRecurringJob(this ApplicationInitializationContext context)
        {
            //using var scope = context.ServiceProvider.CreateScope();
            //var testJob =
            //    scope.ServiceProvider.GetService<TestJob>();
            //RecurringJob.AddOrUpdate("测试Job", () => testJob.ExecuteAsync(), CronType.Minute(1));
            //Log.Information("测试MyJob");
            //Log.Error("测试MyJob");
            //Log.Fatal("测试MyJob");
            //var myJob =
            //scope.ServiceProvider.GetService<MyJob>();
            //RecurringJob.AddOrUpdate("测试MyJob", () => myJob.ExecuteAsync(), CronType.Minute(1));
        }
    }
}