using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientApi.NetApi
{

    public class RestApi<T> where T:class,new ()
    {
        //public static string Token = "";
        public T Get(string server, string url, object pars)
        {
            var type = Method.Get;
            RestResponse<T> reval = GetApiInfo(server, url, pars, type);
            return reval.Data;
        }
        public async Task<T> GetAsync(string server, string url, object pars)
        {
            var type = Method.Get;
            RestResponse<T> reval = await GetApiInfoAsync(server, url, pars, type);
            return reval.Data;
        }
        public T Post(string server, string url, object pars)
        {
            var type = Method.Post;
            RestResponse<T> reval = GetApiInfo(server, url, pars, type);
            return reval.Data;
        }
        public T Delete(string server, string url, object pars)
        {
            var type = Method.Delete;
            RestResponse<T> reval = GetApiInfo(server, url, pars, type);
            return reval.Data;
        }
        public T Put(string server, string url, object pars)
        {
            var type = Method.Put;
            RestResponse<T> reval = GetApiInfo(server, url, pars, type);
            return reval.Data;
        }

        private static RestResponse<T> GetApiInfo(string server, string url, object pars, Method type)
        {
            using (RestClient client = new RestClient(server))
            {
                var request = new RestRequest(url, type);
                //var contactJson = JsonConvert.SerializeObject(contact);
                //request.Parameters.Clear();
                if (pars != null)
                {
                    if (pars?.GetType() != typeof(Dictionary<string, Object>))
                    {
                        request.AddHeader("Content-Type", "application/json");
                        request.AddHeader("Accept", "application/json");
                        //request.AddUrlSegment("business-id", busId);
                        //request.AddParameter("Authorization", string.Format("TOKEN json:{0}", jsonToken), ParameterType.HttpHeader);
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
                if (LoginToken.Token != "")
                {
                    request.AddHeader("Authorization", "Bearer " + LoginToken.Token);
                }
                RestResponse<T> result = client.Execute<T>(request);
                if (!result.IsSuccessful)
                {
                    if (result.Content == null)
                    {
                        //MessageBox.Show($"登录失败，{result.Data?.error}");
                        throw new Exception($"{result.ErrorMessage}");
                    }
                }
                return result;
                //if (pars != null)
                //    request.AddObject(pars);
                //var client = new RestClient(RequestInfo.HttpDomain + url);
                //client.CookieContainer = new System.Net.CookieContainer();
                //IRestResponse<T> reval = client.Execute<T>(request);
                //if (reval.ErrorException != null)
                //{
                //    PubMethod.WirteExp(new Exception(reval.Content));
                //    throw new Exception("请求出错");
                //}
                //return reval;
            }
        }

        private static async Task<RestResponse<T>> GetApiInfoAsync(string server, string url, object pars, Method type)
        {
            using (RestClient client = new RestClient(server))
            {
                var request = new RestRequest(url, type);
                //var contactJson = JsonConvert.SerializeObject(contact);
                //request.Parameters.Clear();
                if (pars != null)
                {
                    if (pars?.GetType() != typeof(Dictionary<string, Object>))
                    {
                        request.AddHeader("Content-Type", "application/json");
                        request.AddHeader("Accept", "application/json");
                        //request.AddUrlSegment("business-id", busId);
                        //request.AddParameter("Authorization", string.Format("TOKEN json:{0}", jsonToken), ParameterType.HttpHeader);
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
                if (LoginToken.Token != "")
                {
                    request.AddHeader("Authorization", "Bearer " + LoginToken.Token);
                }
                RestResponse<T> result = await client.ExecuteAsync<T>(request);
                if (!result.IsSuccessful)
                {
                    if (result.Content == null)
                    {
                        //MessageBox.Show($"登录失败，{result.Data?.error}");
                        throw new Exception($"{result.ErrorMessage}");
                    }
                }
                return result;
                //if (pars != null)
                //    request.AddObject(pars);
                //var client = new RestClient(RequestInfo.HttpDomain + url);
                //client.CookieContainer = new System.Net.CookieContainer();
                //IRestResponse<T> reval = client.Execute<T>(request);
                //if (reval.ErrorException != null)
                //{
                //    PubMethod.WirteExp(new Exception(reval.Content));
                //    throw new Exception("请求出错");
                //}
                //return reval;
            }
        }

    }
}
