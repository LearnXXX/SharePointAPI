using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SharePointGraph
{
    class GraphApiCallHelper
    {
        public static void PutApi(string accessToken, string webApiUrl, byte[] contents)
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", accessToken);
            httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            var content = new ByteArrayContent(contents);
            var result = httpClient.PutAsync(webApiUrl, content).Result;
        }
        public static Task<HttpResponseMessage> GetApi(string accessToken, string webApiUrl)
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", accessToken);
            httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            return httpClient.GetAsync(webApiUrl);
        }

        public static void DeleteApi(string accessToken, string webApiUrl)
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", accessToken);
            httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            var result = httpClient.DeleteAsync(webApiUrl).Result;
        }


        public static Task<HttpResponseMessage> PatchApi(string accessToken, string webApiUrl, string requestsBody)
        {
            var httpClient = new HttpClient();
            var method = new HttpMethod("PATCH");
            var request = new HttpRequestMessage(method, webApiUrl) { Content = new StringContent(requestsBody, Encoding.UTF8, "application/json") };
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", accessToken);
            httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            return httpClient.SendAsync(request);
        }


        public static Task<HttpResponseMessage> PostApi(string accessToken, string webApiUrl, string requestBody)
        {
            var httpClient = new HttpClient();

            var requestContent = new StringContent(requestBody, Encoding.UTF8, "application/json");
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", accessToken);
            httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            return httpClient.PostAsync(webApiUrl, requestContent);

        }
        public static JObject PutApiUploadLargeFileJObject(string accessToken, string webApiUrl, byte[] content, string contentRange)
        {
            var handler = new WebRequestHandler();

            handler.UseProxy = true;
            var httpClient = new HttpClient(handler);
            var requestContent = new ByteArrayContent(content);
            requestContent.Headers.Add("Content-Length", content.Length.ToString());
            requestContent.Headers.Add("Content-Range", contentRange);

            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", accessToken);
            httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            var result = httpClient.PutAsync(webApiUrl, requestContent).Result;
            if (result.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject(result.Content.ReadAsStringAsync().Result) as JObject;
            }
            return null;
        }
        public static JObject PostApiJObject(string accessToken, string webApiUrl, string requestBody)
        {
            var result = PostApi(accessToken, webApiUrl, requestBody).Result;
            if (result.IsSuccessStatusCode)
            {
                var info = result.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject(info) as JObject;
            }
            return null;
        }

        public static JObject PatchApiJObject(string accessToken, string webApiUrl, string requestsBody)
        {
            var result = PatchApi(accessToken, webApiUrl, requestsBody).Result;
            if (result.IsSuccessStatusCode)
            {
                var info = result.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject(info) as JObject;
            }
            return null;
        }

        public static string GetApiResponseContent(string accessToken, string webApiUrl)
        {
            var result = GetApi(accessToken, webApiUrl).Result;
            if (result.IsSuccessStatusCode)
            {
                return result.Content.ReadAsStringAsync().Result;
            }
            return null;
        }

        public static JObject GetApiJObject(string accessToken, string webApiUrl)
        {
            var result = GetApi(accessToken, webApiUrl).Result;
            if (result.IsSuccessStatusCode)
            {
                var info = result.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject(info) as JObject;
            }
            return null;
        }
    }
}
