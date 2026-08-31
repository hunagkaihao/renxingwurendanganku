using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Wcs.Dispatch;

public interface IProcessService : IApplicationService
{
    public Task<ResponseDto> ProcessSeedAsync(AddProcessDto path);

    public Task<ResponseDto> DelAllProcessesAsync();
}