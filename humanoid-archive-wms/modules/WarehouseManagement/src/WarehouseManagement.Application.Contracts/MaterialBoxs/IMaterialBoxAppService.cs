using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using WarehouseManagement.MaterialBoxs.Dto;

namespace WarehouseManagement.MaterialBoxs
{
    public interface IMaterialBoxAppService : IApplicationService
    {
        //创建档案盒
        Task<MaterialBoxDto> CreateAsync(CreateMaterialBoxDto input);

        //更新档案盒
        Task<MaterialBoxDto> UpdateAsync(CreateMaterialBoxDto input);

        //删除档案盒
        Task DeleteAsync(CreateMaterialBoxDto input);

        //获取档案盒清单
        Task<PagedResultDto<MaterialBoxDto>> PageAsync(PagingMaterialBoxListInput input);

        //档案盒绑定标签
        Task<MaterialBoxDto> BindRfid(CreateMaterialBoxDto input);

        //档案盒绑定档案
        Task<bool> BindArchive(string MaterialBoxRfid, string MaterialRfid);

        //档案盒移除档案
        //Task<bool> RemoveArchive(string BRfid, string ARfid);

        //获取档案盒明细
        Task<PagedResultDto<MaterialBoxDetailDto>> DetailAsync(PagingMaterialBoxDetailInput input);



    }
}
