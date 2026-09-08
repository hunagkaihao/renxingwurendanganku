using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using WarehouseManagement.Material.Dto;

namespace WarehouseManagement.Material
{
    public interface IMaterialAppService : IApplicationService
    {
        //创建档案
        Task<MaterialDto> CreateAsync(CreateMaterialDto inpuit);
        //更新档案
        Task<MaterialDto> UpdateAsync(CreateMaterialDto inpuit);
        //删除档案
        Task DeleteAsync(CreateMaterialDto inpuit);
        //获取档案清单
        Task<PagedResultDto<MaterialDto>> PageAsync(PagingMaterialListInput input);
    }
}
