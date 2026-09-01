using Wcs.ConfigTool;
using Wcs.Dispatch;
using Wcs.HttpApiTool;
using Wcs.LogTool;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Volo.Abp.Application.Services;
using Volo.Abp.DependencyInjection;

namespace Wcs.WMS
{
    /// <summary>
    /// WMS服务
    /// </summary>
    public class WMSService : WcsAppService, IWMSService
    {
        private readonly ILogger<WMSService> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _wmsUrl;
        private readonly User _user;
        private readonly JsonSerializerSettings _jsonSettings;
        
        // Token缓存
        private string _cachedToken;
        private DateTime _tokenExpiryTime;

        public WMSService(
            ILogger<WMSService> logger,
            IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _wmsUrl = Settings.Options.WMSUrl;
            _user = Settings.Options.User;
            
            _jsonSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            _jsonSettings.Converters.Add(new StringEnumConverter());
        }

        private async Task<string> LoginAsync()
        {
            try
            {
                using var client = _httpClientFactory.CreateClient();
                
                string jsonContent = JsonConvert.SerializeObject(_user, _jsonSettings);
                using var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await client.PostAsync($"{_wmsUrl}/wms/account/newlogin", content);
                string responseStr = await response.Content.ReadAsStringAsync();

                JObject obj = JObject.Parse(responseStr);
                string token = obj["token"]?.ToString();
                
                if (!string.IsNullOrEmpty(token))
                {
                    // 缓存token，设置过期时间为15分钟
                    _cachedToken = token;
                    _tokenExpiryTime = DateTime.Now.AddMinutes(15);
                }
                
                return token;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "登录WMS失败");
                return null;
            }
        }

        private async Task<HttpClient> CreateAuthenticatedClientAsync()
        {
            var client = _httpClientFactory.CreateClient();
            string token;
            
            // 检查是否有有效的缓存token
            if (!string.IsNullOrEmpty(_cachedToken) && DateTime.Now < _tokenExpiryTime)
            {
                token = _cachedToken;
            }
            else
            {
                // token不存在或已过期，重新登录
                token = await LoginAsync();
            }
            
            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                return client;
            }
            
            return null;
        }

        private async Task<string> SendRequestAsync<T>(T requestDto, string endpoint)
        {
            _logger.LogDebug("开始发送请求到WMS，端点：{Endpoint}", endpoint);
            
            return await SendRequestWithRedirectHandlingAsync(requestDto, endpoint, 0);
        }

        private async Task<string> SendRequestWithRedirectHandlingAsync<T>(T requestDto, string endpoint, int retryCount)
        {
            var client = await CreateAuthenticatedClientAsync();
            if (client == null)
            {
                _logger.LogError("创建认证客户端失败，无法发送请求到WMS，端点：{Endpoint}", endpoint);
                return null;
            }

            try
            {
                string jsonContent = JsonConvert.SerializeObject(requestDto, _jsonSettings);
                using var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                _logger.LogDebug("发送请求到WMS，请求地址：{Url}", $"{_wmsUrl}/{endpoint}");
                var response = await client.PostAsync($"{_wmsUrl}/{endpoint}", content);
                
                // 检查响应状态码
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("WMS返回非成功状态码：{StatusCode}，端点：{Endpoint}", 
                        response.StatusCode, endpoint);
                    
                    // 处理302重定向，重新获取token并重试
                    if ((int)response.StatusCode == 302 && retryCount < 1)
                    {
                        _logger.LogInformation("遇到302重定向，尝试重新获取访问令牌并重试，端点：{Endpoint}", endpoint);
                        
                        // 清除缓存的token
                        _cachedToken = null;
                        _tokenExpiryTime = DateTime.MinValue;
                        
                        // 重新获取token
                        string newToken = await LoginAsync();
                        if (!string.IsNullOrEmpty(newToken))
                        {
                            _logger.LogInformation("重新获取访问令牌成功，开始重试请求，端点：{Endpoint}", endpoint);
                            // 递归重试
                            return await SendRequestWithRedirectHandlingAsync(requestDto, endpoint, retryCount + 1);
                        }
                        else
                        {
                            _logger.LogError("重新获取访问令牌失败，无法重试请求，端点：{Endpoint}", endpoint);
                        }
                    }
                }
                
                string responseContent = await response.Content.ReadAsStringAsync();
                _logger.LogDebug("收到WMS响应，长度：{Length}，端点：{Endpoint}", 
                    responseContent?.Length ?? 0, endpoint);
                
                return responseContent;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "发送请求到WMS失败，端点：{Endpoint}", endpoint);
                return null;
            }
        }

        public async Task<bool> SendTaskStatus(TaskStatusDto taskStatusDto)
        {
            _logger.LogInformation("开始推送WMS任务状态，订单号:{OrderCode}，任务状态:{Status}",
                taskStatusDto.OrderCode, taskStatusDto.Status);
            
            try
            {
                // 增加请求日志，记录完整的请求数据
                string requestJson = JsonConvert.SerializeObject(taskStatusDto, _jsonSettings);
                _logger.LogDebug("推送WMS任务状态请求数据：{RequestData}", requestJson);
                
                string responseStr = await SendRequestAsync(taskStatusDto, "wms/stockTask/wcsSetStockTaskStatus");
                
                if (string.IsNullOrEmpty(responseStr))
                {
                    _logger.LogWarning("订单号:{OrderCode}推送WMS任务状态失败，未收到响应", taskStatusDto.OrderCode);
                    return false;
                }

                _logger.LogDebug("推送WMS任务状态响应数据：{ResponseData}", responseStr);
                
                try
                {
                    JObject obj = JObject.Parse(responseStr);
                    bool success = obj["success"]?.Value<bool>() ?? false;
                    
                    if (success)
                    {
                        _logger.LogInformation("订单号:{OrderCode}推送WMS任务状态成功", taskStatusDto.OrderCode);
                    }
                    else
                    {
                        string errorMessage = obj["message"]?.ToString() ?? "未知错误";
                        _logger.LogWarning("订单号:{OrderCode}推送WMS任务状态失败，WMS返回失败：{ErrorMessage}", 
                            taskStatusDto.OrderCode, errorMessage);
                    }
                    
                    return success;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "解析任务状态响应失败：{Response}", responseStr);
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "推送WMS任务状态时发生异常，订单号:{OrderCode}", taskStatusDto.OrderCode);
                return false;
            }
        }

        public async Task<List<CheckOrder>> GetChkTask(ChkTaskDto chkTaskDto)
        {
            string responseStr = await SendRequestAsync(chkTaskDto, "wms/checkTask/checkTaskPagedGet");
            
            if (string.IsNullOrEmpty(responseStr))
            {
                _logger.LogWarning("获取WMS盘点任务失败");
                return null;
            }

            try
            {
                JObject jObject = JObject.Parse(responseStr);
                return jObject["items"]?.ToObject<List<CheckOrder>>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "解析盘点任务响应失败：{Response}", responseStr);
                return null;
            }
        }

        public async Task<bool> SendChkStatus(ChkStatusDto chkStatusDto)
        {
            string responseStr = await SendRequestAsync(chkStatusDto, "wms/checkTask/checkTaskStatusUpdate");
            
            if (string.IsNullOrEmpty(responseStr))
            {
                _logger.LogWarning("订单号:{OrderCode}推送WMS盘点任务状态失败", chkStatusDto.orderCode);
                return false;
            }

            try
            {
                JObject obj = JObject.Parse(responseStr);
                return obj["success"]?.Value<bool>() ?? false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "解析盘点状态响应失败：{Response}", responseStr);
                return false;
            }
        }

    }
}
