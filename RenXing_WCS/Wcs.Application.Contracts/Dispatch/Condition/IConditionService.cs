using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Wcs.Dispatch;

public interface IConditionService : IApplicationService
{
    public Task<ResponseDto> ConditionSeedsAsync();

    public Task<ResponseDto> DelAllConditionsAsync();

    public Task<List<ConditionDto>> GetAllContitionsAsync();
}