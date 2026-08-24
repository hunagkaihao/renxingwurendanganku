using System;
using System.Threading.Tasks;
using Ecs.DahSpecss.Models;
using Ecs.LogTool;
using Microsoft.Extensions.Logging;
using Volo.Abp.DependencyInjection;

namespace Ecs.DahSpecss;

public class DahSpecsManager : ISingletonDependency
{
    private readonly IDahSpecsRepository _specRepository;
    private readonly ILogger<DahSpecsManager> _logger;

    public DahSpecsManager(
        IDahSpecsRepository specRepository,
        ILogger<DahSpecsManager> logger
    )
    {
        _specRepository = specRepository;
        _logger = logger;
    }

    public async Task<bool> AddDahSpecAsync(string specCode, string specName, int specValue)
    {
        try
        {
            DahSpecs spec = new DahSpecs(specCode, specName, specValue);
            var specExist = await _specRepository.GetListAsync(
                o => o.SpecCode == specCode || o.SpecName == specName || o.SpecValue == specValue)
                .ConfigureAwait(false);

            if (specExist.Count > 0)
                throw new Exception($"已存在规格名为{specName}或规格号为{specCode}或规格值为{specValue}的档案盒规格，添加失败");

            await _specRepository.InsertAsync(spec).ConfigureAwait(false);
            return true;
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            return false;
        }
    }

}