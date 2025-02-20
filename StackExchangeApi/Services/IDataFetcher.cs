using StackExchangeApi.Models;

namespace StackExchangeApi.Services
{
    public interface IDataFetcher
    {
        Task<RootDto> FetchDataAsync(int pageNumber);
    }
}