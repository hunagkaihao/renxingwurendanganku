using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using WarehouseManagement.StockTasks.Aggregates;

namespace WarehouseManagement.StockTasks
{
    public interface IStockTaskRepository : IRepository<StockTask, int>
    {
        /// <summary>
        /// 在当前数据库事务内锁定任务行（包括软删除记录），直到事务结束。
        /// 调用方必须先开启事务，再读取并处理任务最新状态。
        /// </summary>
        /// <param name="id">WMS 任务 ID。</param>
        /// <param name="cancellationToken">取消令牌。</param>
        Task LockStatusUpdateAsync(int id, CancellationToken cancellationToken = default);

        Task<StockTask> FindByIdAsync(
            int id,
            bool includeDetails = true,
            CancellationToken cancellationToken = default);

        Task<StockTask> FindByBarcodeAsync(
            string name,
            bool includeDetails = true,
            CancellationToken cancellationToken = default);

        Task<List<StockTask>> GetPagingListAsync(
            string filter = null,
            int maxResultCount = 10,
            int skipCount = 0,
            bool includeDetails = false,
            CancellationToken cancellationToken = default);

        Task<long> GetPagingCountAsync(string filter = null,
            CancellationToken cancellationToken = default);

    }
}
