using AutoMapper;
using Wcs.Backups;
using Wcs.Caches.Models;
using Wcs.Cells.Models;
using Wcs.Conditions.Models;
using Wcs.DahSpecss.Models;
using Wcs.Dispatch;
using Wcs.Jobs.Models;
using Wcs.Log;
using Wcs.LogTool;
using Wcs.Mjj;
using Wcs.Nodes.Models;
using Wcs.PlcMonitor;
using Wcs.Station;

namespace Wcs;

public class WcsApplicationAutoMapperProfile : Profile
{
    public WcsApplicationAutoMapperProfile()
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
