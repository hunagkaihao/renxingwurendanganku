using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Wcs.Dispatch;

public interface ICellService : IApplicationService
{
    public Task<ResponseDto> CellSeedsAsync(AddCellsDto cellsDto);

    public Task<ResponseDto> CellsAllClearAsync();
}