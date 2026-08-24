using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using WarehouseManagement.ArchiveBoxs.Dto;
namespace WarehouseManagement.ArchiveBoxs
{
    public interface IArchiveBoxAppService : IApplicationService
    {
        //创建档案盒
        Task<ArchiveBoxDto> CreateAsync(CreateArchiveBoxDto input);

        //更新档案盒
        Task<ArchiveBoxDto> UpdateAsync(CreateArchiveBoxDto input);

        //删除档案盒
        Task DeleteAsync(CreateArchiveBoxDto input);

        //获取档案盒清单
        Task<PagedResultDto<ArchiveBoxDto>> PageAsync(PagingArchiveBoxListInput input);

        //档案盒绑定标签
        Task<ArchiveBoxDto> BindRfid(CreateArchiveBoxDto input);

        //档案盒绑定档案
        Task<bool> BindArchive(string ArchiveBoxRfid, string ArchiveRfid);

        //档案盒移除档案
        //Task<bool> RemoveArchive(string BRfid, string ARfid);

        //获取档案盒明细
        Task<PagedResultDto<ArchiveBoxDetailDto>> DetailAsync(PagingArchiveBoxDetailInput input);



    }
}
