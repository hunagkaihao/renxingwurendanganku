using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Wcs.Mjj;

public interface IMjjService : IApplicationService
{
    public Task<MjjStatusDto> GetStatusAsync();

    public Task<List<MjjStatusNmValMapDto>> GetStatusInNmValMapFormAsync();

    public Task<ResponseDto> OpenAsync(MjjOpenDto para);

    public Task<ResponseDto> ResetAsync();

    public Task<ResponseDto> MoveLeftAsync(byte colNo);

    public Task<ResponseDto> MoveRightAsync(byte colNo);
}