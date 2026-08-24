using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Application.Dtos;
using Lion.AbpPro.Extension.Customs.Dtos;
using WarehouseManagement.Cells.Dto;

namespace WarehouseManagement.Cells
{
    public interface ICellAppService : IApplicationService
    {

        /// <summary>
        /// 新增库位
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<CellDto> CreateAsync(CreateCellDto input);
        /// <summary>
        /// 批量创建库位
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task InitCreateCell(CellInitDto input);
        /// <summary>
        /// 自定义创建CELL
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<CellDto> CustomCreateAsync(CustomCreateCellDto input);
        /// <summary>
        /// 创建工作站
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<CellDto> CreateStationAsync(CreateStationDto input);
        /// <summary>
        /// 根据库位编码获取库位
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<CellDto> GetByCodeAsync(CreateCellDto input);

        //Task<CellDto> CreateStationAsync(CreateStationDto input);
        /// <summary>
        /// 分页查询书籍
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<PagedResultDto<CellDto>> GetPagingListAsync(PagingCellListInput input);
        Task<ListResultDto<CellDto>> GetCellListByZAsync(PagingCellListInput input);
        /// <summary>
        /// 更新书籍
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<CellDto> UpdateAsync(UpdateCellDto input);

        /// <summary>
        /// 删除书籍
        /// </summary>
        Task DeleteAsync(IdIntInput input);
        
        Task<ListResultDto<CellDto>> AllListAsync();

        Task SetCellEnable(UpdateCellDto input);
        Task SetCellDisable(UpdateCellDto input);


    }
}
