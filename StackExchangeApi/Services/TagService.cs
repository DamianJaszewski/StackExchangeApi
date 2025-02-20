using StackExchangeApi.Models;

namespace StackExchangeApi.Services
{
    public class TagService : ITagService
    {
        private readonly DataContext _context;
        private readonly ILogger<TagService> _logger;
        private readonly IDataFetcher _dataFetcher;
        private readonly IMapper _mapper;

        public TagService(DataContext context, ILogger<TagService> logger, IDataFetcher dataFetcher, IMapper mapper)
        {
            _context = context;
            _logger = logger;
            _dataFetcher = dataFetcher;
            _mapper = mapper;   
        }

        public async Task PopulateDataAsync()
        {
            try
            {
                for (int i = 1; i <= 10; i++)
                {
                    RootDto rootDto = await _dataFetcher.FetchDataAsync(i);

                    var items = _mapper.MapToItems(rootDto);

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
                var itemsData = _context.Items.ToList();

                if (!itemsData.Any())
                {
                    _logger.LogWarning("No data found in the database.");
                    await PopulateDataAsync();
                    itemsData = _context.Items.ToList();
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
                throw; 
            }
        }

        public async Task<List<Item>> GetPaginatedTagsAsync(TagQueryParams queryParams)
        {
            try
            {
                _logger.LogInformation("Fetching paginated tags.");

                var itemsData = _context.Items.ToList();

                if (!itemsData.Any())
                {
                    _logger.LogWarning("No data found in the database for pagination.");
                    await PopulateDataAsync();
                    itemsData = _context.Items.ToList();
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
    }
}
