using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Uow;
using WarehouseManagement.Checks.Aggregates;
using Check = WarehouseManagement.Checks.Aggregates.Check;

namespace WarehouseManagement.Checks
{
    public class CheckManager : CheckDomainService
    {
        private readonly ICheckRepository _checkRepository;
        private readonly ICheckDetailRepository _checkDetailRepository;

        //private readonly IDistributedCache<Check> _cache;//设置缓存

        //    public CheckManager(
        //ICheckRepository CheckRepository,
        //IDistributedCache<CheckDto> cache)
        //    {
        //        _CheckRepository = CheckRepository;
        //        _cache = cache;
        //    }

        public CheckManager(
            ICheckRepository checkRepository,ICheckDetailRepository checkDetailRepository)
        {
            _checkRepository = checkRepository;
            _checkDetailRepository = checkDetailRepository;
        }

        /// <summary>
        /// 创建盘点计划
        /// </summary>
        /// <param name="areaCode"></param>
        /// <returns></returns>
        public Task<Check> CreateByAreaCodeAsync(Check entity)
        {
            return _checkRepository.InsertAsync(entity);
        }

        /// <summary>
        /// 更新盘点计划（执行）
        /// </summary>
        /// <param name="checkId"></param>
        /// <returns></returns>
        /// <exception cref="UserFriendlyException"></exception>
        [UnitOfWork]
        public async Task UpdateAsync(Check entity,bool sm)
        {
            await _checkRepository.UpdateAsync(entity, sm);
        }

        /// <summary>
        /// 删除盘点计划
        /// </summary>
        /// <param name="checkId"></param>
        /// <returns></returns>
        /// <exception cref="UserFriendlyException"></exception>
        [UnitOfWork]
        public async Task DeleteAsync(string checkCode)
        {
            var entity = await _checkRepository.FindByCheckCodeAsync(checkCode);
            if (entity == null)
                throw new UserFriendlyException(message: "盘点计划不存在");
            if (entity.CheckStatus == CheckStatus.Waiting)
            {
                await _checkRepository.DeleteAsync(entity);
            }
            if (entity.CheckStatus == CheckStatus.Executing)
            {
                if (entity.Details.Any(x => x.ManageId != 0))
                {
                    throw new UserFriendlyException(message: "盘点计划已执行，不能取消");                    
                }
                else
                {
                    await _checkDetailRepository.DeleteAsync(x=>x.CheckId== entity.Id);
                    await _checkRepository.DeleteAsync(entity, true);
                }
            }
            else
            {
                throw new UserFriendlyException(message: "当前状态不能取消");
            }
        }
        [UnitOfWork]
        public async Task DeleteByIdAsync(int checkId)
        {
            var entity = await _checkRepository.FindByIdAsync(checkId);
            if (entity == null)
                throw new UserFriendlyException(message: "盘点计划不存在");
            if (entity.CheckStatus == CheckStatus.Waiting)
            {
                await _checkRepository.DeleteAsync(entity);
            }
            else if (entity.CheckStatus == CheckStatus.Executing)
            {
                if (entity.Details.Any(x => x.ManageId != 0))
                {
                    throw new UserFriendlyException(message: "盘点计划已执行，不能取消");
                }
                else
                {
                    await _checkDetailRepository.DeleteAsync(x => x.CheckId == checkId);
                    await _checkRepository.DeleteAsync(entity, true);
                }
            }
            else
            {
                throw new UserFriendlyException(message: "当前状态不能取消");
            }
        }
        //完成后删除盘点任务
        public async Task DeleteByCheckCodeAsync(string checkCode)
        {
            await _checkRepository.DeleteAsync(x => x.CheckCode == checkCode);
        }

        /// <summary>
        /// 更新实盘数量
        /// </summary>
        /// <param name="checkDetailId"></param>
        /// <param name="realAmount"></param>
        /// <returns></returns>
        public async Task UpdateRealAmountAsync(int checkDetailId,decimal realAmount,string checker)
        {
           CheckDetail checkDetail=await  _checkDetailRepository.FindAsync(x => x.Id == checkDetailId);
            checkDetail.RealAmount_1 = realAmount;
            checkDetail.ProfitLossAmount = checkDetail.Account - realAmount;
            checkDetail.Checker = checker;
            await _checkDetailRepository.UpdateAsync(checkDetail);
        }

        //获取盘点计划
        public async Task<Check> GetCheck(int checkId)
        {
            return (await _checkRepository.GetListAsync(f => f.Id == checkId)).FirstOrDefault();
        }

        //获取执行中的盘点计划
        public async Task<List<Check>> GetExcetingCheck()
        {
            return await _checkRepository.GetListAsync(f => f.CheckStatus == CheckStatus.Executing);
        }
    }
}
