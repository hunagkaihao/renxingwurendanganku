using RestSharp;

namespace Shared.HttpHelp
{
    public static class RestApi
    {
        public static T? Get<T>(string url, string callback, object? pars) where T:class, new()
        {
            var type = Method.Get;
            RestResponse<T> ret = ExecApi<T>(url, callback, pars, type);
            return ret.Data;
        }
        
        public static async Task<T?> GetAsync<T>(string url, string callback, object? pars) where T:class, new()
        {
            var type = Method.Get;
            RestResponse<T> ret = await ExecApiAsync<T>(url, callback, pars, type);
            return ret.Data;
        }

        public static T? Post<T>(string url, string callback, object? pars) where T:class, new()
        {
            var type = Method.Post;
            RestResponse<T> ret = ExecApi<T>(url, callback, pars, type);
            return ret.Data;
        }

        public static async Task<T?> PostAsync<T>(string url, string callback, object? pars) where T:class, new()
        {
            var type = Method.Post;
            RestResponse<T> ret = await ExecApiAsync<T>(url, callback, pars, type);
            return ret.Data;
        }

        public static T? Delete<T>(string url, string callback, object? pars) where T:class, new()
        {
            var type = Method.Delete;
            RestResponse<T> ret = ExecApi<T>(url, callback, pars, type);
            return ret.Data;
        }

        public static async Task<T?> DeleteAsync<T>(string url, string callback, object? pars) where T:class, new()
        {
            var type = Method.Delete;
            RestResponse<T> ret = await ExecApiAsync<T>(url, callback, pars, type);
            return ret.Data;
        }

        public static T? Put<T>(string url, string callback, object? pars) where T:class, new()
        {
            var type = Method.Put;
            RestResponse<T> ret = ExecApi<T>(url, callback, pars, type);
            return ret.Data;
        }

        public static async Task<T?> PutAsync<T>(string url, string callback, object? pars) where T:class, new()
        {
            var type = Method.Put;
            RestResponse<T> ret = await ExecApiAsync<T>(url, callback, pars, type);
            return ret.Data;
        }

        private static RestResponse<T> ExecApi<T>(string baseUrl, string callback, object? pars, Method type) where T:class, new()
        {
            using (RestClient client = new RestClient(baseUrl))
            {
                var request = new RestRequest(callback, type);
                if (pars != null)
                {
                    if (pars.GetType() != typeof(Dictionary<string, Object>))
                    {
                        request.AddHeader("Content-Type", "application/json");
                        request.AddHeader("Accept", "application/json");
                        request.AddParameter("application/json", pars, ParameterType.RequestBody);
                    }
                    else
                    {
                        Dictionary<string, Object> aa = (Dictionary<string, Object>)pars;
                        foreach (var item in aa)
                        {
                            request.AddParameter(item.Key, item.Value.ToString());
                        }
                    }
                }
                else
                {
                    request.AddHeader("Content-Type", "application/json");
                    request.AddHeader("Accept", "application/json");
                }
                RestResponse<T> result = client.Execute<T>(request);
                if (!result.IsSuccessful)
                {
                    throw new Exception($"RestApi访问{callback}失败，发生错误：{result.ErrorMessage}");
                }
                return result;
            }
        }

        private static async Task<RestResponse<T>> ExecApiAsync<T>(string baseUrl, string callback, object? pars, Method type) where T:class, new()
        {
            using (RestClient client = new RestClient(baseUrl))
            {
                try
                {
                    var request = new RestRequest(callback, type);
                    if (pars != null)
                    {
                        if (pars.GetType() != typeof(Dictionary<string, Object>))
                        {
                            request.AddHeader("Content-Type", "application/json");
                            request.AddHeader("Accept", "application/json");
                            request.AddParameter("application/json", pars, ParameterType.RequestBody);
                            // string jsonPars = JsonConvert.SerializeObject(pars);
                            // request.AddJsonBody(jsonPars);
                        }
                        else
                        {
                            Dictionary<string, Object> aa = (Dictionary<string, Object>)pars;
                            foreach (var item in aa)
                            {
                                request.AddParameter(item.Key, item.Value.ToString());
                            }
                        }
                    }
                    else
                    {
                        request.AddHeader("Content-Type", "application/json");
                        request.AddHeader("Accept", "application/json");
                    }
                    RestResponse<T> result = await client.ExecuteAsync<T>(request);
                    if (!result.IsSuccessful)
                    {
                        throw new Exception(result.ErrorMessage);
                    }
                    return result;
                }
                catch(Exception ex)
                {
                    throw new Exception($"RestApi访问{callback}失败，发生错误：{ex.Message}");
                }
            }
        }
    } 
}