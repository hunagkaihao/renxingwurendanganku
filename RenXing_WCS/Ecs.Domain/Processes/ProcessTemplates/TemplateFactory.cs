using System;
using System.Reflection;
using Volo.Abp.DependencyInjection;

namespace Ecs.Processes.ProcessTemplates;

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
            Assembly ass = Assembly.Load("Ecs.Domain");
            Type templateClassType = ass.GetType($"Ecs.Processes.ProcessTemplates.{templateName}");
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