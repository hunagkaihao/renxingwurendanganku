using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseManagement.CheckHiss.Aggregates;

namespace WarehouseManagement.CheckHiss
{
    public class CheckDetailHisManager : CheckDetailHisDomainService
    {
        private readonly ICheckDetailHisRepository _repository;

        public CheckDetailHisManager(ICheckDetailHisRepository repository)
        {
            _repository = repository;
        }

        public async Task<CheckDetailHis> CreateAsync(CheckDetailHis checkDetailHis)
        {
            return await _repository.InsertAsync(checkDetailHis);
        }
        public async Task<CheckDetailHis> UpdateAsync(CheckDetailHis checkDetailHis)
        {
            return await _repository.UpdateAsync(checkDetailHis);
        }

        public async Task<CheckDetailHis> GetById(int id)
        {
            return await _repository.GetAsync(x => x.Id == id);
        }

        public async Task<List<CheckDetailHis>>GetList(int verifyFlag, int checkHisId)
        {
            return await _repository.GetListAsync(x => x.VerifyFlag == verifyFlag & x.CheckId == checkHisId);
        }

        /// <summary>
        /// 判断指定单库位盘点任务是否已经生成过盘点历史。
        /// ManageId 对应盘点时创建的 StockTask.Id，可作为轮询重复回传时的幂等依据。
        /// </summary>
        public async Task<bool> ExistsByManageIdAsync(int manageId)
        {
            List<CheckDetailHis> histories = await _repository.GetListAsync(x => x.ManageId == manageId);
            return histories.Count > 0;
        }
    }
}
