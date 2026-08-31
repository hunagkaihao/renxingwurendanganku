using System;
using System.Reflection;
using Volo.Abp.DependencyInjection;

namespace Wcs.Processes.ProcessTemplates;

public class TemplateFactory : ITransientDependency
{
    private IServiceProvider _serviceProvider;

    public TemplateFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public BaseTemplate CreatePath(string templateName)
    {
        try
        {
            Assembly ass = Assembly.Load("Wcs.Domain");
            Type templateClassType = ass.GetType($"Wcs.Processes.ProcessTemplates.{templateName}");
            if (templateClassType == null)
            {
                return null;
            }

            return (BaseTemplate)_serviceProvider.GetService(templateClassType);
        }
        catch
        {
            return null;
        }
    }
}