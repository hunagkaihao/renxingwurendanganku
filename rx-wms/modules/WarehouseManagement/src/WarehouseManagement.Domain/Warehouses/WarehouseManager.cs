using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using WarehouseManagement.Warehouses;
using WarehouseManagement.Warehouses.Aggregates;

namespace WarehouseManagement.Warehouses
{
    public class WarehouseManager : WarehouseDomainService
    {
        private readonly IWarehouseRepository _warehouseRepository;
        private readonly IWarehouseAreaRepository _warehouseAreaRepository;
        //private readonly IDistributedCache<Warehouse> _cache;//设置缓存

        //    public WarehouseManager(
        //IWarehouseRepository WarehouseRepository,
        //IDistributedCache<WarehouseDto> cache)
        //    {
        //        _WarehouseRepository = WarehouseRepository;
        //        _cache = cache;
        //    }

        public WarehouseManager(
            IWarehouseRepository warehouseRepository,
            IWarehouseAreaRepository warehouseAreaRepository)
        {
            _warehouseRepository = warehouseRepository;
            _warehouseAreaRepository = warehouseAreaRepository;
        }

        /// <summary>
        /// 创建字典类型
        /// </summary>
        /// <param name="code"></param>
        /// <param name="displayText"></param>
        /// <param name="description"></param>
        public Task<Warehouse> CreateAsync(string warehouseCode, string warehouseName, string warehouseType)
        {
            var entity = new Warehouse(warehouseCode, warehouseName, Enum.Parse<WarehouseType>(warehouseType) );
            return _warehouseRepository.InsertAsync(entity);
        }

        public async Task DeleteAsync(int warehouseId)
        {
            var entity = await _warehouseRepository.FindByIdAsync(warehouseId);
            if (entity == null)
                throw new UserFriendlyException(message: "仓库不存在");
            await _warehouseRepository.DeleteAsync(entity);
        }
        public async Task<Warehouse> UpdateAsync(int id, string warehouseCode, string warehouseName, string warehouseType)
        {
            var entity = await _warehouseRepository.FindByIdAsync(id);
            if (entity == null)
                throw new UserFriendlyException(message: "仓库不存在");
            entity.Update(warehouseCode, warehouseName, Enum.Parse<WarehouseType>(warehouseType));
            return await _warehouseRepository.UpdateAsync(entity);
        }

        public Task<WarehouseArea> CreateAreaAsync(int warehouseId, string warehouseAreaCode, string warehouseAreaName, string warehouseAreaType)
        {
            var entity = new WarehouseArea(warehouseId,warehouseAreaCode, warehouseAreaName, Enum.Parse<WarehouseAreaType>(warehouseAreaType));
            return _warehouseAreaRepository.InsertAsync(entity);
        }

        public async Task DeleteAreaAsync(int warehouseAreaId)
        {
            var entity = await _warehouseAreaRepository.FindByIdAsync(warehouseAreaId);
            if (entity == null)
                throw new UserFriendlyException(message: "区域不存在");
            await _warehouseAreaRepository.DeleteAsync(entity);
        }
        public async Task<WarehouseArea> UpdateAreaAsync(int id, int warehouseId, string warehouseAreaCode, string warehouseAreaName, string warehouseAreaType)
        {
            var entity = await _warehouseAreaRepository.FindByIdAsync(id);
            if (entity == null)
                throw new UserFriendlyException(message: "区域不存在");
            entity.Update(warehouseId,warehouseAreaCode, warehouseAreaName, Enum.Parse<WarehouseAreaType>(warehouseAreaType));
            return await _warehouseAreaRepository.UpdateAsync(entity);
        }


    }
}
