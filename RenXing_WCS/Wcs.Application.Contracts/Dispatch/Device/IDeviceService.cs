using Wcs.Dispatch.Device;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Wcs.Dispatch;

public interface IDeviceService : IApplicationService
{
    public Task<DeviceConnStatesDto> GetDeviceConnStateAsync();

    public ResponseDto OpenDoorAsync(string doorCode);

    public Task<DoorStateDto> GetDoorStateAsync(string doorCode);
}