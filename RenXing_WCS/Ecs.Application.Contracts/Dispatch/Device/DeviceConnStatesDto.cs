using System.Collections.Generic;

namespace Ecs.Dispatch;

public class DeviceConnState
{
    public string objectName { get; set; }
    public bool state { get; set; }
}

public class DeviceConnStatesDto
{
    public List<DeviceConnState> commuStates { get; set; }
}