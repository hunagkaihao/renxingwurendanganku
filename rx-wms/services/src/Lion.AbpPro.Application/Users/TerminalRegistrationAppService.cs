using System;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Lion.AbpPro.ConfigurationOptions;
using Lion.AbpPro.Users.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Authorization;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Uow;

namespace Lion.AbpPro.Users
{
    /// <summary>在服务端校验管理员身份，编排账号创建、设备录入和可登录绑定的启用。</summary>
    // 管理员角色由每个公开方法的 EnsureAdmin 使用 ABP CurrentUser 检查，与应用配置返回的角色一致。
    [Authorize]
    [Authorize(IdentityPermissions.Users.Default)]
    [Authorize(IdentityPermissions.Users.Update)]
    public class TerminalRegistrationAppService : AbpProAppService, ITerminalRegistrationAppService
    {
        /// <summary>同一 WMS 实例内串行录入，避免对同一设备并发采集或重复写入。</summary>
        private static readonly SemaphoreSlim EnrollmentGate = new(1, 1);
        /// <summary>兼容 ClientApi 响应的属性大小写。</summary>
        private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };
        /// <summary>复用现有用户管理权限、账号校验和创建逻辑。</summary>
        private readonly IUserAppService _users;
        /// <summary>取得可分配的现有角色。</summary>
        private readonly IIdentityRoleAppService _roles;
        /// <summary>核对目标人员所属租户与账号状态。</summary>
        private readonly IIdentityUserRepository _userRepository;
        /// <summary>维护录入编号及其所属账号。</summary>
        private readonly IRepository<UserBiometricBinding, Guid> _bindings;
        /// <summary>分别提交编号预留与最终启用，外部设备调用失败也保留编号归属。</summary>
        private readonly IUnitOfWorkManager _workManager;
        /// <summary>创建访问受信任本地设备服务的客户端。</summary>
        private readonly IHttpClientFactory _clients;
        /// <summary>设备服务地址及人脸请求超时配置。</summary>
        private readonly BiometricLoginOptions _options;
        /// <summary>持久化人脸和指静脉编号序列，跨租户及 WMS 实例避免同类编号重复分配。</summary>
        private readonly IBiometricNumberAllocator _biometricNumbers;

        /// <summary>注入账号、角色、绑定仓储、工作单元及设备服务配置。</summary>
        public TerminalRegistrationAppService(IUserAppService users, IIdentityRoleAppService roles,
            IIdentityUserRepository userRepository, IRepository<UserBiometricBinding, Guid> bindings,
            IUnitOfWorkManager workManager, IHttpClientFactory clients, IOptionsSnapshot<BiometricLoginOptions> options,
            IBiometricNumberAllocator biometricNumbers)
        {
            _users = users;
            _roles = roles;
            _userRepository = userRepository;
            _bindings = bindings;
            _workManager = workManager;
            _clients = clients;
            _options = options.Value;
            _biometricNumbers = biometricNumbers;
        }

        /// <summary>显式检查当前登录者角色，避免仅依赖前端入口或可伪造的 Cookie。</summary>
        private void EnsureAdmin()
        {
            if (!CurrentUser.IsAuthenticated || !CurrentUser.IsInRole("admin"))
                throw new AbpAuthorizationException("请使用管理员账号登录后进行人员注册。");
        }

        /// <inheritdoc />
        public Task<bool> CheckAccessAsync()
        {
            EnsureAdmin();
            return Task.FromResult(true);
        }

        /// <inheritdoc />
        public Task<PagedResultDto<IdentityUserDto>> GetUsersAsync(PagingUserListInput input)
        {
            EnsureAdmin();
            return _users.ListAsync(input);
        }

        /// <inheritdoc />
        public Task<ListResultDto<IdentityRoleDto>> GetRolesAsync()
        {
            EnsureAdmin();
            return _roles.GetAllListAsync();
        }

        /// <inheritdoc />
        [Authorize(IdentityPermissions.Users.Create)]
        public Task<IdentityUserDto> CreateUserAsync(IdentityUserCreateDto input)
        {
            EnsureAdmin();
            if (input.RoleNames == null || !input.RoleNames.Any())
                throw new UserFriendlyException("请至少为新人员分配一个角色，否则无法登录。");
            return _users.CreateAsync(input);
        }

        /// <inheritdoc />
        public async Task<UserBiometricBindingDto> EnrollAsync(Guid userId, EnrollUserBiometricInput input)
        {
            EnsureAdmin();
            if (input == null || (input.Method != "face" && input.Method != "vein"))
                throw new UserFriendlyException("请选择有效的录入方式。");
            if (input.OperationId == Guid.Empty)
                throw new UserFriendlyException("录入操作标识无效，请重新开始本次录入。");
            if (input.Method == "face" && string.IsNullOrWhiteSpace(input.ImageData))
                throw new UserFriendlyException("请先开启外接摄像头并拍摄人脸。");
            var user = userId == Guid.Empty ? null : await _userRepository.FindAsync(userId);
            if (user == null || user.IsDeleted || user.TenantId != CurrentTenant.Id || !user.IsActive)
                throw new UserFriendlyException("请选择当前租户中有效且已启用的 WMS 人员。");
            if (!Uri.TryCreate(_options.ServiceUrl, UriKind.Absolute, out var serviceUri)
                || (serviceUri.Scheme != "http" && serviceUri.Scheme != "https")
                || _options.TimeoutSeconds < 5 || _options.TimeoutSeconds > 120)
                throw new UserFriendlyException("识别服务地址或超时配置无效，请联系管理员。");
            if (!await EnrollmentGate.WaitAsync(0))
                throw new UserFriendlyException("正在录入其他人员，请等待本次录入结束。");
            var veinLocked = false;
            try
            {
                if (input.Method == "vein")
                {
                    veinLocked = await BiometricLoginService.VeinGate.WaitAsync(0);
                    if (!veinLocked) throw new UserFriendlyException("指静脉设备正在识别或录入，请稍后重试。");
                    // 新页面依赖实时提示，先确认设备程序支持进度接口，再预留编号和开始采集。
                    if (input.OperationId.HasValue)
                        await ReadDeviceEnrollmentProgressAsync(serviceUri, userId, input.OperationId.Value);
                }
                var bindingId = await ReserveIdentifierAsync(userId, input);
                await EnrollDeviceAsync(serviceUri, userId, input);
                // 仅设备明确确认成功后启用；超时、采集失败或断线均保留停用的编号以便核查重试。
                using (var work = _workManager.Begin(requiresNew: true, isTransactional: true))
                {
                    var binding = await _bindings.GetAsync(bindingId);
                    if (binding.UserId != userId || binding.TenantId != CurrentTenant.Id
                        || (input.Method == "face" ? binding.FaceId : binding.FingerId) != input.Identifier)
                        throw new UserFriendlyException("录入期间绑定已变更，请联系管理员核查设备编号。");
                    binding.UpdateIdentifiers(binding.FaceId, binding.FingerId, true);
                    await _bindings.UpdateAsync(binding, autoSave: true);
                    await work.CompleteAsync();
                    return new UserBiometricBindingDto
                    {
                        Id = binding.Id, UserId = binding.UserId, FaceId = binding.FaceId,
                        FingerId = binding.FingerId, IsEnabled = binding.IsEnabled,
                        ConcurrencyStamp = binding.ConcurrencyStamp
                    };
                }
            }
            finally
            {
                if (veinLocked) BiometricLoginService.VeinGate.Release();
                EnrollmentGate.Release();
            }
        }

        /// <inheritdoc />
        public async Task<UserBiometricEnrollmentProgressDto> GetEnrollmentProgressAsync(Guid userId, Guid operationId)
        {
            EnsureAdmin();
            if (operationId == Guid.Empty) throw new UserFriendlyException("录入操作标识无效。");
            var user = userId == Guid.Empty ? null : await _userRepository.FindAsync(userId);
            if (user == null || user.IsDeleted || user.TenantId != CurrentTenant.Id)
                throw new UserFriendlyException("指定人员不存在或不属于当前租户。");
            if (!Uri.TryCreate(_options.ServiceUrl, UriKind.Absolute, out var serviceUri)
                || (serviceUri.Scheme != "http" && serviceUri.Scheme != "https"))
                throw new UserFriendlyException("识别服务地址无效，请联系管理员。");
            return await ReadDeviceEnrollmentProgressAsync(serviceUri, userId, operationId);
        }

        /// <summary>读取设备进度并检查接口版本；仅带业务码的 404 表示操作尚未创建，旧程序缺少路由时明确拒绝。</summary>
        private async Task<UserBiometricEnrollmentProgressDto> ReadDeviceEnrollmentProgressAsync(Uri serviceUri, Guid userId, Guid operationId)
        {
            using var client = _clients.CreateClient();
            client.BaseAddress = new Uri(serviceUri.AbsoluteUri.TrimEnd('/') + "/");
            client.Timeout = TimeSpan.FromSeconds(5);
            try
            {
                using var response = await client.GetAsync($"Vein/VeinRegisterProgress?operationId={operationId:D}");
                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    var body = await response.Content.ReadAsStringAsync();
                    if (!string.IsNullOrWhiteSpace(body))
                    {
                        using var document = JsonDocument.Parse(body);
                        if (document.RootElement.ValueKind == JsonValueKind.Object
                            && document.RootElement.TryGetProperty("code", out var code)
                            && code.ValueKind == JsonValueKind.String && code.GetString() == "VeinRegistrationNotFound")
                            return new UserBiometricEnrollmentProgressDto { OperationId = operationId };
                    }
                    throw new UserFriendlyException("设备程序缺少指静脉实时进度接口，请更新并重启 ClientApi 后再录入。");
                }
                response.EnsureSuccessStatusCode();
                var progress = await response.Content.ReadFromJsonAsync<UserBiometricEnrollmentProgressDto>(JsonOptions);
                if (progress == null || progress.OperationId != operationId
                    || !Guid.TryParse(progress.UserId, out var ownerId) || ownerId != userId)
                    throw new UserFriendlyException("采集进度不属于当前人员，请重新进入录入页面。");
                if (progress.IsComplete && progress.Success == true)
                {
                    progress.Stage = "binding";
                    progress.Message = "六次采集及设备写入已完成，正在绑定 WMS 账号，请稍候。";
                }
                return progress;
            }
            catch (HttpRequestException) { throw new UserFriendlyException("暂时无法获取采集提示，请保持当前页面并查看设备终端提示。"); }
            catch (OperationCanceledException) { throw new UserFriendlyException("采集进度查询超时，请保持当前页面并查看设备终端提示。"); }
            catch (JsonException) { throw new UserFriendlyException("采集进度格式异常，请检查设备服务版本。"); }
        }

        /// <summary>在独立事务中自动分配编号，优先沿用同一人员同类特征的失败预留，唯一索引防止重复归属。</summary>
        private async Task<Guid> ReserveIdentifierAsync(Guid userId, EnrollUserBiometricInput input)
        {
            using var work = _workManager.Begin(requiresNew: true, isTransactional: true);
            UserBiometricBinding binding;
            using (DataFilter.Disable<IMultiTenant>())
            {
                // 只重用同类特征的独立停用预留，避免重录时意外启用包含另一类特征的组合绑定。
                var pending = await _bindings.GetListAsync(item => item.UserId == userId
                    && item.TenantId == CurrentTenant.Id && !item.IsEnabled
                    && (input.Method == "face" ? item.FaceId.HasValue && !item.FingerId.HasValue
                        : item.FingerId.HasValue && !item.FaceId.HasValue));
                binding = pending.OrderBy(item => item.CreationTime)
                    .ThenBy(item => input.Method == "face" ? item.FaceId : item.FingerId).FirstOrDefault();
                var reservedNumber = input.Method == "face" ? binding?.FaceId : binding?.FingerId;
                // 两种录入均忽略旧客户端传入的编号；编号与预留记录必须在同一事务内提交。
                input.Identifier = reservedNumber ?? await _biometricNumbers.AllocateAsync(input.Method);
            }
            if (binding == null)
            {
                binding = new UserBiometricBinding(GuidGenerator.Create(), userId, CurrentTenant.Id,
                    input.Method == "face" ? input.Identifier : null,
                    input.Method == "vein" ? input.Identifier : null, false);
                await _bindings.InsertAsync(binding, autoSave: true);
            }
            await work.CompleteAsync();
            return binding.Id;
        }

        /// <summary>调用 ClientApi 录入并检查业务成功码；指静脉六次采集最多等待 240 秒，不自动重试。</summary>
        private async Task EnrollDeviceAsync(Uri serviceUri, Guid userId, EnrollUserBiometricInput input)
        {
            using var client = _clients.CreateClient();
            client.BaseAddress = new Uri(serviceUri.AbsoluteUri.TrimEnd('/') + "/");
            client.Timeout = TimeSpan.FromSeconds(input.Method == "vein" ? 240 : _options.TimeoutSeconds);
            try
            {
                var veinPath = $"Vein/VeinRegisterResult?userId={userId:D}&fingerId={input.Identifier}";
                if (input.OperationId.HasValue) veinPath += $"&operationId={input.OperationId.Value:D}";
                using var response = input.Method == "face"
                    ? await client.PostAsJsonAsync("Face/FaceAdd", new { UserId = input.Identifier.ToString(), input.ImageData })
                    : await client.PostAsync(veinPath, null);
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadFromJsonAsync<DeviceEnrollmentResult>(JsonOptions);
                var success = input.Method == "face" ? result?.Errno == 0
                    : result?.Success == true && result.FingerId == input.Identifier && result.CompletedSamples == 6;
                if (!success)
                    throw new UserFriendlyException($"录入失败：{(result?.Msg ?? "设备未确认成功").TrimEnd('。', ' ', '.')}。编号已保留，请为同一人员重试。");
            }
            catch (HttpRequestException)
            {
                throw new UserFriendlyException("无法确认设备录入结果，请检查识别服务连接及日志；编号已保留，尚未启用登录。");
            }
            catch (OperationCanceledException)
            {
                throw new UserFriendlyException("设备录入响应超时，请先核查设备结果；编号已保留，尚未启用登录。");
            }
            catch (JsonException)
            {
                throw new UserFriendlyException("设备录入响应格式异常，请核查服务版本；编号已保留，尚未启用登录。");
            }
        }

        /// <summary>读取两类录入接口的成功标记，不接收或保存生物特征模板。</summary>
        private sealed class DeviceEnrollmentResult
        {
            /// <summary>供 JSON 反序列化创建结果。</summary>
            public DeviceEnrollmentResult() { }
            /// <summary>人脸业务码，只有明确的 0 表示成功。</summary>
            public int? Errno { get; set; }
            /// <summary>指静脉成功标记。</summary>
            public bool Success { get; set; }
            /// <summary>设备实际写入的模板编号。</summary>
            public int? FingerId { get; set; }
            /// <summary>指静脉完成的采集次数，成功时必须为 6。</summary>
            public int CompletedSamples { get; set; }
            /// <summary>设备返回的结果说明。</summary>
            public string Msg { get; set; }
        }
    }
}
