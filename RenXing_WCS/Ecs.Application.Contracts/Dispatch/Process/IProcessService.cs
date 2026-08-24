using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Ecs.Dispatch;

public interface IProcessService : IApplicationService
{
    public Task<ResponseDto> ProcessSeedAsync(AddProcessDto path);

    public Task<ResponseDto> DelAllProcessesAsync();
}