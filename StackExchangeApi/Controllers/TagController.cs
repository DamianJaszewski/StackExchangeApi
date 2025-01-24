using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StackExchangeApi.Models;
using System.Text.Json;
namespace StackExchangeApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TagController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly DataContext _context;
        private readonly ILogger<TagController> _logger;

        public TagController(IHttpClientFactory httpClientFactory, DataContext context, ILogger<TagController> logger)
        {
            _httpClient = httpClientFactory.CreateClient("StackExchangeClient");
            _context = context;
            _logger = logger;
        }

        [HttpGet("Populate Data")]
        public async Task<IActionResult> PopulateData()
        {
            string requestUrl = "https://api.stackexchange.com/2.3/tags?page=1&pagesize=100&order=desc&min=1000&max=1200&sort=popular&site=stackoverflow";

            var response = await _httpClient.GetAsync(requestUrl);
            var json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
            }

            Root? entities = JsonSerializer.Deserialize<Root>(json);

            _context.Add(entities);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetTagsAsync()
        {
            try
            {
                Root? data = _context.Roots
                    .Include(x => x.Items)
                    .ThenInclude(y => y.Collectives)
                    .ThenInclude(z => z.ExternalLinks)
                    .FirstOrDefault();

                if (data == null)
                {
                    _logger.LogInformation("Processing GetTags");
                    string requestUrl = "https://api.stackexchange.com/2.3/tags?page=1&pagesize=100&order=desc&min=1000&max=1200&sort=popular&site=stackoverflow";

                    var response = await _httpClient.GetAsync(requestUrl);
                    var json = await response.Content.ReadAsStringAsync();

                    if (!response.IsSuccessStatusCode)
                    {
                        return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
                    }

                    data = JsonSerializer.Deserialize<Root>(json);

                    _context.Add(data);
                    await _context.SaveChangesAsync();

                }

                return Ok(data);

            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet("GetPerentage")]
        public async Task<IActionResult> GetPercentageAsync()
        {
            Root? rootsData = _context.Roots
                   .Include(x => x.Items)
                   .ThenInclude(y => y.Collectives)
                   .ThenInclude(z => z.ExternalLinks)
                   .FirstOrDefault();


            if (rootsData == null)
            {
                //No data
                return Ok("No data");
            }

            var total = rootsData.Items.Sum(x => x.Count);
            var tagsPercentage = new List<TagsPercentage>();

            foreach (var item in rootsData.Items)
            {
                var tag = new TagsPercentage() {
                    TagId = item.Id,
                    Name = item.Name,
                    Count = item.Count,
                    Percentage = (Math.Round(((double)item.Count / total) * 100, 2) ).ToString() + "%",
                };

                tagsPercentage.Add(tag);
            }

            return Ok(tagsPercentage);
        }

        [HttpPost]
        public async Task<IActionResult> GetWithPagination(TagQueryParams queryParams)
        {
            Root? rootsData = _context.Roots
                   .Include(x => x.Items)
                   .ThenInclude(y => y.Collectives)
                   .ThenInclude(z => z.ExternalLinks)
                   .FirstOrDefault();

            if (rootsData == null)
            {
                //No data
                return Ok("No data");
            }

            List<Item> items = rootsData.Items.ToList();

            if (queryParams.IsDescending)
            {
                items = queryParams.OrderBy switch
                {
                    "name" => items.OrderByDescending(i => i.Name).ToList(),
                    "count" => items.OrderByDescending(i => i.Count).ToList(),
                    _ => items.ToList() // Domyślne zachowanie, jeśli `OrderBy` nie pasuje
                };
            }
            else
            {
                items = queryParams.OrderBy switch
                {
                    "name" => items.OrderBy(i => i.Name).ToList(),
                    "count" => items.OrderBy(i => i.Count).ToList(),
                    _ => items.ToList() // Domyślne zachowanie, jeśli `OrderBy` nie pasuje
                };
            }

            var sortedItems = items.Skip(queryParams.PageSize * (queryParams.Page - 1)).Take(queryParams.PageSize).ToList();

            return Ok(sortedItems);
        }
    }
}
