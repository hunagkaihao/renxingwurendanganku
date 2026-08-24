using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Ecs.Station
{
    public interface IStationService : IApplicationService
    {
        public Task<List<StationInfoDto>> GetInformationsAsync(string stationCode);
    }
}