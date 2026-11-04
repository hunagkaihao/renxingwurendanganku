using WarehouseManagement.Goodss.Dto;
using WarehouseManagement.Permissions;
using Lion.AbpPro.Extension.Customs.Dtos;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;
using WarehouseManagement.LogFiles.Dto;

namespace WarehouseManagement.LogFiles
{
    [Authorize(WarehouseManagementPermissions.GoodsManagement.Default)]
    public class LogFileAppService : WarehouseManagementAppService,
         ILogFileAppService //implement the IGoodsAppService
    {
        //private readonly IRepository<Goods, Guid> _goodsRepository;
        /// <summary>
        ///  注意 为了快速直接注入仓库层 规范上是不允许的
        ///  这里注入仓储也只是为了查询分页
        ///  如果是其他的操作全部通过对应manger进行操作
        /// </summary>
        private readonly LogFileManager _logFileManagement;
        public LogFileAppService(LogFileManager logFileManagement)
        {
            _logFileManagement = logFileManagement;
        }


        public async Task<PagedResultDto<LogFileDto>> GetPagingListAsync()
        {
            var result = new PagedResultDto<LogFileDto>();
            var entities = await _logFileManagement.GetListAsync();
            result.TotalCount = entities.Count;
            result.Items = ObjectMapper.Map<List<LogFile>, List<LogFileDto>>(entities);
            return result;
        }   

    }
}
