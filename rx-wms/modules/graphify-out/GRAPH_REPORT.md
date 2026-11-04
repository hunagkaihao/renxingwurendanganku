# Graph Report - modules  (2026-09-09)

## Corpus Check
- cluster-only mode — file stats not available

## Summary
- 3412 nodes · 6179 edges · 189 communities (181 shown, 7 thin omitted)
- Extraction: 93% EXTRACTED · 7% INFERRED · 0% AMBIGUOUS · INFERRED: 423 edges (avg confidence: 0.82)
- Token cost: 0 input · 0 output

## Graph Freshness
- Built from commit: `712e9f04`
- Run `git rev-parse HEAD` and compare to check if the graph is stale.
- Run `graphify update .` after code changes (no API cost).

## Community Hubs (Navigation)
- Community 0
- Community 1
- Community 2
- Community 3
- Community 4
- Community 5
- Community 6
- Community 7
- Community 8
- Community 9
- Community 10
- Community 11
- Community 12
- Community 13
- Community 14
- Community 15
- Community 16
- Community 17
- Community 18
- Community 19
- Community 20
- Community 21
- Community 22
- Community 23
- Community 24
- Community 25
- Community 26
- Community 27
- Community 28
- Community 29
- Community 30
- Community 31
- Community 32
- Community 33
- Community 34
- Community 35
- Community 36
- Community 37
- Community 38
- Community 39
- Community 40
- Community 41
- Community 42
- Community 43
- Community 44
- Community 45
- Community 46
- Community 47
- Community 48
- Community 49
- Community 50
- Community 51
- Community 52
- Community 53
- Community 54
- Community 55
- Community 56
- Community 57
- Community 58
- Community 59
- Community 60
- Community 61
- Community 62
- Community 63
- Community 64
- Community 65
- Community 66
- Community 67
- Community 68
- Community 69
- Community 70
- Community 71
- Community 72
- Community 73
- Community 74
- Community 75
- Community 76
- Community 77
- Community 78
- Community 79
- Community 80
- Community 81
- Community 82
- Community 83
- Community 84
- Community 85
- Community 86
- Community 87
- Community 88
- Community 89
- Community 90
- Community 91
- Community 92
- Community 93
- Community 94
- Community 95
- Community 96
- Community 97
- Community 98
- Community 99
- Community 100
- Community 101
- Community 102
- Community 103
- Community 104
- Community 105
- Community 106
- Community 107
- Community 108
- Community 109
- Community 110
- Community 111
- Community 112
- Community 113
- Community 114
- Community 115
- Community 116
- Community 117
- Community 118
- Community 119
- Community 120
- Community 121
- Community 122
- Community 123
- Community 124
- Community 125
- Community 126
- Community 127
- Community 128
- Community 129
- Community 130
- Community 131
- Community 132
- Community 133
- Community 134
- Community 135
- Community 136
- Community 137
- Community 138
- Community 139
- Community 140
- Community 141
- Community 142
- Community 143
- Community 144
- Community 145
- Community 146
- Community 147
- Community 148
- Community 149
- Community 150
- Community 151
- Community 152
- Community 153
- Community 154
- Community 155
- Community 156
- Community 157
- Community 158
- Community 159
- Community 160
- Community 161
- Community 162
- Community 163
- Community 164
- Community 165
- Community 166
- Community 167
- Community 168
- Community 169
- Community 170
- Community 171
- Community 172
- Community 173
- Community 174
- Community 175
- Community 176
- Community 177
- Community 178
- Community 179
- Community 180
- Community 181
- Community 182
- Community 183
- Community 184
- Community 185
- Community 186
- Community 188

## God Nodes (most connected - your core abstractions)
1. `StockTask` - 79 edges
2. `PlanList` - 78 edges
3. `Material` - 78 edges
4. `MaterialBox` - 74 edges
5. `Goods` - 73 edges
6. `Plan` - 65 edges
7. `WarehouseManagementDbContext` - 56 edges
8. `StockTaskDetail` - 53 edges
9. `StockTaskManager` - 48 edges
10. `MaterialDto` - 45 edges

## Surprising Connections (you probably didn't know these)
- `StockTaskDto` --references--> `TaskStatus`  [EXTRACTED]
  WarehouseManagement/src/WarehouseManagement.Application.Contracts/StockTasks/Dto/StockTaskDto.cs → WarehouseManagement/src/WarehouseManagement.Application.Contracts/StockTasks/Dto/CreateBindBoxStockTaskDto.cs
- `Plan` --references--> `PlanList`  [EXTRACTED]
  WarehouseManagement/src/WarehouseManagement.Domain/Plans/Aggregates/Plan.cs → WarehouseManagement/src/WarehouseManagement.Domain/Plans/Aggregates/PlanList.cs
- `IPlanListRepository` --references--> `PlanList`  [EXTRACTED]
  WarehouseManagement/src/WarehouseManagement.Domain/Plans/IPlanListRepository.cs → WarehouseManagement/src/WarehouseManagement.Domain/Plans/Aggregates/PlanList.cs
- `EfCorePlanListRepository` --references--> `PlanList`  [EXTRACTED]
  WarehouseManagement/src/WarehouseManagement.EntityFrameworkCore/EntityFrameworkCore/Plans/EfCorePlanListRepository.cs → WarehouseManagement/src/WarehouseManagement.Domain/Plans/Aggregates/PlanList.cs
- `WarehouseManagementDbContext` --references--> `PlanList`  [EXTRACTED]
  WarehouseManagement/src/WarehouseManagement.EntityFrameworkCore/EntityFrameworkCore/WarehouseManagementDbContext.cs → WarehouseManagement/src/WarehouseManagement.Domain/Plans/Aggregates/PlanList.cs

## Import Cycles
- None detected.

## Communities (189 total, 7 thin omitted)

### Community 0 - "Community 0"
Cohesion: 0.03
Nodes (61): Guid, PlanList, ArrivalDate, BatchAttr07, BatchAttr08, BatchAttr09, BatchAttr10, BatchAttr11 (+53 more)

### Community 1 - "Community 1"
Cohesion: 0.05
Nodes (42): MaterialBoxDomainService, Guid, List, MaterialBox, CellId, CellModel, ClassCode, ClassName (+34 more)

### Community 2 - "Community 2"
Cohesion: 0.05
Nodes (41): WarehouseManagement.Plans.Dto, CreatePlanDetailInput, GoodsCode, GoodsId, PlanId, WarehouseCode, CreatePlanDto, AreaCode (+33 more)

### Community 3 - "Community 3"
Cohesion: 0.04
Nodes (51): Decimal, Guid, String, Material, CheckFrequency, ChenWendate, ClassCode, ClassType (+43 more)

### Community 4 - "Community 4"
Cohesion: 0.04
Nodes (44): Decimal, Guid, String, Goods, AbcFlag, AbolishFlag, CheckFrequency, ExpireWarnTime (+36 more)

### Community 5 - "Community 5"
Cohesion: 0.05
Nodes (43): Decimal, PlanListDto, BatchAttr07, BatchAttr08, BatchAttr09, BatchAttr10, BatchAttr11, BatchAttr12 (+35 more)

### Community 6 - "Community 6"
Cohesion: 0.08
Nodes (29): PagedResultDto, Guid, List, Check, AccuracyFlag, AreaCode, BatchNo, BeginTime (+21 more)

### Community 7 - "Community 7"
Cohesion: 0.10
Nodes (25): CreateWarehouseAreaDto, WarehouseAreaCode, WarehouseAreaName, WarehouseAreaType, WarehouseId, PagingWarehouseListInput, Filter, WarehouseAreaDto (+17 more)

### Community 8 - "Community 8"
Cohesion: 0.05
Nodes (37): Decimal, MaterialBoxDetail, ArrivalDate, BackFlag, CreatorUserId, DeleterUserId, EntryTime, GoodsBatchNo (+29 more)

### Community 9 - "Community 9"
Cohesion: 0.06
Nodes (27): WarehouseManagement.WcsTasks.Dto, WarehouseManagement.WcsTasks, WarehouseManagement.StockTasks.Dto, DaysSumDto, Days, StockInSums, StockOutSums, DeleteStockTaskDetailInput (+19 more)

### Community 10 - "Community 10"
Cohesion: 0.06
Nodes (33): IDistributedEventHandler, ILocalEventHandler, ITransientDependency, IDistributedEventBus, StockTaskCanceledHisLocalEventHandler, IDistributedEventBus, Task, StockTaskCanceledLocalEventHandler (+25 more)

### Community 11 - "Community 11"
Cohesion: 0.11
Nodes (22): GoodsDto, GoodsCode, GoodsConstProperty1, GoodsName, GoodsSpec, GoodsStatus, GoodsUnits, GoodsSelectDto (+14 more)

### Community 12 - "Community 12"
Cohesion: 0.05
Nodes (36): DateTime, Decimal, Guid, StockTaskDetail, ArrivalDate, BackFlag, Borrower, BorrowerDate (+28 more)

### Community 13 - "Community 13"
Cohesion: 0.12
Nodes (10): CheckDetailHisDomainService, AllowAnonymous, ICurrentUser, IdIntInput, Task, UnitOfWork, CheckAppService, List (+2 more)

### Community 14 - "Community 14"
Cohesion: 0.06
Nodes (29): ManageStatus, Guid, List, ManageType, StockTask, CellModel, Details, EndCellCode (+21 more)

### Community 15 - "Community 15"
Cohesion: 0.06
Nodes (33): Guid, List, Plan, AreaCode, Details, HdDefineStr1, HdDefineStr2, HdDefineStr3 (+25 more)

### Community 16 - "Community 16"
Cohesion: 0.06
Nodes (37): DateTime, Decimal, Guid, TaskHisDetail, ArrivalDate, BackFlag, Borrower, BorrowerDate (+29 more)

### Community 17 - "Community 17"
Cohesion: 0.13
Nodes (9): FbqOptions, IIdentityUserAppService, StockTaskDomainService, ICurrentUser, List, Task, UnitOfWork, StockTaskManager (+1 more)

### Community 18 - "Community 18"
Cohesion: 0.06
Nodes (35): DateTime, Decimal, MaterialBoxDetailDto, ArrivalDate, BackFlag, BoxBarcode, CreationTime, CreatorUserId (+27 more)

### Community 19 - "Community 19"
Cohesion: 0.07
Nodes (23): WarehouseManagement.Plans, WarehouseManagement.EntityFrameworkCore.Plans, WarehouseManagement.Plans.Aggregates, PlanExecuteType, Automatic, Manual, PlanPriority, Delay (+15 more)

### Community 20 - "Community 20"
Cohesion: 0.06
Nodes (35): String, MaterialDto, ChenWendate, ClassCode, ClassType, Director, EndNo, GoodsAJCode (+27 more)

### Community 21 - "Community 21"
Cohesion: 0.06
Nodes (34): Guid, Cell, AreaId, BelongArea, Cell_x, Cell_y, Cell_z, CellCode (+26 more)

### Community 22 - "Community 22"
Cohesion: 0.07
Nodes (20): AbpAutoMapperOptions, AbpModule, WarehouseManagement, WarehouseManagementApplicationContractsModule, WarehouseManagementRemoteServiceConsts, ServiceConfigurationContext, WarehouseManagementApplicationModule, WarehouseManagementErrorCodes (+12 more)

### Community 23 - "Community 23"
Cohesion: 0.06
Nodes (28): EntityDto, PagingCellOutput, Cell_x, Cell_y, Cell_z, CellCode, CellName, CellType (+20 more)

### Community 24 - "Community 24"
Cohesion: 0.06
Nodes (33): String, CreateMaterialDto, ChenWendate, ClassCode, ClassType, Director, EndNo, GoodsAJCode (+25 more)

### Community 25 - "Community 25"
Cohesion: 0.06
Nodes (31): AbpDbContext, DbSet, CheckHis, ModelBuilder, Rfid, WarehouseManagementDbContext, Archivebox, Archiveboxdetail (+23 more)

### Community 26 - "Community 26"
Cohesion: 0.15
Nodes (8): Boolean, IdIntInput, List, ManageStatus, PagedResultDto, Task, UnitOfWork, StockTaskAppService

### Community 27 - "Community 27"
Cohesion: 0.13
Nodes (20): Guid, WarehouseArea, TenantId, WarehouseAreaCode, WarehouseAreaFlag, WarehouseAreaGroup, WarehouseAreaName, WarehouseAreaOrder (+12 more)

### Community 28 - "Community 28"
Cohesion: 0.06
Nodes (29): DateTime, String, ManageMain, ArchiveBoxRfid, CellModel, CreationTime, CreatorUserId, DeleterUserId (+21 more)

### Community 29 - "Community 29"
Cohesion: 0.09
Nodes (25): DateTime, String, PagingTaskHisListInput, EndCreationTime, Filter, ManageStatus, StartCreationTime, Decimal (+17 more)

### Community 30 - "Community 30"
Cohesion: 0.13
Nodes (19): Guid, LogicArea, LogicAreaCode, LogicAreaFlag, LogicAreaGroup, LogicAreaName, LogicAreaOrder, LogicAreaRemark (+11 more)

### Community 31 - "Community 31"
Cohesion: 0.08
Nodes (19): AbpControllerBase, AbpExceptionLocalizationOptions, ApplicationService, WarehouseManagement.Localization, IMvcBuilder, IPermissionDefinitionContext, LocalizableString, PermissionDefinitionProvider (+11 more)

### Community 32 - "Community 32"
Cohesion: 0.14
Nodes (18): Guid, Warehouse, TenantId, WarehouseCode, WarehouseFlag, WarehouseName, WarehouseOrder, WarehouseRemark (+10 more)

### Community 33 - "Community 33"
Cohesion: 0.08
Nodes (24): CheckDetailDomainService, Guid, CheckDetail, Account, BeginTime, BoxBarcode, CellName, Checker (+16 more)

### Community 34 - "Community 34"
Cohesion: 0.09
Nodes (23): CheckHisDomainService, Guid, CheckHis, AccuracyFlag, AreaCode, BatchNo, BeginTime, CheckCode (+15 more)

### Community 35 - "Community 35"
Cohesion: 0.09
Nodes (14): WarehouseManagement.EntityFrameworkCore.Warehouses, WarehouseManagement.Warehouses.Aggregates, WarehouseManagement.Warehouses, WarehouseManagement.Permissions, WarehouseAreaType, J, L, P (+6 more)

### Community 36 - "Community 36"
Cohesion: 0.08
Nodes (19): WarehouseManagement.EntityFrameworkCore.Goodss, WarehouseManagement.Goodss, WarehouseManagement.Goodss.Aggregates, AreaType, CounterArea, CounterCacheArea, CounterWaitingArea, FloorWaitingArea (+11 more)

### Community 37 - "Community 37"
Cohesion: 0.11
Nodes (18): net5.0, Abp (9.0.0), Microsoft.Extensions.FileProviders.Embedded (6.0.0), Serilog (2.10.0), Swashbuckle.AspNetCore.Annotations (6.2.3), Volo.Abp.AspNetCore.Mvc (5.1.3), Volo.Abp.Authorization (5.1.3), Volo.Abp.AutoMapper (5.1.3) (+10 more)

### Community 38 - "Community 38"
Cohesion: 0.19
Nodes (10): CancellationToken, List, Task, ICellRepository, Cell, CancellationToken, IWarehouseManagementDbContext, List (+2 more)

### Community 39 - "Community 39"
Cohesion: 0.16
Nodes (8): WarehouseManagement.MaterialBoxs.Dto, WarehouseManagement.MaterialBoxs.Aggregates, WarehouseManagement.Material, WarehouseManagement.MaterialBoxs, WarehouseManagement.EntityFrameworkCore.ArchiveBoxs, WarehouseManagement.EntityFrameworkCore.Faces, WarehouseManagement.Cells, WarehouseManagement.EntityFrameworkCore.ArchiveBoxDetails

### Community 40 - "Community 40"
Cohesion: 0.14
Nodes (13): AllowAnonymous, Authorize, IdIntInput, List, ListResultDto, PagedResultDto, Task, CellAppService (+5 more)

### Community 41 - "Community 41"
Cohesion: 0.13
Nodes (15): CellDto, Cell_x, Cell_y, Cell_z, CellCode, CellName, CellStatus, CellType (+7 more)

### Community 42 - "Community 42"
Cohesion: 0.15
Nodes (18): AddVeinDto, FingerId, UserId, VeinDto, Error, Success, List, Task (+10 more)

### Community 43 - "Community 43"
Cohesion: 0.07
Nodes (26): DateTime, MaterialBoxDto, CellCode, CellId, CellModel, ClassCode, ClassName, ClassType (+18 more)

### Community 44 - "Community 44"
Cohesion: 0.24
Nodes (8): Boolean, HttpPost, IdIntInput, List, PagedResultDto, SwaggerOperation, Task, StockTasksController

### Community 45 - "Community 45"
Cohesion: 0.16
Nodes (11): WarehouseDomainService, Authorize, IdIntInput, List, PagedResultDto, Task, WarehouseAppService, Task (+3 more)

### Community 46 - "Community 46"
Cohesion: 0.08
Nodes (26): Guid, CheckDetailHisDto, Account, BeginTime, BoxBarcode, CellName, Checker, CheckId (+18 more)

### Community 47 - "Community 47"
Cohesion: 0.08
Nodes (26): Guid, CheckDetailHis, Account, BeginTime, BoxBarcode, CellName, Checker, CheckId (+18 more)

### Community 48 - "Community 48"
Cohesion: 0.13
Nodes (11): CellDomainService, ISettingAppService, Cell, List, Task, CellManager, XBigToSmall, YBigToSmall (+3 more)

### Community 49 - "Community 49"
Cohesion: 0.14
Nodes (12): WarehouseManagement.Material.Dto, PagingBase, PagingMaterialListInput, Filter, PagedResultDto, Task, IMaterialAppService, HttpPost (+4 more)

### Community 50 - "Community 50"
Cohesion: 0.15
Nodes (6): Boolean, IdIntInput, List, PagedResultDto, Task, IStockTaskAppService

### Community 51 - "Community 51"
Cohesion: 0.13
Nodes (15): WarehouseManagement.CheckHiss, WarehouseManagement.CheckHiss.Aggregates, WarehouseManagement.EntityFrameworkCore.CheckHiss, EfCoreRepository, IRepository, ICheckDetailHisRepository, ICheckHisRepository, ICheckDetailRepository (+7 more)

### Community 52 - "Community 52"
Cohesion: 0.08
Nodes (19): IReadOnlyCollection, List, List, CheckOrderCreateDto, Orders, Priority, QueryCode, OrderDto (+11 more)

### Community 53 - "Community 53"
Cohesion: 0.08
Nodes (21): Guid, List, ManageStatus, TaskHis, Details, EndCellPosition, ManageBeginTime, ManageConfirmTime (+13 more)

### Community 54 - "Community 54"
Cohesion: 0.09
Nodes (15): WarehouseManagement.Cells.Dto, CellInitDto, Cell_x, Cell_y, Cell_z, CreateStationDto, CellCode, CellName (+7 more)

### Community 55 - "Community 55"
Cohesion: 0.09
Nodes (14): WarehouseManagement.Warehouses.Dto, PagingWarehouseAreaListInput, Filter, UpdateWarehouseAreaDto, Id, WarehouseAreaCode, WarehouseAreaName, WarehouseAreaType (+6 more)

### Community 56 - "Community 56"
Cohesion: 0.12
Nodes (13): WarehouseManagement.Checks, WarehouseManagement.Checks.Aggregates, WarehouseManagement.EntityFrameworkCore.Checks, CheckStatus, Complete, Executing, Finish, Waiting (+5 more)

### Community 57 - "Community 57"
Cohesion: 0.11
Nodes (16): IHttpClientFactory, OrderCodeDto, OrderCode, StockOrderCreateDto, EndNode, OrderCode, PlateCode, Priority (+8 more)

### Community 58 - "Community 58"
Cohesion: 0.22
Nodes (9): CancellationToken, List, Task, IGoodsRepository, CancellationToken, IWarehouseManagementDbContext, List, Task (+1 more)

### Community 59 - "Community 59"
Cohesion: 0.09
Nodes (21): ManageStatus, CabinetComplete, CabinetWait, Cancel, CompeleteRequest, Complete, Error, ExceptionComplete (+13 more)

### Community 60 - "Community 60"
Cohesion: 0.15
Nodes (8): WarehouseManagement.TaskHiss, WarehouseManagement.TaskHiss.Aggregates, WarehouseManagement.EntityFrameworkCore.TaskHiss, WarehouseManagement.TaskHiss.Dto, Profile, CreateTaskHisDto, StockTaskId, WarehouseManagementApplicationAutoMapperProfile

### Community 61 - "Community 61"
Cohesion: 0.20
Nodes (10): Boolean, OpenDoorForOrderDto, message, success, Task, IWcsTaskAppService, HttpPost, SwaggerOperation (+2 more)

### Community 62 - "Community 62"
Cohesion: 0.18
Nodes (6): WarehouseManagement.EntityFrameworkCore.StockTasks, WarehouseManagement.EventHandlers, WarehouseManagement.StockTasks, WarehouseManagement.EntityFrameworkCore.StockTaskDetails, WarehouseManagement.StockTasks.Aggregates, TaskCompletedHisEto

### Community 63 - "Community 63"
Cohesion: 0.12
Nodes (17): IMultiTenant, Guid, Rfid, BoxId, PrintStatus, RfidCode, RfidTypeCode, Status (+9 more)

### Community 64 - "Community 64"
Cohesion: 0.16
Nodes (11): MaterialDomainService, CancellationToken, Task, IMaterialRepository, List, Task, MaterialManager, CancellationToken (+3 more)

### Community 65 - "Community 65"
Cohesion: 0.10
Nodes (20): CheckDetailDto, Account, BeginTime, BoxBarcode, CellName, Checker, CheckId, CompleteFlag (+12 more)

### Community 66 - "Community 66"
Cohesion: 0.16
Nodes (12): AddFaceDto, ImageDate, UserId, FaceDto, Error, Success, Task, Task (+4 more)

### Community 67 - "Community 67"
Cohesion: 0.16
Nodes (15): CancellationToken, List, Task, IVeinRepository, Guid, Vein, FingerId, TenantId (+7 more)

### Community 68 - "Community 68"
Cohesion: 0.15
Nodes (8): WarehouseManagement.EntityFrameworkCore.RfidCodes, WarehouseManagement.RfidCodes.Aggregates, WarehouseManagement.Fingers.Aggregates, WarehouseManagement.Material.Aggregates, WarehouseManagement.Faces.Aggregates, WarehouseManagement.EntityFrameworkCore.Fingers, WarehouseManagement.EntityFrameworkCore, VeinEfCoreQueryableExtensions

### Community 69 - "Community 69"
Cohesion: 0.16
Nodes (9): WarehouseManagement.Faces, WarehouseManagement.Faces.Dto, WarehouseManagement.Fingers.Dto, WarehouseManagement.Fingers, WarehouseManagement.Face, IFaceAppService, FaceAppService, FaceController (+1 more)

### Community 70 - "Community 70"
Cohesion: 0.11
Nodes (13): WarehouseManagement.Goodss.Dto, CreateGoodsDto, GoodsCode, GoodsConstProperty1, GoodsName, GoodsSpec, GoodsUnits, GoodsTypeSelectDto (+5 more)

### Community 71 - "Community 71"
Cohesion: 0.19
Nodes (11): PagingGoodsListInput, Filter, GoodsCode, GoodsName, GoodsSpec, Authorize, IdIntInput, List (+3 more)

### Community 72 - "Community 72"
Cohesion: 0.24
Nodes (6): Task, ICurrentUser, List, Task, UnitOfWork, PlanManager

### Community 73 - "Community 73"
Cohesion: 0.30
Nodes (7): HttpPost, IdIntInput, ListResultDto, PagedResultDto, SwaggerOperation, Task, CellsController

### Community 74 - "Community 74"
Cohesion: 0.14
Nodes (12): WarehouseManagement.LogFiles.Dto, WarehouseManagement.LogFiles, IApplicationService, IHttpContextAccessor, PlanDomainService, ILogFileAppService, IStockTaskDetailAppService, LogFileAppService (+4 more)

### Community 75 - "Community 75"
Cohesion: 0.14
Nodes (15): FullAuditedAggregateRoot, Guid, Face, ImageDate, TenantId, UserId, CancellationToken, Task (+7 more)

### Community 76 - "Community 76"
Cohesion: 0.11
Nodes (18): Guid, CheckHisDto, AccuracyFlag, AreaCode, BatchNo, BeginTime, CheckCode, CheckStatus (+10 more)

### Community 77 - "Community 77"
Cohesion: 0.11
Nodes (17): ManageType, EmptyStockIn, EmptyStockOut, FullSotckUp, FullStockDown, HpAnnualCheckDown, HPBatchStockIn, HPFullStockIn (+9 more)

### Community 78 - "Community 78"
Cohesion: 0.12
Nodes (17): AuditedEntityDto, Decimal, PlanDetailDto, GoodsBatchNo, GoodsCode, PlanListQty, PlanListRemark, ManageStatus (+9 more)

### Community 79 - "Community 79"
Cohesion: 0.12
Nodes (10): WarehouseManagement.Checks.Dto, CreateCheckDto, AreaCode, PagingCheckDetailInput, CheckId, Filter, StockBarcode, UpdateCheckDetailDto (+2 more)

### Community 80 - "Community 80"
Cohesion: 0.23
Nodes (9): CancellationToken, List, Task, IPlanRepository, CancellationToken, IWarehouseManagementDbContext, List, Task (+1 more)

### Community 81 - "Community 81"
Cohesion: 0.23
Nodes (9): CancellationToken, List, Task, IStockTaskRepository, CancellationToken, IWarehouseManagementDbContext, List, Task (+1 more)

### Community 82 - "Community 82"
Cohesion: 0.37
Nodes (6): HttpPost, IdIntInput, PagedResultDto, SwaggerOperation, Task, ChecksController

### Community 83 - "Community 83"
Cohesion: 0.22
Nodes (6): RfidCodeDomainService, PagedResultDto, Task, MaterialBoxAppService, Task, RfidCodeManager

### Community 84 - "Community 84"
Cohesion: 0.12
Nodes (15): ManageType, StockTaskDto, EndCellCode, EndCellId, ManageLaneWay, ManageStatus, ManageStatusString, ManageTypeCode (+7 more)

### Community 85 - "Community 85"
Cohesion: 0.23
Nodes (9): CancellationToken, List, Task, ITaskHisRepository, CancellationToken, IWarehouseManagementDbContext, List, Task (+1 more)

### Community 86 - "Community 86"
Cohesion: 0.27
Nodes (4): IdIntInput, PagedResultDto, Task, ICheckAppService

### Community 87 - "Community 87"
Cohesion: 0.13
Nodes (14): CreateMaterialBoxDto, CellModel, ClassCode, ClassName, ClassType, Director, Id, MaterialBoxName (+6 more)

### Community 88 - "Community 88"
Cohesion: 0.16
Nodes (8): CommuStateDto, ObjectName, State, List, ResultCommuStatesDto, CommuStates, Task, WcsTaskAppService

### Community 89 - "Community 89"
Cohesion: 0.22
Nodes (4): IdIntInput, PagedResultDto, Task, PlanAppService

### Community 90 - "Community 90"
Cohesion: 0.14
Nodes (13): GoodsClass, GoodsClassCode, GoodsClassFlag, GoodsClassName, GoodsClassOrder, GoodsClassParentId, GoodsClassRemark, GoodsType (+5 more)

### Community 91 - "Community 91"
Cohesion: 0.32
Nodes (5): CancellationToken, IWarehouseManagementDbContext, List, Task, EfCoreMaterialBoxRepository

### Community 92 - "Community 92"
Cohesion: 0.27
Nodes (5): CheckDomainService, List, Task, UnitOfWork, CheckManager

### Community 93 - "Community 93"
Cohesion: 0.27
Nodes (5): GoodsDomainService, List, Task, UnitOfWork, GoodsManager

### Community 94 - "Community 94"
Cohesion: 0.15
Nodes (13): CustomCreateCellDto, Cell_x, Cell_y, Cell_z, CellCode, CellGroup, CellModel, CellName (+5 more)

### Community 95 - "Community 95"
Cohesion: 0.15
Nodes (13): CheckDto, AccuracyFlag, AreaCode, BatchNo, BeginTime, CheckCode, CheckStatus, CheckType (+5 more)

### Community 96 - "Community 96"
Cohesion: 0.17
Nodes (11): LogFileDto, CreateTime, LogFileName, LogFileUrl, ModifyTime, PagedResultDto, Task, HttpPost (+3 more)

### Community 97 - "Community 97"
Cohesion: 0.24
Nodes (5): PagingMaterialBoxDetailInput, MaterialBoxId, PagedResultDto, Task, IMaterialBoxAppService

### Community 98 - "Community 98"
Cohesion: 0.15
Nodes (12): String, CreateStockTaskDto, EndCellCode, EndCellId, ManageStatus, ManageTypeCode, MaterialBoxId, MaterialCode (+4 more)

### Community 99 - "Community 99"
Cohesion: 0.15
Nodes (12): Decimal, StockTaskDetailDto, GoodsBand, GoodsBatchNo, GoodsCode, GoodsName, GoodsProperty1, GoodsSpec (+4 more)

### Community 100 - "Community 100"
Cohesion: 0.21
Nodes (7): OpenDoorDto, OrderCode, Boolean, ResultWcsTaskDto, Message, QueryCode, Success

### Community 101 - "Community 101"
Cohesion: 0.15
Nodes (12): Decimal, StockTaskDetailEto, BackFlag, BoxBarcode, GoodsId, ManageListQuantity, PlanDetailId, StockTaskDetailId (+4 more)

### Community 102 - "Community 102"
Cohesion: 0.24
Nodes (8): CreateRfidCodeDto, Id, RfidCode, RfidTypeCode, List, PagedResultDto, Task, IRfidCodeAppService

### Community 103 - "Community 103"
Cohesion: 0.17
Nodes (11): CreateWarehouseDto, WarehouseCode, WarehouseName, WarehouseType, WarehouseDto, WarehouseCode, WarehouseFlag, WarehouseName (+3 more)

### Community 104 - "Community 104"
Cohesion: 0.18
Nodes (9): List, ListResultSstatesDto, orderStates, ResultStatesDto, ErrorInfo, ExecState, HappenTime, OrderCode (+1 more)

### Community 105 - "Community 105"
Cohesion: 0.18
Nodes (10): List, PagedResultDto, Task, LogFile, CreateTime, LogFileName, LogFileUrl, ModifyTime (+2 more)

### Community 106 - "Community 106"
Cohesion: 0.17
Nodes (11): PlanType, CanPause, ManageTypeCode, PlanTypeClass, PlanTypeCode, PlanTypeFlag, PlanTypeGroup, PlanTypeInout (+3 more)

### Community 107 - "Community 107"
Cohesion: 0.45
Nodes (5): HttpPost, PagedResultDto, SwaggerOperation, Task, MaterialBoxsController

### Community 108 - "Community 108"
Cohesion: 0.20
Nodes (9): Task, BoardAppService, List, SevenDayTasksDto, Invalue, Keys, Outvalue, TotalCount (+1 more)

### Community 109 - "Community 109"
Cohesion: 0.20
Nodes (8): String, UpdateStockTaskDto, EndCellId, Id, ManageStatus, ManageTypeCode, StartCellId, StockBarcode

### Community 110 - "Community 110"
Cohesion: 0.18
Nodes (10): WcsOrderStatus, CabinetClose, CabinetOpen, Cancel, Complete, ExceptionComplete, Executing, RobotIncell (+2 more)

### Community 111 - "Community 111"
Cohesion: 0.35
Nodes (6): HttpPost, List, PagedResultDto, SwaggerOperation, Task, RfidCodesController

### Community 112 - "Community 112"
Cohesion: 0.22
Nodes (4): WarehouseManagement.RfidCodes, WarehouseManagement.RfidCodes.Dto, PagingRfidListInput, Filter

### Community 113 - "Community 113"
Cohesion: 0.20
Nodes (10): DomainService, IDistributedEventBus, IObjectMapper, IUnitOfWorkManager, Type, RfidCodeDomainService, DistributedEventBus, ObjectMapper (+2 more)

### Community 114 - "Community 114"
Cohesion: 0.24
Nodes (6): MaterialBoxDetailDomainService, Boolean, List, PagedResultDto, Task, MaterialBoxDetailManager

### Community 115 - "Community 115"
Cohesion: 0.33
Nodes (6): Task, IBoardAppService, HttpPost, SwaggerOperation, Task, BoardsController

### Community 116 - "Community 116"
Cohesion: 0.27
Nodes (7): face, ImageDate, UserId, GetFaceDto, Error, Face, Success

### Community 117 - "Community 117"
Cohesion: 0.20
Nodes (9): String, CreateBindBoxStockTaskDto, EndCellCode, EndCellId, ManageStatus, ManageTypeCode, StartCellCode, StartCellId (+1 more)

### Community 118 - "Community 118"
Cohesion: 0.20
Nodes (9): Decimal, UpdateStockTaskDetailDto, GoodsBatchNo, Id, ManageListQuantity, PlanListId, StockTaskId, StorageBoxDetailId (+1 more)

### Community 119 - "Community 119"
Cohesion: 0.24
Nodes (7): PagingTaskHisDetailInput, Filter, TaskHisId, ManageStatus, PagedResultDto, Task, TaskHisAppService

### Community 120 - "Community 120"
Cohesion: 0.20
Nodes (7): CheckOrderResultDto, CellCode, OrderCode, QueryCode, List, ResultCheckDto, Cells

### Community 121 - "Community 121"
Cohesion: 0.31
Nodes (4): PlcNodeDto, NodeName, ResultPlcNodeDto, NodeValue

### Community 122 - "Community 122"
Cohesion: 0.20
Nodes (9): IDistributedEventBus, IObjectMapper, IUnitOfWorkManager, Type, CellDomainService, DistributedEventBus, ObjectMapper, ObjectMapperContext (+1 more)

### Community 123 - "Community 123"
Cohesion: 0.20
Nodes (9): IDistributedEventBus, IObjectMapper, IUnitOfWorkManager, Type, CheckDetailDomainService, DistributedEventBus, ObjectMapper, ObjectMapperContext (+1 more)

### Community 124 - "Community 124"
Cohesion: 0.20
Nodes (9): IDistributedEventBus, IObjectMapper, IUnitOfWorkManager, Type, CheckDetailHisDomainService, DistributedEventBus, ObjectMapper, ObjectMapperContext (+1 more)

### Community 125 - "Community 125"
Cohesion: 0.20
Nodes (9): IDistributedEventBus, IObjectMapper, IUnitOfWorkManager, Type, CheckDomainService, DistributedEventBus, ObjectMapper, ObjectMapperContext (+1 more)

### Community 126 - "Community 126"
Cohesion: 0.20
Nodes (9): IDistributedEventBus, IObjectMapper, IUnitOfWorkManager, Type, CheckHisDomainService, DistributedEventBus, ObjectMapper, ObjectMapperContext (+1 more)

### Community 127 - "Community 127"
Cohesion: 0.20
Nodes (9): IDistributedEventBus, IObjectMapper, IUnitOfWorkManager, Type, ERPApiDomainService, DistributedEventBus, ObjectMapper, ObjectMapperContext (+1 more)

### Community 128 - "Community 128"
Cohesion: 0.20
Nodes (9): IDistributedEventBus, IObjectMapper, IUnitOfWorkManager, Type, GoodsDomainService, DistributedEventBus, ObjectMapper, ObjectMapperContext (+1 more)

### Community 129 - "Community 129"
Cohesion: 0.20
Nodes (9): IDistributedEventBus, IObjectMapper, IUnitOfWorkManager, Type, IOControlDomainService, DistributedEventBus, ObjectMapper, ObjectMapperContext (+1 more)

### Community 130 - "Community 130"
Cohesion: 0.20
Nodes (9): IDistributedEventBus, IObjectMapper, IUnitOfWorkManager, Type, MaterialBoxDetailDomainService, DistributedEventBus, ObjectMapper, ObjectMapperContext (+1 more)

### Community 131 - "Community 131"
Cohesion: 0.20
Nodes (9): IDistributedEventBus, IObjectMapper, IUnitOfWorkManager, Type, MaterialBoxDomainService, DistributedEventBus, ObjectMapper, ObjectMapperContext (+1 more)

### Community 132 - "Community 132"
Cohesion: 0.20
Nodes (9): IDistributedEventBus, IObjectMapper, IUnitOfWorkManager, Type, MaterialDomainService, DistributedEventBus, ObjectMapper, ObjectMapperContext (+1 more)

### Community 133 - "Community 133"
Cohesion: 0.20
Nodes (9): IDistributedEventBus, IObjectMapper, IUnitOfWorkManager, Type, PlanDomainService, DistributedEventBus, ObjectMapper, ObjectMapperContext (+1 more)

### Community 134 - "Community 134"
Cohesion: 0.20
Nodes (9): IDistributedEventBus, IObjectMapper, IUnitOfWorkManager, Type, ReceiveRestApiDomainService, DistributedEventBus, ObjectMapper, ObjectMapperContext (+1 more)

### Community 135 - "Community 135"
Cohesion: 0.20
Nodes (9): IDistributedEventBus, IObjectMapper, IUnitOfWorkManager, Type, SendRestApiDomainService, DistributedEventBus, ObjectMapper, ObjectMapperContext (+1 more)

### Community 136 - "Community 136"
Cohesion: 0.20
Nodes (9): IDistributedEventBus, IObjectMapper, IUnitOfWorkManager, Type, ShelfDomainService, DistributedEventBus, ObjectMapper, ObjectMapperContext (+1 more)

### Community 137 - "Community 137"
Cohesion: 0.20
Nodes (9): IDistributedEventBus, IObjectMapper, IUnitOfWorkManager, Type, StockTaskDomainService, DistributedEventBus, ObjectMapper, ObjectMapperContext (+1 more)

### Community 138 - "Community 138"
Cohesion: 0.20
Nodes (9): IDistributedEventBus, IObjectMapper, IUnitOfWorkManager, Type, StorageBoxDetailDomainService, DistributedEventBus, ObjectMapper, ObjectMapperContext (+1 more)

### Community 139 - "Community 139"
Cohesion: 0.20
Nodes (9): IDistributedEventBus, IObjectMapper, IUnitOfWorkManager, Type, StorageBoxDomainService, DistributedEventBus, ObjectMapper, ObjectMapperContext (+1 more)

### Community 140 - "Community 140"
Cohesion: 0.20
Nodes (9): IDistributedEventBus, IObjectMapper, IUnitOfWorkManager, Type, TaskHisDomainService, DistributedEventBus, ObjectMapper, ObjectMapperContext (+1 more)

### Community 141 - "Community 141"
Cohesion: 0.20
Nodes (9): IDistributedEventBus, IObjectMapper, IUnitOfWorkManager, Type, TestSiteDomainService, DistributedEventBus, ObjectMapper, ObjectMapperContext (+1 more)

### Community 142 - "Community 142"
Cohesion: 0.20
Nodes (9): IDistributedEventBus, IObjectMapper, IUnitOfWorkManager, Type, WarehouseDomainService, DistributedEventBus, ObjectMapper, ObjectMapperContext (+1 more)

### Community 143 - "Community 143"
Cohesion: 0.20
Nodes (9): IDistributedEventBus, IObjectMapper, IUnitOfWorkManager, Type, WcsTaskDomainService, DistributedEventBus, ObjectMapper, ObjectMapperContext (+1 more)

### Community 144 - "Community 144"
Cohesion: 0.25
Nodes (5): WarehouseManagement.Boards, WarehouseManagement.Boards.Dto, TodayTasksDto, TodayInct, TodayOutCt

### Community 145 - "Community 145"
Cohesion: 0.22
Nodes (5): WarehouseManagement.CheckHiss.Dto, PagingCheckHisDto, CheckId, Filter, StockBarcode

### Community 146 - "Community 146"
Cohesion: 0.36
Nodes (4): TaskHisDomainService, List, Task, TaskHisManager

### Community 147 - "Community 147"
Cohesion: 0.22
Nodes (8): CellManagement, GoodsManagement, PlanManagement, StockTaskManagement, StorageBoxManagement, TestSiteManagement, WarehouseManagement, WarehouseManagementPermissions

### Community 148 - "Community 148"
Cohesion: 0.22
Nodes (8): Decimal, CreateStockTaskDetailInput, GoodsBatchNo, ManageListQuantity, PlanListId, StockTaskId, StorageBoxDetailId, TaskDetailRemark

### Community 149 - "Community 149"
Cohesion: 0.25
Nodes (7): WcsCallBackRequest, ErrorInfo, ExecutionStep, HappenTime, OrderCode, PlateCode, Status

### Community 150 - "Community 150"
Cohesion: 0.31
Nodes (5): List, PagedResultDto, Task, UnitOfWork, RfidCodeAppService

### Community 151 - "Community 151"
Cohesion: 0.22
Nodes (5): CellRunStatus, Disable, Enable, Run, Selected

### Community 152 - "Community 152"
Cohesion: 0.22
Nodes (8): PlanListStatus, Cancel, Executing, Finish, Pause, Waiting, WaitingExecute, WaitingSend

### Community 153 - "Community 153"
Cohesion: 0.22
Nodes (8): StockTaskType, ManageTypeClass, ManageTypeCode, ManageTypeFlag, ManageTypeGroup, ManageTypeInout, ManageTypeName, ManageTypeOrder

### Community 154 - "Community 154"
Cohesion: 0.39
Nodes (5): CancellationToken, IWarehouseManagementDbContext, List, Task, EfCorePlanListRepository

### Community 155 - "Community 155"
Cohesion: 0.25
Nodes (5): WarehouseManagement.Settings, ISettingDefinitionContext, SettingDefinitionProvider, WarehouseManagementSettingDefinitionProvider, WarehouseManagementSettings

### Community 156 - "Community 156"
Cohesion: 0.25
Nodes (7): Entity, GoodsType, GoodsTypeCode, GoodsTypeFlag, GoodsTypeName, GoodsTypeOrder, GoodsTypeRemark

### Community 157 - "Community 157"
Cohesion: 0.25
Nodes (6): IModuleInstallingPipelineBuilder, ModuleInstallingContext, ModuleInstallingPipeline, ModuleInstallingPipelineBuilderBase, Task, WarehouseManagementInstallerPipelineBuilder

### Community 158 - "Community 158"
Cohesion: 0.32
Nodes (5): PagedResultDto, Task, CheckHisAppService, PagingCheckDetailHisDto, CheckId

### Community 159 - "Community 159"
Cohesion: 0.25
Nodes (7): DateTime, String, PagingCheckListInput, EndCreationTime, Filter, ManageStatus, StartCreationTime

### Community 160 - "Community 160"
Cohesion: 0.25
Nodes (8): Guid, List, FaceLoginDto, Id, Name, Roles, Token, UserName

### Community 161 - "Community 161"
Cohesion: 0.25
Nodes (7): DateTime, String, PagingStockTaskListInput, EndCreationTime, Filter, ManageStatus, StartCreationTime

### Community 162 - "Community 162"
Cohesion: 0.25
Nodes (7): CellStatus, Exception, Forbiden, Full, Have, Nohave, Pallet

### Community 163 - "Community 163"
Cohesion: 0.25
Nodes (7): WcsTaskStatus, Accepted, Canceled, Completed, Executing, ForceCompleted, Unknown

### Community 164 - "Community 164"
Cohesion: 0.29
Nodes (6): StockInfoDto, ArchiveInTotalCt, ArchiveTotalCt, BorrowedTotalCt, BoxInTotalCt, BoxTotalCt

### Community 165 - "Community 165"
Cohesion: 0.29
Nodes (7): UpdateGoodsDto, GoodsCode, GoodsConstProperty1, GoodsName, GoodsSpec, GoodsUnits, Id

### Community 166 - "Community 166"
Cohesion: 0.29
Nodes (7): RfidCodeDto, BoxId, PrintStatus, RfidCode, RfidTypeCode, Status, WriteStatus

### Community 167 - "Community 167"
Cohesion: 0.43
Nodes (3): PagedResultDto, Task, MaterialAppService

### Community 168 - "Community 168"
Cohesion: 0.48
Nodes (4): CancellationToken, List, Task, IPlanListRepository

### Community 169 - "Community 169"
Cohesion: 0.29
Nodes (6): CellType, CTUCell, ErrorStation, Station, WallCell, WorkStation

### Community 170 - "Community 170"
Cohesion: 0.29
Nodes (6): GoodsBaseDto, GoodsBand, GoodsCode, GoodsName, GoodsSpec, GoodsUnits

### Community 171 - "Community 171"
Cohesion: 0.29
Nodes (6): PlanStatus, Cancel, Executing, Finish, Pause, Waiting

### Community 172 - "Community 172"
Cohesion: 0.48
Nodes (5): HttpPost, PagedResultDto, SwaggerOperation, Task, CheckHissController

### Community 173 - "Community 173"
Cohesion: 0.33
Nodes (3): WarehouseManagement.EntityFrameworkCore.Cells, IQueryable, CellEfCoreQueryableExtensions

### Community 174 - "Community 174"
Cohesion: 0.33
Nodes (3): WarehouseManagement.EntityFrameworkCore.Material, IQueryable, MaterialEfCoreQueryableExtensions

### Community 175 - "Community 175"
Cohesion: 0.33
Nodes (6): UpdateCellDto, CellCode, CellName, CellType, Id, WarehouseId

### Community 176 - "Community 176"
Cohesion: 0.47
Nodes (3): PagedResultDto, Task, ICheckHisAppService

### Community 177 - "Community 177"
Cohesion: 0.33
Nodes (5): Decimal, CreateStockTaskDetailByBarcodeInput, ManageListQuantity, StockTaskId, StorageBoxBarCode

### Community 178 - "Community 178"
Cohesion: 0.40
Nodes (4): PagingStockTaskDetailInput, Filter, MaterialId, StockTaskId

### Community 179 - "Community 179"
Cohesion: 0.40
Nodes (4): PickOutDto, ArchiveBoxId, ArchiveId, Userid

### Community 180 - "Community 180"
Cohesion: 0.50
Nodes (3): PagingMaterialBoxListInput, Filter, MaterialBoxName

## Knowledge Gaps
- **1441 isolated node(s):** `CellManagement`, `GoodsManagement`, `PlanManagement`, `StockTaskManagement`, `StorageBoxManagement` (+1436 more)
  These have ≤1 connection - possible missing edges or undocumented components. (Counts symbols only; 1825 node(s) total have ≤1 connection when file, concept and rationale nodes are included.)
- **7 thin communities (<3 nodes) omitted from report** — run `graphify query` to explore isolated nodes.

## Suggested Questions
_Questions this graph is uniquely positioned to answer:_

- **Why does `WarehouseManagement` connect `Community 22` to `Community 128`, `Community 129`, `Community 130`, `Community 131`, `Community 132`, `Community 133`, `Community 134`, `Community 135`, `Community 136`, `Community 137`, `Community 138`, `Community 139`, `Community 140`, `Community 141`, `Community 142`, `Community 143`, `Community 157`, `Community 31`, `Community 60`, `Community 122`, `Community 123`, `Community 124`, `Community 125`, `Community 126`, `Community 127`?**
  _High betweenness centrality (0.151) - this node is a cross-community bridge._
- **Why does `WarehouseManagementDbContext` connect `Community 25` to `Community 0`, `Community 1`, `Community 3`, `Community 4`, `Community 6`, `Community 8`, `Community 12`, `Community 14`, `Community 15`, `Community 16`, `Community 22`, `Community 153`, `Community 27`, `Community 30`, `Community 32`, `Community 33`, `Community 38`, `Community 47`, `Community 53`, `Community 181`, `Community 67`, `Community 68`, `Community 75`, `Community 90`, `Community 106`?**
  _High betweenness centrality (0.104) - this node is a cross-community bridge._
- **Why does `Material` connect `Community 3` to `Community 64`, `Community 68`, `Community 167`, `Community 39`, `Community 75`, `Community 174`, `Community 25`, `Community 90`, `Community 60`, `Community 63`?**
  _High betweenness centrality (0.098) - this node is a cross-community bridge._
- **What connects `CellManagement`, `GoodsManagement`, `PlanManagement` to the rest of the system?**
  _1441 weakly-connected nodes found - possible documentation gaps or missing edges._
- **Should `Community 0` be split into smaller, more focused modules?**
  _Cohesion score 0.03225806451612903 - nodes in this community are weakly interconnected._
- **Should `Community 1` be split into smaller, more focused modules?**
  _Cohesion score 0.054098360655737705 - nodes in this community are weakly interconnected._
- **Should `Community 2` be split into smaller, more focused modules?**
  _Cohesion score 0.053185271770894216 - nodes in this community are weakly interconnected._