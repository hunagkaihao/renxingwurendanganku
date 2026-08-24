using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Application.Dtos;
using Lion.AbpPro.Extension.Customs.Dtos;
using WarehouseManagement.Goodss.Dto;

namespace WarehouseManagement.Goodss
{
    public interface IGoodsAppService : IApplicationService
    {

        /// <summary>
        /// 新增物料
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<GoodsDto> CreateAsync(CreateGoodsDto input);
        /// <summary>
        /// 批量创建物料
        /// </summary>
        /// <param name="goodsBaseDtos"></param>
        /// <returns></returns>
        Task CreateManyAsync(List<GoodsBaseDto> inputs);
        /// <summary>
        /// 分页查询书籍
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<PagedResultDto<GoodsDto>> GetPagingListAsync(PagingGoodsListInput input);

        /// <summary>
        /// 更新书籍
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<GoodsDto> UpdateAsync(UpdateGoodsDto input);

        /// <summary>
        /// 删除书籍
        /// </summary>
        Task DeleteAsync(IdIntInput input);
        /// <summary>
        /// 通过编码查找物料
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<GoodsDto> FindByCodeAsync(PagingGoodsListInput input);
        /// <summary>
        /// 根据名称获取选项
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<List<GoodsSelectDto>> GetSelectOptionsByNameAsync(PagingGoodsListInput input);


    }
}
