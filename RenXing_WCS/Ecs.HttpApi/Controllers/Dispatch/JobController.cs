using System.Collections.Generic;
using System.Threading.Tasks;
using Ecs.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace Ecs.Dispatch;

[Route("ecs/dispatch")]
[ApiController]
public class JobController : EcsController, IJobService
{
    private readonly IJobService _jobService;

    public JobController(IJobService jobService)
    {
        _jobService = jobService;
    }

    [HttpPost("allJobCmdDel")]
    [MiddlewareFilter(typeof(ApiLogPipeline))]
    public async Task<ResponseDto> DelAllJobCmdsAsync()
    {
        return await _jobService.DelAllJobCmdsAsync().ConfigureAwait(false);
    }

    [HttpPost("allJobWorkerDel")]
    [MiddlewareFilter(typeof(ApiLogPipeline))]
    public async Task<ResponseDto> DelAllJobWorkersAsync()
    {
        return await _jobService.DelAllJobWorkersAsync().ConfigureAwait(false);
    }

    [HttpGet("allJobCmdGet")]
    [MiddlewareFilter(typeof(ApiLogPipeline))]
    public async Task<List<JobCmdDto>> GetAllJobCmdsAsync()
    {
        return await _jobService.GetAllJobCmdsAsync().ConfigureAwait(false);
    }

    [HttpGet("allJobWorkerGet")]
    [MiddlewareFilter(typeof(ApiLogPipeline))]
    public async Task<List<JobWorkerDto>> GetAllJobWorkersAsync()
    {
        return await _jobService.GetAllJobWorkersAsync().ConfigureAwait(false);
    }

    [HttpPost("jobCmdSeed")]
    [MiddlewareFilter(typeof(ApiLogPipeline))]
    public async Task<ResponseDto> JobCmdSeedsAsync()
    {
        return await _jobService.JobCmdSeedsAsync().ConfigureAwait(false);
    }

    [HttpPost("jobWorkerSeed")]
    [MiddlewareFilter(typeof(ApiLogPipeline))]
    public async Task<ResponseDto> JobWorkerSeedsAsync()
    {
        return await _jobService.JobWorkerSeedsAsync().ConfigureAwait(false);
    }
}