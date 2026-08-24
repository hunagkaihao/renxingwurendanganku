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
using WarehouseManagement.Goodss.Aggregates;

namespace WarehouseManagement.Goodss
{
    [Authorize(WarehouseManagementPermissions.GoodsManagement.Default)]
    public class GoodsAppService : WarehouseManagementAppService,
         IGoodsAppService //implement the IGoodsAppService
    {
        //private readonly IRepository<Goods, Guid> _goodsRepository;
        /// <summary>
        ///  注意 为了快速直接注入仓库层 规范上是不允许的
        ///  这里注入仓储也只是为了查询分页
        ///  如果是其他的操作全部通过对应manger进行操作
        /// </summary>
        private readonly IGoodsRepository _goodsRepository;
        private readonly GoodsManager _goodsManagement;
        public GoodsAppService(IGoodsRepository goodsRepository, GoodsManager goodsManagement)
        {
            _goodsRepository = goodsRepository;
            _goodsManagement = goodsManagement;
            //_goodsManager = goodsManager;
            //GetPolicyName = GoodsStorePermissions.Goodss.Default;
            //GetListPolicyName = GoodsStorePermissions.Goodss.Default;
            //CreatePolicyName = GoodsStorePermissions.Goodss.Create;
            //UpdatePolicyName = GoodsStorePermissions.Goodss.Edit;
            //DeletePolicyName = GoodsStorePermissions.Goodss.Delete;
        }
        [Authorize(WarehouseManagementPermissions.GoodsManagement.Create)]
        public async Task<GoodsDto> CreateAsync(CreateGoodsDto input)
        {
            //var goodsEntity = base.ObjectMapper.Map<CreateGoodsDto, Goods>(input);
            //var goods=  await _goodsRepository.InsertAsync(goodsEntity);
            var goods = await _goodsManagement.CreateAsync(input.GoodsCode, input.GoodsName, input.GoodsSpec, input.GoodsConstProperty1
                , input.GoodsUnits);
            return  base.ObjectMapper.Map<Goods, GoodsDto>(goods);
        }
        [Authorize(WarehouseManagementPermissions.GoodsManagement.Create)]
        public async Task CreateManyAsync(List<GoodsBaseDto> inputs)
        {
            //var goodsEntity = base.ObjectMapper.Map<CreateGoodsDto, Goods>(input);
            //var goods=  await _goodsRepository.InsertAsync(goodsEntity);
             await _goodsManagement.CreateManyAsync(inputs);
        }

        public async Task<PagedResultDto<GoodsDto>> GetPagingListAsync(PagingGoodsListInput input)
        {

            var result = new PagedResultDto<GoodsDto>();
            var totalCount = await _goodsRepository.GetPagingCountAsync(input.Filter,input.GoodsCode,input.GoodsSpec);
            result.TotalCount = totalCount;
            if (totalCount <= 0) return result;

            var entities = await _goodsRepository.GetPagingListAsync(input.Filter, input.GoodsCode, input.GoodsSpec, 
                input.PageSize,
                input.SkipCount, false);
            result.Items = ObjectMapper.Map<List<Goods>, List<GoodsDto>>(entities);

            return result;
        }

        /// <summary>
        /// 更新用户
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [Authorize(WarehouseManagementPermissions.GoodsManagement.Update)]
        public virtual async Task<GoodsDto> UpdateAsync(UpdateGoodsDto input)
        {
            var goods= await _goodsManagement.UpdateAsync(input.Id,input.GoodsCode,input.GoodsName,input.GoodsSpec, input.GoodsConstProperty1, input.GoodsUnits);
            return base.ObjectMapper.Map<Goods, GoodsDto>(goods);
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        [Authorize(WarehouseManagementPermissions.GoodsManagement.Delete)]
        public virtual async Task DeleteAsync(IdIntInput input)
        {
            await _goodsManagement.DeleteAsync(input.Id);
            //await _goodsRepository.DeleteAsync(input.Id);
        }
        public async Task<GoodsDto> FindByCodeAsync(PagingGoodsListInput input)
        {
            var goods = await _goodsRepository.FindByCodeAsync(input.GoodsCode);
            return base.ObjectMapper.Map<Goods, GoodsDto>(goods);
        }

        public async Task<List<GoodsSelectDto>> GetSelectOptionsByNameAsync(PagingGoodsListInput input)
        {
            var goodss = await _goodsRepository.GetSelectOptionsAsync(input.GoodsName,input.GoodsSpec);
            List<GoodsSelectDto> goodsSelectDtos = new List<GoodsSelectDto>();
            foreach (var good in goodss)
            {
                GoodsSelectDto goodsSelectDto = new GoodsSelectDto()
                {
                    Label = good.GoodsName + "&" + good.GoodsSpec,
                    Value = good.GoodsCode
                };
                goodsSelectDtos.Add(goodsSelectDto);
            }  
            return await Task.FromResult(goodsSelectDtos);

        }

    }
}
