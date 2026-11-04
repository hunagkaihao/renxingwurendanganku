using System.Threading.Tasks;
using Lion.AbpPro.Extension.Customs.Dtos;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using WarehouseManagement.CheckHiss.Dto;

namespace WarehouseManagement.CheckHiss
{
    public interface ICheckHisAppService : IApplicationService
    {
        
        Task<PagedResultDto<CheckHisDto>> GetPagingListAsync(PagingCheckHisDto input);

        Task<PagedResultDto<CheckDetailHisDto>>GetPagingDetailListAsync(PagingCheckDetailHisDto input);

    }
}
