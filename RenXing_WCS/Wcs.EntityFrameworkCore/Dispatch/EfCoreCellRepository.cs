using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Wcs.Cells;
using Wcs.Cells.Models;
using Wcs.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Wms.EntityFrameworkCore.Repositories.ArchiveBoxs
{
    public class EfCoreCellRepository : EfCoreRepository<WcsDbContext, DispatchCell, int>, ICellRepository
    {
        public EfCoreCellRepository(IDbContextProvider<WcsDbContext> dbContextProvider) : base(dbContextProvider)
        {

        }

        public async Task<DispatchCell> FindByCellCodeAsync(string cellCode, CancellationToken cancelToken = default)
        {
            var dbSet = await GetDbSetAsync().ConfigureAwait(false);
            var cells = await dbSet.AsNoTracking().Where(o => o.CellCode == cellCode).ToListAsync(cancelToken);
            if(cells.Count > 1)
                throw new Exception($"库位{cellCode}数量不止1个，数据错误");
            if(cells.Count == 1) return cells[0];
            else return null;
        }

        public async Task<DispatchCell> FindByPlcCellXYZAsync(int row, int layer, int section, int colInSection, CancellationToken cancelToken = default)
        {
            var dbSet = await GetDbSetAsync().ConfigureAwait(false);
            var cells = await dbSet.AsNoTracking().Where(o => 
                o.RowForPlc == row && 
                o.LayerForPlc == layer && 
                o.SectNoForPlc == section &&
                o.ColNoInSectForPlc == colInSection).ToListAsync(cancelToken);
            if(cells.Count > 1)
                throw new Exception($"{row}排{layer}层{section}节{colInSection}列的库位数量不止1个，数据错误");
            if(cells.Count == 1) return cells[0];
            else return null;
        }

        public async Task<DispatchCell> FindByWmsCellXYZAsync(int row, int col, int layer, CancellationToken cancelToken = default)
        {
            var dbSet = await GetDbSetAsync().ConfigureAwait(false);
            var cells = await dbSet.AsNoTracking().Where(o => o.Row == row && o.Col == col && o.Layer == layer).ToListAsync(cancelToken);
            if(cells.Count > 1)
                throw new Exception($"{row}排{col}列{layer}层的库位数量不止1个，数据错误");
            if(cells.Count == 1) return cells[0];
            else return null;
        }
    }
}