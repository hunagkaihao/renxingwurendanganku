using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using WarehouseManagement.Cells;

namespace WarehouseManagement.EntityFrameworkCore.Cells
{
    public class EfCoreCellRepository : EfCoreRepository<IWarehouseManagementDbContext, Cell, int>, ICellRepository
    {
        public EfCoreCellRepository(IDbContextProvider<IWarehouseManagementDbContext> dbContextProvider) : base(dbContextProvider)
        {

        }
        public async Task<List<Cell>> GetPagingListAsync(string filter = null, int warehouseId = 0, string cellType = null, int maxResultCount = 10, int skipCount = 0, bool includeDetails = false, CancellationToken cancellationToken = default)
        {
            return await (await GetDbSetAsync())
                .IncludeDetails(includeDetails)
                .WhereIf(!filter.IsNullOrWhiteSpace(),
                    e => (e.CellName.Contains(filter)))
                .WhereIf(warehouseId!=0,
                    e => (e.WarehouseId==warehouseId))
                 .WhereIf(!cellType.IsNullOrWhiteSpace(),
                    e => (e.CellType==Enum.Parse<CellType>(cellType)))
                .OrderByDescending(e => e.CreationTime)
                .PageBy(skipCount, maxResultCount)
                .ToListAsync(GetCancellationToken(cancellationToken));
        }

        public async Task<long> GetPagingCountAsync(string filter = null, int warehouseId = 0, string cellType = null, CancellationToken cancellationToken = default)
        {
            return await (await GetDbSetAsync())
                .WhereIf(!filter.IsNullOrWhiteSpace(),
                    e => (e.CellName.Contains(filter) || e.CellCode.Contains(filter)))
                .WhereIf(warehouseId != 0,
                    e => (e.WarehouseId == warehouseId))
                .WhereIf(!cellType.IsNullOrWhiteSpace(),
                    e => (e.CellType == Enum.Parse<CellType>(cellType)))
                .CountAsync(cancellationToken: cancellationToken);
        }



        /// <summary>
        /// 通过排获取cellList
        /// </summary>
        /// <param name="cellZ"></param>
        /// <param name="maxResultCount"></param>
        /// <param name="skipCount"></param>
        /// <param name="includeDetails"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<List<Cell>> GetCellListByZAsync(int cellZ =0, bool includeDetails = false, CancellationToken cancellationToken = default)
        {
            return await (await GetDbSetAsync())
                .IncludeDetails(includeDetails)
                .Where(e => e.Cell_z== cellZ)
                .Where(e => e.CellType == CellType.Cell)
                .OrderBy(e => e.Cell_y)
                .ThenBy(e => e.Cell_x)
                .ToListAsync(GetCancellationToken(cancellationToken));
        }

        public async Task<long> GetCountAsync(string filter = null, CancellationToken cancellationToken = default)
        {
            return await (await GetDbSetAsync())
                .WhereIf(!filter.IsNullOrWhiteSpace(), x => x.CellName.Contains(filter))
                .LongCountAsync(GetCancellationToken(cancellationToken));
        }

        public async Task<Cell> FindByIdAsync(int id, bool includeDetails = false, CancellationToken cancellationToken = default)
        {
            return await(await GetDbSetAsync())
                //.IncludeDetails(includeDetails)//包含明细
                .OrderBy(t => t.CreationTime)
                .FirstOrDefaultAsync(t => t.Id == id, GetCancellationToken(cancellationToken));
        }

        public async Task<Cell> FindByNameAsync(string goodsName, bool includeDetails = false, CancellationToken cancellationToken = default)
        {
            return await(await GetDbSetAsync())
                //.IncludeDetails(includeDetails)//包含明细
                .OrderBy(t => t.CreationTime)
                .FirstOrDefaultAsync(t => t.CellName == goodsName, GetCancellationToken(cancellationToken));
        }



        public async Task<Cell> FindByCodeAsync(string cellCode, bool includeDetails = false, CancellationToken cancellationToken = default)
        {
            return await (await GetDbSetAsync())
                //.IncludeDetails(includeDetails)//包含明细
                .OrderBy(t => t.CreationTime)
                .FirstOrDefaultAsync(t => t.CellCode == cellCode||t.CellName==cellCode, GetCancellationToken(cancellationToken));
        }

        

    }
}
