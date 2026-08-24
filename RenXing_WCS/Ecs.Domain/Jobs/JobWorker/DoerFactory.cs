using System;
using System.Reflection;
using Volo.Abp.DependencyInjection;

namespace Ecs.Jobs.JobWorker
{
    public class JobWorkerFactory : ITransientDependency
    {
        private IServiceProvider _serviceProvider;

        public JobWorkerFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IJobWorker CreateJobWorker(string jobWorkerName)
        {
            try
            {
                Assembly ass = Assembly.Load("Ecs.Domain");
                Type jobClassType = ass.GetType($"Ecs.Jobs.JobWorker.{jobWorkerName}");
                if (jobClassType == null)
                {
                    return null;
                }

                return (IJobWorker)_serviceProvider.GetService(jobClassType);
            }
            catch
            {
                return null;
            }
        }
    }
}