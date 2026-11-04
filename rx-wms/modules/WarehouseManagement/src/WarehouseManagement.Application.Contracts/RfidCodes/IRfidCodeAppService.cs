using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using WarehouseManagement.RfidCodes.Dto;

namespace WarehouseManagement.RfidCodes
{
    public interface IRfidCodeAppService : IApplicationService
    {
        //创建标签
        Task<RfidCodeDto> CreateAsync(CreateRfidCodeDto input);

        //批量创建标签
        Task CreateManyAsync(List<CreateRfidCodeDto> input);

        //获取标签数据
        Task<PagedResultDto<RfidCodeDto>> PageAsync(PagingRfidListInput input);

        //删除标签
        Task DeleteAsync(CreateRfidCodeDto input);
        
    }
}
