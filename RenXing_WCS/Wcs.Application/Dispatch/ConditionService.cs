using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wcs.Conditions;
using Wcs.Conditions.Models;
using Wcs.LogTool;
using Microsoft.Extensions.Logging;
using Volo.Abp.Domain.Repositories;

namespace Wcs.Dispatch;

public class ConditionService : WcsAppService, IConditionService
{
    private readonly ILogger<ConditionService> _logger;
    private readonly ConditionManager _conditionManager;
    private readonly IRepository<DispatchCondition, int> _conditionRepository;

    public ConditionService(
        ILogger<ConditionService> logger,
        ConditionManager conditionManager,
        IRepository<DispatchCondition, int> conditionRepository)
    {
        _logger = logger;
        _conditionManager = conditionManager;
        _conditionRepository = conditionRepository;
    }

    public async Task<ResponseDto> ConditionSeedsAsync()
    {
        try
        {

            await _conditionRepository.InsertAsync(
                await _conditionManager.CreateDispatchCondition("Plc1.Lm_State", "PLC", "龙门状态"));
            await CurrentUnitOfWork.SaveChangesAsync().ConfigureAwait(false);

            //await _conditionRepository.InsertAsync(
            //    await _conditionManager.CreateDispatchCondition("Plc1.Lm_Zero", "PLC", "龙门原点"));
            //await CurrentUnitOfWork.SaveChangesAsync().ConfigureAwait(false);

            //await _conditionRepository.InsertAsync(
            //    await _conditionManager.CreateDispatchCondition("Plc1.Lm_SafePos", "PLC", "龙门避让位"));
            //await CurrentUnitOfWork.SaveChangesAsync().ConfigureAwait(false);

            //await _conditionRepository.InsertAsync(
            //    await _conditionManager.CreateDispatchCondition("IsPower", "Mjj.Status", "密集架是否上电"));
            //await CurrentUnitOfWork.SaveChangesAsync().ConfigureAwait(false);

            //await _conditionRepository.InsertAsync(
            //    await _conditionManager.CreateDispatchCondition("ColumnStatus", "Mjj.Status", "密集架列状态"));
            //await CurrentUnitOfWork.SaveChangesAsync().ConfigureAwait(false);


            return new ResponseDto(){ success = true, message = "添加成功" };
        }
        catch(Exception ex)
        {
            _logger.Error(ex.Message);
            return new ResponseDto(){ success = false, message = ex.Message };
        }
    }

    public async Task<ResponseDto> DelAllConditionsAsync()
    {
        try
        {
            var conditions = await _conditionRepository.GetListAsync().ConfigureAwait(false);
            if (conditions == null || conditions.Count == 0)
                return new ResponseDto() { success = true, message = "删除成功" };

            foreach(var condition in conditions)
                await _conditionRepository.DeleteAsync(condition).ConfigureAwait(false);

            return new ResponseDto() { success = true, message = "删除成功" };
        }
        catch(Exception ex)
        {
            _logger.Error(ex.Message);
            return new ResponseDto(){ success = false, message = ex.Message };
        }
    }

    public async Task<List<ConditionDto>> GetAllContitionsAsync()
    {
        try
        {
            var conditions = await _conditionRepository.GetListAsync().ConfigureAwait(false);
            if(conditions == null || conditions.Count == 0)
                return new List<ConditionDto>();

            conditions = conditions.OrderBy(o => o.Id).ToList();
            return ObjectMapper.Map<List<DispatchCondition>, List<ConditionDto>>(conditions);
        }
        catch(Exception ex)
        {
            _logger.Error(ex.Message);
            return new List<ConditionDto>();
        }
    }
}