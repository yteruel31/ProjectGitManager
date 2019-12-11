using System;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;

namespace PGM.Lib
{
    public class HttpClientRepository : IHttpClientRepository
    {
        public async Task<HttpResult> Get(string url)
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                return new HttpResult(false, await response.Content.ReadAsStringAsync());
            }

            return new HttpResult(true, await response.Content.ReadAsStringAsync());
        }

        public async Task<HttpResult> Post(string url, string body)
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.PostAsync(url, new StringContent(body, Encoding.UTF8, "application/json"));

            if (!response.IsSuccessStatusCode)
            {
                return new HttpResult(false, await response.Content.ReadAsStringAsync());
            }

            return new HttpResult(true, await response.Content.ReadAsStringAsync());
        }
    }

    public class HttpResult
    {
        public HttpResult(bool hasSucceeded, string response)
        {
            HasSucceeded = hasSucceeded;
            Response = response;
        }
        public bool HasSucceeded { get; }
        public string Response { get; }
    }

    public interface IHttpClientRepository
    {
        Task<HttpResult> Get(string url);

        Task<HttpResult> Post(string url, string body);
    }
}