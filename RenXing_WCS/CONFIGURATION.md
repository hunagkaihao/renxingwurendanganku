# WCS 配置文件

WCS 使用 `Wcs.HttpApi.Host/appsettings.yaml`，主程序还会按当前环境加载可选的
`appsettings.{Environment}.yaml`，例如 `appsettings.Development.yaml`。
配置节、字段名称和参数值沿用原 JSON 配置；本次仅迁移格式，不调整业务逻辑。

所有 YAML 文件均禁用热重载。修改配置后需要重启 WCS 服务。

项目及配置统一使用 `Wcs`，解决方案入口为 `Wcs.sln`，主程序项目为
`Wcs.HttpApi.Host`。配置节使用 `Wcs`（如 `Wcs:BaseUrl`，对应环境变量
`Wcs__BaseUrl`），OAuth scope 也使用 `Wcs`。
更新已有部署时，需要同步修改旧配置节、环境变量和命令行参数中的名称；
如使用外部认证服务，还需同步注册新的 Swagger 客户端 ID 与 `Wcs` scope。
本地 YAML 中相关名称已更新，历史备份和旧发布目录不作原地修改，部署时应重新发布。

参数说明直接使用 YAML `#` 注释，不作为配置键值保存。旧 JSON 中的
“无人档案库调度配置”“wms与龙门plc对库位的定义”“Mjj配置说明”已改为参数旁的注释；
原带 `//` 前缀的心跳示例也已改为注释，仍不启用心跳配置。
注释中的时间单位、取值限制及“当前未读取”说明依据本次检查的 WCS 源码。

## 读取范围

- 主程序：原位置替换框架默认的 appsettings JSON 配置源，保留环境配置、
  开发环境 User Secrets、环境变量和命令行参数的原有覆盖顺序。
- `WcsToolsModule` 与静态 `Settings`：维持原有行为，只读取程序目录下的
  `appsettings.yaml`，不新增环境配置或环境变量覆盖。
- EF 设计时工厂：从原来的 Host 目录读取 `appsettings.yaml`。

## 部署与保密

- `appsettings*.yaml` 已设置为复制到构建输出和发布目录。
- `appsettings*.json` 不再复制到构建输出或发布目录。更新已有部署时，
  应先备份并移走旧的 appsettings JSON 文件，避免框架创建主程序时预读取旧文件。
- `appsettings.yaml` 纳入版本控制，保留配置项和中文注释，口令使用
  `__SET_LOCALLY__` 占位符，部署前必须填写实际值并核对环境参数。
  本机原有完整配置保留在工作区，另有被忽略的 `appsettings.yaml.local.bak` 备份；
  工作区实际口令与仓库占位符不同属于预期差异，后续提交前必须排除这些口令变更。
- 本次本地转换保留原 JSON 为 `appsettings.json.bak`，仅供人工回退参考。
  程序不自动回退读取 JSON；恢复旧版本时再配合恢复旧配置文件名。
- 原 `appsettings.json` 曾被 Git 跟踪，即使已有忽略规则仍在版本记录中。
  本次变更删除该跟踪路径；新 YAML 随源码提交和分发。
- 提交前已通过解决方案编译和 YAML 解析检查，未执行设备联调或发布。
