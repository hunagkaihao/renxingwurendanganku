using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Application.Dtos;
using Lion.AbpPro.Extension.Customs.Dtos;
using WarehouseManagement.TaskHiss.Dto;

namespace WarehouseManagement.TaskHiss
{
    public interface ITaskHisAppService : IApplicationService
    {

        //Task<TaskHisDto> CreateAsync(CreateTaskHisDto input);

        Task<PagedResultDto<TaskHisDto>> GetPagingListAsync(PagingTaskHisListInput input);

        Task<PagedResultDto<TaskHisDetailDto>> GetPagingDetailListAsync(
    PagingTaskHisDetailInput input);
        //Task DeleteAsync(IdIntInput input);



        


    }
}
