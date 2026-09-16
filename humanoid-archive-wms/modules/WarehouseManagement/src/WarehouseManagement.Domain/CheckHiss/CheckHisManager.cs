using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseManagement.CheckHiss.Aggregates;

namespace WarehouseManagement.CheckHiss
{
    public class CheckHisManager : CheckHisDomainService
    {
        private readonly ICheckHisRepository _checkHisRepository;
        public CheckHisManager(ICheckHisRepository checkHisRepository)
        {
            _checkHisRepository = checkHisRepository;
        }
        public async Task<CheckHis> CreateAsync(CheckHis check)
        {
            // 明细历史需要立即使用主记录的自增 ID，必须在返回前保存。
            return await _checkHisRepository.InsertAsync(check, true);
        }
        public async Task<List<CheckHis>> GetHisAsync(string checkCode)
        {
            return await _checkHisRepository.GetListAsync(f => f.CheckCode == checkCode);
        }

        public async Task<CheckHis> GetHisByIdAsync(int id)
        {
            return (await _checkHisRepository.GetListAsync(f => f.Id == id)).FirstOrDefault();
        }

        public async Task<CheckHis> UpdateAsync(CheckHis check)
        {
           return await _checkHisRepository.UpdateAsync(check);
        }


    }
}
