using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Uow;
using Volo.Abp.Users;
using WarehouseManagement.Cells;
using WarehouseManagement.Checks;
using WarehouseManagement.Goodss;
using WarehouseManagement.Plans.Aggregates;
using WarehouseManagement.StockTasks;
using WarehouseManagement.StockTasks.Aggregates;
namespace WarehouseManagement.Plans
{
    public class PlanManager : PlanDomainService
    {
        private readonly IPlanRepository _planRepository;
        private readonly IPlanListRepository _planListRepository;
        private readonly ICellRepository _cellRepository;
        private readonly GoodsManager _goodsManager;
        private readonly CellManager _cellManager;
        private readonly ICurrentUser _currentUser;

        //private readonly IDistributedCache<Plan> _cache;//设置缓存

        //    public PlanManager(
        //IPlanRepository PlanRepository,
        //IDistributedCache<PlanDto> cache)
        //    {
        //        _planRepository = PlanRepository;
        //        _cache = cache;
        //    }

        public PlanManager(
            IPlanRepository planRepository, IPlanListRepository planListRepository,ICellRepository cellRepository
            , GoodsManager goodsManager
            , CellManager cellManager
            , ICurrentUser currentUser)
        {
            _planRepository = planRepository;
            _planListRepository = planListRepository;
            _cellRepository = cellRepository;
            _goodsManager = goodsManager;
            _cellManager = cellManager;
            _currentUser=currentUser;
        }

        public async Task DeleteAsync(int PlanId)
        {
            var entity = await _planRepository.FindByIdAsync(PlanId);
            if (entity == null)
                throw new UserFriendlyException(message: "物料盒不存在");
            await _planRepository.DeleteAsync(entity);
        }
        public async Task<Plan> UpdateAsync(int id)
        {
            var entity = await _planRepository.FindByIdAsync(id);
            if (entity == null)
                throw new UserFriendlyException(message: "物料盒不存在");
            //entity.Update(manageTypeCode,stockBarcode,startCellId,endCellId, startCellCode, endCellCode);
            return await _planRepository.UpdateAsync(entity);
        }

        public async Task<Plan> Update(Plan plan)
        {
            return await _planRepository.UpdateAsync(plan);
        }
        public async Task<Plan> CreateDetailAsync(int PlanId)
        {
            var entity = await _planRepository.FindByIdAsync(PlanId);
            if (entity == null)
                //throw new DataDictionaryDomainException(message: "数据字典不存在");
                throw new UserFriendlyException(message: "物料盒不存在");
            //if (entity.Details.Any(e => e.Id == PlanDetailId))
            //{
            //    //throw new DataDictionaryDomainException(message: $"字典项{code}已存在");
            //    throw new UserFriendlyException(message: "物料盒明细项不存在");
            //}
            //entity.AddDetail(storageBoxDetailId, goodsId, quantity, taskDetailRemark);
            return await _planRepository.UpdateAsync(entity);
        }
        [UnitOfWork]
        public async Task<Plan> CreateAsync(string planTypeCode, string planBillNo, string planBillDate, string planCreater,
    int planPriority, int planExecuteType, string planRemark,List<PlanListDto> planListDtos)
        {
            var entity = new Plan(planTypeCode, planBillNo, planBillDate, planCreater, planPriority, planExecuteType, planRemark);
            foreach (var item in planListDtos)
            {
                var goodsEntity = await _goodsManager.GetByCodeAsync(item.GoodsCode);
                if (goodsEntity == null)
                {
                    throw new UserFriendlyException(message: $"物料{item.GoodsCode}不存在！");
                }
                entity.AddDetail(entity.Id, planBillNo, planPriority, goodsEntity.Id, item.GoodsCode,
                    item.GoodsBatchNo, item.PlanListQty, item.PlanListRemark);
            }
            entity = await _planRepository.InsertAsync(entity,true);
            

            
            return entity;
        }

        public async Task<Plan> CreatePlanAsync(string planTypeCode,string areaCode)
        {
            var entity = new Plan(planTypeCode, areaCode);
            //是否自动执行
            return await _planRepository.InsertAsync(entity);
        }

       
    

        [UnitOfWork]
        public async Task<Plan> SetAsCompletedAsync(int PlanId)
        {
            try
            {
                var entity = await _planRepository.FindByIdAsync(PlanId);
                if (entity == null)
                    throw new UserFriendlyException(message: "计划不存在");
                entity.PlanStatus = PlanStatus.Finish;
                return await _planRepository.UpdateAsync(entity);
            }
            catch (Exception e)
            {

                throw new UserFriendlyException(message: e.Message);
            }
        }
        [UnitOfWork]
        public async Task<Plan> SetAsExecutingAsync(int PlanId)
        {
            var entity = await _planRepository.FindByIdAsync(PlanId);
            if (entity == null)
                throw new UserFriendlyException(message: "计划不存在");
           
            return await _planRepository.UpdateAsync(entity);

        }
        public async Task<Plan> SetAsCancelAsync(int PlanId)
        {
            var entity = await _planRepository.FindByIdAsync(PlanId);
            if (entity == null)
                throw new UserFriendlyException(message: "计划不存在");
            //entity.SetAsCancel();
            return await _planRepository.UpdateAsync(entity);
        }
        public async Task<Plan> FindByIdAsync(int planId)
        {
            return await _planRepository.FindByIdAsync(planId);
        }

        public async Task<PlanList> FindByListIdAsync(int planListId)
        {
            return await _planListRepository.FindByIdAsync(planListId);
        }

        public async Task UpdateManyAsync(List<PlanList> planLists)
        {
            await _planListRepository.UpdateManyAsync(planLists);
        }

       
       
        /// <summary>
        /// 出入库任务更新计划数量和状态
        /// </summary>
        /// <param name="stockTaskDetails"></param>
        /// <returns></returns>
        [UnitOfWork]
        public async Task UpdateExcuteQtyAsync(List<StockTaskDetail> stockTaskDetails)
        {
            List<int> planListIds= stockTaskDetails.Select(x => (int)x.PlanDetailId).ToList();
            var planlists = await _planListRepository.GetListAsync(f=> planListIds.Contains(f.Id));
            foreach (var item in planlists)
            {
                var stockTaskDetail = stockTaskDetails.Find(f=>f.PlanDetailId==item.Id);
                if (item.PlanListExecuteQty + stockTaskDetail.ManageListQuantity > item.PlanListCreateQty)
                {
                    Log.Error($"计划ID{item.Id},任务明细ID{stockTaskDetail.Id}执行数量{item.PlanListExecuteQty }+{stockTaskDetail.ManageListQuantity}超过组箱数量{item.PlanListCreateQty}");
                }
                else
                {
                    item.PlanListExecuteQty = item.PlanListExecuteQty + stockTaskDetail.ManageListQuantity;
                }
            }
            await _planListRepository.UpdateManyAsync(planlists);

        }
        /// <summary>
        /// 出入库任务完成更新计划数量和状态
        /// </summary>
        /// <param name="stockTaskDetails"></param>
        /// <returns></returns>
        [UnitOfWork]
        public async Task UpdateCompleteQtyAsync(List<StockTaskDetail> stockTaskDetails)
        {
            List<int> planListIds = stockTaskDetails.Select(x => (int)x.PlanDetailId).ToList();
            var planlists = await _planListRepository.GetListAsync(f => planListIds.Contains(f.Id));
            foreach (var item in planlists)
            {
                var stockTaskDetail = stockTaskDetails.Find(f => f.PlanDetailId == item.Id);
                if (item.PlanListFinishedQty + stockTaskDetail.ManageListQuantity > item.PlanListExecuteQty)
                {
                    Log.Error($"计划ID{item.Id},任务明细ID{stockTaskDetail.Id}完成数量{item.PlanListFinishedQty }+{stockTaskDetail.ManageListQuantity}超过执行数量{item.PlanListExecuteQty}");
                }
                else
                {
                    item.PlanListFinishedQty = item.PlanListFinishedQty + stockTaskDetail.ManageListQuantity;
                }
            }
            //检查计划状态，更新状态
            await _planListRepository.UpdateManyAsync(planlists);

        }
        /// <summary>
        /// 出入库任务取消更新计划数量和状态
        /// </summary>
        /// <param name="stockTaskDetails"></param>
        /// <returns></returns>
        [UnitOfWork]
        public async Task UpdateCancelQtyAsync(List<StockTaskDetail> stockTaskDetails)
        {
            List<int> planListIds = stockTaskDetails.Select(x => (int)x.PlanDetailId).ToList();
            var planlists = await _planListRepository.GetListAsync(f => planListIds.Contains(f.Id));
            foreach (var item in planlists)
            {
                var stockTaskDetail = stockTaskDetails.Find(f => f.PlanDetailId == item.Id);
                if (item.PlanListExecuteQty - stockTaskDetail.ManageListQuantity <0)
                {
                    Log.Error($"计划ID{item.Id},任务明细ID{stockTaskDetail.Id}取消执行数量{item.PlanListExecuteQty }-{stockTaskDetail.ManageListQuantity}小于0");
                    item.PlanListExecuteQty = 0;
                }
                else
                {
                    item.PlanListExecuteQty = item.PlanListExecuteQty - stockTaskDetail.ManageListQuantity;
                }
            }
            await _planListRepository.UpdateManyAsync(planlists);

        }

        //获取执行中的盘点计划
        public async Task<List<Plan>> GetExcetingPlan()
        {
            return await _planRepository.GetListAsync(f => f.PlanStatus == PlanStatus.Executing);
        }
    }
}
