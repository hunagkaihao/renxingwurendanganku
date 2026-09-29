using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using WarehouseManagement.Material.Dto;

namespace WarehouseManagement.Material
{
    public interface IMaterialAppService : IApplicationService
    {
        /// <summary>
        /// 使用基础信息创建物料。
        /// </summary>
        /// <param name="input">物料的七项基础信息，不包含主键及档案、RFID 字段。</param>
        /// <returns>已保存的物料 ID 和七项基础信息。</returns>
        Task<CreateMaterialResultDto> CreateAsync(CreateMaterialInputDto input);
        //更新档案
        Task<MaterialDto> UpdateAsync(CreateMaterialDto inpuit);
        //删除档案
        Task DeleteAsync(CreateMaterialDto inpuit);
        //获取档案清单
        Task<PagedResultDto<MaterialDto>> PageAsync(PagingMaterialListInput input);
    }
}
