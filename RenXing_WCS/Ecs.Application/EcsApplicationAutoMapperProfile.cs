using AutoMapper;
using Ecs.Backups;
using Ecs.Caches.Models;
using Ecs.Cells.Models;
using Ecs.Conditions.Models;
using Ecs.DahSpecss.Models;
using Ecs.Dispatch;
using Ecs.Jobs.Models;
using Ecs.Log;
using Ecs.LogTool;
using Ecs.Mjj;
using Ecs.Nodes.Models;
using Ecs.PlcMonitor;
using Ecs.Station;

namespace Ecs;

public class EcsApplicationAutoMapperProfile : Profile
{
    public EcsApplicationAutoMapperProfile()
    {
        /* You can configure your AutoMapper mapping configuration here.
         * Alternatively, you can split your mapping configurations
         * into multiple profile classes for a better organization. */
         CreateMap<MonitorValue, MonitorDto>();
         CreateMap<StationInfo, StationInfoDto>();
         CreateMap<MjjStatus, MjjStatusDto>();
         CreateMap<DispatchCell, DispatchCellDto>();
         CreateMap<OrderInRedis, OrderInfoDto>();
         CreateMap<SqliteLogItem, LogDto>();
         CreateMap<DispatchNode, DispatchNodeDto>();
         CreateMap<DispatchNodeDto, DispatchNode>();
         CreateMap<DispatchNodeCmd, DispatchNodeCmdDto>();
         CreateMap<DispatchNodeCmdDto, DispatchNodeCmd>();
         CreateMap<DispatchNodeType, DispatchNodeTypeDto>();
         CreateMap<DispatchNodeTypeDto, DispatchNodeType>();
         CreateMap<DispatchCache, CacheDto>();
         CreateMap<AddCacheDto, DispatchCache>();
         CreateMap<DispatchCondition, ConditionDto>();
         CreateMap<DispatchJobCmd, JobCmdDto>();
         CreateMap<DispatchJobWorker, JobWorkerDto>();
         CreateMap<DahSpecs, DahSpecDto>();
    }
}
