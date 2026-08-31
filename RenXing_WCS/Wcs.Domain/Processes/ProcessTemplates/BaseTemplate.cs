using System.Collections.Generic;
using Wcs.Processes.Models;

namespace Wcs.Processes.ProcessTemplates;

public class BaseTemplate
{
    public int ProcessId { get; set; }
    public string StartNode { get; set; } = string.Empty;
    public string EndNode { get; set; } = string.Empty;
    public DispatchProcess Process { get; set; } = new DispatchProcess();
    public List<DispatchProcessStep> Details { get; set; } = new List<DispatchProcessStep>();
    public List<DispatchProcessStepPrecondition> Preconditions { get; set; } = new List<DispatchProcessStepPrecondition>();
    public List<DispatchProcessStepResource> Resources { get; set; } = new List<DispatchProcessStepResource>();

    public virtual void Build() { }
}

