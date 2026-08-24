using Lion.AbpPro.Extension.Customs.Dtos;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using WarehouseManagement.Goodss.Dto;

namespace WarehouseManagement.Goodss
{
    [Route("Goodss")]
    public class GoodssController : WarehouseManagementController, IGoodsAppService
    {
        private readonly IGoodsAppService _goodsAppService;

        public GoodssController(IGoodsAppService goodsAppService)
        {
            _goodsAppService = goodsAppService;
        }

        [HttpPost("create")]
        [SwaggerOperation(summary: "创建物料", Tags = new[] { "Goodss" })]
        public async Task<GoodsDto> CreateAsync(CreateGoodsDto input)
        {
            return await _goodsAppService.CreateAsync(input);
        }
        [HttpPost("createMany")]
        [SwaggerOperation(summary: "批量创建物料", Tags = new[] { "Goodss" })]
        public async Task CreateManyAsync(List<GoodsBaseDto> inputs)
        {
            await _goodsAppService.CreateManyAsync(inputs);
        }
        [HttpPost("delete")]
        [SwaggerOperation(summary: "删除物料", Tags = new[] { "Goodss" })]
        public async Task DeleteAsync(IdIntInput input)
        {
            await _goodsAppService.DeleteAsync(input);
        }
        [HttpPost("findByCode")]
        [SwaggerOperation(summary: "通过编码查询物料", Tags = new[] { "Goodss" })]
        public async Task<GoodsDto> FindByCodeAsync(PagingGoodsListInput input)
        {
            return await _goodsAppService.FindByCodeAsync(input);
        }
        [HttpPost("page")]
        [SwaggerOperation(summary: "获取清单", Tags = new[] { "Goodss" })]
        public async Task<PagedResultDto<GoodsDto>> GetPagingListAsync(PagingGoodsListInput input)
        {
            return await _goodsAppService.GetPagingListAsync(input); ;
        }
        [HttpPost("update")]
        [SwaggerOperation(summary: "更新物料", Tags = new[] { "Goodss" })]
        public async Task<GoodsDto> UpdateAsync(UpdateGoodsDto input)
        {
            return await _goodsAppService.UpdateAsync(input);
        }
        [HttpPost("getSelectOptions")]
        [SwaggerOperation(summary: "获取select清单", Tags = new[] { "Goodss" })]
        public async Task<List<GoodsSelectDto>> GetSelectOptionsByNameAsync(PagingGoodsListInput input)
        {
            return await _goodsAppService.GetSelectOptionsByNameAsync(input);
        }
    }
}
