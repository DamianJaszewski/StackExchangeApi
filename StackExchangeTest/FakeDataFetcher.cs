using StackExchangeApi.Models;
using StackExchangeApi.Services;

namespace StackExchangeTest
{
    public class FakeDataFetcher : IDataFetcher
    {
        public async Task<RootDto> FetchDataAsync(int pageNumber)
        {
            var rootDto = new RootDto()
            {
                HasMore = false,
                QuotaMax = 100,
                QuotaRemaining = 100,
                Items = new List<ItemDto>()
                {
                    new ItemDto()
                    {
                        HasSynonyms = true,
                        IsModeratorOnly = true,
                        IsRequired = true,
                        Count = 10,
                        Name = "csharp",
                    },
                    new ItemDto()
                    {
                        HasSynonyms = true,
                        IsModeratorOnly = true,
                        IsRequired = true,
                        Count = 30,
                        Name = "python",
                    },
                    new ItemDto()
                    {
                        HasSynonyms = true,
                        IsModeratorOnly = true,
                        IsRequired = true,
                        Count = 60,
                        Name = "java",
                    }
                }
            };

            return rootDto;
        }
    }
}
