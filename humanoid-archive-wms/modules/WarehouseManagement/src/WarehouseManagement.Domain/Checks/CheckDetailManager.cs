using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseManagement.ArchiveBoxs.Aggregates;
using WarehouseManagement.ArchiveBoxs.Dto;
using WarehouseManagement.Checks.Aggregates;
using WarehouseManagement.Checks.Dto;

namespace WarehouseManagement.Checks
{
    public class CheckDetailManager :CheckDetailDomainService
    {
        private readonly ICheckDetailRepository _checkDetailRepository;

        public CheckDetailManager(ICheckDetailRepository checkDetailRepository)
        {
            _checkDetailRepository = checkDetailRepository;
        }

        public async Task<CheckDetail> CreateCheckDetailAsync(CheckDetailDto checkDetail)
        {
            var entity = base.ObjectMapper.Map<CheckDetailDto, CheckDetail>(checkDetail);
            return await _checkDetailRepository.InsertAsync(entity);
        }

        //获取盘点计划明细
        public async Task<List<CheckDetail>> GetCheckDetail(int checkId)
        {
            return await _checkDetailRepository.GetListAsync(f => f.CheckId == checkId);
        }
        //获取盘点计划明细
        public async Task<List<CheckDetail>> GetCheckDetailByStockId(int stockId)
        {
            return await _checkDetailRepository.GetListAsync(f => f.ManageId == stockId);
        }
        
    }
}
