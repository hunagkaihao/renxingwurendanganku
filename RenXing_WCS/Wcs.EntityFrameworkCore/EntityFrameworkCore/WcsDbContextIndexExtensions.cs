using Microsoft.EntityFrameworkCore;
using Wcs.Caches.Models;
using Wcs.Cells.Models;
using Wcs.Conditions.Models;
using Wcs.DahSpecss.Models;
using Wcs.Jobs.Models;
using Wcs.Nodes.Models;
using Wcs.Orders.Models;
using Wcs.Processes.Models;
using Wcs.Tasks.Models;

namespace Wcs.EntityFrameworkCore;

/// <summary>调度业务查询索引；存量 MySQL 使用仓库根目录 scripts/database/optimize-query-indexes.sql 同步。</summary>
public static class WcsDbContextIndexExtensions
{
    /// <summary>
    /// 按仓储查询条件配置非唯一索引，保留原有业务重复校验和异常处理。
    /// 联合索引覆盖其最左前缀，避免为相同查询重复建立单列索引。
    /// </summary>
    /// <param name="builder">WCS 模型构建器，兼容 MySQL 和 SQLite。</param>
    public static void ConfigureDispatchQueryIndexes(this ModelBuilder builder)
    {
        // EfCoreCellRepository：库位编码、WMS 坐标、PLC 坐标（含按排层读取）。
        builder.Entity<DispatchCell>().HasIndex(x => x.CellCode);
        builder.Entity<DispatchCell>().HasIndex(x => new { x.Row, x.Col, x.Layer });
        builder.Entity<DispatchCell>().HasIndex(x => new
            { x.RowForPlc, x.LayerForPlc, x.SectNoForPlc, x.ColNoInSectForPlc })
            .HasDatabaseName("IX_dispatchcells_PlcCoordinates");

        // EfCoreDahSpecsRepository / EfCoreCacheRepository：规格及缓存位置精确定位。
        builder.Entity<DahSpecs>().HasIndex(x => x.SpecCode);
        builder.Entity<DahSpecs>().HasIndex(x => x.SpecName);
        builder.Entity<DispatchCache>().HasIndex(x => x.CachePos);
        builder.Entity<DispatchCache>().HasIndex(x => x.Specs);

        // NodeManager / ConditionManager：节点编码、任务资源归属、节点指令和条件名。
        builder.Entity<DispatchNode>().HasIndex(x => x.NodeCode);
        builder.Entity<DispatchNode>().HasIndex(x => x.TaskIdOwnIt);
        builder.Entity<DispatchNodeType>().HasIndex(x => x.NodeTypeCode);
        builder.Entity<DispatchNodeCmd>().HasIndex(x => new { x.NodeTypeCode, x.NodeCmdName });
        builder.Entity<DispatchCondition>().HasIndex(x => x.ConditionName);

        // ProcessManager：路径匹配、步骤读取、同一步骤的资源和前置条件读取。
        builder.Entity<DispatchProcess>().HasIndex(x => new { x.StartNodeCode, x.EndNodeCode });
        builder.Entity<DispatchProcessStep>().HasIndex(x => new { x.ProcessId, x.Sequence });
        builder.Entity<DispatchProcessStepResource>().HasIndex(x => new { x.ProcessId, x.Sequence });
        builder.Entity<DispatchProcessStepPrecondition>().HasIndex(x => new { x.ProcessId, x.Sequence });

        // OrderManager / TaskManager / JobManager：订单、待执行状态及任务步骤关联。
        builder.Entity<DispatchOrder>().HasIndex(x => x.OrderCode);
        builder.Entity<DispatchOrder>().HasIndex(x => new { x.State, x.OrderType });
        builder.Entity<DispatchTask>().HasIndex(x => x.OrderCode);
        builder.Entity<DispatchTask>().HasIndex(x => x.State);
        builder.Entity<DispatchJob>().HasIndex(x => new { x.TaskId, x.ProcessSequence });
        builder.Entity<DispatchJobCmd>().HasIndex(x => x.JobCmdClassName);
        builder.Entity<DispatchJobWorker>().HasIndex(x => x.JobWorkerClassName);

        // 盘点反馈按查询批次或订单/库位读取，订单联合索引也覆盖订单号查询。
        builder.Entity<DispatchChkOrderRslt>().HasIndex(x => x.QueryCode);
        builder.Entity<DispatchChkOrderRslt>().HasIndex(x => new { x.OrderCode, x.CellCode });
    }
}
