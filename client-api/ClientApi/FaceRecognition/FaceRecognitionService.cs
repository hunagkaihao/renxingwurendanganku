using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text.Json;

namespace ClientApi.FaceRecognition;

/// <summary>管理 DeepFace 身份服务；只监听回环接口，按配置执行单帧反欺骗。</summary>
public sealed class FaceRecognitionService : BackgroundService
{
    private readonly IConfiguration _configuration;
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<FaceRecognitionService> _logger;
    private readonly IHostApplicationLifetime _lifetime;
    private readonly HttpClient _client = new(new SocketsHttpHandler { UseProxy = false });
    private Process? _process;
    private volatile bool _ready;

    /// <summary>注入配置、项目路径、日志与应用生命周期。</summary>
    public FaceRecognitionService(IConfiguration configuration, IWebHostEnvironment environment,
        ILogger<FaceRecognitionService> logger, IHostApplicationLifetime lifetime)
    {
        _configuration = configuration;
        _environment = environment;
        _logger = logger;
        _lifetime = lifetime;
        // 超时由每次请求的配置快照和取消令牌控制，支持运行时调整。
        _client.Timeout = Timeout.InfiniteTimeSpan;
    }

    /// <summary>启动内部服务并预加载身份模型；启动失败阻止主服务接受识别请求。</summary>
    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        if (!_configuration.GetValue("FaceRecognition:Enabled", true))
        {
            await base.StartAsync(cancellationToken);
            return;
        }
        var bundled = Path.Combine(AppContext.BaseDirectory, "FaceRecognition");
        // 始终运行本项目的后端源码；ProjectPath 只选择 Python 和模型资源，避免绕过新增配置逻辑。
        var moduleRoot = File.Exists(Path.Combine(bundled, "backend", "client_api.py"))
            ? bundled : Path.Combine(_environment.ContentRootPath, "FaceRecognition");
        var configured = _configuration["FaceRecognition:ProjectPath"];
        var project = !string.IsNullOrWhiteSpace(configured)
            ? Path.GetFullPath(configured, _environment.ContentRootPath)
            : File.Exists(Path.Combine(bundled, "runtime", "python.exe"))
                ? bundled : Path.GetFullPath("../../DeepFace", _environment.ContentRootPath);
        var python = Path.Combine(project, "runtime", "python.exe");
        if (!File.Exists(python)) python = Path.Combine(project, ".venv", "Scripts", "python.exe");
        if (!File.Exists(python) || !File.Exists(Path.Combine(moduleRoot, "backend", "client_api.py")))
            throw new InvalidOperationException("未找到识别运行环境，请运行 scripts/prepare-face-runtime.ps1，或配置 FaceRecognition:ProjectPath。");

        var token = Convert.ToHexString(RandomNumberGenerator.GetBytes(32));
        var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        var port = ((IPEndPoint)listener.LocalEndpoint).Port;
        listener.Stop();
        _client.BaseAddress = new Uri($"http://127.0.0.1:{port}/");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var start = new ProcessStartInfo(python)
        {
            WorkingDirectory = moduleRoot, UseShellExecute = false, CreateNoWindow = true,
            RedirectStandardOutput = true, RedirectStandardError = true
        };
        start.ArgumentList.Add("-X");
        start.ArgumentList.Add("utf8");
        start.ArgumentList.Add("-m");
        start.ArgumentList.Add("backend.client_api");
        start.Environment["CLIENT_FACE_TOKEN"] = token;
        start.Environment["CLIENT_FACE_PORT"] = port.ToString(System.Globalization.CultureInfo.InvariantCulture);
        start.Environment["CLIENT_FACE_PARENT_PID"] = Environment.ProcessId.ToString(System.Globalization.CultureInfo.InvariantCulture);
        start.Environment["CLIENT_FACE_MODELS_DIR"] = Path.Combine(project, "data", "models");
        var dataPath = _configuration["FaceRecognition:DataPath"] ?? "data/face-recognition";
        start.Environment["CLIENT_FACE_DATA_DIR"] = Path.GetFullPath(dataPath, _environment.ContentRootPath);
        _process = new Process { StartInfo = start };
        // 子服务的模型诊断保持原始文本，不记录图片、访问令牌或特征。
        _process.OutputDataReceived += (_, args) => { if (args.Data != null) _logger.LogDebug("DeepFace: {Message}", args.Data); };
        _process.ErrorDataReceived += (_, args) => { if (args.Data != null) _logger.LogInformation("DeepFace: {Message}", args.Data); };
        try
        {
            _process.Start();
            _process.BeginOutputReadLine();
            _process.BeginErrorReadLine();
            using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            timeout.CancelAfter(TimeSpan.FromSeconds(_configuration.GetValue("FaceRecognition:StartupTimeoutSeconds", 180)));
            while (true)
            {
                timeout.Token.ThrowIfCancellationRequested();
                if (_process.HasExited) throw new InvalidOperationException($"识别服务启动失败，退出码 {_process.ExitCode}，请查看 DeepFace 日志。");
                try
                {
                    using var status = await SendAsync(HttpMethod.Get, "api/status", null, timeout.Token);
                    if (status.RootElement.GetProperty("service").GetString() != "client-api-face")
                        throw new InvalidOperationException("内部识别服务身份不正确。");
                    break;
                }
                catch (HttpRequestException) { await Task.Delay(200, timeout.Token); }
            }
            var settings = ReadSettings();
            using var warmed = await SendAsync(HttpMethod.Post, "api/warmup", new { settings }, timeout.Token);
            if (!warmed.RootElement.GetProperty("ready").GetBoolean())
                throw new InvalidOperationException("身份模型尚未就绪。");
            _ready = true;
            _logger.LogInformation("DeepFace 身份模型已就绪，单帧反欺骗启用：{LivenessEnabled}。", settings.Liveness.Enabled);
            await base.StartAsync(cancellationToken);
        }
        catch
        {
            await CloseProcessAsync();
            throw;
        }
    }

    /// <summary>监视识别子进程；意外退出时停止主服务，避免继续提供失效识别接口。</summary>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (_process == null) return;
        try { await _process.WaitForExitAsync(stoppingToken); }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested) { return; }
        _ready = false;
        _logger.LogError("DeepFace 识别服务意外退出，退出码 {ExitCode}。", _process.ExitCode);
        _lifetime.StopApplication();
    }

    /// <summary>返回匹配用户 ID；陌生人、人脸库为空和候选不唯一均返回 0。</summary>
    public async Task<int> MatchAsync(string image, CancellationToken cancellationToken)
    {
        EnsureReady();
        using var response = await SendAsync(HttpMethod.Post, "api/recognize", new { image, settings = ReadSettings() }, cancellationToken);
        var result = response.RootElement;
        var status = result.GetProperty("status").GetString();
        if (status is "unknown" or "ambiguous" or "empty") return 0;
        if (status == "matched" && result.GetProperty("user_id").TryGetInt32(out var userId) && userId > 0) return userId;
        throw new FaceServiceException("识别服务返回了无效身份结果。");
    }

    /// <summary>将图片录入指定业务用户；格式、质量和用户 ID 由内部服务统一校验。</summary>
    public async Task EnrollAsync(string userId, string image, CancellationToken cancellationToken)
    {
        EnsureReady();
        using var response = await SendAsync(HttpMethod.Post, "api/enroll", new { user_id = userId, image, settings = ReadSettings() }, cancellationToken);
    }

    /// <summary>删除指定业务用户的人脸及全部样本。</summary>
    public async Task DeleteAsync(string userId, CancellationToken cancellationToken)
    {
        EnsureReady();
        using var response = await SendAsync(HttpMethod.Delete, "api/people/" + Uri.EscapeDataString(userId), null, cancellationToken);
    }

    /// <summary>读取当前 YAML 配置快照；格式错误映射到原有业务失败，不使用默认值掩盖错误。</summary>
    private FaceRecognitionSettings ReadSettings()
    {
        try
        {
            return _configuration.GetSection("FaceRecognition").Get<FaceRecognitionSettings>() ?? new();
        }
        catch (InvalidOperationException error)
        {
            throw new FaceServiceException("人脸配置格式错误，请检查 Matching、Storage 和 Liveness 设置。", error);
        }
    }

    /// <summary>禁用或未就绪状态明确失败。</summary>
    private void EnsureReady()
    {
        if (!_ready) throw new FaceServiceException("人脸识别未启用或服务未就绪。");
    }

    /// <summary>发送已认证的内部请求，保留预期的输入错误并校验响应格式。</summary>
    private async Task<JsonDocument> SendAsync(HttpMethod method, string path, object? body, CancellationToken cancellationToken)
    {
        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeout.CancelAfter(ReadRequestTimeout());
        using var request = new HttpRequestMessage(method, path);
        if (body != null) request.Content = JsonContent.Create(body);
        using var response = await _client.SendAsync(request, timeout.Token);
        var text = await response.Content.ReadAsStringAsync(timeout.Token);
        JsonDocument document;
        try { document = JsonDocument.Parse(text); }
        catch (JsonException error) { throw new FaceServiceException("识别服务返回格式错误。", error); }
        if (response.IsSuccessStatusCode) return document;
        using (document)
        {
            var message = document.RootElement.TryGetProperty("error", out var value) ? value.GetString() : null;
            throw new FaceServiceException(message ?? "识别服务请求失败。");
        }
    }

    /// <summary>读取当前请求超时，单位秒，必须为正整数；配置错误保持业务失败。</summary>
    private TimeSpan ReadRequestTimeout()
    {
        try
        {
            var seconds = _configuration.GetValue("FaceRecognition:RequestTimeoutSeconds", 180);
            // CancellationTokenSource 的计时器最多接受 4294967294 毫秒。
            if (seconds <= 0 || seconds > 4_294_967)
                throw new InvalidOperationException("请求超时超出计时器支持范围。");
            return TimeSpan.FromSeconds(seconds);
        }
        catch (InvalidOperationException error)
        {
            throw new FaceServiceException("请将 FaceRecognition:RequestTimeoutSeconds 配置为 1～4294967 的整数秒数。", error);
        }
    }

    /// <summary>应用正常停止时关闭自己启动的进程及其子进程。</summary>
    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _ready = false;
        await base.StopAsync(cancellationToken);
        await CloseProcessAsync();
    }

    /// <summary>终止托管进程，等待退出后释放句柄。</summary>
    private async Task CloseProcessAsync()
    {
        if (_process == null) return;
        try
        {
            if (!_process.HasExited) _process.Kill(entireProcessTree: true);
            await _process.WaitForExitAsync().WaitAsync(TimeSpan.FromSeconds(5));
        }
        finally { _process.Dispose(); _process = null; }
    }

    /// <summary>释放 HTTP 连接；异常宿主退出时子进程的父进程监视器负责清理。</summary>
    public override void Dispose()
    {
        _client.Dispose();
        base.Dispose();
    }
}

/// <summary>内部识别的预期失败，供接口映射到原有失败返回格式。</summary>
public sealed class FaceServiceException : Exception
{
    /// <summary>保留可操作的错误信息和可选原始原因。</summary>
    public FaceServiceException(string message, Exception? cause = null) : base(message, cause) { }
}
