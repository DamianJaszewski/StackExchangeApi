using Microsoft.Extensions.Logging;
using StackExchangeApi.Models;
using System.Net.Http;
using System.Text.Json;

namespace StackExchangeApi.Services
{
    public class DataFetcher : IDataFetcher
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<TagService> _logger;

        public DataFetcher(HttpClient httpClient, ILogger<TagService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }
        public async Task<RootDto> FetchDataAsync(int pageNumber)
        {
            try
            {
                string requestUrl = $"https://api.stackexchange.com/2.3/tags?page={pageNumber}&pagesize=100&order=desc&min=&max=&sort=popular&site=stackoverflow";
                _httpClient.DefaultRequestHeaders.Add("User-Agent", "YourAppName");
                var response = await _httpClient.GetAsync(requestUrl);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("External API returned an error: {StatusCode}", response.StatusCode);
                    throw new HttpRequestException(await response.Content.ReadAsStringAsync());
                }

                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<RootDto>(json);

            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP request failed.");
                throw;
            }
        }

    }
}
