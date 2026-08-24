using System.Collections.Generic;
using System.Threading.Tasks;
using Ecs.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace Ecs.Dispatch;

[Route("ecs/dispatch")]
[ApiController]
public class NodeController : EcsController, INodeService
{
    private readonly INodeService _nodeService;

    public NodeController(INodeService nodeService)
    {
        _nodeService = nodeService;
    }

    [HttpPost("node/allNodeCmdClear")]
    public async Task<ResponseDto> ClearAllNodeCmdsAsync()
    {
        return await _nodeService.ClearAllNodeCmdsAsync().ConfigureAwait(false);
    }

    [HttpPost("node/allNodeClear")]
    public async Task<ResponseDto> ClearAllNodesAsync()
    {
        return await _nodeService.ClearAllNodesAsync().ConfigureAwait(false);
    }

    [HttpPost("node/allNodeTypeClear")]
    public async Task<ResponseDto> ClearAllNodeTypesAsync()
    {
        return await _nodeService.ClearAllNodeTypesAsync().ConfigureAwait(false);
    }

    [HttpGet("node/allNodeCmdGet")]
    public async Task<List<DispatchNodeCmdDto>> GetAllNodeCmdsAsync()
    {
        return await _nodeService.GetAllNodeCmdsAsync().ConfigureAwait(false);
    }

    [HttpGet("node/allNodeGet")]
    public async Task<List<DispatchNodeDto>> GetAllNodesAsync()
    {
        return await _nodeService.GetAllNodesAsync().ConfigureAwait(false);
    }

    [HttpGet("node/allNodeTypeGet")]
    public async Task<List<DispatchNodeTypeDto>> GetAllNodeTypesAsync()
    {
        return await _nodeService.GetAllNodeTypesAsync().ConfigureAwait(false);
    }

    [HttpPost("node/nodeCmdSeed")]
    public async Task<ResponseDto> NodeCmdSeedAsync()
    {
        return await _nodeService.NodeCmdSeedAsync().ConfigureAwait(false);
    }

    [HttpPost("node/nodeSeed")]
    public async Task<ResponseDto> NodeSeedAsync()
    {
        return await _nodeService.NodeSeedAsync().ConfigureAwait(false);
    }

    [HttpPost("node/nodeTypeSeed")]
    public async Task<ResponseDto> NodeTypeSeedAsync()
    {
        return await _nodeService.NodeTypeSeedAsync().ConfigureAwait(false);
    }
}