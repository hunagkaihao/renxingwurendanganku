using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;
using WarehouseManagement.ArchiveBoxs.Aggregates;
using WarehouseManagement.ArchiveBoxs.Dto;
namespace WarehouseManagement.ArchiveBoxs
{
    public class ArchiveBoxManager : ArchiveBoxDomainService
    {

        private readonly IArchiveBoxRepository _archiveBoxRepository;

        public ArchiveBoxManager(IArchiveBoxRepository archiveBoxRepository            )
        {

            _archiveBoxRepository = archiveBoxRepository;
        }


        public async Task DeleteAsync(int boxId)
        {
            var entity = await _archiveBoxRepository.FindByIdAsync(boxId);
            if (entity == null)
                throw new UserFriendlyException(message: "档案盒不存在");
            await _archiveBoxRepository.DeleteAsync(entity);
        }
        public async Task<ArchiveBox> UpdateAsync(int boxId, string cellCode, string cellType)
        {
            var entity = await _archiveBoxRepository.FindByIdAsync(boxId);
            if (entity == null)
                throw new UserFriendlyException(message: "档案盒不存在"); 
            entity.Update(cellCode, cellType);
            return await _archiveBoxRepository.UpdateAsync(entity);
        }

        public async Task<bool> IsExistCode(string boxName)
        {
            var box = await _archiveBoxRepository.FindByBoxNameAsync(boxName);
            if (box != null)
                return true;
            return false;
        }

        public async Task<ArchiveBox> GetArchiveBoxById(int id)
        {
            return await _archiveBoxRepository.FindByIdAsync(id);
        }

        public async Task<List<ArchiveBox>> GetAll()
        {
            return await _archiveBoxRepository.GetListAsync();
        }

        public async Task<ArchiveBox> GetArchiveBoxByBoxName(string boxName)
        {
            return await _archiveBoxRepository.FindByBoxNameAsync(boxName);
        }

        public async Task<ArchiveBox> GetArchiveBoxByBarcode(int cellId)
        {
            return await _archiveBoxRepository.FindByCellIdAsync(cellId);
        }

        public async Task<ArchiveBox> GetArchiveBoxByRfidCode(string rfidCode)
        {
            return await _archiveBoxRepository.FindByRfidCodeAsync(rfidCode);
        }

        public async Task<ArchiveBox> GetArchiveBoxByCellId(int cellId)
        {
            return await _archiveBoxRepository.FindByCellIdAsync(cellId);
        }
        //标签是否被绑定
        public async Task<bool> CheckUsedBoxRfid(string rfidCode)
        {
            var achiveBoxobj = await _archiveBoxRepository.GetListAsync(x => x.ArchiveBoxRfid == rfidCode);
            if (achiveBoxobj.FirstOrDefault() != null)
            {
                return true;
            }
            return false;
        }
        //更新档案盒所在库位
        public async Task<ArchiveBox> UpdateStockCellAsync(string archiveBoxcode, int cellId)
        {
            var entity = await _archiveBoxRepository.FindByArchiveBoxcodeAsync(archiveBoxcode);
            if (entity == null)
                throw new UserFriendlyException(message: "档案盒不存在");
            entity.SetCell(cellId);
            return await _archiveBoxRepository.UpdateAsync(entity);
        }
        //出库档案盒所在库位
        public async Task<ArchiveBox> UpdateStockOutCellAsync(string archiveBoxcode)
        {
            var entity = await _archiveBoxRepository.FindByArchiveBoxcodeAsync(archiveBoxcode);
            if (entity == null)
                throw new UserFriendlyException(message: "档案盒不存在");
            entity.SetCell(0);
            return await _archiveBoxRepository.UpdateAsync(entity);
        }
    }
}
