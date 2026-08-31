using System.Threading;
using System.Threading.Tasks;
using Wcs.Cells.Models;
using Volo.Abp.Domain.Repositories;

namespace Wcs.Cells;

public interface ICellRepository : IRepository<DispatchCell, int>
{
    public Task<DispatchCell> FindByCellCodeAsync(string cellCode, CancellationToken cancelToken = default);
    public Task<DispatchCell> FindByWmsCellXYZAsync(int row, int col, int layer, CancellationToken cancelToken = default);
    public Task<DispatchCell> FindByPlcCellXYZAsync(int row, int layer, int section, int colInSection, CancellationToken cancelToken = default);
}