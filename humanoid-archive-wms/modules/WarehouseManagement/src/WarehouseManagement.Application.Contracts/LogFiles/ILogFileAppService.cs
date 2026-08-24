using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Application.Dtos;
using WarehouseManagement.LogFiles.Dto;

namespace WarehouseManagement.LogFiles
{
    public interface ILogFileAppService : IApplicationService
    {
        /// <summary>
        /// 分页日志文件
        /// </summary>
        /// <returns></returns>
        Task<PagedResultDto<LogFileDto>> GetPagingListAsync();



    }
}
