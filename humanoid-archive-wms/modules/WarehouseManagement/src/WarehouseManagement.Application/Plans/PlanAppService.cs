using System.Linq;
using System.Threading.Tasks;
using Lion.AbpPro.Extension.Customs.Dtos;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using WarehouseManagement.Plans.Aggregates;
using WarehouseManagement.Plans.Dto;
using WarehouseManagement.StockTasks;

namespace WarehouseManagement.Plans
{
    //[Authorize(WarehouseManagementPermissions.PlanManagement.Default)]
    public class PlanAppService : WarehouseManagementAppService,
         IPlanAppService //implement the IPlanAppService
    {
        //private readonly IRepository<Plan, Guid> _PlanRepository;
        /// <summary>
        ///  注意 为了快速直接注入仓库层 规范上是不允许的
        ///  这里注入仓储也只是为了查询分页
        ///  如果是其他的操作全部通过对应manger进行操作
        /// </summary>
        private readonly IPlanRepository _PlanRepository;
        private readonly PlanManager _planManager;
        private readonly StockTaskManager _StockTaskManager;

        public PlanAppService(IPlanRepository PlanRepository, PlanManager PlanManagement, StockTaskManager stockTaskManager)
        {
            _PlanRepository = PlanRepository;
            _planManager = PlanManagement;
            _StockTaskManager = stockTaskManager;
        }

        public async Task<PagedResultDto<PlanDto>> GetPagingListAsync(PagingPlanListInput input)
        {

            var queryable = await _PlanRepository.GetQueryableAsync();

            //Prepare a query to join books and authors
            var query = from Plan in queryable
                        //where Plan.CreationTime >= input.StartCreationTime & Plan.CreationTime <= input.EndCreationTime
                        //& Plan.StockBarcode.Contains(input.Filter.IsNullOrEmpty() ? "" : input.Filter.Trim())
                        ////& Plan.ManageStatus.ToString().Contains(input.ManageStatus=="All"?"":input.ManageStatus)
                        //& (input.ManageStatus == "All" ? 1==1 :Plan.ManageStatus == Enum.Parse<ManageStatus>(input.ManageStatus))
                        select new { Plan };

            //Paging
            query = query
                //.OrderBy(NormalizeSorting(input.Sorting))
                .OrderBy(f => f.Plan.Id)
                .Skip(input.SkipCount)
                .Take(1000);
            //.Take(input.MaxResultCount);

            //Execute the query and get a list
            var queryResult = await AsyncExecuter.ToListAsync(query);

            //Convert the query result to a list of BookDto objects
            var PlanDtos = queryResult.Select(x =>
            {
                var PlanDtos = ObjectMapper.Map<Plan, PlanDto>(x.Plan);
                return PlanDtos;
            }).ToList();

            //Get the total count with another query
            //var totalCount = await _PlanDetailRepository.GetCountAsync();
            var totalCount = queryResult.Count();

            return new PagedResultDto<PlanDto>(
                totalCount,
                PlanDtos
            );
        }

        public virtual async Task<PlanDto> UpdateAsync(UpdatePlanDto input)
        {
            var Plan= await _planManager.UpdateAsync(input.Id);
            return base.ObjectMapper.Map<Plan, PlanDto>(Plan);
        }

        public virtual async Task DeleteAsync(IdIntInput input)
        {
            await _planManager.DeleteAsync(input.Id);
            //await _PlanRepository.DeleteAsync(input.Id);
        }
        public async Task CreatePlanAsync(CreatePlanDto input)
        {
            await _planManager.CreatePlanAsync(input.PlanTypeCode,input.AreaCode);
        }


        //执行计划
        public async Task<bool> SetExecuting(int planId)
        {
            var entity = await _planManager.FindByIdAsync(planId);
            if (entity == null)
                throw new UserFriendlyException(message: "计划不存在");

            return await _StockTaskManager.ExecutePlan(entity);

        }

        //取消计划
        public async Task SetCancel(int planId)
        {
            var entity = await _planManager.FindByIdAsync(planId);
            if (entity == null)
                throw new UserFriendlyException(message: "计划不存在");
            if(entity.PlanStatus != PlanStatus.Waiting)
            {
                await _StockTaskManager.CancelExecutingPlan(planId);
            }
            
            await _planManager.DeleteAsync(planId);

        }
    }
}
