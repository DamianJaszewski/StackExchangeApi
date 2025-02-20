using StackExchangeApi.Models;

namespace StackExchangeApi.Services
{
    public interface ITagService
    {
        Task<List<TagsPercentage>> CalculateTagsPercentageAsync();
        Task<List<Item>> GetPaginatedTagsAsync(TagQueryParams queryParams);
        Task PopulateDataAsync();
    }
}