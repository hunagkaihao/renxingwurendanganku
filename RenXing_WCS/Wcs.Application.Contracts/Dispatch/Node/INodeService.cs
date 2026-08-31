using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Wcs.Dispatch;

public interface INodeService : IApplicationService
{
    public Task<ResponseDto> ClearAllNodeTypesAsync();
    public Task<ResponseDto> ClearAllNodeCmdsAsync();
    public Task<ResponseDto> ClearAllNodesAsync();
    public Task<ResponseDto> NodeTypeSeedAsync();
    public Task<ResponseDto> NodeCmdSeedAsync();
    public Task<ResponseDto> NodeSeedAsync();
    public Task<List<DispatchNodeTypeDto>> GetAllNodeTypesAsync();
    public Task<List<DispatchNodeCmdDto>> GetAllNodeCmdsAsync();
    public Task<List<DispatchNodeDto>> GetAllNodesAsync();
}