using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Lion.AbpPro.ConfigurationOptions;
using Lion.AbpPro.Users.Dtos;
using Microsoft.Extensions.Options;
using Volo.Abp;
using Volo.Abp.DependencyInjection;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Domain.Repositories;

namespace Lion.AbpPro.Users
{
    /// <summary>调用可信设备服务获取识别编号，再按数据库绑定解析 WMS 用户 GUID。</summary>
    public class BiometricLoginService : ITransientDependency
    {
        /// <summary>指静脉 SDK 一次只能采集一根手指，登录识别与管理员录入共用此锁串行占用设备。</summary>
        internal static readonly SemaphoreSlim VeinGate = new(1, 1);

        /// <summary>与 ClientApi 的 JSON 属性命名兼容。</summary>
        private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

        /// <summary>创建具有连接池的设备 HTTP 客户端。</summary>
        private readonly IHttpClientFactory _clientFactory;

        /// <summary>本次请求使用的识别服务地址与超时配置。</summary>
        private readonly BiometricLoginOptions _options;

        /// <summary>限定绑定所属租户，避免同名账号跨租户复用人员编号。</summary>
        private readonly ICurrentTenant _currentTenant;

        /// <summary>持久化的用户与识别编号关联，按租户隔离查询。</summary>
        private readonly IRepository<UserBiometricBinding, Guid> _bindings;

        /// <summary>注入设备连接配置、当前租户及数据库绑定仓储。</summary>
        public BiometricLoginService(IHttpClientFactory clientFactory, IOptionsSnapshot<BiometricLoginOptions> options,
            ICurrentTenant currentTenant, IRepository<UserBiometricBinding, Guid> bindings)
        {
            _clientFactory = clientFactory;
            _options = options.Value;
            _currentTenant = currentTenant;
            _bindings = bindings;
        }

        /// <summary>完成本次真实识别，返回唯一绑定的 WMS 用户 GUID；未匹配、停用或设备失败时拒绝登录。</summary>
        /// <param name="input">识别方式及人脸照片。</param>
        /// <param name="cancellationToken">页面离开或请求取消时终止登录。</param>
        public async Task<Guid> RecognizeUserIdAsync(BiometricLoginInput input, CancellationToken cancellationToken)
        {
            if (input == null || (input.Method != "face" && input.Method != "vein"))
                throw new UserFriendlyException("请选择人脸或指静脉登录。");
            if (!Uri.TryCreate(_options.ServiceUrl, UriKind.Absolute, out var serviceUri)
                || (serviceUri.Scheme != Uri.UriSchemeHttp && serviceUri.Scheme != Uri.UriSchemeHttps)
                || _options.TimeoutSeconds < 5 || _options.TimeoutSeconds > 120)
                throw new UserFriendlyException("识别服务地址或超时配置无效，请联系管理员。");

            if (!await _bindings.AnyAsync(binding => binding.TenantId == _currentTenant.Id && binding.IsEnabled
                && (input.Method == "face" ? binding.FaceId.HasValue : binding.FingerId.HasValue), cancellationToken))
                throw new UserFriendlyException("尚未建立此登录方式的账号与识别编号绑定，请联系管理员。");

            using var client = _clientFactory.CreateClient();
            client.BaseAddress = new Uri(serviceUri.AbsoluteUri.TrimEnd('/') + "/");
            client.Timeout = TimeSpan.FromSeconds(_options.TimeoutSeconds);
            int recognizedId;
            try
            {
                recognizedId = input.Method == "face"
                    ? await RecognizeFaceAsync(client, input.ImageData, cancellationToken)
                    : await RecognizeVeinAsync(client, cancellationToken);
            }
            catch (HttpRequestException)
            {
                throw new UserFriendlyException("无法连接识别服务，请检查一体机设备服务后重试，或使用账号密码登录。");
            }
            catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
            {
                throw new UserFriendlyException("识别服务响应超时，请重试或使用账号密码登录。");
            }
            catch (JsonException)
            {
                throw new UserFriendlyException("识别服务返回格式异常，请检查设备服务版本。");
            }

            cancellationToken.ThrowIfCancellationRequested();
            var bindings = await _bindings.GetListAsync(binding => binding.TenantId == _currentTenant.Id
                && binding.IsEnabled && (input.Method == "face" ? binding.FaceId : binding.FingerId) == recognizedId,
                cancellationToken: cancellationToken);
            var accounts = bindings.Select(binding => binding.UserId).Distinct().ToList();
            if (accounts.Count != 1)
                throw new UserFriendlyException(accounts.Count == 0
                    ? "识别成功，但该编号尚未绑定 WMS 账号，请联系管理员。"
                    : "识别编号绑定了多个账号，请联系管理员修正绑定。");
            return accounts[0];
        }

        /// <summary>发送照片进行人脸比对；返回正整数人员编号，其他结果均作为登录失败处理。</summary>
        private static async Task<int> RecognizeFaceAsync(HttpClient client, string image, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(image)) throw new UserFriendlyException("请先开启摄像头并拍摄人脸。");
            using var response = await client.PostAsJsonAsync("Face/FaceMatch", new { ImageData = image }, cancellationToken);
            response.EnsureSuccessStatusCode();
            var id = await response.Content.ReadFromJsonAsync<int>(cancellationToken: cancellationToken);
            if (id <= 0) throw new UserFriendlyException(id == 0
                ? "未识别到已录入的人脸，请正对摄像头重试。"
                : "人脸识别失败，请检查照片及识别服务后重试。");
            return id;
        }

        /// <summary>完成一次指静脉采集，返回设备模板编号；SDK 不支持取消时仍等待设备释放后再允许下一次采集。</summary>
        private static async Task<int> RecognizeVeinAsync(HttpClient client, CancellationToken cancellationToken)
        {
            if (!await VeinGate.WaitAsync(0, cancellationToken))
                throw new UserFriendlyException("指静脉设备正在识别，请稍后重试。");
            try
            {
                using var response = await client.GetAsync("Vein/VeinMatchResult", CancellationToken.None);
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadFromJsonAsync<VeinRecognitionResult>(JsonOptions);
                cancellationToken.ThrowIfCancellationRequested();
                if (result == null || !result.Success || result.FingerId.GetValueOrDefault() <= 0)
                    throw new UserFriendlyException(string.IsNullOrWhiteSpace(result?.Msg)
                        ? "未识别到已录入的指静脉，请重新放置手指后重试。" : result.Msg);
                return result.FingerId.Value;
            }
            finally
            {
                VeinGate.Release();
            }
        }

        /// <summary>ClientApi 指静脉识别结果，只接收成功状态、设备编号和可展示的提示。</summary>
        private sealed class VeinRecognitionResult
        {
            /// <summary>供 JSON 反序列化创建识别结果。</summary>
            public VeinRecognitionResult() { }

            /// <summary>是否成功识别到模板。</summary>
            public bool Success { get; set; }
            /// <summary>识别到的正整数模板编号，失败时可为空。</summary>
            public int? FingerId { get; set; }
            /// <summary>设备返回的操作提示。</summary>
            public string Msg { get; set; }
        }
    }
}
