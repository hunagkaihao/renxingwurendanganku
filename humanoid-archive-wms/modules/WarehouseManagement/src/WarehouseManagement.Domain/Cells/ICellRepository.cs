using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using WarehouseManagement.Cells;

namespace WarehouseManagement.Cells
{
    public interface ICellRepository : IRepository<Cell, int>
    {
        Task<Cell> FindByIdAsync(
            int id,
            bool includeDetails = false,
            CancellationToken cancellationToken = default);

        Task<Cell> FindByNameAsync(
            string name,
            bool includeDetails = false,
            CancellationToken cancellationToken = default);

        Task<Cell> FindByCodeAsync(
            string name,
            bool includeDetails = false,
            CancellationToken cancellationToken = default);

        Task<List<Cell>> GetPagingListAsync(
            string filter = null,
            int warehouseId = 0,
            string cellType = null,
            int maxResultCount = 10,
            int skipCount = 0,
            bool includeDetails = false,
            CancellationToken cancellationToken = default);

        Task<long> GetPagingCountAsync(string filter = null,
            int warehouseId = 0,
            string cellType = null,
            CancellationToken cancellationToken = default);



        Task<List<Cell>> GetCellListByZAsync(int cellZ =0,
    bool includeDetails = false,
    CancellationToken cancellationToken = default);


    }
}
