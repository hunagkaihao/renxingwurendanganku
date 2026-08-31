using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Wcs.LogTool;
using Microsoft.Extensions.Logging;

namespace Wcs.Mjj;

public class MjjService : WcsAppService, IMjjService
{
    private readonly MjjManager _mjjManager;
    private readonly ILogger<MjjService> _logger;

    public MjjService(MjjManager mjjManager, ILogger<MjjService> logger)
    {
        _mjjManager = mjjManager;
        _logger = logger;
    }

    public async Task<MjjStatusDto> GetStatusAsync()
    {
        try
        {
            MjjStatus status = await _mjjManager.GetMjjStatusAync().ConfigureAwait(false);
            if(status == null)
                return new MjjStatusDto();
            else
                return ObjectMapper.Map<MjjStatus, MjjStatusDto>(status); //属性名称大小写不敏感的，属性数量可以有差异
        }
        catch(Exception e)
        {
            _logger.Error(e.Message);
            return new MjjStatusDto();
        }
    }

    public async Task<List<MjjStatusNmValMapDto>> GetStatusInNmValMapFormAsync()
    {
        try
        {
            MjjStatus status = await _mjjManager.GetMjjStatusAync().ConfigureAwait(false);
            if(status == null)
                return new List<MjjStatusNmValMapDto>();
            else
            {
                List<MjjStatusNmValMapDto> result = new List<MjjStatusNmValMapDto>();
                Type type = status.GetType();
                foreach(var pro in type.GetProperties())
                {
                    result.Add(new MjjStatusNmValMapDto(){
                        tagName = pro.Name,
                        tagValue = pro.GetValue(status).ToString()
                    });
                }
                return result;
            }
        }
        catch(Exception e)
        {
            _logger.Error(e.Message);
            return new List<MjjStatusNmValMapDto>();
        }
    }

    public async Task<ResponseDto> MoveLeftAsync(byte colNo)
    {
        try
        {
            var ret = await _mjjManager.MoveLeftAsync(colNo).ConfigureAwait(false);
            return new ResponseDto(){ success = ret.success, message = ret.errMsg };
        }
        catch(Exception ex)
        {
            _logger.Error(ex.Message);
            return new ResponseDto(){ success = false, message = ex.Message };
        }
    }

    public async Task<ResponseDto> MoveRightAsync(byte colNo)
    {
        try
        {
            var ret = await _mjjManager.MoveRightAsync(colNo).ConfigureAwait(false);
            return new ResponseDto(){ success = ret.success, message = ret.errMsg };
        }
        catch(Exception ex)
        {
            _logger.Error(ex.Message);
            return new ResponseDto(){ success = false, message = ex.Message };
        }
    }

    public async Task<ResponseDto> OpenAsync(MjjOpenDto para)
    {
        try
        {
            var ret = await _mjjManager.OpenMjjAsync(para.colNo, para.zyNo, para.state).ConfigureAwait(false);
            return new ResponseDto(){ success = ret.success, message = ret.errMsg };
        }
        catch(Exception ex)
        {
            _logger.Error(ex.Message);
            return new ResponseDto(){ success = false, message = ex.Message };
        }
    }

    public async Task<ResponseDto> ResetAsync()
    {
        try
        {
            var ret = await _mjjManager.ResetMjjAsync().ConfigureAwait(false);
            return new ResponseDto(){ success = ret.success, message = ret.errMsg };
        }
        catch(Exception ex)
        {
            _logger.Error(ex.Message);
            return new ResponseDto(){ success = false, message = ex.Message };
        }
    }
}