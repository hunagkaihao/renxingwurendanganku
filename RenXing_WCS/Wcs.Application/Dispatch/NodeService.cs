using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wcs.LogTool;
using Wcs.Nodes;
using Wcs.Nodes.Models;
using Wcs.Processes;
using Wcs.Processes.ProcessTemplates;
using Microsoft.Extensions.Logging;
using Volo.Abp;

namespace Wcs.Dispatch;

public class NodeService : WcsAppService, INodeService
{
    private readonly ILogger<NodeService> _logger;
    private readonly NodeManager _nodeManager;
    private readonly ProcessManager _processManager;
    private readonly TemplateFactory _templateFactory;
    private readonly DoorConfigurationInitializer _doorInitializer;

    public NodeService(
        ILogger<NodeService> logger,
        NodeManager nodeManager,
        ProcessManager processManager,
        TemplateFactory templateFactory,
        DoorConfigurationInitializer doorInitializer)
    {
        _logger = logger;
        _nodeManager = nodeManager;
        _processManager = processManager;
        _templateFactory = templateFactory;
        _doorInitializer = doorInitializer;
    }

    public async Task<ResponseDto> ClearAllNodeCmdsAsync()
    {
        try
        {
            bool ret = await _nodeManager.DelAllNodeCmdsAsync().ConfigureAwait(false);
            return new ResponseDto() { success = ret, message = ret ? "删除成功" : "删除失败" };
        }
        catch(Exception ex)
        {
            _logger.Error(ex.Message);
            return new ResponseDto() { success = false, message = ex.Message };
        }
    }

    public async Task<ResponseDto> ClearAllNodesAsync()
    {
        try
        {
            bool ret = await _nodeManager.DelAllNodesAsync().ConfigureAwait(false);
            return new ResponseDto() { success = ret, message = ret ? "删除成功" : "删除失败" };
        }
        catch(Exception ex)
        {
            _logger.Error(ex.Message);
            return new ResponseDto() { success = false, message = ex.Message };
        }
    }

    public async Task<ResponseDto> ClearAllNodeTypesAsync()
    {
        try
        {
            bool ret = await _nodeManager.DelAllNodeTypesAsync().ConfigureAwait(false);
            return new ResponseDto() { success = ret, message = ret ? "删除成功" : "删除失败" };
        }
        catch(Exception ex)
        {
            _logger.Error(ex.Message);
            return new ResponseDto() { success = false, message = ex.Message };
        }
    }

    public async Task<List<DispatchNodeCmdDto>> GetAllNodeCmdsAsync()
    {
        try
        {
            var cmds = await _nodeManager.GetAllNodeCmdsAsync().ConfigureAwait(false);
            if(cmds == null)
                return new List<DispatchNodeCmdDto>();
            return ObjectMapper.Map<List<DispatchNodeCmd>, List<DispatchNodeCmdDto>>(cmds);
        }
        catch(Exception ex)
        {
            _logger.Error(ex.Message);
            return new List<DispatchNodeCmdDto>();
        }
    }

    public async Task<List<DispatchNodeDto>> GetAllNodesAsync()
    {
        try
        {
            var nodeDic = await _nodeManager.GetAllNodesAsync().ConfigureAwait(false);
            if(nodeDic == null)
                return new List<DispatchNodeDto>();
            return ObjectMapper.Map<List<DispatchNode>, List<DispatchNodeDto>>(nodeDic.Values.ToList());
        }
        catch(Exception ex)
        {
            _logger.Error(ex.Message);
            return new List<DispatchNodeDto>();
        }
    }

    public async Task<List<DispatchNodeTypeDto>> GetAllNodeTypesAsync()
    {
        try
        {
            var nodeTypes = await _nodeManager.GetAllNodeTypesAsync().ConfigureAwait(false);
            if(nodeTypes == null)
                return new List<DispatchNodeTypeDto>();
            return ObjectMapper.Map<List<DispatchNodeType>, List<DispatchNodeTypeDto>>(nodeTypes);
        }
        catch(Exception ex)
        {
            _logger.Error(ex.Message);
            return new List<DispatchNodeTypeDto>();
        }
    }

    public async Task<ResponseDto> NodeCmdSeedAsync()
    {
        try
        {
            await _nodeManager.AddNodeCmdAsync(new DispatchNodeCmd("12", WcsConsts.NodeType_DoorOpen, 10));
            await _nodeManager.AddNodeCmdAsync(new DispatchNodeCmd("13", WcsConsts.NodeType_LMToZeroPos, 1));
            await _nodeManager.AddNodeCmdAsync(new DispatchNodeCmd("13", WcsConsts.NodeType_LMToSafePos, 5));
            await _nodeManager.AddNodeCmdAsync(new DispatchNodeCmd("13", WcsConsts.NodeType_ReadCell, 4));
            await _nodeManager.AddNodeCmdAsync(new DispatchNodeCmd("13", WcsConsts.NodeType_LMOutPick, 2));
            await _nodeManager.AddNodeCmdAsync(new DispatchNodeCmd("13", WcsConsts.NodeType_LMOutPlace, 3));
            await _nodeManager.AddNodeCmdAsync(new DispatchNodeCmd("13", WcsConsts.NodeType_LMInPick, 7));
            await _nodeManager.AddNodeCmdAsync(new DispatchNodeCmd("13", WcsConsts.NodeType_LMInPlace, 8));
            await _nodeManager.AddNodeCmdAsync(new DispatchNodeCmd("13", WcsConsts.NodeType_LMMovePick, 11));
            await _nodeManager.AddNodeCmdAsync(new DispatchNodeCmd("13", WcsConsts.NodeType_LMMovePlace, 12));
                        
            return new ResponseDto(){ success = true, message = "success" };           
        }
        catch(Exception e)
        {
            _logger.Error(e.Message);
            return new ResponseDto(){ success = false, message = e.Message };
        }
    }

    public async Task<ResponseDto> NodeSeedAsync()
    {
        try
        {
            await _nodeManager.AddNodeAsync(new DispatchNode(9){
                NodeCode = "13001", NodeName = "龙门夹爪", NodeTypeCode = "13", DASpecs = "any",
                CmdTagName = "Plc1.Lm_Cmd", ResponseTagName = "Plc1.Lm_Response",
                TaskIdOwnIt = -1, NodeState = EnumDispatchNodeState.Idle
            });
            await _nodeManager.AddNodeAsync(new DispatchNode(10){
                NodeCode = "15001", NodeName = "密集架", NodeTypeCode = "15", DASpecs = "any",
                CmdTagName = "-", ResponseTagName = "-",
                TaskIdOwnIt = -1, NodeState = EnumDispatchNodeState.Idle
            });
            await _nodeManager.AddNodeAsync(new DispatchNode(11){
                NodeCode = "17001", NodeName = "缓存位1", NodeTypeCode = "17", DASpecs = "_1cm",
                CmdTagName = "-", ResponseTagName = "-",
                TaskIdOwnIt = -1, NodeState = EnumDispatchNodeState.Idle
            });
            await _nodeManager.AddNodeAsync(new DispatchNode(12){
                NodeCode = "17002", NodeName = "缓存位2", NodeTypeCode = "17", DASpecs = "_1cm",
                CmdTagName = "-", ResponseTagName = "-",
                TaskIdOwnIt = -1, NodeState = EnumDispatchNodeState.Idle
            });
            await _nodeManager.AddNodeAsync(new DispatchNode(13){
                NodeCode = "17003", NodeName = "缓存位3", NodeTypeCode = "17", DASpecs = "_2cm",
                CmdTagName = "-", ResponseTagName = "-",
                TaskIdOwnIt = -1, NodeState = EnumDispatchNodeState.Idle
            });
            await _nodeManager.AddNodeAsync(new DispatchNode(14){
                NodeCode = "17004", NodeName = "缓存位4", NodeTypeCode = "17", DASpecs = "_2cm",
                CmdTagName = "-", ResponseTagName = "-",
                TaskIdOwnIt = -1, NodeState = EnumDispatchNodeState.Idle
            });
            await _nodeManager.AddNodeAsync(new DispatchNode(15){
                NodeCode = "17005", NodeName = "缓存位5", NodeTypeCode = "17", DASpecs = "_5cm",
                CmdTagName = "-", ResponseTagName = "-",
                TaskIdOwnIt = -1, NodeState = EnumDispatchNodeState.Idle
            });
            await _nodeManager.AddNodeAsync(new DispatchNode(16){
                NodeCode = "17006", NodeName = "缓存位6", NodeTypeCode = "17", DASpecs = "_3cm",
                CmdTagName = "-", ResponseTagName = "-",
                TaskIdOwnIt = -1, NodeState = EnumDispatchNodeState.Idle
            });
            await _nodeManager.AddNodeAsync(new DispatchNode(17){
                NodeCode = "18001", NodeName = "虚拟对象1", NodeTypeCode = "18", DASpecs = "any",
                CmdTagName = "-", ResponseTagName = "-",
                TaskIdOwnIt = -1, NodeState = EnumDispatchNodeState.Idle
            });
            await _nodeManager.AddNodeAsync(new DispatchNode(18){
                NodeCode = "18002", NodeName = "虚拟对象2", NodeTypeCode = "18", DASpecs = "any",
                CmdTagName = "-", ResponseTagName = "-",
                TaskIdOwnIt = -1, NodeState = EnumDispatchNodeState.Idle
            });
            await _nodeManager.AddNodeAsync(new DispatchNode(19){
                NodeCode = "18003", NodeName = "虚拟对象3", NodeTypeCode = "18", DASpecs = "any",
                CmdTagName = "-", ResponseTagName = "-",
                TaskIdOwnIt = -1, NodeState = EnumDispatchNodeState.Idle
            });
            await _nodeManager.AddNodeAsync(new DispatchNode(20){
                NodeCode = "18004", NodeName = "虚拟对象4", NodeTypeCode = "18", DASpecs = "any",
                CmdTagName = "-", ResponseTagName = "-",
                TaskIdOwnIt = -1, NodeState = EnumDispatchNodeState.Idle
            });
            await _nodeManager.AddNodeAsync(new DispatchNode(21){
                NodeCode = "18005", NodeName = "虚拟对象5", NodeTypeCode = "18", DASpecs = "any",
                CmdTagName = "-", ResponseTagName = "-",
                TaskIdOwnIt = -1, NodeState = EnumDispatchNodeState.Idle
            });
            await _nodeManager.AddNodeAsync(new DispatchNode(22){
                NodeCode = "18006", NodeName = "虚拟对象6", NodeTypeCode = "18", DASpecs = "any",
                CmdTagName = "-", ResponseTagName = "-",
                TaskIdOwnIt = -1, NodeState = EnumDispatchNodeState.Idle
            });
            await _nodeManager.AddNodeAsync(new DispatchNode(23){
                NodeCode = "18007", NodeName = "虚拟对象7", NodeTypeCode = "18", DASpecs = "any",
                CmdTagName = "-", ResponseTagName = "-",
                TaskIdOwnIt = -1, NodeState = EnumDispatchNodeState.Idle
            });
            await _nodeManager.AddNodeAsync(new DispatchNode(24){
                NodeCode = "18008", NodeName = "虚拟对象8", NodeTypeCode = "18", DASpecs = "any",
                CmdTagName = "-", ResponseTagName = "-",
                TaskIdOwnIt = -1, NodeState = EnumDispatchNodeState.Idle
            });
            await _nodeManager.AddNodeAsync(new DispatchNode(25){
                NodeCode = "18009", NodeName = "虚拟对象9", NodeTypeCode = "18", DASpecs = "any",
                CmdTagName = "-", ResponseTagName = "-",
                TaskIdOwnIt = -1, NodeState = EnumDispatchNodeState.Idle
            });
            await _nodeManager.AddNodeAsync(new DispatchNode(26){
                NodeCode = "18010", NodeName = "虚拟对象10", NodeTypeCode = "18", DASpecs = "any",
                CmdTagName = "-", ResponseTagName = "-",
                TaskIdOwnIt = -1, NodeState = EnumDispatchNodeState.Idle
            });
            await _nodeManager.AddNodeAsync(new DispatchNode(27){
                NodeCode = "18011", NodeName = "虚拟对象11", NodeTypeCode = "18", DASpecs = "any",
                CmdTagName = "-", ResponseTagName = "-",
                TaskIdOwnIt = -1, NodeState = EnumDispatchNodeState.Idle
            });
            await _nodeManager.AddNodeAsync(new DispatchNode(28){
                NodeCode = "18012", NodeName = "虚拟对象12", NodeTypeCode = "18", DASpecs = "any",
                CmdTagName = "-", ResponseTagName = "-",
                TaskIdOwnIt = -1, NodeState = EnumDispatchNodeState.Idle
            });
            await _nodeManager.AddNodeAsync(new DispatchNode(29){
                NodeCode = "18013", NodeName = "虚拟对象13", NodeTypeCode = "18", DASpecs = "any",
                CmdTagName = "-", ResponseTagName = "-",
                TaskIdOwnIt = -1, NodeState = EnumDispatchNodeState.Idle
            });
            await _nodeManager.AddNodeAsync(new DispatchNode(30){
                NodeCode = "18014", NodeName = "虚拟对象14", NodeTypeCode = "18", DASpecs = "any",
                CmdTagName = "-", ResponseTagName = "-",
                TaskIdOwnIt = -1, NodeState = EnumDispatchNodeState.Idle
            });
            await _nodeManager.AddNodeAsync(new DispatchNode(31){
                NodeCode = "18015", NodeName = "虚拟对象15", NodeTypeCode = "18", DASpecs = "any",
                CmdTagName = "-", ResponseTagName = "-",
                TaskIdOwnIt = -1, NodeState = EnumDispatchNodeState.Idle
            });
            await _nodeManager.AddNodeAsync(new DispatchNode(32){
                NodeCode = "18016", NodeName = "虚拟对象16", NodeTypeCode = "18", DASpecs = "any",
                CmdTagName = "-", ResponseTagName = "-",
                TaskIdOwnIt = -1, NodeState = EnumDispatchNodeState.Idle
            });
                        
            await _doorInitializer.EnsureAsync();
            return new ResponseDto(){ success = true, message = "success" };           
        }
        catch(Exception e)
        {
            _logger.Error(e.Message);
            return new ResponseDto(){ success = false, message = e.Message };
        }
    }

    public async Task<ResponseDto> NodeTypeSeedAsync()
    {
        try
        {
            await _nodeManager.AddNodeTypeAsync(new DispatchNodeType("取档口", "12", "取档口"));
            await _nodeManager.AddNodeTypeAsync(new DispatchNodeType("龙门夹爪", "13", "龙门夹爪"));
            await _nodeManager.AddNodeTypeAsync(new DispatchNodeType("密集架", "15", "密集架"));
            await _nodeManager.AddNodeTypeAsync(new DispatchNodeType("缓存箱", "17", "缓存箱"));
            await _nodeManager.AddNodeTypeAsync(new DispatchNodeType("虚拟对象", "18", "虚拟对象"));
                        
            return new ResponseDto(){ success = true, message = "success" };           
        }
        catch(Exception e)
        {
            _logger.Error(e.Message);
            return new ResponseDto(){ success = false, message = e.Message };
        }
    }

}