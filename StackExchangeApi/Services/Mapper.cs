using StackExchangeApi.Models;

namespace StackExchangeApi.Services
{
    public class Mapper : IMapper
    {
        public List<Item> MapToItems(RootDto rootDto)
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
