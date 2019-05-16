using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace WebAPI.Infrastructure.Services
{
    public class ForwarderService : IForwarderService
    {
        private readonly HttpClient _httpClient;

        public ForwarderService(IConfiguration configuration, HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.BaseAddress = new Uri(configuration["Config:WebAPIUrl"]);
        }

        public async Task<Tuple<HttpStatusCode, TOutput>> ForwardRequest<TInput, TOutput>(string apiUrl, TInput data, HttpMethod method)
        {
            var contentData = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(new HttpRequestMessage
            {
                RequestUri = new Uri(apiUrl, UriKind.Relative),
                Content = contentData,
                Method = method
            });

            var stringData = await response.Content.ReadAsStringAsync();
            var jsonData = JsonConvert.DeserializeObject<TOutput>(stringData);

            return Tuple.Create(response.StatusCode, jsonData);
        }
    }
}