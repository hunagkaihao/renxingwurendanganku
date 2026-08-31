using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Wcs.PlcMonitor
{
    public interface IPlcMonitorService : IApplicationService
    {
        public Task<List<MonitorDto>> GetMonitorsAsync();

        // public Task<Dictionary<string, string>> GetMonitorsInDicFormAsync();
    }
}
