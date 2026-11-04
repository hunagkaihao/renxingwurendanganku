using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Application.Dtos;
using Lion.AbpPro.Extension.Customs.Dtos;
using WarehouseManagement.Warehouses.Dto;

namespace WarehouseManagement.Warehouses
{
    public interface IWarehouseAppService : IApplicationService
    {

        /// <summary>
        /// 新增仓库
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<WarehouseDto> CreateAsync(CreateWarehouseDto input);

        //Task<WarehouseDto> CreateStationAsync(CreateStationDto input);
        /// <summary>
        /// 分页查询书籍
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<PagedResultDto<WarehouseDto>> GetPagingListAsync(PagingWarehouseListInput input);
        /// <summary>
        /// 更新仓库
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<WarehouseDto> UpdateAsync(UpdateWarehouseDto input);
        /// <summary>
        /// 删除仓库
        /// </summary>
        Task DeleteAsync(IdIntInput input);
        /// <summary>
        /// 新增仓库
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<WarehouseAreaDto> CreateAreaAsync(CreateWarehouseAreaDto input);

        //Task<WarehouseAreaDto> CreateStationAsync(CreateStationDto input);
        /// <summary>
        /// 分页查询书籍
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<PagedResultDto<WarehouseAreaDto>> GetAreaPagingListAsync(PagingWarehouseAreaListInput input);
        /// <summary>
        /// 更新仓库
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<WarehouseAreaDto> UpdateAreaAsync(UpdateWarehouseAreaDto input);
        /// <summary>
        /// 删除仓库
        /// </summary>
        Task DeleteAreaAsync(IdIntInput input);


    }
}
