using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Wcs.Dispatch;

public interface IDahSpecService : IApplicationService
{
    public Task<ResponseDto> AddDahSpecAsync(AddDahSpecDto spec);

    public Task<ResponseDto> DelAllDahSpecsAsync();

    public Task<List<DahSpecDto>> GetAllDahSpecsAsync();
}