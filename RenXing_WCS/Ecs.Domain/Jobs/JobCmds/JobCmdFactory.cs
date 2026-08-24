using System;
using System.Reflection;
using Volo.Abp.DependencyInjection;

namespace Ecs.Jobs.JobCmds;

public class JobCmdFactory : ITransientDependency
{
    private IServiceProvider _serviceProvider;

    public JobCmdFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IJobCmd CreateStep(string stepName)
    {
        try
        {
            Assembly ass = Assembly.Load("Ecs.Domain");
            Type stepClassType = ass.GetType($"Ecs.Jobs.JobCmds.{stepName}");
            if (stepClassType == null)
            {
                return null;
            }

            return (IJobCmd)_serviceProvider.GetService(stepClassType);
        }
        catch
        {
            return null;
        }
    }
}