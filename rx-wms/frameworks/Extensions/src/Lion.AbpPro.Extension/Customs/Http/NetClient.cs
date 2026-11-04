using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Lion.AbpPro.Extension.Customs.Http
{
    /// <summary>
    /// 自定义第三方网络调用接口类
    /// </summary>
    public class NetClient
    {


        public static bool serverConnectted = false;
        public static string Token = "";
        public async static Task<string> HttpPostAsync(string url, string body)
        {
            Encoding encoding = Encoding.UTF8;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.Accept = "text/html, application/xhtml+xml, */*";
            request.ContentType = "application/json";
            // request.Headers.Add("Authorization", "Bearer " + LoginService.Token);
            try
            {
                byte[] buffer = encoding.GetBytes(body);
                request.ContentLength = buffer.Length;
                request.GetRequestStream().Write(buffer, 0, buffer.Length);
                HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync();
                using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                {
                    return reader.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static string HttpPost(string url, string body)
        {
            //serverConnectted = Ping();
            Encoding encoding = Encoding.UTF8;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.Accept = "text/html, application/xhtml+xml, */*";
            request.ContentType = "application/json";
            if (Token != "")
            {
                request.Headers.Add("Authorization", "Bearer " + Token);
            }
            try
            {
                byte[] buffer = encoding.GetBytes(body);
                request.ContentLength = buffer.Length;
                request.GetRequestStream().Write(buffer, 0, buffer.Length);
                HttpWebResponse response = null;
                try
                {
                    response = (HttpWebResponse)request.GetResponse();
                }
                catch (WebException ex)
                {
                    response = (HttpWebResponse)ex.Response;
                }
                if (response == null)
                {
                    if (!serverConnectted)
                    {
                        throw new Exception("网络异常，服务器连接超时!");
                    }
                    throw new Exception("接口异常!");
                }
                using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                {
                    return reader.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }


        public async static Task<string> HttpGetAsync(string url)
        {
            Encoding encoding = Encoding.UTF8;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.Accept = "text/html, application/xhtml+xml, */*";
            request.ContentType = "application/json";
            //request.Headers.Add("Authorization", "Bearer " + LoginService.Token);

            HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync();
            //HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
            {
                return reader.ReadToEnd();
            }
        }
        public static string HttpGet(string url)
        {
            Encoding encoding = Encoding.UTF8;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Timeout = 5000;
            request.Method = "GET";
            request.Accept = "text/html, application/xhtml+xml, */*";
            request.ContentType = "application/json";
            //request.Headers.Add("Authorization", "Bearer " + LoginService.Token);
            if (Token != "")
            {
                request.Headers.Add("Authorization", "Bearer " + Token);
            }
            HttpWebResponse response = null;
            try
            {
                response = (HttpWebResponse)request.GetResponse();
            }
            catch (WebException ex)
            {
                response = (HttpWebResponse)ex.Response;
            }

            if (response == null)
            {
                if (!serverConnectted)
                {
                    throw new Exception("网络异常，服务器连接超时!");
                }
                throw new Exception("接口异常!");
            }
            using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
            {
                return reader.ReadToEnd();
            }
        }

        public static string HttpDelete(string url)
        {
            Encoding encoding = Encoding.UTF8;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Timeout = 5000;
            request.Method = "DELETE";
            request.Accept = "text/html, application/xhtml+xml, */*";
            request.ContentType = "application/json";
            //request.Headers.Add("Authorization", "Bearer " + LoginService.Token);
            if (Token != "")
            {
                request.Headers.Add("Authorization", "Bearer " + Token);
            }
            HttpWebResponse response = null;
            try
            {
                response = (HttpWebResponse)request.GetResponse();
            }
            catch (WebException ex)
            {
                response = (HttpWebResponse)ex.Response;
            }

            if (response == null)
            {
                if (!serverConnectted)
                {
                    throw new Exception("网络异常，服务器连接超时!");
                }
                throw new Exception("接口异常!");
            }
            using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
