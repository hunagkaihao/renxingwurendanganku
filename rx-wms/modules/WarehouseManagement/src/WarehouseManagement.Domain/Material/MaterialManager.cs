using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseManagement.MaterialBoxs.Aggregates;
using WarehouseManagement.MaterialBoxs;
using WarehouseManagement.Material;
using MaterialAggregate = WarehouseManagement.Material.Aggregates.Material;

namespace WarehouseManagement.Material
{
    public class MaterialManager : MaterialDomainService
    {
        private readonly IMaterialRepository _archiveRepository;
        public MaterialManager(IMaterialRepository archiveRepository)
        {
            _archiveRepository = archiveRepository;
        }
        public async Task<MaterialAggregate> GetArchiveById(int archiveId)
        {
            return await _archiveRepository.FindByIdAsync(archiveId);
        }

        public async Task<List<MaterialAggregate>> GetAll()
        {
            return await _archiveRepository.GetListAsync();
        }
        public async Task<MaterialAggregate> GetArchiveByRfidCode(string rfidCode)
        {
            return await _archiveRepository.FindByRfidCodeAsync(rfidCode);
        }
        public async Task<bool> CheckUsedBoxRfid(string rfidCode)
        {
            var achiveBoxobj = await _archiveRepository.GetListAsync(x => x.RfidId == rfidCode);
            if (achiveBoxobj.FirstOrDefault() != null)
            {
                return true;
            }
            return false;
        }
    }
}
