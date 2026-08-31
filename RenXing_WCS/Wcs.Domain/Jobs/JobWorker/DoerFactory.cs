using System;
using System.Reflection;
using Volo.Abp.DependencyInjection;

namespace Wcs.Jobs.JobWorker
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
                Assembly ass = Assembly.Load("Wcs.Domain");
                Type jobClassType = ass.GetType($"Wcs.Jobs.JobWorker.{jobWorkerName}");
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