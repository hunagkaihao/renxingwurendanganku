using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Application.Dtos;
using Lion.AbpPro.Extension.Customs.Dtos;
using WarehouseManagement.Plans.Dto;

namespace WarehouseManagement.Plans
{
    public interface IPlanAppService : IApplicationService
    {

        Task<PagedResultDto<PlanDto>> GetPagingListAsync(PagingPlanListInput input);

        /// <summary>
        /// 修改计划
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<PlanDto> UpdateAsync(UpdatePlanDto input);

        //Task UpdateDetailAsync(UpdatePlanDetailDto input);
        /// <summary>
        /// 删除计划
        /// </summary>
        Task DeleteAsync(IdIntInput input);

        //创建批量入库计划
        Task CreatePlanAsync(CreatePlanDto createPlanDto);

        Task<bool> SetExecuting(int planId);
        //创建疲劳任务计划





    }
}
