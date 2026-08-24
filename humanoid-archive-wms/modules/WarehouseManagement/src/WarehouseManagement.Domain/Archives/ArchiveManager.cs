using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseManagement.ArchiveBoxs.Aggregates;
using WarehouseManagement.ArchiveBoxs;
using WarehouseManagement.Archives;
using WarehouseManagement.Archives.Aggregates;

namespace WarehouseManagement.Archives
{
    public class ArchiveManager : ArchiveDomainService
    {
        private readonly IArchiveRepository _archiveRepository;
        public ArchiveManager(IArchiveRepository archiveRepository)
        {
            _archiveRepository = archiveRepository;
        }
        public async Task<Archive> GetArchiveById(int archiveId)
        {
            return await _archiveRepository.FindByIdAsync(archiveId);
        }

        public async Task<List<Archive>> GetAll()
        {
            return await _archiveRepository.GetListAsync();
        }
        public async Task<Archive> GetArchiveByRfidCode(string rfidCode)
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
