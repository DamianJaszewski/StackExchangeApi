using StackExchangeApi.Models;

namespace StackExchangeApi.Services
{
    public interface IMapper
    {
        List<Item> MapToItems(RootDto rootDto);
    }
}