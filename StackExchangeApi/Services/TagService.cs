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
                for (int i = 1; i <= 10; i++)
                {
                    RootDto rootDto = await FetchDataAsync(i);

                    var items = Mapper.MapToItems(rootDto);

                    _context.Items.AddRange(items);
                }
                
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
                var itemsData = _context.Items
                    .Include(y => y.Collectives)
                    .ThenInclude(z => z.ExternalLinks)
                    .ToList();

                if (itemsData == null)
                {
                    _logger.LogWarning("No data found in the database.");
                    return null!;
                }

                var total = itemsData.Sum(x => x.Count);
                return itemsData.Select(item => new TagsPercentage
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

                var itemsData = _context.Items
                    .Include(y => y.Collectives)
                    .ThenInclude(z => z.ExternalLinks)
                    .ToList();

                if (itemsData == null)
                {
                    _logger.LogWarning("No data found in the database for pagination.");
                    return null!;
                }

                var items = queryParams.IsAscending
                    ? queryParams.OrderBy switch
                    {
                        "name" => itemsData.OrderByDescending(i => i.Name).ToList(),
                        "count" => itemsData.OrderByDescending(i => i.Count).ToList(),
                        _ => itemsData.ToList()
                    }
                    : queryParams.OrderBy switch
                    {
                        "name" => itemsData.OrderBy(i => i.Name).ToList(),
                        "count" => itemsData.OrderBy(i => i.Count).ToList(),
                        _ => itemsData.ToList()
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

        public async Task<RootDto> FetchDataAsync(int pageNumber)
        {
            try
            {
                string requestUrl = $"https://api.stackexchange.com/2.3/tags?page={pageNumber}&pagesize=100&order=desc&min=&max=&sort=popular&site=stackoverflow";
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

        public static class Mapper
        {
            public static List<Item> MapToItems(RootDto rootDto)
            {
                if (rootDto == null || rootDto.Items == null)
                    return new List<Item>();

                return rootDto.Items.Select(MapToItem).ToList();
            }

            private static Item MapToItem(ItemDto itemDto)
            {
                if (itemDto == null)
                    throw new ArgumentNullException(nameof(itemDto));

                return new Item
                {
                    HasSynonyms = itemDto.HasSynonyms,
                    IsModeratorOnly = itemDto.IsModeratorOnly,
                    IsRequired = itemDto.IsRequired,
                    Count = itemDto.Count,
                    Name = itemDto.Name,
                    Collectives = itemDto.Collectives?.Select(MapToCollective).ToList() ?? new List<Collective>()
                };
            }

            private static Collective MapToCollective(CollectiveDto collectiveDto)
            {
                if (collectiveDto == null)
                    throw new ArgumentNullException(nameof(collectiveDto));

                return new Collective
                {
                    Description = collectiveDto.Description,
                    Link = collectiveDto.Link,
                    Name = collectiveDto.Name,
                    Slug = collectiveDto.Slug,
                    Tags = collectiveDto.Tags ?? new List<string>(),
                    ExternalLinks = collectiveDto.ExternalLinks?.Select(MapToExternalLink).ToList() ?? new List<ExternalLink>()
                };
            }

            private static ExternalLink MapToExternalLink(ExternalLinkDto externalLinkDto)
            {
                if (externalLinkDto == null)
                    throw new ArgumentNullException(nameof(externalLinkDto));

                return new ExternalLink
                {
                    Type = externalLinkDto.Type,
                    Link = externalLinkDto.Link
                };
            }
        }

    }
}
