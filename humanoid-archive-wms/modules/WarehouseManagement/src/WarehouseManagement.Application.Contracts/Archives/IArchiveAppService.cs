using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using WarehouseManagement.Archives.Dto;

namespace WarehouseManagement.Archives
{
    public interface IArchiveAppService : IApplicationService
    {
        //创建档案
        Task<ArchiveDto> CreateAsync(CreateArchiveDto inpuit);
        //更新档案
        Task<ArchiveDto> UpdateAsync(CreateArchiveDto inpuit);
        //删除档案
        Task DeleteAsync(CreateArchiveDto inpuit);
        //获取档案清单
        Task<PagedResultDto<ArchiveDto>> PageAsync(PagingArchiveListInput input);
    }
}
