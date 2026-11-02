# 本地生物识别测试前端

Vue 3 + Element Plus + Vite。提供人脸上传/拍照、录入、识别、删除，以及指静脉六次连续采集注册、识别、删除。图片与响应只保存在页面内存，录入结果由后端持久保存。

## 启动

使用 Node.js 22.12+（推荐 24 LTS），在项目根目录启动后端。只测人脸时关闭未连接的指静脉设备，并指定独立测试库：

```powershell
dotnet run --project ./ClientApi/ClientApi.csproj --launch-profile ClientApi -- --Vein:Enable false --FaceRecognition:DataPath "E:/Tuta/project/client-api/data/face-test"
```

在另一个终端启动前端：

```powershell
cd web
npm install
npm run dev
```

访问 http://localhost:5173。默认 `/api` 由 Vite 代理到 `http://localhost:5123`，无需改后端跨域配置。

如后端使用 5032 等其他端口，复制 `.env.example` 为 `.env.local`，修改 `API_PROXY_TARGET` 并重启前端。也可在页面填写完整 HTTP(S) API 地址，但直接连接需要后端允许页面来源跨域。连接检查读取 OpenAPI 文档，只确认 API 可访问，不验证模型或设备可用性。

## 操作流程

- 人脸：填写正整数用户 ID，上传不超过 4 MB 的 JPEG/PNG/BMP 或开启摄像头拍照；录入后识别同一张照片，再用同一人的另一张照片验证效果。录入同一用户会追加样本；删除会删除全部样本，页面要求确认。摄像头需要 localhost 或 HTTPS，并在拍照、关闭、切换标签或页面卸载时释放。
- 人脸识别：正数为用户 ID，0 为陌生人、空库或候选歧义，-1 为输入或服务失败。录入/删除返回 `errno=0` 才是成功，`99` 为业务失败。活体由后端配置控制，默认关闭。
- 指静脉：连接厂商配套 Windows x64 设备，配置 `Vein:Enable`、串口、波特率及协议后重启后端。填写非零设备模板编号，避免覆盖已有模板；注册时按后端提示放入/移开同一手指六次。页面调用 `/Vein/VeinRegisterResult` 和 `/Vein/VeinMatchResult`，展示执行结果、已采集样本数和 SDK 错误码。只有设备写入成功才显示注册成功；设备未就绪时明确失败。识别返回模板编号，不是业务用户 ID。需重启更新后的后端才能使用新接口；旧 `/Vein/VeinRegister` 与 `/Vein/VeinMatch` 保留兼容。
- 指静脉删除同时调用 `AMSApiUrl` 指定的后台删除记录；设备删除成功并不保证后台删除成功，需要检查后台与日志。测试页面串行操作，但不要从其他页面或客户端同时操作同一设备。
- 本页面不使用 `VeinRegisterOnce`：该接口在第六次采集依赖业务后台，且当前上传的特征缓冲区与请求累计特征不一致。该后端问题未在本前端任务中修改。

## 验证与构建

```powershell
npm test
npm run build
npm run preview
```

构建输出为 `web/dist`；preview 默认 http://localhost:4173，保留同样的 API 代理。部署到其他静态服务器时需配置 `/api` 反向代理，或在页面填写允许跨域的 API 地址。

请求超时为 240 秒，不自动重试注册、录入或删除；超时后实际操作结果可能未知，应先检查后端日志。
