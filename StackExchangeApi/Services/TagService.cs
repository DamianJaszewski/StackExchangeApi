using Microsoft.EntityFrameworkCore;
using StackExchangeApi.Controllers;
using StackExchangeApi.Models;
using System.Text.Json;

namespace StackExchangeApi.Services
{
    public class TagService
    {
        private readonly HttpClient _httpClient;
        private readonly DataContext _context;
        private readonly ILogger<TagService> _logger;

        public TagService(IHttpClientFactory httpClientFactory, DataContext context, ILogger<TagService> logger)
        {
            _httpClient = httpClientFactory.CreateClient("StackExchangeClient");
            _context = context;
            _logger = logger;
        }

        public async Task PopulateDataAsync()
        {
            try
            {
                string requestUrl = "https://api.stackexchange.com/2.3/tags?page=1&pagesize=100&order=desc&min=1000&max=1200&sort=popular&site=stackoverflow";

                var response = await _httpClient.GetAsync(requestUrl);
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("External API returned an error: {StatusCode}", response.StatusCode);
                    throw new HttpRequestException(await response.Content.ReadAsStringAsync());
                }

                var json = await response.Content.ReadAsStringAsync();
                var entities = JsonSerializer.Deserialize<Root>(json);

                _context.Add(entities);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Data populated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while populating data.");
                throw;
            }
        }

        public async Task<List<TagsPercentage>> CalculateTagsPercentageAsync()
        {
            try
            {
                _logger.LogInformation("Started calculating tag percentages.");
                var rootsData = _context.Roots
                    .Include(x => x.Items)
                    .ThenInclude(y => y.Collectives)
                    .ThenInclude(z => z.ExternalLinks)
                    .FirstOrDefault();

                if (rootsData == null)
                {
                    _logger.LogWarning("No data found in the database.");
                    return null!;
                }

                var total = rootsData.Items.Sum(x => x.Count);
                return rootsData.Items.Select(item => new TagsPercentage
                {
                    TagId = item.Id,
                    Name = item.Name,
                    Count = item.Count,
                    Percentage = $"{Math.Round((double)item.Count / total * 100, 2)}%"
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while calculating tag percentages.");
                throw; // Można dodać lepszą obsługę wyjątków np. CustomException
            }
        }

        public async Task<List<Item>> GetPaginatedTagsAsync(TagQueryParams queryParams)
        {
            try
            {
                _logger.LogInformation("Fetching paginated tags.");

                var rootsData = _context.Roots
                    .Include(x => x.Items)
                    .ThenInclude(y => y.Collectives)
                    .ThenInclude(z => z.ExternalLinks)
                    .FirstOrDefault();

                if (rootsData == null)
                {
                    _logger.LogWarning("No data found in the database for pagination.");
                    return null!;
                }

                var items = queryParams.IsAscending
                    ? queryParams.OrderBy switch
                    {
                        "name" => rootsData.Items.OrderByDescending(i => i.Name).ToList(),
                        "count" => rootsData.Items.OrderByDescending(i => i.Count).ToList(),
                        _ => rootsData.Items.ToList()
                    }
                    : queryParams.OrderBy switch
                    {
                        "name" => rootsData.Items.OrderBy(i => i.Name).ToList(),
                        "count" => rootsData.Items.OrderBy(i => i.Count).ToList(),
                        _ => rootsData.Items.ToList()
                    };

                return items
                    .Skip(queryParams.PageSize * (queryParams.Page - 1))
                    .Take(queryParams.PageSize)
                    .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching paginated tags.");
                throw;
            }
        }
    }
}
