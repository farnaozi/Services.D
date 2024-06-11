using Services.D.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Services.D.Infrastructure.Services
{
    public class ServiceBase
    {
        #region *** private fields
        private readonly IHttpContextAccessor _httpContextAccessor;
        private string Token => _httpContextAccessor.HttpContext.Request.Headers[HeaderNames.Authorization];
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILoggerRepo _loggerRepo;
        private HttpClient _httpClient { get; set; }
        #endregion

        #region *** ctor
        public ServiceBase(IHttpClientFactory httpClientFactory,
            IHttpContextAccessor httpContextAccessor,
            ILoggerRepo loggerRepo)
        {
            _loggerRepo = loggerRepo;
            _httpContextAccessor = httpContextAccessor;
            _httpClientFactory = httpClientFactory;
            _httpClient = _httpClientFactory.CreateClient();
        }
        #endregion

        #region *** internal
        internal async Task<Tuple<bool, string>> SendRequest(string requestUrl, HttpMethod httpMethod, object request = null)
        {
            try
            {
                await _loggerRepo.LogInfo($"requestUrl -> {requestUrl}");

                if (request != null)
                    await _loggerRepo.LogInfo($"request -> {JsonConvert.SerializeObject(request)}");

                var httpRequest = new HttpRequestMessage()
                {
                    Method = httpMethod,
                    RequestUri = new Uri(requestUrl)
                };

                httpRequest.Headers.Add("Authorization", Token);

                if (request != null)
                {
                    StringContent content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
                    httpRequest.Content = content;
                }

                using (var response = await _httpClient.SendAsync(httpRequest))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();

                    return Tuple.Create(response.IsSuccessStatusCode, apiResponse);
                }
            }
            catch (Exception ex)
            {
                await _loggerRepo.LogError(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
                return Tuple.Create(false, ex.Message);
            }
        }
        #endregion
    }
}
