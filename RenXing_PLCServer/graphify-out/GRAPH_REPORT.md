# Graph Report - E:\Tuta\project\renxingwurendanganku\RenXing_PLCServer  (2026-08-24)

## Corpus Check
- Corpus is ~38,659 words - fits in a single context window. You may not need a graph.

## Summary
- 887 nodes · 1723 edges · 52 communities (43 shown, 9 thin omitted)
- Extraction: 92% EXTRACTED · 8% INFERRED · 0% AMBIGUOUS · INFERRED: 131 edges (avg confidence: 0.8)
- Token cost: 0 input · 0 output

## Community Hubs (Navigation)
- [[_COMMUNITY_Plc Driver Siemens 0|Plc Driver Siemens 0]]
- [[_COMMUNITY_Plc Jobs 1|Plc Jobs 1]]
- [[_COMMUNITY_Plc Cache 2|Plc Cache 2]]
- [[_COMMUNITY_Plc 3|Plc 3]]
- [[_COMMUNITY_By My SQL 4|By My SQL 4]]
- [[_COMMUNITY_Driver Siemens Types 5|Driver Siemens Types 5]]
- [[_COMMUNITY_Plc Driver Simulation 6|Plc Driver Simulation 6]]
- [[_COMMUNITY_Service By File 7|Service By File 7]]
- [[_COMMUNITY_By Sta Ex 8|By Sta Ex 8]]
- [[_COMMUNITY_Plc Driver Siemens 9|Plc Driver Siemens 9]]
- [[_COMMUNITY_Plc Cache 10|Plc Cache 10]]
- [[_COMMUNITY_Redis IRedis Cli 11|Redis IRedis Cli 11]]
- [[_COMMUNITY_Logger ILogger 12|Logger ILogger 12]]
- [[_COMMUNITY_Http Help 13|Http Help 13]]
- [[_COMMUNITY_Plc Jobs 14|Plc Jobs 14]]
- [[_COMMUNITY_Driver Siemens Types 15|Driver Siemens Types 15]]
- [[_COMMUNITY_Driver Siemens Protocol 16|Driver Siemens Protocol 16]]
- [[_COMMUNITY_Plc Driver Siemens 17|Plc Driver Siemens 17]]
- [[_COMMUNITY_Plc Driver Siemens 18|Plc Driver Siemens 18]]
- [[_COMMUNITY_Plc Driver Siemens 19|Plc Driver Siemens 19]]
- [[_COMMUNITY_Plc Driver Siemens 20|Plc Driver Siemens 20]]
- [[_COMMUNITY_Plc Jobs 21|Plc Jobs 21]]
- [[_COMMUNITY_Driver Siemens Protocol 22|Driver Siemens Protocol 22]]
- [[_COMMUNITY_Redis Cli Reg 23|Redis Cli Reg 23]]
- [[_COMMUNITY_Driver Siemens Types 24|Driver Siemens Types 24]]
- [[_COMMUNITY_Driver Siemens Types 25|Driver Siemens Types 25]]
- [[_COMMUNITY_Plc Jobs 26|Plc Jobs 26]]
- [[_COMMUNITY_Plc Jobs 27|Plc Jobs 27]]
- [[_COMMUNITY_Plc Jobs 28|Plc Jobs 28]]
- [[_COMMUNITY_Plc Jobs 29|Plc Jobs 29]]
- [[_COMMUNITY_Plc Jobs 30|Plc Jobs 30]]
- [[_COMMUNITY_Plc Jobs 31|Plc Jobs 31]]
- [[_COMMUNITY_Plc Jobs 32|Plc Jobs 32]]
- [[_COMMUNITY_Plc Jobs 33|Plc Jobs 33]]
- [[_COMMUNITY_Plc Jobs 34|Plc Jobs 34]]
- [[_COMMUNITY_Driver Siemens Types 35|Driver Siemens Types 35]]
- [[_COMMUNITY_Driver Siemens Types 36|Driver Siemens Types 36]]
- [[_COMMUNITY_Config 37|Config 37]]
- [[_COMMUNITY_Driver Siemens Internal 38|Driver Siemens Internal 38]]
- [[_COMMUNITY_Plc Driver Siemens 39|Plc Driver Siemens 39]]
- [[_COMMUNITY_Driver Siemens Types 40|Driver Siemens Types 40]]
- [[_COMMUNITY_Driver Siemens Types 41|Driver Siemens Types 41]]
- [[_COMMUNITY_Driver Siemens Types 42|Driver Siemens Types 42]]
- [[_COMMUNITY_Driver Siemens Types 43|Driver Siemens Types 43]]
- [[_COMMUNITY_Driver Siemens Types 44|Driver Siemens Types 44]]
- [[_COMMUNITY_Driver Siemens Compat 45|Driver Siemens Compat 45]]
- [[_COMMUNITY_Driver Siemens Protocol 46|Driver Siemens Protocol 46]]
- [[_COMMUNITY_Driver Siemens Types 47|Driver Siemens Types 47]]
- [[_COMMUNITY_Driver Siemens Helper 48|Driver Siemens Helper 48]]
- [[_COMMUNITY_Driver Siemens Types 49|Driver Siemens Types 49]]
- [[_COMMUNITY_Driver Siemens Types 50|Driver Siemens Types 50]]
- [[_COMMUNITY_Driver Siemens Types 51|Driver Siemens Types 51]]

## God Nodes (most connected - your core abstractions)
1. `IRedisClient` - 35 edges
2. `S7.Net.Types` - 31 edges
3. `ILog` - 29 edges
4. `RedisClientByStaEx` - 26 edges
5. `JobHelper` - 25 edges
6. `CacheInRedis` - 24 edges
7. `ICache` - 22 edges
8. `DataType` - 22 edges
9. `Plc` - 22 edges
10. `PlcCore` - 20 edges

## Surprising Connections (you probably didn't know these)
- `CacheInRedis` --references--> `ILog`  [EXTRACTED]
  PlcServer.Cache/CacheInRedis.cs → Shared/Logger/ILogger/ILog.cs
- `PlcCore` --references--> `IDeviceService`  [EXTRACTED]
  PlcServer.Core/PlcCore.cs → PlcServer.Devices/IDeviceServices/IDeviceService.cs
- `PlcCore` --references--> `ILog`  [EXTRACTED]
  PlcServer.Core/PlcCore.cs → Shared/Logger/ILogger/ILog.cs
- `DeviceServiceInMySql` --references--> `ILog`  [EXTRACTED]
  PlcServer.Devices/DeviceServices/DeviceServiceByMySql/DeviceServiceInMySql.cs → Shared/Logger/ILogger/ILog.cs
- `SiemensPlc` --references--> `IDeviceService`  [EXTRACTED]
  PlcServer.Driver.Siemens/SiemensPlc.cs → PlcServer.Devices/IDeviceServices/IDeviceService.cs

## Import Cycles
- None detected.

## Communities (52 total, 9 thin omitted)

### Community 0 - "Plc Driver Siemens 0"
Cohesion: 0.06
Nodes (35): ICollection, CpuType, DataType, VarType, bool, IEnumerable, int, Int16 (+27 more)

### Community 1 - "Plc Jobs 1"
Cohesion: 0.07
Nodes (35): PlcServer.Devices.DeviceServices.DeviceServiceByFile, PlcServer.Driver.Simulation, PlcServer.Devices.Models, PlcServer.Devices.DeviceServices.DeviceServiceByMySql, Shared.Config, Shared.Redis.RedisCliReg, PlcServer.Driver.Siemens, Shared.Logger.LogRegister (+27 more)

### Community 2 - "Plc Cache 2"
Cohesion: 0.05
Nodes (19): CancellationTokenSource, IServiceProvider, Action, Task, ICache, CancellationToken, Dictionary, KeyValuePair (+11 more)

### Community 3 - "Plc 3"
Cohesion: 0.05
Nodes (53): log4net (2.0.15), Microsoft.Extensions.Configuration.Binder (7.0.4), Microsoft.Extensions.Configuration.Json (7.0.0), Microsoft.Extensions.Hosting (7.0.1), Microsoft.Extensions.Hosting.Systemd (7.0.0), RestSharp (110.2.0), PlcServer.Cache, net6.0 (+45 more)

### Community 4 - "By My SQL 4"
Cohesion: 0.06
Nodes (17): Shared.Logger.ILogger.Models, Shared.Logger.LogByMySQL, Shared.Logger.ILogger.Enumeration, Shared.Logger.LogByLog4Net, DbContext, ILog, LogGrade, List (+9 more)

### Community 5 - "Driver Siemens Types 5"
Cohesion: 0.09
Nodes (12): Encoding, Func, IEnumerable, Type, Class, LReal, S7String, S7WString (+4 more)

### Community 6 - "Plc Driver Simulation 6"
Cohesion: 0.09
Nodes (16): EnumQuality, EnumTagAccess, bool, string, PlcTag, Dictionary, int, string (+8 more)

### Community 7 - "Service By File 7"
Cohesion: 0.11
Nodes (13): List, DeviceServiceInFile, List, object, DeviceServiceInMySql, DbContextOptionsBuilder, DbSet, ModelBuilder (+5 more)

### Community 8 - "By Sta Ex 8"
Cohesion: 0.10
Nodes (6): Action, ConnectionMultiplexer, int, KeyValuePair, Task, RedisClientByStaEx

### Community 9 - "Plc Driver Siemens 9"
Cohesion: 0.10
Nodes (18): PduType, bool, byte, CancellationToken, int, Stream, Task, COTP (+10 more)

### Community 10 - "Plc Cache 10"
Cohesion: 0.10
Nodes (6): Action, ConnectionMultiplexer, int, string, Task, CacheInRedis

### Community 11 - "Redis IRedis Cli 11"
Cohesion: 0.16
Nodes (4): Action, KeyValuePair, Task, IRedisClient

### Community 12 - "Logger ILogger 12"
Cohesion: 0.15
Nodes (6): IDisposable, IHostedService, CancellationToken, Task, AgvJob, ILog

### Community 13 - "Http Help 13"
Cohesion: 0.17
Nodes (5): Shared.HttpHelp, Method, RestResponse, Task, RestApi

### Community 14 - "Plc Jobs 14"
Cohesion: 0.17
Nodes (3): bool, string, JobHelper

### Community 15 - "Driver Siemens Types 15"
Cohesion: 0.26
Nodes (5): DateTime, IList, DateTime, int, DateTimeLong

### Community 16 - "Driver Siemens Protocol 16"
Cohesion: 0.20
Nodes (5): S7WriteMultiple, IList, Serialization, List, ByteArray

### Community 17 - "Plc Driver Siemens 17"
Cohesion: 0.19
Nodes (4): Dictionary, Task, SiemensPlc, Real

### Community 18 - "Plc Driver Siemens 18"
Cohesion: 0.27
Nodes (6): S7.Net.Protocol.S7, S7.Net.Protocol, S7.Net.Types, S7.Net.Helper, S7.Net, ReadWriteErrorCode

### Community 19 - "Plc Driver Siemens 19"
Cohesion: 0.21
Nodes (9): Exception, ErrorCode, InvalidDataException, PlcException, InvalidAddressException, InvalidVariableTypeException, TPDUInvalidException, TPKTInvalidException (+1 more)

### Community 20 - "Plc Driver Siemens 20"
Cohesion: 0.17
Nodes (3): Int32, UInt32, Conversion

### Community 21 - "Plc Jobs 21"
Cohesion: 0.26
Nodes (5): Task, CancellationToken, string, Task, WcsDoor1Job

### Community 22 - "Driver Siemens Protocol 22"
Cohesion: 0.20
Nodes (5): ArgumentOutOfRangeException, ConnectionRequest, Tsap, CpuType, TsapPair

### Community 23 - "Redis Cli Reg 23"
Cohesion: 0.20
Nodes (8): Microsoft.Extensions.DependencyInjection.Abstractions (7.0.0), net7.0, StackExchange.Redis (2.6.104), Microsoft.NET.Sdk, net7.0, Microsoft.NET.Sdk, net7.0, Microsoft.NET.Sdk

### Community 24 - "Driver Siemens Types 24"
Cohesion: 0.27
Nodes (3): BitArray, Bit, Timer

### Community 25 - "Driver Siemens Types 25"
Cohesion: 0.27
Nodes (4): IEnumerable, UInt16, Counter, UInt16

### Community 26 - "Plc Jobs 26"
Cohesion: 0.36
Nodes (4): CancellationToken, string, Task, WcsDoor5Job

### Community 27 - "Plc Jobs 27"
Cohesion: 0.39
Nodes (4): CancellationToken, string, Task, WcsDoor2Job

### Community 28 - "Plc Jobs 28"
Cohesion: 0.39
Nodes (4): CancellationToken, string, Task, WcsDoor3Job

### Community 29 - "Plc Jobs 29"
Cohesion: 0.39
Nodes (4): CancellationToken, string, Task, WcsDoor4Job

### Community 30 - "Plc Jobs 30"
Cohesion: 0.39
Nodes (4): CancellationToken, string, Task, WcsDoor6Job

### Community 31 - "Plc Jobs 31"
Cohesion: 0.39
Nodes (4): CancellationToken, string, Task, WcsDoor7Job

### Community 32 - "Plc Jobs 32"
Cohesion: 0.39
Nodes (4): CancellationToken, string, Task, WcsDoor8Job

### Community 33 - "Plc Jobs 33"
Cohesion: 0.39
Nodes (4): CancellationToken, string, Task, WcsLmJob

### Community 34 - "Plc Jobs 34"
Cohesion: 0.39
Nodes (4): CancellationToken, string, Task, WcsMoverJob

### Community 36 - "Driver Siemens Types 36"
Cohesion: 0.32
Nodes (3): Int32, UInt32, Single

### Community 37 - "Config 37"
Cohesion: 0.43
Nodes (7): List, AgvJobConfig, BackGroundJob, ConfigData, PlcNodeSetting, PlcSetting, Settings

### Community 38 - "Driver Siemens Internal 38"
Cohesion: 0.33
Nodes (5): S7.Net.Internal, Func, object, Task, TaskQueue

### Community 39 - "Plc Driver Siemens 39"
Cohesion: 0.33
Nodes (4): CancellationToken, Stream, Task, StreamExtensions

### Community 40 - "Driver Siemens Types 40"
Cohesion: 0.38
Nodes (3): Int32, UInt32, Double

### Community 44 - "Driver Siemens Types 44"
Cohesion: 0.50
Nodes (4): Attribute, int, S7StringAttribute, S7StringType

### Community 45 - "Driver Siemens Compat 45"
Cohesion: 0.50
Nodes (4): Close(), Connect(), TcpClient, TcpClientMixins

### Community 46 - "Driver Siemens Protocol 46"
Cohesion: 0.40
Nodes (4): int, Header, Offsets, Parameter

## Knowledge Gaps
- **63 isolated node(s):** `net6.0`, `StackExchange.Redis (2.6.122)`, `Microsoft.NET.Sdk`, `net6.0`, `Microsoft.NET.Sdk` (+58 more)
  These have ≤1 connection - possible missing edges or undocumented components.
- **9 thin communities (<3 nodes) omitted from report** — run `graphify query` to explore isolated nodes.

## Suggested Questions
_Questions this graph is uniquely positioned to answer:_

- **Why does `ILog` connect `Logger ILogger 12` to `Plc Jobs 32`, `Plc Jobs 33`, `Plc Cache 2`, `Plc Jobs 34`, `By My SQL 4`, `Plc Driver Simulation 6`, `Service By File 7`, `Plc Cache 10`, `Plc Jobs 14`, `Plc Driver Siemens 17`, `Plc Jobs 21`, `Plc Jobs 26`, `Plc Jobs 27`, `Plc Jobs 28`, `Plc Jobs 29`, `Plc Jobs 30`, `Plc Jobs 31`?**
  _High betweenness centrality (0.229) - this node is a cross-community bridge._
- **Why does `SiemensPlc` connect `Plc Driver Siemens 17` to `Plc Driver Siemens 0`, `Plc Jobs 1`, `Plc Cache 2`, `Plc Driver Simulation 6`, `Service By File 7`, `Logger ILogger 12`?**
  _High betweenness centrality (0.221) - this node is a cross-community bridge._
- **Why does `S7.Net.Types` connect `Plc Driver Siemens 18` to `Plc Jobs 1`, `Driver Siemens Types 35`, `Driver Siemens Types 36`, `Driver Siemens Types 5`, `Driver Siemens Types 40`, `Driver Siemens Types 41`, `Driver Siemens Types 42`, `Driver Siemens Types 43`, `Driver Siemens Types 44`, `Driver Siemens Protocol 46`, `Driver Siemens Types 47`, `Driver Siemens Protocol 16`, `Driver Siemens Types 49`, `Driver Siemens Types 15`, `Plc Driver Siemens 17`, `Driver Siemens Types 50`, `Driver Siemens Types 24`, `Driver Siemens Types 25`?**
  _High betweenness centrality (0.148) - this node is a cross-community bridge._
- **What connects `net6.0`, `StackExchange.Redis (2.6.122)`, `Microsoft.NET.Sdk` to the rest of the system?**
  _63 weakly-connected nodes found - possible documentation gaps or missing edges._
- **Should `Plc Driver Siemens 0` be split into smaller, more focused modules?**
  _Cohesion score 0.05570175438596491 - nodes in this community are weakly interconnected._
- **Should `Plc Jobs 1` be split into smaller, more focused modules?**
  _Cohesion score 0.06885758998435054 - nodes in this community are weakly interconnected._
- **Should `Plc Cache 2` be split into smaller, more focused modules?**
  _Cohesion score 0.053185271770894216 - nodes in this community are weakly interconnected._