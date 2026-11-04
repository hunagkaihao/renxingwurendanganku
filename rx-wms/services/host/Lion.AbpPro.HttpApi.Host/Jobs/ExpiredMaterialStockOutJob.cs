using System.Threading.Tasks;
using WarehouseManagement.StockTasks;

namespace Lion.AbpPro.Jobs
{
    public class ExpiredMaterialStockOutJob
    {
        private readonly StockTaskAppService _stockTaskAppService;

        public ExpiredMaterialStockOutJob(StockTaskAppService stockTaskAppService)
        {
            _stockTaskAppService = stockTaskAppService;
        }

        public Task ExecuteAsync()
        {
            return _stockTaskAppService.DispatchExpiredStockOutTasksAsync();
        }
    }
}
