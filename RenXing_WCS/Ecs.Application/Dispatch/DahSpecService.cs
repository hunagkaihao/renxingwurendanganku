using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ecs.DahSpecss;
using Ecs.DahSpecss.Models;
using Ecs.LogTool;
using Microsoft.Extensions.Logging;

namespace Ecs.Dispatch;

public class DahSpecService : EcsAppService, IDahSpecService
{
    private readonly DahSpecsManager _dahSpecManager;
    private readonly IDahSpecsRepository _dahSpecsRepository;
    private readonly ILogger<DahSpecService> _logger;

    public DahSpecService(
        DahSpecsManager dahSpecManager,
        IDahSpecsRepository dahSpecsRepository,
        ILogger<DahSpecService> logger)
    {
        _dahSpecManager = dahSpecManager;
        _dahSpecsRepository = dahSpecsRepository;
        _logger = logger;
    }

    public async Task<ResponseDto> AddDahSpecAsync(AddDahSpecDto spec)
    {
        try
        {
            bool ret = await _dahSpecManager.AddDahSpecAsync(spec.SpecCode, spec.SpecName, spec.SpecValue).ConfigureAwait(false);
            return new ResponseDto(){ success = ret, message = ret ? "添加成功" : "添加失败" };
        }
        catch(Exception ex)
        {
            _logger.Error(ex.Message);
            return new ResponseDto(){ success = false, message = ex.Message };
        }
    }

    public async Task<ResponseDto> DelAllDahSpecsAsync()
    {
        try
        {
            await _dahSpecsRepository.DeleteAsync(o => o.Id > 0).ConfigureAwait(false);
            return new ResponseDto(){ success = true, message = "删除成功" };
        }
        catch(Exception ex)
        {
            _logger.Error(ex.Message);
            return new ResponseDto(){ success = false, message = ex.Message };
        }
    }

    public async Task<List<DahSpecDto>> GetAllDahSpecsAsync()
    {
        try
        {
            var specs = await _dahSpecsRepository.GetAllDahSpecsAsync().ConfigureAwait(false);
            specs = specs == null ? new List<DahSpecs>() : specs;
            return ObjectMapper.Map<List<DahSpecs>, List<DahSpecDto>>((List<DahSpecs>)specs);
        }
        catch(Exception ex)
        {
            _logger.Error(ex.Message);
            return new List<DahSpecDto>();
        }
    }
}