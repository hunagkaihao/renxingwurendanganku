using Ecs.Dispatch.Device;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Ecs.Dispatch;

public interface IDeviceService : IApplicationService
{
    public Task<DeviceConnStatesDto> GetDeviceConnStateAsync();

    public ResponseDto OpenDoorAsync(string doorCode);

    public Task<DoorStateDto> GetDoorStateAsync(string doorCode);
}