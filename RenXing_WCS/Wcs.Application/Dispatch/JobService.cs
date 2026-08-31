using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wcs.Jobs;
using Wcs.Jobs.Models;
using Wcs.LogTool;
using Microsoft.Extensions.Logging;

namespace Wcs.Dispatch;

public class JobService : WcsAppService, IJobService
{
    private readonly JobManager _jobCmdManager;
    private readonly ILogger<JobService> _logger;

    public JobService(
        JobManager jobCmdManager,
        ILogger<JobService> logger)
    {
        _jobCmdManager = jobCmdManager;
        _logger = logger;
    }

    public async Task<ResponseDto> DelAllJobCmdsAsync()
    {
        try
        {
            bool ret = await _jobCmdManager.DelAllJobCmdsAsync().ConfigureAwait(false);
            return new ResponseDto() { success = ret, message = ret ? "删除成功" : "删除失败" };
        }
        catch(Exception ex)
        {
            _logger.Error(ex.Message);
            return new ResponseDto() { success = false, message = ex.Message };
        }
    }

    public async Task<ResponseDto> DelAllJobWorkersAsync()
    {
        try
        {
            bool ret = await _jobCmdManager.DelAllJobWorkersAsync().ConfigureAwait(false);
            return new ResponseDto() { success = ret, message = ret ? "删除成功" : "删除失败" };
        }
        catch(Exception ex)
        {
            _logger.Error(ex.Message);
            return new ResponseDto() { success = false, message = ex.Message };
        }
    }

    public async Task<List<JobCmdDto>> GetAllJobCmdsAsync()
    {
        try
        {
            var cmds = await _jobCmdManager.GetAllJobCmdsAsync().ConfigureAwait(false);
            if(cmds == null)
                return new List<JobCmdDto>();
            cmds = cmds.OrderBy(o => o.Id).ToList();
            return ObjectMapper.Map<List<DispatchJobCmd>, List<JobCmdDto>>(cmds);
        }
        catch(Exception ex)
        {
            _logger.Error(ex.Message);
            return new List<JobCmdDto>();
        }
    }

    public async Task<List<JobWorkerDto>> GetAllJobWorkersAsync()
    {
        try
        {
            var workers = await _jobCmdManager.GetAllJobWorkersAsync().ConfigureAwait(false);
            if(workers == null)
                return new List<JobWorkerDto>();
            workers = workers.OrderBy(o => o.Id).ToList();
            return ObjectMapper.Map<List<DispatchJobWorker>, List<JobWorkerDto>>(workers);
        }
        catch(Exception ex)
        {
            _logger.Error(ex.Message);
            return new List<JobWorkerDto>();
        }
    }

    public async Task<ResponseDto> JobCmdSeedsAsync()
    {
        try
        {
            await _jobCmdManager.AddJobCmdAsync(new DispatchJobCmd(1, "OpenDoorCmd", "取档口打开"));
            await _jobCmdManager.AddJobCmdAsync(new DispatchJobCmd(2, "LMToZeroPosCmd", "龙门回原点"));
            await _jobCmdManager.AddJobCmdAsync(new DispatchJobCmd(3, "LMToSafePosCmd", "龙门回避让位"));
            await _jobCmdManager.AddJobCmdAsync(new DispatchJobCmd(4, "LMReadCellCmd", "龙门读库位信息"));
            await _jobCmdManager.AddJobCmdAsync(new DispatchJobCmd(5, "LMAtZeroPosJudgeCmd", "龙门在原点判断"));
            await _jobCmdManager.AddJobCmdAsync(new DispatchJobCmd(6, "LMAtSafePosJudgeCmd", "龙门在避让位判断"));
            await _jobCmdManager.AddJobCmdAsync(new DispatchJobCmd(7, "LMInPickCmd", "龙门入库取货"));
            await _jobCmdManager.AddJobCmdAsync(new DispatchJobCmd(8, "LMInPlaceCmd", "龙门入库放货"));
            await _jobCmdManager.AddJobCmdAsync(new DispatchJobCmd(9, "LMOutPickCmd", "龙门出库取货"));
            await _jobCmdManager.AddJobCmdAsync(new DispatchJobCmd(10, "LMOutPlaceCmd", "龙门出库放货"));
            await _jobCmdManager.AddJobCmdAsync(new DispatchJobCmd(11, "LMMovePickCmd", "龙门移库取货"));
            await _jobCmdManager.AddJobCmdAsync(new DispatchJobCmd(12, "LMMovePlaceCmd", "龙门移库放货"));            
            await _jobCmdManager.AddJobCmdAsync(new DispatchJobCmd(13, "XnLastChkOdJudgeCmd", "最后一个盘点订单判断"));
            await _jobCmdManager.AddJobCmdAsync(new DispatchJobCmd(14, "XnLastChkTaskJudgeCmd", "最后一个盘点任务判断"));
            await _jobCmdManager.AddJobCmdAsync(new DispatchJobCmd(15, "XnLastInTaskJudgeCmd", "最后一个入库任务判断"));
            await _jobCmdManager.AddJobCmdAsync(new DispatchJobCmd(16, "XnLastOutTaskJudgeCmd", "最后一个出库任务判断"));
            await _jobCmdManager.AddJobCmdAsync(new DispatchJobCmd(17, "XnLastMoveTaskJudgeCmd", "最后一个移库任务判断"));
            await _jobCmdManager.AddJobCmdAsync(new DispatchJobCmd(18, "NullCmd", "空命令"));
            await _jobCmdManager.AddJobCmdAsync(new DispatchJobCmd(19, "AllocateCacheCmd", "分配缓存"));
            await _jobCmdManager.AddJobCmdAsync(new DispatchJobCmd(20, "XnCacheAllocatedJudgeCmd", "是否分配到缓存判断"));
            
            return new ResponseDto(){ success = true, message = "success" };           
        }
        catch(Exception e)
        {
            _logger.Error(e.Message);
            return new ResponseDto(){ success = false, message = e.Message };
        }
    }

    public async Task<ResponseDto> JobWorkerSeedsAsync()
    {
        try
        {
            await _jobCmdManager.AddJobWorkerAsync(new DispatchJobWorker(){
                JobWorkerClassName = "DefaultJobWorker", Describe = "默认job执行器"
            });
            await _jobCmdManager.AddJobWorkerAsync(new DispatchJobWorker(){
                JobWorkerClassName = "JudgeJobWorker", Describe = "条件判断执行器"
            });
            return new ResponseDto(){ success = true, message = "" };            
        }
        catch(Exception e)
        {
            _logger.Error(e.Message);
            return new ResponseDto(){ success = false, message = e.Message };
        }
    }
}