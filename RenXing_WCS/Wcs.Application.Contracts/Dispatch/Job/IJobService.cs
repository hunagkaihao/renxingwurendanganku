using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Wcs.Dispatch;

public interface IJobService : IApplicationService
{
    public Task<ResponseDto> JobCmdSeedsAsync();
    public Task<ResponseDto> DelAllJobCmdsAsync();
    public Task<List<JobCmdDto>> GetAllJobCmdsAsync();
    public Task<ResponseDto> JobWorkerSeedsAsync();
    public Task<ResponseDto> DelAllJobWorkersAsync();
    public Task<List<JobWorkerDto>> GetAllJobWorkersAsync();
}