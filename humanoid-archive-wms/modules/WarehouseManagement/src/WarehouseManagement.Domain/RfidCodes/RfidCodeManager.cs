using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;

namespace WarehouseManagement.RfidCodes
{
    public class RfidCodeManager : RfidCodeDomainService
    {
        private readonly IRfidRepository _rfidRepository;

        public RfidCodeManager(IRfidRepository rfidCodeRepository)
        {

            _rfidRepository = rfidCodeRepository;
        }

        public async Task DeleteAsync(int boxId)
        {
            var entity = await _rfidRepository.FindByIdAsync(boxId);
            if (entity == null)
                throw new UserFriendlyException(message: "标签不存在");
            await _rfidRepository.DeleteAsync(entity);
        }
        //没有返回false
        public async Task<bool> CheckExistRfidCode(string rfidCode, int rfidType)
        {
            var rfidobj = await _rfidRepository.GetListAsync(x => x.RfidCode == rfidCode && x.RfidTypeCode == rfidType);
            if (rfidobj.Count != 0)
            {
                return true;
            }
            return false;
        }
    }
}
