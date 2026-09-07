using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;
using WarehouseManagement.MaterialBoxs.Dto;
using WarehouseManagement.MaterialBoxs.Aggregates;

namespace WarehouseManagement.MaterialBoxs
{
    public class MaterialBoxManager : MaterialBoxDomainService
    {

        private readonly IMaterialBoxRepository _materialBoxRepository;

        public MaterialBoxManager(IMaterialBoxRepository materialBoxRepository            )
        {

            _materialBoxRepository = materialBoxRepository;
        }


        public async Task DeleteAsync(int boxId)
        {
            var entity = await _materialBoxRepository.FindByIdAsync(boxId);
            if (entity == null)
                throw new UserFriendlyException(message: "档案盒不存在");
            await _materialBoxRepository.DeleteAsync(entity);
        }
        public async Task<MaterialBox> UpdateAsync(int boxId, string cellCode, string cellType)
        {
            var entity = await _materialBoxRepository.FindByIdAsync(boxId);
            if (entity == null)
                throw new UserFriendlyException(message: "档案盒不存在"); 
            entity.Update(cellCode, cellType);
            return await _materialBoxRepository.UpdateAsync(entity);
        }

        public async Task<bool> IsExistCode(string boxName)
        {
            var box = await _materialBoxRepository.FindByBoxNameAsync(boxName);
            if (box != null)
                return true;
            return false;
        }

        public async Task<MaterialBox> GetArchiveBoxById(int id)
        {
            return await _materialBoxRepository.FindByIdAsync(id);
        }

        public async Task<List<MaterialBox>> GetAll()
        {
            return await _materialBoxRepository.GetListAsync();
        }

        public async Task<MaterialBox> GetArchiveBoxByBoxName(string boxName)
        {
            return await _materialBoxRepository.FindByBoxNameAsync(boxName);
        }

        public async Task<MaterialBox> GetArchiveBoxByBarcode(int cellId)
        {
            return await _materialBoxRepository.FindByCellIdAsync(cellId);
        }

        public async Task<MaterialBox> GetArchiveBoxByRfidCode(string rfidCode)
        {
            return await _materialBoxRepository.FindByRfidCodeAsync(rfidCode);
        }

        public async Task<MaterialBox> GetArchiveBoxByCellId(int cellId)
        {
            return await _materialBoxRepository.FindByCellIdAsync(cellId);
        }
        //标签是否被绑定
        public async Task<bool> CheckUsedBoxRfid(string rfidCode)
        {
            var achiveBoxobj = await _materialBoxRepository.GetListAsync(x => x.MaterialBoxRfid == rfidCode);
            if (achiveBoxobj.FirstOrDefault() != null)
            {
                return true;
            }
            return false;
        }
        //更新档案盒所在库位
        public async Task<MaterialBox> UpdateStockCellAsync(string archiveBoxcode, int cellId)
        {
            var entity = await _materialBoxRepository.FindByArchiveBoxcodeAsync(archiveBoxcode);
            if (entity == null)
                throw new UserFriendlyException(message: "档案盒不存在");
            entity.SetCell(cellId);
            return await _materialBoxRepository.UpdateAsync(entity);
        }
        //出库档案盒所在库位
        public async Task<MaterialBox> UpdateStockOutCellAsync(string archiveBoxcode)
        {
            var entity = await _materialBoxRepository.FindByArchiveBoxcodeAsync(archiveBoxcode);
            if (entity == null)
                throw new UserFriendlyException(message: "档案盒不存在");
            entity.SetCell(0);
            return await _materialBoxRepository.UpdateAsync(entity);
        }
    }
}
