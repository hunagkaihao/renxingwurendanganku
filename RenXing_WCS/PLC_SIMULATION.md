# WCS 本地 PLC 交互模拟

用于 WMS → WCS 联调。开启后，WCS 在内部模拟默认 `Plc1` 点位、设备就绪状态及动作反馈，无需启动 PLCServer 或连接实体 PLC。WCS 仍执行原有命令生成、前置条件、资源分配、工步完成判断和 WMS 状态反馈。

## 启用

在 WCS 的 `Wcs.HttpApi.Host/appsettings.yaml` 的现有 `Wcs:` 节中设置：

```yaml
Wcs:
  PlcSimulation: true
  PlcSimulationDelayMs: 1000
```

不要重复添加第二个 `Wcs:` 节。开关默认 `false`，延迟单位为毫秒，最少 50 毫秒。配置不热加载，修改后需重启 WCS。

当前业务配置从运行目录的 `appsettings.yaml` 加载：从源码运行请重新生成；使用发布程序请修改发布目录中的文件。此开关不要仅写入 `appsettings.Development.yaml`，当前 `WcsToolsModule` 的业务选项绑定不读取该覆盖文件。

WMS 保持 `WCSEnable: true`、`WCSSimulation: false`，`WCSServer` 指向本地 WCS，WCS 的 WMS 地址和账号也需对应测试 WMS。WCS 自身的数据库、Redis、节点、库位、流程和缓存位配置仍需可用，调度仍须处于运行状态。模拟不会跳过 WMS 入库确认、开门授权或业务数据校验。

## 行为

- 龙门入出库取放货、移库、回原点、回避让位：先反馈执行中，延迟后反馈完成，保留当前命令号和 JobId。
- 柜门：按 `Wcs:DoorCodes` 清单生成调度开门、直接开门及状态点，默认 1～8 门；追加 `12009`、`12010` 后支持新门。详见 `DOOR_CONFIGURATION.md`。
- 移载机构：模拟伸出、缩回反馈和位置。
- 龙门空闲、原点、避让位及心跳：提供模拟点位，前置条件和监控继续走原入口。
- 盘点：按当前扫描段的已配置库位逐个反馈；WCS 保存当前库位结果后，才反馈下一库位，最后发全部完成信号。每个模拟库位报告条码 `0`，正常业务处理将其转换为 `empty`。这用于验证盘点结果流转，不表示与 WMS 账面一致。
- 未定义的 PLC/点位、错误帧长度、未知动作被拒绝，不自动当成完成。

模拟数据仅存在 WCS 进程内存中，不连接 PLCServer 的 Redis 客户端，也不改写真实 PLC 点位。关闭开关并重启即恢复原通信方式。

切换模式时请先结束测试任务；模拟设备状态不跨 WCS 重启恢复。重新启动后下发新的测试任务，避免沿用上次已发送设备命令、尚在等待反馈的任务。此开关只模拟 PLC 接口，不模拟独立的密集架控制器等其他外部系统。

## 验证

无需 PLCServer 即可运行自动回归验证：

```powershell
dotnet run --project RenXing_WCS/tests/Wcs.PlcSimulationTests/Wcs.PlcSimulationTests.csproj
```

验证覆盖同步/异步 PLC 入口、事件订阅、所有普通龙门动作、8 个门、移载位置、多库位盘点及连续批次、盘点确认后推进、错误命令拒绝、模拟模式不访问设备 Redis，以及默认真实路由。

业务联调时，启动测试数据库、Redis、WMS 和 WCS，确认 WCS 启动日志显示“PLC 模拟已启用”；从 WMS 下发一笔入出库任务，完成正常业务确认，观察工步推进及 WMS 的 Completed 状态。自动验证不替代这一整链验收。
