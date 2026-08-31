using Wcs.LogTool;
using Wcs.Processes;
using Wcs.Processes.ProcessTemplates;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Volo.Abp;

namespace Wcs.Dispatch;

public class ProcessService : WcsAppService, IProcessService
{
    private readonly ILogger<ProcessService> _logger;
    private readonly ProcessManager _processManager;
    private readonly TemplateFactory _templateFactory;

    public ProcessService(
        ILogger<ProcessService> logger,
        ProcessManager processManager,
        TemplateFactory templateFactory)
    {
        _logger = logger;
        _processManager = processManager;
        _templateFactory = templateFactory;
    }

    public async Task<ResponseDto> DelAllProcessesAsync()
    {
        try
        {
            bool ret = await _processManager.DelAllProcessesAsync().ConfigureAwait(false);
            return new ResponseDto(){ success = ret, message = ret ? "删除成功" : "删除失败" };
        }
        catch(Exception ex)
        {
            _logger.Error(ex.Message);
            return new ResponseDto(){ success = false, message = ex.Message };
        }
    }

    public async Task<ResponseDto> ProcessSeedAsync(AddProcessDto process)
    {
        try
        {
            Check.NotNullOrEmpty(process.ProcessTemplateName, nameof(process.ProcessTemplateName));
            BaseTemplate template = _templateFactory.CreatePath(process.ProcessTemplateName);
            if(template == null)
                return new ResponseDto() { success = false, message = $"根据{process.ProcessTemplateName}创建过程失败" };
            
            template.ProcessId = Check.Positive(process.ProcessId, nameof(process.ProcessId));
            template.StartNode = Check.NotNullOrEmpty(process.StartNodeCode, nameof(process.StartNodeCode));
            template.EndNode = Check.NotNullOrEmpty(process.EndNodeCode, nameof(process.EndNodeCode));
            template.Build();

            if(await _processManager.AddProcessSeedAsync(template).ConfigureAwait(false))
                return new ResponseDto() { success = true, message = "success" };
            else
                return new ResponseDto() { success = false, message = "failed" };
        }
        catch(Exception e)
        {
            _logger.Error(e.Message);
            return new ResponseDto(){ success = false, message = e.Message };
        }
    }
}