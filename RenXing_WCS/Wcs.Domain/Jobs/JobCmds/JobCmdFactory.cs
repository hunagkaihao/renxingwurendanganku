using System;
using System.Reflection;
using Volo.Abp.DependencyInjection;

namespace Wcs.Jobs.JobCmds;

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
            Assembly ass = Assembly.Load("Wcs.Domain");
            Type stepClassType = ass.GetType($"Wcs.Jobs.JobCmds.{stepName}");
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