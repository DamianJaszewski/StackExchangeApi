using System.Text.Json.Serialization;

namespace StackExchangeApi.Models
{
    public class ItemDto
    {
        [JsonPropertyName("has_synonyms")]
        public bool HasSynonyms { get; set; }

        [JsonPropertyName("is_moderator_only")]
        public bool IsModeratorOnly { get; set; }

        [JsonPropertyName("is_required")]
        public bool IsRequired { get; set; }

        [JsonPropertyName("count")]
        public int Count { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("collectives")]
        public ICollection<CollectiveDto> Collectives { get; set; }
    }
}
