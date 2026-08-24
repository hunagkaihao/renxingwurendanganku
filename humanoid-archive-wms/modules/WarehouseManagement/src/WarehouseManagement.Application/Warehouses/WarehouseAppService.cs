using WarehouseManagement.Warehouses.Dto;
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
using WarehouseManagement.Warehouses;
using WarehouseManagement.Warehouses.Aggregates;
using System.Net.Http;
using Lion.AbpPro.Extension.Customs.Http;

namespace WarehouseManagement.Warehouses
{
    [Authorize(WarehouseManagementPermissions.WarehouseManagement.Default)]
    public class WarehouseAppService : WarehouseManagementAppService,
         IWarehouseAppService //implement the IWarehouseAppService
    {
        //private readonly IRepository<Warehouse, Guid> _warehouseRepository;
        /// <summary>
        ///  注意 为了快速直接注入仓库层 规范上是不允许的
        ///  这里注入仓储也只是为了查询分页
        ///  如果是其他的操作全部通过对应manger进行操作
        /// </summary>
        private readonly IWarehouseRepository _warehouseRepository;
        private readonly IWarehouseAreaRepository _warehouseAreaRepository;
        private readonly WarehouseManager _warehouseManagement;
        //private readonly IHttpClientFactory _httpClientFactory;
        public WarehouseAppService(IWarehouseRepository warehouseRepository,
            IWarehouseAreaRepository warehouseAreaRepository,
            WarehouseManager warehouseManagement)
        {
            _warehouseRepository = warehouseRepository;
            _warehouseAreaRepository = warehouseAreaRepository;
            _warehouseManagement = warehouseManagement;
            //_warehouseManager = warehouseManager;
            //GetPolicyName = WarehouseStorePermissions.Warehouses.Default;
            //GetListPolicyName = WarehouseStorePermissions.Warehouses.Default;
            //CreatePolicyName = WarehouseStorePermissions.Warehouses.Create;
            //UpdatePolicyName = WarehouseStorePermissions.Warehouses.Edit;
            //DeletePolicyName = WarehouseStorePermissions.Warehouses.Delete;
            //_httpClientFactory = httpClientFactory;
        }
        [Authorize(WarehouseManagementPermissions.WarehouseManagement.Create)]
        public async Task<WarehouseDto> CreateAsync(CreateWarehouseDto input)
        {
            //var warehouseEntity = base.ObjectMapper.Map<CreateWarehouseDto, Warehouse>(input);
            //var warehouse=  await _warehouseRepository.InsertAsync(warehouseEntity);
            var warehouse = await _warehouseManagement.CreateAsync(input.WarehouseCode,input.WarehouseName, input.WarehouseType);
            return  base.ObjectMapper.Map<Warehouse, WarehouseDto>(warehouse);
        }      

        public async Task<PagedResultDto<WarehouseDto>> GetPagingListAsync(PagingWarehouseListInput input)
        {

            // 通过access token 获取用户信息
            //Dictionary<string, string> headers = new Dictionary<string, string>
            //    { { "Authorization", $"Bearer {accessToken}" } };
            //var response =
            //    await _httpClientFactory.PostAsync<PagingWarehouseListInput, PagedResultDto<WarehouseDto>>("agv", "http://localhost:44315/Warehouses/stationpage", new PagingWarehouseListInput() { PageIndex=1,PageSize=10});


            var result = new PagedResultDto<WarehouseDto>();
            var totalCount = await _warehouseRepository.GetPagingCountAsync(input.Filter);
            result.TotalCount = totalCount;
            if (totalCount <= 0) return result;

            var entities = await _warehouseRepository.GetPagingListAsync(input.Filter, input.PageSize,
                input.SkipCount, false);
            result.Items = ObjectMapper.Map<List<Warehouse>, List<WarehouseDto>>(entities);

            return result;
        }


        /// <summary>
        /// 更新仓库
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [Authorize(WarehouseManagementPermissions.WarehouseManagement.Update)]
        public virtual async Task<WarehouseDto> UpdateAsync(UpdateWarehouseDto input)
        {
            var warehouse= await _warehouseManagement.UpdateAsync(input.Id,input.WarehouseCode,input.WarehouseName, input.WarehouseType);
            return base.ObjectMapper.Map<Warehouse, WarehouseDto>(warehouse);
        }

        /// <summary>
        /// 删除仓库
        /// </summary>
        [Authorize(WarehouseManagementPermissions.WarehouseManagement.Delete)]
        public virtual async Task DeleteAsync(IdIntInput input)
        {
            await _warehouseManagement.DeleteAsync(input.Id);
            //await _warehouseRepository.DeleteAsync(input.Id);
        }
        [Authorize(WarehouseManagementPermissions.WarehouseManagement.Create)]
        public async Task<WarehouseAreaDto> CreateAreaAsync(CreateWarehouseAreaDto input)
        {
            var warehouseArea = await _warehouseManagement.CreateAreaAsync(input.WarehouseId, input.WarehouseAreaCode, input.WarehouseAreaName, input.WarehouseAreaType);
            return base.ObjectMapper.Map<WarehouseArea, WarehouseAreaDto>(warehouseArea);
        }

        public async Task<PagedResultDto<WarehouseAreaDto>> GetAreaPagingListAsync(PagingWarehouseAreaListInput input)
        {

            // 通过access token 获取用户信息
            //Dictionary<string, string> headers = new Dictionary<string, string>
            //    { { "Authorization", $"Bearer {accessToken}" } };
            //var response =
            //    await _httpClientFactory.PostAsync<PagingWarehouseListInput, PagedResultDto<WarehouseDto>>("agv", "http://localhost:44315/Warehouses/stationpage", new PagingWarehouseListInput() { PageIndex=1,PageSize=10});


            var result = new PagedResultDto<WarehouseAreaDto>();
            var totalCount = await _warehouseAreaRepository.GetPagingCountAsync(input.Filter);
            result.TotalCount = totalCount;
            if (totalCount <= 0) return result;

            var entities = await _warehouseAreaRepository.GetPagingListAsync(input.Filter, input.PageSize,
                input.SkipCount, false);
            result.Items = ObjectMapper.Map<List<WarehouseArea>, List<WarehouseAreaDto>>(entities);

            return result;
        }


        /// <summary>
        /// 更新仓库
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [Authorize(WarehouseManagementPermissions.WarehouseManagement.Update)]
        public virtual async Task<WarehouseAreaDto> UpdateAreaAsync(UpdateWarehouseAreaDto input)
        {
            var warehouseArea = await _warehouseManagement.UpdateAreaAsync(input.Id, input.WarehouseId, input.WarehouseAreaCode, input.WarehouseAreaName, input.WarehouseAreaType);
            return base.ObjectMapper.Map<WarehouseArea, WarehouseAreaDto>(warehouseArea);
        }

        /// <summary>
        /// 删除仓库
        /// </summary>
        [Authorize(WarehouseManagementPermissions.WarehouseManagement.Delete)]
        public virtual async Task DeleteAreaAsync(IdIntInput input)
        {
            await _warehouseManagement.DeleteAreaAsync(input.Id);
            //await _warehouseRepository.DeleteAsync(input.Id);
        }

    }
}
